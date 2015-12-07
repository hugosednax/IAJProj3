using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DecisionMakingActions;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Decomposers;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures;
using RAIN.Navigation;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using UnityEngine;
using UnityEngine.UI;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts
{
    public class AutonomousCharacter : MonoBehaviour
    {
        //constants
        public const string SURVIVE_GOAL = "Survive";
        public const string REST_GOAL = "Rest";
        public const string EAT_GOAL = "Eat";
        public const string GET_RICH_GOAL = "GetRich";
        public const string CONQUER_GOAL = "Conquer";

        public const float DECISION_MAKING_INTERVAL = 2f;
        //public fields to be set in Unity Editor
        public GameManager.GameManager GameManager;
        public TextMesh ActionText;
        public Text SurviveGoalText;
        public Text EatGoalText;
        public Text RestGoalText;
        public Text GetRichGoalText;
        public Text ConquerGoalText;
        public Text TotalProcessingTimeText;
        public Text BestDiscontentmentText;
        public Text ProcessedActionsText;
        public Text BestActionText;


        public Goal RestGoal { get; private set; }
        public Goal SurviveGoal { get; private set; }
        public Goal GetRichGoal { get; private set; }
        public Goal EatGoal { get; private set; }
        public Goal ConquerGoal { get; private set; }
        public List<Goal> Goals { get; set; }
        public List<Action> Actions { get; set; }
        public Action CurrentAction { get; private set; }
        public DynamicCharacter Character { get; private set; }
        public FixedTargeter Targeter { get; set; }
        public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }
        public InfluenceMap RedInfluenceMap { get; set; }
        public InfluenceMap GreenInfluenceMap { get; set; }
        public InfluenceMap ResourceInfluenceMap { get; set; }

        public List<IInfluenceUnit> RedFlags { get; set; }
        public List<IInfluenceUnit> GreenFlags { get; set; }
        public Dictionary<NavigationGraphNode,IInfluenceUnit> ActiveResources { get; set; }
        public Dictionary<LocationRecord, float> CombinedInfluence { get; set; }
        public Dictionary<LocationRecord, float> SecurityMap { get; set; }
        public Vector3 BestFlagPosition { get; set; }
        public LocationRecord BestFlagLocationRecord { get; set; }
        public float BestCombinedInfluence { get; set; }


        //private fields for internal use only
        public NavMeshPathGraph navMesh;
        private AStarPathfinding aStarPathFinding;
        private PathFindingDecomposer decomposer;
        

        private bool draw;
        private uint influenceMapDebugMode;

        private float nextUpdateTime = 0.0f;
        private float previousGold = 0.0f;

        public void Start()
        {
            this.draw = true;
            this.influenceMapDebugMode = 0;

            this.navMesh = NavigationManager.Instance.NavMeshGraphs[0];
            this.Character = new DynamicCharacter(this.gameObject);

           
            //initialization of the movement algorithms
            this.aStarPathFinding = new NodeArrayAStarPathFinding(this.navMesh, new EuclideanDistanceHeuristic(), this);
            this.aStarPathFinding.NodesPerSearch = 500;

            var steeringPipeline = new SteeringPipeline
            {
                MaxAcceleration = 40.0f,
                MaxConstraintSteps = 2,
                Character = this.Character.KinematicData,
            };

            this.decomposer = new PathFindingDecomposer(steeringPipeline, this.aStarPathFinding);
            this.Targeter = new FixedTargeter(steeringPipeline);
            steeringPipeline.Targeters.Add(this.Targeter);
            steeringPipeline.Decomposers.Add(this.decomposer);
            steeringPipeline.Actuator = new FollowPathActuator(steeringPipeline);

            this.Character.Movement = steeringPipeline;

            //initialization of the Influence Maps
            this.RedInfluenceMap = new InfluenceMap(this.navMesh,new SimpleUnorderedList(), new ClosedLocationRecordDictionary(), new LinearInfluenceFunction(), 0.1f);
            this.GreenInfluenceMap = new InfluenceMap(this.navMesh, new SimpleUnorderedList(), new ClosedLocationRecordDictionary(), new LinearInfluenceFunction(), 0.1f);
            this.ResourceInfluenceMap = new InfluenceMap(this.navMesh, new SimpleUnorderedList(), new ClosedLocationRecordDictionary(), new LinearInfluenceFunction(), 0.1f);
            this.CombinedInfluence = new Dictionary<LocationRecord, float>();
            this.SecurityMap = new Dictionary<LocationRecord, float>();

            //initialization of the GOB decision making
            //let's start by creating 5 main goals
            //the eat goal is the only goal that increases at a fixed rate per second, it increases at a rate of 0.1 per second
            this.SurviveGoal = new Goal(SURVIVE_GOAL, 2.0f);
            this.EatGoal = new Goal(EAT_GOAL, 1.0f)
            {
                ChangeRate = 0.1f
            };
            this.GetRichGoal = new Goal(GET_RICH_GOAL, 1.0f)
            {
                InsistenceValue = 5.0f,
                ChangeRate = 0.2f
            };
            this.RestGoal = new Goal(REST_GOAL, 1.0f);
            this.ConquerGoal = new Goal(CONQUER_GOAL, 1.5f)
            {
                InsistenceValue = 5.0f
            };

            this.Goals = new List<Goal>();
            this.Goals.Add(this.SurviveGoal);
            this.Goals.Add(this.EatGoal);
            this.Goals.Add(this.GetRichGoal);
            this.Goals.Add(this.RestGoal);
            this.Goals.Add(this.ConquerGoal);

            //initialize the available actions

            var restAction = new Rest(this);
            this.Actions = new List<Action>();
            this.Actions.Add(restAction);
            this.Actions.Add(new PlaceFlag(this));

            this.ActiveResources = new Dictionary<NavigationGraphNode, IInfluenceUnit>();
            int boars = 0;
            int chests = 0;
            int trees = 0;
            int beds = 0;

            foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
            {
                chests++;
                this.Actions.Add(new PickUpChest(this, chest));
                this.AddResource(new Resource(this.navMesh.QuantizeToNode(chest.transform.position, 1.0f)));
            }

            foreach (var tree in GameObject.FindGameObjectsWithTag("Tree"))
            {
                trees++;
                this.Actions.Add(new GetArrows(this, tree));
                this.AddResource(new Resource(this.navMesh.QuantizeToNode(tree.transform.position, 1.0f)));
            }
            
            foreach (var bed in GameObject.FindGameObjectsWithTag("Bed"))
            {
                beds++;
                this.Actions.Add(new Sleep(this, bed));
                this.AddResource(new Resource(this.navMesh.QuantizeToNode(bed.transform.position, 1.0f)));
            }

            foreach (var boar in GameObject.FindGameObjectsWithTag("Boar"))
            {
                boars++;
                this.AddResource(new Resource(this.navMesh.QuantizeToNode(boar.transform.position, 1.0f)));
                this.Actions.Add(new MeleeAttack(this, boar));
                this.Actions.Add(new Shoot(this, boar));
            }

            this.ResourceInfluenceMap.Initialize(this.ActiveResources.Values.ToList());

            //flags used for the influence map
            this.RedFlags = new List<IInfluenceUnit>();
            foreach (var redFlag in GameObject.FindGameObjectsWithTag("RedFlag"))
            {
                this.RedFlags.Add(new Flag(this.navMesh.QuantizeToNode(redFlag.transform.position, 1.0f), FlagColor.Red));
            }
            

            this.GreenFlags = new List<IInfluenceUnit>();
            foreach (var greenFlag in GameObject.FindGameObjectsWithTag("GreenFlag"))
            {
                this.GreenFlags.Add(new Flag(this.navMesh.QuantizeToNode(greenFlag.transform.position, 1.0f), FlagColor.Green));
            }

            this.RedInfluenceMap.Initialize(this.RedFlags);
            this.GreenInfluenceMap.Initialize(this.GreenFlags);


            var worldModel = new CurrentStateWorldModelFEAR(this.GameManager, this.Actions, this.Goals);
            worldModel.InitArrays( boars, 1, trees,  beds,  chests);

            this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel,this.Actions,this.Goals);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.ChangeDebugInfluenceMap();
            }

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

                //first step, perceptions
                //update the agent's goals based on the state of the world
                this.SurviveGoal.InsistenceValue = 10 - this.GameManager.characterData.HP;
                this.EatGoal.InsistenceValue = this.GameManager.characterData.Hunger;
                this.RestGoal.InsistenceValue = 10 - this.GameManager.characterData.Energy;

                //the get rich goal is managed by DecisionMaking process because it does not correspond to a physiological need handled in the world
                this.GetRichGoal.InsistenceValue += 0.1f; //decay
                if (this.GetRichGoal.InsistenceValue > 10)
                {
                    this.GetRichGoal.InsistenceValue = 10.0f;
                }
                this.ConquerGoal.InsistenceValue += 0.1f;
                if (this.ConquerGoal.InsistenceValue > 10)
                {
                    this.ConquerGoal.InsistenceValue = 10;
                }

                if (this.GameManager.characterData.Money > this.previousGold)
                {
                    this.GetRichGoal.InsistenceValue -= this.GameManager.characterData.Money - this.previousGold;
                    this.previousGold = this.GameManager.characterData.Money;
                }

                this.SurviveGoalText.text = "Survive: " + this.SurviveGoal.InsistenceValue;
                this.EatGoalText.text = "Eat: " + this.EatGoal.InsistenceValue;
                this.RestGoalText.text = "Rest: " + this.RestGoal.InsistenceValue;
                this.GetRichGoalText.text = "GetRich: " + this.GetRichGoal.InsistenceValue;
                this.ConquerGoalText.text = "Conquer: " + this.ConquerGoal.InsistenceValue;

                //initialize GOAP Decision Making Proccess
                this.GOAPDecisionMaking.InitializeDecisionMakingProcess();
            }

            //process the influence maps
            if (this.RedInfluenceMap.InProgress)
            {
                if (this.RedInfluenceMap.MapFloodDijkstra())
                {
                    this.CalculateCombinedInfluence();
                }
            }
            if (this.GreenInfluenceMap.InProgress)
            {
                if (this.GreenInfluenceMap.MapFloodDijkstra())
                {
                    this.CalculateCombinedInfluence();
                }
            }
            if (this.ResourceInfluenceMap.InProgress)
            {
                if (this.ResourceInfluenceMap.MapFloodDijkstra())
                {
                    this.CalculateCombinedInfluence();
                }
            }

            if (this.GOAPDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOAPDecisionMaking.ChooseAction();
                if (action != null)
                {
                    action.Execute();
                    this.CurrentAction = action;
                    this.ActionText.text = this.CurrentAction.Name;
                }
            }

            this.TotalProcessingTimeText.text = "Processing Time: " + this.GOAPDecisionMaking.TotalProcessingTime;
            this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue;
            this.ProcessedActionsText.text = "Action comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;
            
            if (this.GOAPDecisionMaking.BestAction != null)
            {
                var actionText = "";
                foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
                {
                    actionText += "\n" + action.Name;
                }
                this.BestActionText.text = "Best Action Sequence: " + actionText;
            }
            else
            {
                this.BestActionText.text = "Best Action Sequence:\nNone";
            }

            //Debug.Log(Targeter.Target.Position);
            this.Character.Update();
        }

        private void AddResource(Resource resource)
        {
            if (!this.ActiveResources.ContainsKey(resource.Location))
            {
                this.ActiveResources.Add(resource.Location,resource);
            }
        }

        private void CalculateCombinedInfluence()
        {
            //this can only be done when all influence maps are calculated
            if (this.RedInfluenceMap.InProgress || this.GreenInfluenceMap.InProgress || this.ResourceInfluenceMap.InProgress)
            {
                return;
            }

            this.CombinedInfluence.Clear();
            this.SecurityMap.Clear();
            this.BestCombinedInfluence = 0;
            this.BestFlagPosition = Vector3.zero;
            this.BestFlagLocationRecord = null;
            #region red
            foreach (var redLocationRecord in this.RedInfluenceMap.Closed.All())
            {
                LocationRecord resources = this.ResourceInfluenceMap.Closed.SearchInClosed(redLocationRecord);
                LocationRecord green = this.GreenInfluenceMap.Closed.SearchInClosed(redLocationRecord);

                float resourcesInf = 0f;
                float greenInf = 0f;

                if (resources != null)
                    resourcesInf = resources.Influence;

                if (green != null)
                    greenInf = green.Influence;

                float security = redLocationRecord.Influence - greenInf;

                float quality = security * resourcesInf / (redLocationRecord.Influence * redLocationRecord.Influence);
                if (quality > BestCombinedInfluence)
                {
                    BestCombinedInfluence = quality;
                    BestFlagPosition = redLocationRecord.Location.Position;
                    BestFlagLocationRecord = redLocationRecord;
                }
                SecurityMap.Add(redLocationRecord, security);
                CombinedInfluence.Add(redLocationRecord, quality);
            }
            #endregion

            #region resources
            foreach (var resourcesLocationRecord in this.ResourceInfluenceMap.Closed.All())
            {
                if (CombinedInfluence.ContainsKey(resourcesLocationRecord))
                    continue;

                LocationRecord red = this.RedInfluenceMap.Closed.SearchInClosed(resourcesLocationRecord);
                LocationRecord green = this.GreenInfluenceMap.Closed.SearchInClosed(resourcesLocationRecord);

                float redInf = 1f;
                float greenInf = 0f;

                if (green != null)
                    greenInf = green.Influence;

                if (red != null)
                    redInf = red.Influence;

                float security = redInf - greenInf;
                float quality = security * resourcesLocationRecord.Influence / (redInf * redInf);

                if (quality > BestCombinedInfluence)
                {
                    BestCombinedInfluence = quality;
                    BestFlagPosition = resourcesLocationRecord.Location.Position;
                    BestFlagLocationRecord = resourcesLocationRecord;
                }
                SecurityMap.Add(resourcesLocationRecord, security);
                CombinedInfluence.Add(resourcesLocationRecord, quality);
            }
            #endregion

            #region pseudocode
            /*
                PSEUDOCODE

                for each LocationRecord locationRecord in any map{
                    if locationRecord is also in any of the remaining maps{
                        float quality; 
                        if(i_am_red)
                            quality = red * resources / green ;
                        else
                            quality = green * resources / red ;

                        if(quality > BestCombinedInfluence){
                            BestCombinedInfluence = quality;
                            BestFlagPosition = nodeRecord.position;
                        }

                        CombinedInfluence.Add(nodeRecord, quality);
                    }
                }
                */



                //implement the method
                //DO NOT FORGET to also update the BestCombinedInfluence and BestFlagPosition properties
            //they will be needed for the PlaceFlag action
            #endregion
        }

        public void UpdateRedFlags(ICollection<GameObject> redFlags)
        {
            //flags used for the influence map
            this.RedFlags = new List<IInfluenceUnit>();
            foreach (var redFlag in redFlags)
            {
                this.RedFlags.Add(new Flag(this.navMesh.QuantizeToNode(redFlag.transform.position, 1.0f), FlagColor.Red));
            }

            this.RedInfluenceMap.Initialize(this.RedFlags);
            this.BestCombinedInfluence = 0;
        }

        public void UpdateResources(ICollection<GameObject> resources)
        {
            this.ActiveResources.Clear();
            foreach (var resource in resources)
            {
                this.AddResource(new Resource(this.navMesh.QuantizeToNode(resource.transform.position, 1.0f)));
            }

            this.ResourceInfluenceMap.Initialize(this.ActiveResources.Values.ToList());
            this.BestCombinedInfluence = 0;
        }

        private void ChangeDebugInfluenceMap()
        {
            this.influenceMapDebugMode = (++this.influenceMapDebugMode) % 5;
            /*if (this.influenceMapDebugMode == 3)
            {
                this.influenceMapDebugMode = 0;
            }
            else
            {
                this.influenceMapDebugMode++;
            }*/
            //DebugDebugInfluenceMap();
        }

        private void DebugDebugInfluenceMap()
        {
            if (this.influenceMapDebugMode == 0)
            {
                Debug.Log("Red");
            }else if (this.influenceMapDebugMode == 1)
            {
                Debug.Log("Green");
            }
            else if (this.influenceMapDebugMode == 2)
            {
                Debug.Log("Resources");
            }
            else if (this.influenceMapDebugMode == 3)
            {
                Debug.Log("Combined");
            }
        }
        public void OnDrawGizmos()
        {
            if (this.draw)
            {
                var size = new Vector3(2, 1, 2);

                if (this.influenceMapDebugMode == 0)
                {
                    var red = Color.red;
                    Gizmos.color = red;
                    foreach (var locationRecord in this.RedInfluenceMap.Closed.All())
                    {
                        red.a = (1 / 5.0f) * locationRecord.Influence;
                        Gizmos.color = red;
                        Gizmos.DrawCube(locationRecord.Location.LocalPosition, size);
                    }
                }
                else if (this.influenceMapDebugMode == 1)
                {
                    var green = Color.green;
                    Gizmos.color = green;
                    foreach (var locationRecord in this.GreenInfluenceMap.Closed.All())
                    {
                        green.a = 1 / 5.0f * locationRecord.Influence;
                        Gizmos.color = green;
                        Gizmos.DrawCube(locationRecord.Location.LocalPosition, size);
                    }
                }
                else if (this.influenceMapDebugMode == 2)
                {
                    var yellow = Color.yellow;
                    Gizmos.color = yellow;
                    foreach (var locationRecord in this.ResourceInfluenceMap.Closed.All())
                    {
                        yellow.a = 1 / 2.5f * locationRecord.Influence;
                        Gizmos.color = yellow;
                        Gizmos.DrawCube(locationRecord.Location.LocalPosition, size);
                    }
                }
                else if (this.influenceMapDebugMode == 3)
                {
                    var blue = Color.blue;
                    Gizmos.color = blue;
                    foreach (var locationRecord in this.CombinedInfluence)
                    {
                        blue.a = 1 / 5.0f * locationRecord.Value;
                        Gizmos.color = blue;
                        Gizmos.DrawCube(locationRecord.Key.Location.LocalPosition, size);
                    }
                }
                
                //draw the current Solution Path if any (for debug purposes)
                if (this.decomposer.UnsmoothedPath != null)
                {
                    var previousPosition = this.Character.KinematicData.position;
                    foreach (var pathPosition in this.decomposer.UnsmoothedPath.PathPositions)
                    {
                        Debug.DrawLine(previousPosition, pathPosition, Color.red);
                        previousPosition = pathPosition;
                    }

                    previousPosition = this.Character.KinematicData.position;
                    foreach (var pathPosition in this.decomposer.CurrentPath.PathPositions)
                    {
                        Debug.DrawLine(previousPosition, pathPosition, Color.green);
                        previousPosition = pathPosition;
                    }
                }

                //draw the nodes in Open and Closed Sets
                //if (this.aStarPathFinding != null)
                //{
                //    Gizmos.color = Color.cyan;

                //    if (this.aStarPathFinding.Open != null)
                //    {
                //        foreach (var nodeRecord in this.aStarPathFinding.Open.All())
                //        {
                //            Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                //        }
                //    }

                //    Gizmos.color = Color.blue;

                //    if (this.aStarPathFinding.Closed != null)
                //    {
                //        foreach (var nodeRecord in this.aStarPathFinding.Closed.All())
                //        {
                //            Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                //        }
                //    }
                //}
            }
        }
    }
}

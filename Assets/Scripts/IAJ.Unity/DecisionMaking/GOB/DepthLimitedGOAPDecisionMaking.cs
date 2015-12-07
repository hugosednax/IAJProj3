using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 2;
        public const int MAX_PROCESSED_ACTIONS = 250;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModelDictionary InitialWorldModel { get; set; }
        private List<Action> Actions { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModelDictionary currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Actions = actions;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.PositiveInfinity;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;
            int currentDepth = 0;
            var startTime = Time.realtimeSinceStartup;
            Action[] actions = new Action[MAX_DEPTH];

            while(currentDepth >= 0){
                if (processedActions > MAX_PROCESSED_ACTIONS)
                {
                    InProgress = true;
                    return null;
                }
                float currentValue = Models[currentDepth].CalculateDiscontentment(Goals);
                if(currentDepth >= MAX_DEPTH){
                    if(currentValue < BestDiscontentmentValue){
                        BestDiscontentmentValue = currentValue;
                        BestAction = actions[0];
                        BestDiscontentmentValue = BestDiscontentmentValue;
                        actions.CopyTo(BestActionSequence,0);
                        TotalProcessingTime = Time.realtimeSinceStartup - startTime;
                        
                    }
                    processedActions++;
                    TotalActionCombinationsProcessed++;
                    currentDepth -= 1;
                    continue;
                }
                Action nextAction = Models[currentDepth].GetNextAction();
                
                if (nextAction != null){
                    Models[currentDepth+1] = Models[currentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(Models[currentDepth+1]);
                    actions[currentDepth] = nextAction;
                    currentDepth += 1;
                }
                else
                    currentDepth -= 1;
            }
            InProgress = false;
            return BestAction;
        }
    }
}

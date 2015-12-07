using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModelFEAR : WorldModel
    {
        private object[] Properties { get; set; }
        private object[] Resources { get; set; }
        private float[] GoalValues { get; set; }
        int boars;
        int trees;
        int beds;
        int arrows;
        int chests;

        public WorldModelFEAR(List<Action> actions) : base(actions)
        {
            this.Properties = new object[5];
            this.GoalValues = new float[5];
            PopulateObject(Properties, 0f);
            Populatefloat(GoalValues, 0f);
        }

        public WorldModelFEAR(WorldModelFEAR parent) : base(parent)
        {
            this.Properties = new object[5];
            this.GoalValues = new float[5];
            PopulateProperties();
            Populatefloat(GoalValues, 0f);
        }

        public void InitArrays(int boars, int arrows, int trees, int beds, int chests)
        {
            this.boars = boars;
            this.trees = trees;
            this.beds = beds;
            this.chests = chests;
            this.arrows = arrows;
            this.Resources = new object[boars+arrows+trees+beds+chests];
            PopulateObject(Resources, 0f);
        }

        void PopulateObject(object[] arr, object value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        void PopulateProperties()
        {
            Properties[0] = 0f;
            Properties[1] = 0f;
            Properties[2] = 0f;
            Properties[3] = 0;
            Properties[4] = 0f;
        }

        void Populatefloat(float[] arr, float value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public override object GetProperty(string propertyName)
        {
            if (propertyName == "Energy")
            {
                return this.Properties[0];
            }
            else if (propertyName == "Arrows")
            {
                return this.Properties[1];
            }
            else if (propertyName == "Health")
            {
                return this.Properties[2];
            }
            else if (propertyName == "Money")
            {
                return this.Properties[3];
            }
            else if (propertyName == "Hunger")
            {
                return this.Properties[4];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetProperty(propertyName);
            }
            else for (int i = 0; i < 50; i++)
                {
                    if (this.Resources[i].Equals(propertyName))
                    {
                        return this.Resources[i];
                    }
                }
            return null; 
        }

        public override void SetProperty(string propertyName, object value)
        {
            if (propertyName.Equals("Energy"))
            {
                this.Properties[0] = value;
                return;
            }
            else if (propertyName.Equals("Arrows"))
            {
                this.Properties[1] = value;
                return;
            }
            else if (propertyName.Equals("Health"))
            {
                this.Properties[2] = value;
                return;
            }
            else if (propertyName.Equals("Money"))
            {
                this.Properties[3] = value;
                return;
            }
            else if (propertyName.Equals("Hunger"))
            {
                this.Properties[4] = value;
                return;
            }
            else if (propertyName.Contains("Boar"))
            {
                Debug.Log("test0" + propertyName);
                Debug.Log("test" + (int)( + propertyName[6]));
                this.Resources[0 + propertyName[6]] = value;
                return;
            }
            else if (propertyName.Contains("Tree"))
            {
                Debug.Log("test0" + propertyName);
                Debug.Log("test" + (int)(0 + propertyName[6]));
                this.Resources[boars + propertyName[6]] = value;
                return;
            }
            else if (propertyName.Contains("Bed"))
            {
                Debug.Log("test0" + propertyName);
                Debug.Log("test" + (int)(0 + propertyName[6]));
                this.Resources[boars+trees + propertyName[5]] = value;
                return;
            }
            else if (propertyName.Contains("Chest"))
            {
                Debug.Log("test0" + propertyName);
                Debug.Log("test " + (boars + trees + beds) +" "+ propertyName[7] + " "+ (int)(boars + trees + beds + propertyName[7]));
                this.Resources[boars + trees + beds + propertyName[7]] = value;
                return;
            }
            else if (propertyName.Contains("Arrows"))
            {
                Debug.Log("test0" + propertyName);
                Debug.Log("test" + (int)(0 + propertyName[6]));
                this.Resources[boars + trees + beds + chests + propertyName[8]] = value;
                return;
            }
            return;
        }

        public override float GetGoalValue(string goalName)
        {
            if (goalName.Equals("Survive")) return this.GoalValues[0];
            if (goalName.Equals("Rest")) return this.GoalValues[1];
            if (goalName.Equals("Eat")) return this.GoalValues[2];
            if (goalName.Equals("GetRich")) return this.GoalValues[3];
            if (goalName.Equals("Conquer")) return this.GoalValues[4];
            else return 0;
        }

        public override void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            if (goalName.Equals("Survive")) this.GoalValues[0] = limitedValue;
            else if (goalName.Equals("Rest")) this.GoalValues[1] = limitedValue;
            else if (goalName.Equals("Eat")) this.GoalValues[2] = limitedValue;
            else if (goalName.Equals("GetRich")) this.GoalValues[3] = limitedValue;
            else if (goalName.Equals("Conquer")) this.GoalValues[4] = limitedValue;
        }

        public override WorldModel GenerateChildWorldModel()
        {
            return new WorldModelFEAR(this);
        }

        public override float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public override Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }
    }
}


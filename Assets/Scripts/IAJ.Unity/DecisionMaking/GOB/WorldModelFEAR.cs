using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModelFEAR : WorldModel
    {
        private object[] PropertiesArray { get; set; }
        private object[] Resources { get; set; }
        private float[] GoalValues { get; set; }
        int boars;
        int trees;
        int beds;
        int chests;

        public WorldModelFEAR(List<Action> actions) : base(actions)
        {
            this.PropertiesArray = new object[5];
            this.GoalValues = new float[5];
            Populatefloat(GoalValues, 0f);
            PopulateProperties();
        }

        public WorldModelFEAR(WorldModelFEAR parent) : base(parent)
        {
            this.PropertiesArray = new object[5];
            this.GoalValues = new float[5];
            Populatefloat(GoalValues, 0f);
            PopulateProperties();
            InitArrays(parent.boars, parent.trees, parent.beds, parent.chests);
        }

        public void InitArrays(int boars, int trees, int beds, int chests)
        {
            this.boars = boars;
            this.trees = trees;
            this.beds = beds;
            this.chests = chests;
            this.Resources = new object[boars+trees+beds+chests];
            int count = boars + trees + beds + chests;
            PopulateObject(Resources, true, count);
        }

        void PopulateObject(object[] arr, object value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                arr[i] = value;
            }
        }

        void PopulateProperties()
        {
            PropertiesArray[0] = 0.0f;
            PropertiesArray[1] = 0;
            PropertiesArray[2] = 0;
            PropertiesArray[3] = 0;
            PropertiesArray[4] = 0.0f;
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
            if (propertyName.Equals(Properties.ENERGY))
            {
                return this.PropertiesArray[0];
            }
            else if (propertyName.Equals(Properties.ARROWS))
            {
                return this.PropertiesArray[1];
            }
            else if (propertyName.Equals(Properties.HP))
            {
                return this.PropertiesArray[2];
            }
            else if (propertyName.Equals(Properties.MONEY))
            {
                return this.PropertiesArray[3];
            }
            else if (propertyName.Equals(Properties.HUNGER))
            {
                return this.PropertiesArray[4];
            }
            else if (propertyName.Contains("Boar"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                return this.Resources[0 + cloneNumber];
            }
            else if (propertyName.Contains("Tree"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                return this.Resources[boars + cloneNumber];
            }
            else if (propertyName.Contains("Bed"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                return this.Resources[boars + trees + cloneNumber];
            }
            else if (propertyName.Contains("Chest"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                //Debug.Log("Length : " + Resources.Length);
                //Debug.Log("Wanted: " + boars + trees + beds + cloneNumber);
                return this.Resources[boars + trees + beds + cloneNumber];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetProperty(propertyName);
            }
            return null; 
        }

        public override void SetProperty(string propertyName, object value)
        {
            if (propertyName.Equals("Energy"))
            {
                this.PropertiesArray[0] = value;
                return;
            }
            else if (propertyName.Equals("Arrows"))
            {
                this.PropertiesArray[1] = value;
                return;
            }
            else if (propertyName.Equals("Health"))
            {
                this.PropertiesArray[2] = value;
                return;
            }
            else if (propertyName.Equals("Money"))
            {
                this.PropertiesArray[3] = value;
                return;
            }
            else if (propertyName.Equals("Hunger"))
            {
                this.PropertiesArray[4] = value;
                return;
            }
            else if (propertyName.Contains("Boar"))
            {
                int cloneNumber = getCloneNumber(propertyName);

               //Debug.Log("test0" + propertyName);
               // Debug.Log("test" + cloneNumber);
                this.Resources[0 + cloneNumber] = value;
                return;
            }
            else if (propertyName.Contains("Tree"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                //Debug.Log("test0" + propertyName);
               /// Debug.Log("test" + cloneNumber);
                this.Resources[boars + cloneNumber] = value;
                return;
            }
            else if (propertyName.Contains("Bed"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                //Debug.Log("test0" + propertyName);
                //Debug.Log("test" + cloneNumber);
                this.Resources[boars + trees + cloneNumber] = value;
                return;
            }
            else if (propertyName.Contains("Chest"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                //Debug.Log("test0" + propertyName);
                //Debug.Log("test " + (boars + trees + beds) +" "+ propertyName[7] + " "+ cloneNumber);
                //Debug.Log("Length : " + Resources.Length);
                //Debug.Log("Wanted: " + boars + trees + beds + cloneNumber);
                this.Resources[boars + trees + beds + cloneNumber] = value;
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

        private int getCloneNumber(string propertyName) 
        {
            int cloneNumber = 0;
            if (propertyName.Contains("("))
            {
                char delimeter1 = '(';
                char delimeter2 = ')';

                string[] splits = propertyName.Split(delimeter1);
                cloneNumber = Int32.Parse(splits[1].Split(delimeter2)[0]);
            }
            return cloneNumber;
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


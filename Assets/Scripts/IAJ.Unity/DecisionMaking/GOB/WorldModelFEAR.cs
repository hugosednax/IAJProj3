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
        public const float nullGoal = -999.0f;
        public WorldModelFEAR(List<Action> actions) : base(actions)
        {
            this.PropertiesArray = new object[6];
            this.GoalValues = new float[5];
            Populatefloat(GoalValues, nullGoal);
            PopulateProperties();
        }

        public WorldModelFEAR(WorldModelFEAR parent) : base(parent)
        {
            this.PropertiesArray = new object[6];
            parent.PropertiesArray.CopyTo(this.PropertiesArray, 0);
            
            this.GoalValues = new float[5];
            parent.GoalValues.CopyTo(this.GoalValues, 0);

            this.Resources = new object[parent.Resources.Length];
            parent.Resources.CopyTo(this.Resources, 0);
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
                arr[i] = null;
            }
        }

        void PopulateProperties()
        {
            PropertiesArray[0] = null;
            PropertiesArray[1] = null;
            PropertiesArray[2] = null;
            PropertiesArray[3] = null;
            PropertiesArray[4] = null;
            PropertiesArray[5] = null;
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
                if (this.PropertiesArray[0] != null)
                {
                    return this.PropertiesArray[0];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Equals(Properties.ARROWS))
            {
                if (this.PropertiesArray[1] != null)
                {
                    return this.PropertiesArray[1];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Equals(Properties.HP))
            {
                if (this.PropertiesArray[2] != null)
                {
                    return this.PropertiesArray[2];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Equals(Properties.MONEY))
            {
                if (this.PropertiesArray[3] != null)
                {
                    return this.PropertiesArray[3];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Equals(Properties.HUNGER))
            {
                if (this.PropertiesArray[4] != null)
                {
                    return this.PropertiesArray[4];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Equals(Properties.POSITION))
            {
                if (this.PropertiesArray[5] != null)
                {
                    return this.PropertiesArray[5];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Contains("Boar"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                if (this.Resources[0+cloneNumber] != null)
                {
                    return this.Resources[0 + cloneNumber];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Contains("Tree"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                if (this.Resources[boars + cloneNumber] != null)
                {
                    return this.Resources[boars + cloneNumber];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Contains("Bed"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                if (this.Resources[boars + trees + cloneNumber] != null)
                {
                    return this.Resources[boars + trees + cloneNumber];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            else if (propertyName.Contains("Chest"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                if (this.Resources[boars + trees + beds + cloneNumber] != null)
                {
                    return this.Resources[boars + trees + beds + cloneNumber];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetProperty(propertyName);
                }
                else
                {
                    return null;
                }
            }
            Debug.Log("Property name: " + propertyName);
            throw new Exception();
        }

        public override void SetProperty(string propertyName, object value)
        {
            if (propertyName.Equals(Properties.ENERGY))
            {
                this.PropertiesArray[0] = value;
                return;
            }
            else if (propertyName.Equals(Properties.ARROWS))
            {
                this.PropertiesArray[1] = value;
                return;
            }
            else if (propertyName.Equals(Properties.HP))
            {
                this.PropertiesArray[2] = value;
                return;
            }
            else if (propertyName.Equals(Properties.MONEY))
            {
                this.PropertiesArray[3] = value;
                return;
            }
            else if (propertyName.Equals(Properties.HUNGER))
            {
                this.PropertiesArray[4] = value;
                return;
            }
            else if (propertyName.Equals(Properties.POSITION))
            {
                this.PropertiesArray[5] = value;
                return;
            }
            else if (propertyName.Contains("Boar"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                this.Resources[0 + cloneNumber] = value;
                return;
            }
            else if (propertyName.Contains("Tree"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                this.Resources[boars + cloneNumber] = value;
                return;
            }
            else if (propertyName.Contains("Bed"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                this.Resources[boars + trees + cloneNumber] = value;
                return;
            }
            else if (propertyName.Contains("Chest"))
            {
                int cloneNumber = getCloneNumber(propertyName);
                this.Resources[boars + trees + beds + cloneNumber] = value;
                return;
            }
            Debug.Log("Property name: " + propertyName);
            throw new Exception();
        }

        public override float GetGoalValue(string goalName)
        {
            if (goalName.Equals(AutonomousCharacter.SURVIVE_GOAL))
            {
                if (this.GoalValues[0] != -999.0f)
                {
                    return this.GoalValues[0];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetGoalValue(goalName);
                }
                else
                {
                    return 0;
                }
            }
            else if (goalName.Equals(AutonomousCharacter.REST_GOAL))
            {
                if (this.GoalValues[1] != -999.0f)
                {
                    return this.GoalValues[1];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetGoalValue(goalName);
                }
                else
                {
                    return 0;
                }
            }
            else if (goalName.Equals(AutonomousCharacter.EAT_GOAL))
            {
                if (this.GoalValues[2] != -999.0f)
                {
                    return this.GoalValues[2];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetGoalValue(goalName);
                }
                else
                {
                    return 0;
                }
            }
            else if (goalName.Equals(AutonomousCharacter.GET_RICH_GOAL))
            {
                if (this.GoalValues[3] != -999.0f)
                {
                    return this.GoalValues[3];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetGoalValue(goalName);
                }
                else
                {
                    return 0;
                }
            }
            else if (goalName.Equals(AutonomousCharacter.CONQUER_GOAL))
            {
                if (this.GoalValues[4] != -999.0f)
                {
                    return this.GoalValues[4];
                }
                else if (this.Parent != null)
                {
                    return this.Parent.GetGoalValue(goalName);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                Debug.Log("Goal name: " + goalName);
                throw new Exception();
            }
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

            if (goalName.Equals(AutonomousCharacter.SURVIVE_GOAL)) this.GoalValues[0] = limitedValue;
            else if (goalName.Equals(AutonomousCharacter.REST_GOAL)) this.GoalValues[1] = limitedValue;
            else if (goalName.Equals(AutonomousCharacter.EAT_GOAL)) this.GoalValues[2] = limitedValue;
            else if (goalName.Equals(AutonomousCharacter.GET_RICH_GOAL)) this.GoalValues[3] = limitedValue;
            else if (goalName.Equals(AutonomousCharacter.CONQUER_GOAL)) this.GoalValues[4] = limitedValue;
            else
            {
                Debug.Log("Goal name: " + goalName);
                throw new Exception();
            }
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


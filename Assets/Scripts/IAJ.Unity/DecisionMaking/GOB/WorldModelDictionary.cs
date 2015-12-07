using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModelDictionary : WorldModel
    {
        private Dictionary<string, object> Properties { get; set; }
        private Dictionary<string, float> GoalValues { get; set; }

        public WorldModelDictionary(List<Action> actions) : base(actions)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
        }

        public WorldModelDictionary(WorldModel parent) : base(parent)
        {
            this.Properties = new Dictionary<string, object>();
            this.GoalValues = new Dictionary<string, float>();
        }

        public override object GetProperty(string propertyName)
        {
            //recursive implementation of WorldModel
            if (this.Properties.ContainsKey(propertyName))
            {
                return this.Properties[propertyName];
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

        public override void SetProperty(string propertyName, object value)
        {
            this.Properties[propertyName] = value;
        }

        public override float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.GoalValues.ContainsKey(goalName))
            {
                return this.GoalValues[goalName];
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

            this.GoalValues[goalName] = limitedValue;
        }

        public override WorldModel GenerateChildWorldModel()
        {
            return new WorldModelDictionary(this);
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

using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModelFEAR : WorldModel
    {
        private string[] ResourceID { get; set; }
        private object[] Properties { get; set; }
        private object[] Resources { get; set; }
        private float[] GoalValues { get; set; }
        //private Dictionary<string, object> Properties { get; set; }
        //private Dictionary<string, float> GoalValues { get; set; }

        public WorldModelFEAR(List<Action> actions) : base(actions)
        {
            this.ResourceID = new string[50];
            this.Properties = new object[5];
            this.Resources = new object[50];
            this.GoalValues = new float[5];
            //this.Properties = new Dictionary<string, object>();
            //this.GoalValues = new Dictionary<string, float>();
        }

        public WorldModelFEAR(WorldModelFEAR parent) : base(parent)
        {
            //this.Properties = new Dictionary<string, object>();
            //this.GoalValues = new Dictionary<string, float>();
            this.ResourceID = new string[50];
            this.Properties = new object[5];
            this.Resources = new object[5];
            this.GoalValues = new float[5];
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
                    if (this.ResourceID[i].Equals(propertyName))
                    {
                        return this.Resources[i];
                    }
                }
            return null; 
        }

        public override void SetProperty(string propertyName, object value)
        {
            if (propertyName == "Energy")
            {
                this.Properties[0] = value;
                return;
            }
            else if (propertyName == "Arrows")
            {
                this.Properties[1] = value;
                return;
            }
            else if (propertyName == "Health")
            {
                this.Properties[2] = value;
                return;
            }
            else if (propertyName == "Money")
            {
                this.Properties[3] = value;
                return;
            }
            else if (propertyName == "Hunger")
            {
                this.Properties[4] = value;
                return;
            }
            else for (int i = 0; i < 50; i++)
                {
                    if (this.ResourceID[i] == null)
                    {
                        this.Resources[i] = value;
                        this.ResourceID[i] = propertyName;
                        break;
                    }

                    else if (this.ResourceID[i].Equals(propertyName))
                    {
                        this.Resources[i] = value;
                        break;
                    }
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
            if (goalName.Equals("Rest")) this.GoalValues[1] = limitedValue;
            if (goalName.Equals("Eat")) this.GoalValues[2] = limitedValue;
            if (goalName.Equals("GetRich")) this.GoalValues[3] = limitedValue;
            if (goalName.Equals("Conquer")) this.GoalValues[4] = limitedValue;
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


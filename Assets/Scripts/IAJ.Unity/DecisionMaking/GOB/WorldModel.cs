﻿using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public abstract class WorldModel
    {
        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }

        protected WorldModel Parent { get; set; }

        public virtual void Initialize()
        {
        }

        public WorldModel(List<Action> actions)
        {
            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.Actions = parent.Actions;
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public abstract object GetProperty(string propertyName);
        public abstract void SetProperty(string propertyName, object value);
        public abstract float GetGoalValue(string goalName);
        public abstract void SetGoalValue(string goalName, float value);
        public abstract WorldModel GenerateChildWorldModel();
        public virtual float CalculateDiscontentment(List<Goal> goals)
        {
            return 0.0f;
        }
        public virtual Action GetNextAction()
        {
            return null;
        }
    }
}

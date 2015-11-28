using System;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PlaceFlag : Action
    {
        protected AutonomousCharacter Character { get; set; }

        public PlaceFlag(AutonomousCharacter character) : base("PlaceFlag")
        {
            this.Character = character;
        }

        public override float GetDuration()
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override float GetDuration(WorldModel worldModel)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override float GetGoalChange(Goal goal)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override bool CanExecute()
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public override void Execute()
        {

            //TODO: implement
            throw new NotImplementedException();
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
    }
}

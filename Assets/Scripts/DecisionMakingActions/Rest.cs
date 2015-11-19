using System;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Rest : Action
    {
        private AutonomousCharacter Character{ get; set; }

        public Rest(AutonomousCharacter character) : base("Rest")
        {
            this.Character = character;
            this.Duration = 0.5f;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.REST_GOAL) change -= 0.1f;
            return change;
        }

        public override bool CanExecute()
        {
            return this.Character.GameManager.characterData.Energy < 9.0f;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return (float)worldModel.GetProperty(Properties.ENERGY) < 9.0f;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var restValue = worldModel.GetGoalValue(AutonomousCharacter.REST_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.REST_GOAL, restValue - 1.0f);
            var survival = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetProperty(AutonomousCharacter.SURVIVE_GOAL, survival + 1.0f);
            var eat = worldModel.GetGoalValue(AutonomousCharacter.EAT_GOAL);
            worldModel.SetProperty(AutonomousCharacter.EAT_GOAL, eat + 0.2f);
        }

        public override void Execute()
        {
            this.Character.GameManager.Rest();
        }
    }
}

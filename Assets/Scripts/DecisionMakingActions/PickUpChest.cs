using System;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {

        public PickUpChest(AutonomousCharacter character, GameObject target) : base("PickUpChest",character,target)
        {
        }


        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.EAT_GOAL) return +1.0f;
            else if (goal.Name == AutonomousCharacter.REST_GOAL)
            {
                var distance =
                    (this.Target.transform.position - this.Character.Character.KinematicData.position).magnitude;
                //0.5 for the attack action and +0.01 * distance because of the walk 
                return 0.5f + distance * 0.01f;
            }
            else if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) return 0.0f;

            return 0;
        }

        public override bool CanExecute()
        {
            return base.CanExecute();
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return base.CanExecute(worldModel);
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var restValue = worldModel.GetGoalValue(AutonomousCharacter.REST_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.REST_GOAL, restValue + 0.5f);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + 0.5f);
            var money = (int)worldModel.GetProperty(Properties.MONEY);
            worldModel.SetProperty(Properties.MONEY, money + 5);
            var richGoal = (float)worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL, richGoal - 2);

            worldModel.SetProperty(this.Target.name, false);
        }
    }
}

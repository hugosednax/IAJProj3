using System;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetArrows : WalkToTargetAndExecuteAction
    {
        public GetArrows(AutonomousCharacter character, GameObject target) : base("GetArrows",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Energy > 3.0f &&
                this.Character.GameManager.characterData.Arrows < 3;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute()) return false;
            return (float)worldModel.GetProperty(Properties.ENERGY) > 3.0f &&
                (int)worldModel.GetProperty(Properties.ARROWS) < 3;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetArrows(this.Target);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - 0.5f);
            worldModel.SetProperty(Properties.ARROWS, 10);

            worldModel.SetProperty(this.Target.name, false);
        }


    }
}

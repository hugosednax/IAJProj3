using System;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures;
using UnityEngine;

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
            return 0.5f;
        }

        public override float GetDuration(WorldModel worldModel)
        {
            return 0.5f;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.CONQUER_GOAL) return - 2f;
            return change;
        }

        public override bool CanExecute()
        {
            return this.Character.GameManager.characterData.Energy > 3.0f && Character.CombinedInfluence[Character.BestFlagLocationRecord] > 2.0f;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            /*NOTE: I added the variable BestFlagLocationRecord because
            its easier then iterating through the list to find the location record that matches the BestFlagPosition
            */
            return (float)worldModel.GetProperty(Properties.ENERGY) > 3.0f && Character.CombinedInfluence[Character.BestFlagLocationRecord] > 2.0f;
        }

        public override void Execute()
        {
            this.Character.Targeter.Target.Position = Character.BestFlagPosition;
            this.Character.GameManager.PlaceFlag(Character.BestFlagPosition);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var conquerGoal = worldModel.GetGoalValue(AutonomousCharacter.CONQUER_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.CONQUER_GOAL, conquerGoal - 2.0f);
            var energy = (float)worldModel.GetProperty(Properties.ENERGY);
            worldModel.SetProperty(Properties.ENERGY, energy + 2);
        }
    }
}

using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis
{
    public class LinearInfluenceFunction : IInfluenceFunction
    {
        public float DetermineInfluence(IInfluenceUnit unit, Vector3 location)
        {
            float I0 = unit.DirectInfluence;
            float d = (unit.Location.Position - location).magnitude;
            return I0 / (1 + d);
        }
    }
}

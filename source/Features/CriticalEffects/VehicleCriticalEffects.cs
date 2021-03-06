﻿using CustomComponents;

namespace MechEngineer.Features.CriticalEffects
{
    [CustomComponent("VehicleCriticalEffects")]
    public class VehicleCriticalEffects : CriticalEffects
    {
        public override string GetActorTypeDescription()
        {
            return "Vehicle";
        }
    }
}
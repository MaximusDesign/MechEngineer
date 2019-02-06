﻿using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineSearcher
    {
        internal static Result SearchInventory(IEnumerable<MechComponentRef> componentRefs)
        {
            var result = new Result();
            foreach (var componentRef in componentRefs)
            {
                var componentDef = componentRef.Def;

                var engineHeatSinkDef = componentDef.GetComponent<EngineHeatSinkDef>();
                if (engineHeatSinkDef != null)
                {
                    result.HeatSinks.Add(componentRef);
                    continue;
                }

                if (componentDef.Is<CoolingDef>(out var coolingDef))
                {
                    result.CoolingDef = coolingDef;
                    continue;
                }

                if (componentDef.Is<Weights>(out var weightSavings))
                {
                    result.Weights.Combine(weightSavings);
                }
                
                if (!componentDef.IsEnginePart())
                {
                    continue;
                }

                result.Parts.Add(componentRef);

                if (componentDef.Is<EngineCoreDef>(out var coreDef))
                {
                    result.CoreDef = coreDef;
                }

                if (componentDef.Is<EngineHeatBlockDef>(out var blockDef))
                {
                    result.HeatBlockDef = blockDef;
                }
            }

            return result;
        }

        internal class Result
        {
            internal CoolingDef CoolingDef;
            internal EngineHeatBlockDef HeatBlockDef;
            internal EngineCoreDef CoreDef;
            internal List<MechComponentRef> Parts = new List<MechComponentRef>();
            internal Weights Weights = new Weights();
            internal List<MechComponentRef> HeatSinks = new List<MechComponentRef>();
        }
    }
}
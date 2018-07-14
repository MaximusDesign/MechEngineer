﻿using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal static class ValidationFacade
    {
        internal static void ValidateMech(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            EngineCoreRefHandler.Shared.ValidateMech(mechDef, errorMessages);
            EngineCoreDefHandler.Shared.ValidateMech(mechDef, errorMessages);
            EngineTypeHandler.Shared.ValidateMech(mechDef, errorMessages);
 //           DynamicSlotHandler.Shared.ValidateMech(mechDef, errorMessages);
        }

        internal static MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            var validators = new IValidateDrop[]
            {
                EngineHeat.Shared,
                //DynamicSlotHandler.Shared // we dont want the drop check, we allow over use of inventory until save (similar to overtonnage)
            };

            foreach (var validator in validators)
            {
                var result = validator.ValidateDrop(dragItem, widget);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
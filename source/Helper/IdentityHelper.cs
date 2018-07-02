﻿using System;
using BattleTech;

namespace MechEngineer
{
    internal class IdentityHelper : IIdentifier
    {
        public string Prefix { get; set; }
        public ChassisLocations AllowedLocations { get; set; }
        public ComponentType ComponentType { get; set; }

        public bool IsCustomType(MechComponentDef def)
        {
            if (ComponentType != ComponentType.NotSet && def.ComponentType != ComponentType)
            {
                return false;
            }

            if (AllowedLocations != ChassisLocations.None)
            {
                if (!OnlyAllowedIn(def, AllowedLocations))
                {
                    return false;
                }
            }

            if (Prefix != null && !def.Description.Id.StartsWith(Prefix))
            {
                return false;
            }

            return true;
        }

        private static bool OnlyAllowedIn(MechComponentDef componentDef, ChassisLocations locations)
        {
            return (componentDef.AllowedLocations & locations) != 0 // def can be inserted in locations
                   && (componentDef.AllowedLocations & ~locations) == 0; // def can't be inserted anywhere outside of locations
        }
    }

    internal class IdentityFuncHelper : IIdentifier
    {
        private readonly Func<MechComponentDef, bool> func;

        public IdentityFuncHelper(Func<MechComponentDef, bool> func)
        {
            this.func = func;
        }

        public bool IsCustomType(MechComponentDef def)
        {
            return func(def);
        }
    }
}
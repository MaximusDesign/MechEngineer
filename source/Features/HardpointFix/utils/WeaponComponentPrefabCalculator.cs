﻿using System.Collections.Generic;
using System.Linq;
using BattleTech;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.HardpointFix.utils
{
    internal class WeaponComponentPrefabCalculator
    {
        private readonly ChassisDef chassisDef;
        private readonly IDictionary<MechComponentRef, string> mapping = new Dictionary<MechComponentRef, string>();

        internal WeaponComponentPrefabCalculator(ChassisDef chassisDef, List<MechComponentRef> componentRefs, ChassisLocations location = ChassisLocations.All)
        {
            this.chassisDef = chassisDef;
            componentRefs = componentRefs
                .OrderByDescending(c => ((WeaponDef) c.Def).InventorySize)
                .ThenByDescending(c => ((WeaponDef) c.Def).Tonnage)
                .ToList();

            if (!MechDefBuilder.Locations.Contains(location))
            {
                location = ChassisLocations.All;
            }

            if (location == ChassisLocations.All)
            {
                foreach (var tlocation in MechDefBuilder.Locations)
                {
                    var locationComponentRefs = componentRefs.Where(c => c.MountedLocation == tlocation).ToList();
                    CalculateMappingForLocation(tlocation, locationComponentRefs);
                }
            }
            else
            {
                CalculateMappingForLocation(location, componentRefs);
            }
        }

        internal int NotMappedPrefabNameCount { private set; get; }

        internal string GetPrefabName(MechComponentRef componentRef)
        {
            return mapping.TryGetValue(componentRef, out var value) ? value : null;
        }

        internal string GetNewPrefabName(MechComponentRef componentRef, ChassisLocations location)
        {
            var availablePrefabNames = GetAvailablePrefabNamesForLocation(location);
            return GetAvailableWeaponComponentPrefabName(componentRef.Def.PrefabIdentifier.ToLower(), availablePrefabNames);
        }

        private void CalculateMappingForLocation(ChassisLocations location, List<MechComponentRef> locationComponentRefs)
        {
            foreach (var componentRef in locationComponentRefs)
            {
                var prefabName = GetNewPrefabName(componentRef, location);
                if (prefabName == null)
                {
                    //Control.mod.Logger.LogDebug("could not find prefabName for " + (componentRef != null ? (componentRef.Def != null ? componentRef.Def.PrefabIdentifier : null) : null));
                    NotMappedPrefabNameCount++;
                    continue;
                }

                mapping[componentRef] = prefabName;
            }
        }
        
        private static string GetAvailableWeaponComponentPrefabName(string prefabId, List<string> availablePrefabNames)
        {
            var compatibleTerms = Control.settings.HardpointFix.WeaponPrefabMappings
                .Where(x => x.PrefabIdentifier == prefabId)
                .Select(x => x.HardpointCandidates)
                .SingleOrDefault();

            if (compatibleTerms == null)
            {
                compatibleTerms = new[] {prefabId.ToLowerInvariant()};
            }

            var prefabName = compatibleTerms.Select(t => availablePrefabNames.FirstOrDefault(n => n.Contains("_" + t + "_"))).FirstOrDefault(n => n != null);
            //Control.mod.Logger.LogDebug("found prefabName " + prefabName);
            return prefabName;
        }

        private List<string> GetAvailablePrefabNamesForLocation(ChassisLocations location)
        {
            var hardpointDatas = chassisDef.HardpointDataDef.HardpointData
                .Where(x => x.location == VHLUtils.GetStringFromLocation(location));

            // SelectMany with string[][] crashes if compiled with Mono.CSharp (2.1.0.0) for Unity, newer Mono and VS compilers are fine
            var hpsets = new List<string[]>();
            foreach (var hardpointData in hardpointDatas)
            {
                foreach (var hpset in hardpointData.weapons)
                {
                    hpsets.Add(hpset);
                }
            }

            return hpsets
                .Where(hpset => !hpset.Intersect(mapping.Values).Any()) // only include hardpoint sets not yet used
                // commented out code not required, instead of filling the least used, we get the complete weapons list and sort by weapon sizes and fill from nicest index to ugliest index
                //.Select(hpset => RemoveUnwantedHardpoints(location, hpset)) // that allows our length order to work better
                //.OrderBy(hpset => hpset.Length) // sort hardpoints by how many weapon types are supported (use up the ones with less options first) - no -> use index order, lower index = nicer looking
                .SelectMany(hpset => hpset) // we don't care about groups anymore, just flatten everything into one stream
                .ToList();
        }
    }
}
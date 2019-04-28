﻿using System;
using BattleTech;
using Harmony;
using MechEngineer.Features.Engines.StaticHandler;

namespace MechEngineer.Features.Engines.Patches
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats2_Patch
    {
        // change the movement stats when loading into a combat game the first time
        public static void Postfix(Mech __instance)
        {
            try
            {
                EngineMisc.InitEffectStats(__instance);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
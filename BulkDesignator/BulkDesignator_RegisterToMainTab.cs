using ModButtons;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BulkDesignator
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.github.harmony.rimworld.maarx.bulkdesignator");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(MainTabWindow_ModButtons))]
    [HarmonyPatch("DoWindowContents")]
    class Patch_MainTabWindow_ModButtons_DoWindowContents
    {
        static void Prefix(MainTabWindow_ModButtons __instance, ref Rect canvas)
        {
            BulkDesignator_RegisterToMainTab.ensureMainTabRegistered();
        }
    }

    class BulkDesignator_RegisterToMainTab
    {
        public static bool wasRegistered = false;

        public static void ensureMainTabRegistered()
        {
            if (wasRegistered) { return; }

            Log.Message("Hello from BulkDesignator_RegisterToMainTab ensureMainTabRegistered");

            List<List<ModButton_Text>> columns = MainTabWindow_ModButtons.columns;

            List<ModButton_Text> buttons = new List<ModButton_Text>();

            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Cancel All Hunting";
                },
                delegate {
                    Find.CurrentMap.designationManager.RemoveAllDesignationsOfDef(DesignationDefOf.Hunt);
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Cancel All Cut/Harvest";
                },
                delegate {
                    Find.CurrentMap.designationManager.RemoveAllDesignationsOfDef(DesignationDefOf.CutPlant);
                    Find.CurrentMap.designationManager.RemoveAllDesignationsOfDef(DesignationDefOf.HarvestPlant);
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Cancel All Smooth Wall";
                },
                delegate {
                    Find.CurrentMap.designationManager.RemoveAllDesignationsOfDef(DesignationDefOf.SmoothWall);
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Cancel All Smooth Floor";
                },
                delegate {
                    Find.CurrentMap.designationManager.RemoveAllDesignationsOfDef(DesignationDefOf.SmoothFloor);
                }
            ));
            buttons.Add(new ModButton_Text(
                delegate
                {
                    return "Cancel All Remove Floor";
                },
                delegate {
                    Find.CurrentMap.designationManager.RemoveAllDesignationsOfDef(DesignationDefOf.RemoveFloor);
                }
            ));

            columns.Add(buttons);

            wasRegistered = true;
        }
    }
}

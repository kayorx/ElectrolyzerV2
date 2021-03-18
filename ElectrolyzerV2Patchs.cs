using System;
using System.Collections.Generic;
using Database;
using Harmony;
using STRINGS;

namespace ElectrolyzerV2
{
    public class ElectrolyzerV2Patchs
    {

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]  


        public static class Mod_OnLoad
        {


            public static LocString NAME = new LocString(STRINGS.UI.FormatAsLink("Electrolyzer V2", "Electrolyzer V2"),
                                                "STRINGS.BUILDINGS.PREFABS." + ElectrolyzerV2Config.ID.ToUpper() + ".NAME");

            public static LocString DESC = new LocString("A better " + STRINGS.UI.FormatAsLink("Electrolyzer", "Electrolyzer") + " for your colony.", "STRINGS.BUILDINGS.PREFABS." + ElectrolyzerV2Config.ID.ToUpper() + ".DESC");

            public static LocString EFFECT = new LocString("Consumes " + STRINGS.UI.FormatAsLink("Water", "Water") + " and " + STRINGS.UI.FormatAsLink("Carbon Dioxide", "Carbon Dioxide") + " to turn into " + STRINGS.UI.FormatAsLink("Oxygen", "Oxygen") + ". Emites a bit of " + STRINGS.UI.FormatAsLink("Hydrogen", "Hydrogen"), "STRINGS.BUILDINGS.PREFABS." + ElectrolyzerV2Config.ID.ToUpper() + ".EFFECT");


            public static void OnLoad()
            {
                Debug.Log("The mod was loaded successfully!");
            }


            [HarmonyPatch(typeof(GeneratedBuildings))]
            [HarmonyPatch("LoadGeneratedBuildings")]
            public static class BEPatch
            {

                public static void Prefix()
                {
                    Strings.Add(NAME.key.String, NAME.text);
                    Strings.Add(DESC.key.String, DESC.text);
                    Strings.Add(EFFECT.key.String, EFFECT.text);
                    ModUtil.AddBuildingToPlanScreen("HVAC", ElectrolyzerV2Config.ID);
                }


                private static void PostFix()
                {
                    object obj = Activator.CreateInstance(typeof(ElectrolyzerV2Config));
                    BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
                }



            }





        }

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                List<String> ls = new List<string>(Techs.TECH_GROUPING["ImprovedGasPiping"]) { ElectrolyzerV2Config.ID };
                Techs.TECH_GROUPING["ImprovedGasPiping"] = ls.ToArray();
            }
        }
    }
}

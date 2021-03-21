using System.Collections.Generic;
using UnityEngine;
using TUNING;
using Harmony;
using STRINGS;



namespace ElectrolyzerV2
{
	public class ElectrolyzerV2Patchs
	{
		public static void RegStrings(string id, string name, string description, string effect)
		{
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, id));
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.DESC", description);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.EFFECT", effect);
		}



		public static class Mod_OnLoad
		{
			public static void OnLoad()
			{
			}
		}

		[HarmonyPatch(typeof(GeneratedBuildings))]
		[HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
		public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
		{
			public static void Prefix()
			{
				RegStrings(ElectrolyzerV2Config.ID, ElectrolyzerV2Config.Name, ElectrolyzerV2Config.Description, ElectrolyzerV2Config.Effect);
				ModUtil.AddBuildingToPlanScreen("HVAC", ElectrolyzerV2Config.ID);
			}
		}

		[HarmonyPatch(typeof(Db))]
		[HarmonyPatch("Initialize")]
		public static class Db_Initialize_Patch
		{
			public static void Prefix()
			{
				/*List<String> ls = new List<string>(Techs.TECH_GROUPING["ImprovedGasPiping"]) { ElectrolyzerV2Config.ID };
				Techs.TECH_GROUPING["ImprovedGasPiping"] = ls.ToArray();*/
			}
		}

		[HarmonyPatch(typeof(Database.Techs))]
		[HarmonyPatch("Init")]
		public static class Techs_Init_Patch
		{
			public static void Postfix(Database.Techs __instance)
			{
				Tech tech = __instance.TryGet("ImprovedGasPiping");
				tech.unlockedItemIDs.Add(ElectrolyzerV2Config.ID);
			}
		}
	}
}
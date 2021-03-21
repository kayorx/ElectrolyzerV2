using System.Collections.Generic;
using TUNING;
using UnityEngine;
using STRINGS;
using BUILDINGS = TUNING.BUILDINGS;

namespace ElectrolyzerV2
{
	class ElectrolyzerV2Config : IBuildingConfig
	{

		public static string ID = "ElectrolyzerV2";
		public static string Name = "Electrolyzer V2";
		public static string Description = "A better Electrolyzer for your colony.";
		public static string Effect = "Consumes " + UI.FormatAsLink("Water", "Water") + " and " + UI.FormatAsLink("Carbon Dioxide", "Carbon Dioxide") + " to turn into " + UI.FormatAsLink("Oxygen", "Oxygen") + " Emits a bit of " + UI.FormatAsLink("Hydrogen", "Hydrogen") + ".";


		public override BuildingDef CreateBuildingDef()
		{


			string id = "ElectrolyzerV2";
			int width = 2;
			int height = 2;
			string anim = "belectrolyzer_kanim";
			int hitpoints = 30;
			float construction_time = 60f;
			float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
			string[] all_METALS = MATERIALS.ALL_METALS;
			float melting_point = 800f;
			BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
			EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, all_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.03f);
			buildingDef.RequiresPowerInput = true;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.EnergyConsumptionWhenActive = 192f;
			buildingDef.ExhaustKilowattsWhenActive = 0.25f;
			buildingDef.SelfHeatKilowattsWhenActive = 1f;
			buildingDef.ViewMode = OverlayModes.Oxygen.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 1));
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			Tag tag2 = SimHashes.CarbonDioxide.CreateTag();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
			Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
			electrolyzer.maxMass = 1.8f;
			electrolyzer.hasMeter = true;			
			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 3f;
			conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
			Storage storage = go.AddOrGet<Storage>();
			storage.capacityKg = 200f;
			storage.showInUI = true;
			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.invertElementFilter = true;
			conduitDispenser.elementFilter = new SimHashes[]
			{
			SimHashes.Water
			};
			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
			{
			new ElementConverter.ConsumedElement(new Tag("Water"), 1f)			
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[]
			{
			new ElementConverter.OutputElement(1.10000f, SimHashes.Oxygen, 343.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.05500f, SimHashes.Hydrogen, 343.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.30000f, SimHashes.DirtyWater, 0f, false, true, 0f, 0.5f, 1f, byte.MaxValue, 0)
			};

			PassiveElementConsumer passiveElementConsumer = go.AddComponent<PassiveElementConsumer>();
			passiveElementConsumer.elementToConsume = SimHashes.CarbonDioxide;
			passiveElementConsumer.consumptionRate = 2f;
			passiveElementConsumer.consumptionRadius = 6;
			passiveElementConsumer.showInStatusPanel = true;
			passiveElementConsumer.storeOnConsume = false;
			passiveElementConsumer.capacityKG = 4000f;
			passiveElementConsumer.isRequired = false;
			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			go.AddOrGet<AnimTileable>();
			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}



		public const float WATER2OXYGEN_RATIO = 2.5f;

		public const float OXYGEN_TEMPERATURE = 343.15f;

		private static readonly List<Storage.StoredItemModifier> PollutedWaterStorageModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal
	};

	}
}

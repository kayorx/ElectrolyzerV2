using System.Collections.Generic;
using TUNING;
using UnityEngine;
using STRINGS;
using BUILDINGS = TUNING.BUILDINGS;

namespace ElectrolyzerV2
{
	class ElectrolyzerV2Config : IBuildingConfig
	{

		public const string ID = "ElectrolyzerV2";		

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
			buildingDef.RequiresPowerInput = false;
			buildingDef.PowerInputOffset = new CellOffset(1, 0);
			buildingDef.EnergyConsumptionWhenActive = 120f;
			buildingDef.ExhaustKilowattsWhenActive = 0.09f;
			buildingDef.SelfHeatKilowattsWhenActive = 0.07f;
			buildingDef.ViewMode = OverlayModes.Oxygen.ID;
			buildingDef.AudioCategory = "HollowMetal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
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
			ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
			{
			new ElementConverter.ConsumedElement(new Tag("Water"), 1f)
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[]
			{
			new ElementConverter.OutputElement(1.10000f, SimHashes.Oxygen, 343.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.05500f, SimHashes.Hydrogen, 343.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0)
			};

			//Consume Carbon Dioxide
			ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
			elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
			elementConsumer.consumptionRate = 2f;
			elementConsumer.consumptionRadius = 6;
			elementConsumer.showInStatusPanel = true;
			elementConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
			elementConsumer.isRequired = false;

			Prioritizable.AddRef(go);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
		}



		public const float WATER2OXYGEN_RATIO = 2.5f;

		public const float OXYGEN_TEMPERATURE = 343.15f;

	}
}

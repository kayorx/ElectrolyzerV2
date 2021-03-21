using System;
using KSerialization;
using UnityEngine;



namespace ElectrolyzerV2

{


	[SerializationConfig(KSerialization.MemberSerialization.OptIn)]
	public class ElectrolyzerV2 : StateMachineComponent<ElectrolyzerV2.StatesInstance>
	{

		protected override void OnSpawn()
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			if (this.hasMeter)
			{
				this.meter = new MeterController(component, "U2H_meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(-0.4f, 0.5f, -0.1f), new string[]
				{
				"U2H_meter_target",
				"U2H_meter_tank",
				"U2H_meter_waterbody",
				"U2H_meter_level"
				});
			}
			base.smi.StartSM();
			this.UpdateMeter();
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		public void UpdateMeter()
		{
			if (this.hasMeter)
			{
				float positionPercent = Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg);
				this.meter.SetPositionPercent(positionPercent);
			}
		}

		private bool RoomForPressure
		{
			get
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				num = Grid.CellAbove(num);
				return !GameUtil.FloodFillCheck<ElectrolyzerV2>(new Func<int, ElectrolyzerV2, bool>(ElectrolyzerV2.OverPressure), this, num, 3, true, true);
			}
		}


		private static bool OverPressure(int cell, ElectrolyzerV2 ElectrolyzerV2)
		{
			return Grid.Mass[cell] > ElectrolyzerV2.maxMass;
		}


		[SerializeField]
		public float maxMass = 2.5f;


		[SerializeField]
		public bool hasMeter = true;


		[MyCmpAdd]
		private Storage storage;

		[MyCmpGet]
		//private ElementConverter emitter;

		[MyCmpReq]
		private Operational operational;

		private MeterController meter;

		public class StatesInstance : GameStateMachine<ElectrolyzerV2.States, ElectrolyzerV2.StatesInstance, ElectrolyzerV2, object>.GameInstance
		{
			public StatesInstance(ElectrolyzerV2 smi) : base(smi)
			{
			}
		}

		public class States : GameStateMachine<ElectrolyzerV2.States, ElectrolyzerV2.StatesInstance, ElectrolyzerV2>
		{

			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.disabled;
				this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (ElectrolyzerV2.StatesInstance smi) => !smi.master.operational.IsOperational).EventHandler(GameHashes.OnStorageChange, delegate (ElectrolyzerV2.StatesInstance smi)
				{
					smi.master.UpdateMeter();
				});
				this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (ElectrolyzerV2.StatesInstance smi) => smi.master.operational.IsOperational);
				this.waiting.Enter("Waiting", delegate (ElectrolyzerV2.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				}).EventTransition(GameHashes.OnStorageChange, this.converting, (ElectrolyzerV2.StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
				this.converting.Enter("Ready", delegate (ElectrolyzerV2.StatesInstance smi)
				{
					smi.master.operational.SetActive(true, false);
				}).Transition(this.waiting, (ElectrolyzerV2.StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms).Transition(this.overpressure, (ElectrolyzerV2.StatesInstance smi) => !smi.master.RoomForPressure, UpdateRate.SIM_200ms);
				this.overpressure.Enter("OverPressure", delegate (ElectrolyzerV2.StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				}).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, null).Transition(this.converting, (ElectrolyzerV2.StatesInstance smi) => smi.master.RoomForPressure, UpdateRate.SIM_200ms);
			}

			public GameStateMachine<ElectrolyzerV2.States, ElectrolyzerV2.StatesInstance, ElectrolyzerV2, object>.State disabled;

			public GameStateMachine<ElectrolyzerV2.States, ElectrolyzerV2.StatesInstance, ElectrolyzerV2, object>.State waiting;

			public GameStateMachine<ElectrolyzerV2.States, ElectrolyzerV2.StatesInstance, ElectrolyzerV2, object>.State converting;

			public GameStateMachine<ElectrolyzerV2.States, ElectrolyzerV2.StatesInstance, ElectrolyzerV2, object>.State overpressure;
		}
	}
}
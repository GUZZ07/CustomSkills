using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CustomSkills
{
	public struct DelayOperation
	{
		public Action Operation { get; }
		public int Delay
		{
			get;
			private set;
		}
		public bool Ended
		{
			get => Delay == -1;
		}
		public DelayOperation(Action operation,int delay)
		{
			Operation = operation;
			Delay = delay;
		}
		public void Update()
		{
			if (Ended)
			{
				return;
			}
			if (Delay-- == 0)
			{
				Operation();
				Delay = -1;
			}
		}
	}


	public struct ProjShiftOperation
	{
		public Projectile Proj { get; }
		public int Delay { get; private set; }
		public Vector Velocity { get; }
		public bool Ended
		{
			get => Delay == -1;
		}
		public ProjShiftOperation(Projectile proj, Vector velocity, int delay)
		{
			Proj = proj;
			Velocity = velocity;
			Delay = delay;
		}
		public void Update()
		{
			if (Ended)
			{
				return;
			}
			if (Delay-- == 0)
			{
				if (Proj.active)
				{
					Proj.velocity = Velocity;
					Proj.SendData();
				}
				Delay = -1;
			}
		}
	}


	public struct ProjTask
	{
		public Action<Projectile> Operation { get; }
		public Projectile Proj { get; }
		public int Delay { get; private set; }
		public bool Ended
		{
			get => Delay == -1;
		}
		public ProjTask(Projectile proj, Action<Projectile> operation, int delay)
		{
			Proj = proj;
			Operation = operation;
			Delay = delay;
		}
		public void Update()
		{
			if (Ended)
			{
				return;
			}
			if (Delay-- == 0)
			{
				Operation(Proj);
				Delay = -1;
			}
		}
	}
}

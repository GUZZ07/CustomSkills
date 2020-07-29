using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace SkillDesigner.Libs
{
	public class ProjData
	{
		public int CreateDelay
		{
			get;
			set;
		}
		public int LaunchDelay
		{
			get;
			set;
		}
		public int Knockback
		{
			get;
			set;
		}
		public int Damage
		{
			get;
			set;
		}
		public int ProjType
		{
			get;
			set;
		}
		public Vector Position
		{
			get;
			set;
		}
		public double SpeedAngle
		{
			get;
			set;
		}
		public float Speed
		{
			get;
			set;
		}
		public ProjShift[] Shifts = { };
		/// <summary>
		/// 是否随着玩家攻击方向旋转(旋转角为从正右方转向玩家攻击方向)
		/// </summary>
		public bool RotateWithPlayer
		{
			get;
			set;
		}

		public ProjData Clone()
		{
			return new ProjData
			{
				CreateDelay = CreateDelay,
				LaunchDelay = LaunchDelay,
				ProjType = ProjType,
				Damage = Damage,
				Speed = Speed,
				SpeedAngle = SpeedAngle,
				Position = Position,
				Knockback = Knockback
			};
		}
	}


	public struct ProjShift
	{
		public int Delay
		{
			get;
			set;
		}
		public Vector Velocity;
	}
}

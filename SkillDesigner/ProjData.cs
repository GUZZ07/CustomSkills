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
		public float SpeedAngle
		{
			get;
			set;
		}
		public float Speed
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
}

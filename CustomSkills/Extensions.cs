using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CustomSkills
{
	public static class Extensions
	{
		public static void SendData(this Projectile proj, int client = -1, int ignore = -1)
		{
			NetMessage.SendData((int)PacketTypes.ProjectileNew, client, ignore, null, proj.whoAmI);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="player"></param>
		/// <param name="type"></param>
		/// <param name="damage"></param>
		/// <param name="knockback"></param>
		/// <param name="pos">相对位置</param>
		/// <param name="velocity"></param>
		/// <returns></returns>
		public static int NewProj(this Player player, double rotateAngle, int type, int damage, float knockback, Vector pos, Vector velocity)
		{
			pos.Angle += rotateAngle;
			velocity.Angle += rotateAngle;
			int idx = Projectile.NewProjectile((Vector2)pos + player.Center, velocity, type, damage, knockback, player.whoAmI);
			Main.projectile[idx].SendData();
			return idx;
		}
		public static int NewProj(this Player player, double rotateAngle, ProjData data)
		{
			return player.NewProj(rotateAngle, data.ProjType, data.Damage, data.Knockback, data.Position, data.Velocity);
		}


		#region Numbers
		/// <summary>
		/// 检查number是否属于[min, max]
		/// </summary>
		/// <param name="number"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static bool InRange(in this int number, int min, int max)
		{
			return min <= number && number <= max;
		}
		#endregion
	}
}

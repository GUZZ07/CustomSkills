using CustomSkills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace CustomSkills
{
    public class Skill
    {
        public const int MaxSlot = 5;

        public SkillData Data { get; }

        public int ID
        {
            get;
            set;
        }

        public int CD { get; }
        public string Name { get; }
        public string Author { get; }
        public string Description { get; }
        /// <summary>
        /// 完整介绍
        /// </summary>
        public string Introduction { get; }

        public Skill(SkillData data)
        {
            Data = data;
            CD = data.CoolDown;
            Name = data.SkillName;
            Author = data.Author;
            Description = data.SkillDescription;

            Introduction = $@"{Name}
作者:       {Author}
CD:         {CD / 60.0: 0.00}s
技能描述    {Description}:";

            for (int i = 0; i < Data.ProjDatas.Length; i++)
            {
                ref var projData = ref Data.ProjDatas[i];
                projData.Calculate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="rotateAngle">不是玩家攻击角度，是从正右方转向攻击角度的转角</param>
        public void Release(TSPlayer player, double rotateAngle)
        {
            foreach (var data in Data.ProjDatas)
            {
                void apply()
                {
                    var plugin = MainPlugin.Instance;
                    Projectile proj;
                    double angle = rotateAngle;
                    if (!data.RotateWithPlayer)
                    {
                        angle = 0;
                    }
                    if (data.LaunchDelay == 0)
                    {
                        int idx = player.TPlayer.NewProj(angle, data);
                        proj = Main.projectile[idx];
                    }
                    else
                    {
                        int idx = player.TPlayer.NewProj(angle, data.ProjType, data.Damage, data.Knockback, data.Position, default);
                        proj = Main.projectile[idx];
                        var velocity = data.Velocity;
                        velocity.Angle += angle;
                        plugin.Add(new ProjShiftOperation(proj, velocity, data.LaunchDelay));
                    }
                    foreach (var shift in data.Shifts)
                    {
                        var velocity = shift.Velocity;
                        velocity.Angle += angle;
                        plugin.Add(new ProjShiftOperation(proj, velocity, data.LaunchDelay + shift.Delay));
                    }
                }
                if (data.CreateDelay == 0)
                {
                    apply();
                }
                else
                {
                    MainPlugin.Instance.Add(new DelayOperation(apply, data.CreateDelay));
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

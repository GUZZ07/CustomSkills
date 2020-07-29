using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace CustomSkills
{
    public class PlayerInfo
    {
        private int skillCheckDelay;

        public int ItemUseDelay
        {
            get;
            set;
        }
        public bool ControlUseItem
        {
            get => TPlayer.controlUseItem;
        }
        public Item HeldItem
        {
            get => TPlayer.HeldItem;
        }
        public double ItemUseAngle
        {
            get
            {
                double angle = TPlayer.itemRotation;
                if (TPlayer.direction == -1)
                {
                    angle += Math.PI;
                }
                return angle;
            }
        }

        public int Index { get; }
        public TSPlayer TSPlayer
        {
            get => TShock.Players[Index];
        }
        public Player TPlayer
        {
            get => Main.player[Index];
        }

        public PlayerData Data
        {
            get;
            set;
        }
        public PlayerSkillData[] Skills
        {
            get => Data.Skills;
        }

        public int Timer
        {
            get;
            private set;
        }

        public PlayerInfo(int index)
        {
            Index = index;
            Data = MainPlugin.Instance.PlayerDatas.GetData(TSPlayer);
        }

        #region BindSkill
        public void UnBind(int slot)
        {
            Skills[slot].ID = null;
            Skills[slot].SkillName = null;
            SaveData();
        }
        public void BindSkill(int slot, Skill skill, bool byProj, int bindId)
        {
            Skills[slot] = new PlayerSkillData
            {
                SkillName = skill.Name,
                ID = MainPlugin.Instance.Skills.GetIDByName(skill.Name),
                BindByProj = byProj,
                BindID = (short)bindId,
                CD = Skills[slot].CD
            };
            SaveData();
        }
        #endregion
        #region SaveData
        public void SaveData()
        {
            MainPlugin.Instance.PlayerDatas.SaveData(Data);
        }
        #endregion
        #region Events
        public void OnNewProj(GetDataHandlers.NewProjectileEventArgs args)
        {
            if (skillCheckDelay != 0)
            {
                return;
            }
            for (int i = 0; i < Skills.Length; i++)
            {
                ref var skill = ref Skills[i];
                if (skill.ID != null && skill.CD == 0)
                {
                    if (skill.IsBindTo(Main.projectile[args.Index]))
                    {
                        var vel = (Vector)args.Velocity;
                        skill.Release(TSPlayer, vel.Angle);
                        skillCheckDelay += 20;
                    }
                }
            }
        }
        public void OnGetData(GetDataEventArgs args)
        {
            switch (args.MsgID)
            {
                case PacketTypes.PlayerAnimation:
                    {
                        using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
                        using var reader = new BinaryReader(stream);
                        int index = reader.ReadByte();
                        index = Index;
                        Player player3 = TPlayer;
                        var itemRotation = reader.ReadSingle();
                        int itemAnimation = reader.ReadInt16();
                        TPlayer.itemRotation = itemRotation;
                        TPlayer.itemAnimation = itemAnimation;
                        TPlayer.channel = HeldItem.channel;
                        if (ItemUseDelay == 0 && ControlUseItem)
                        {
                            OnUseItem(HeldItem);
                            ItemUseDelay += HeldItem.useTime;
                        }
                        args.Handled = true;
                        break;
                    }
            }
        }

        private void OnUseItem(Item item)
        {
            if (skillCheckDelay != 0)
            {
                return;
            }
            for (int i = 0; i < Skills.Length; i++)
            {
                ref var skill = ref Skills[i];
                if (skill.ID != null && skill.CD == 0)
                {
                    if (skill.IsBindTo(item))
                    {
                        skill.Release(TSPlayer, ItemUseAngle);
                        skillCheckDelay += 20;
                    }
                }
            }
        }

        public void Update()
        {
            Timer++;
            if (Timer % 60 == 0)
            {
                SendStatusText($@"技能状态
    {Skills[0]}
    {Skills[1]}
    {Skills[2]}
    {Skills[3]}
    {Skills[4]}");
            }
            if (ItemUseDelay > 0)
            {
                ItemUseDelay--;
            }
            if (skillCheckDelay > 0)
            {
                skillCheckDelay--;
            }
            for (int i = 0; i < Skill.MaxSlot; i++)
            {
                if (Skills[i].ID == null)
                {
                    continue;
                }
                else if (Skills[i].CD > 0)
                {
                    Skills[i].CD--;
                }
            }
        }

        public void OnLeave()
        {
            MainPlugin.Instance.PlayerDatas.SaveData(Data);
        }
        #endregion
        private static readonly string EndLine19 = new string('\n', 19);
        private static readonly string EndLine5 = new string('\n', 5);
        public void SendStatusText(string text)
        {
            text = EndLine19 + text + EndLine5;
            TSPlayer.SendData(PacketTypes.Status, text);
        }
    }

    public class PlayerData
    {
        public int UserID
        {
            get;
            set;
        }
        public PlayerSkillData[] Skills
        {
            get;
            set;
        }
        public PlayerData()
        {
            Skills = new PlayerSkillData[Skill.MaxSlot];
        }
        public void Load()
        {
            ResetIDs();
        }
        public void ResetIDs()
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                ref var skill = ref Skills[i];
                if (skill.SkillName != null)
                {
                    skill.ID = MainPlugin.Instance.Skills.GetIDByName(skill.SkillName);
                }
            }
        }
    }

    #region PlayerSkillData
    public struct PlayerSkillData
    {
        [JsonIgnore]
        public int CD { get; set; }
        public string SkillName { get; set; }
        [JsonIgnore]
        public int? ID { get; set; }
        public bool BindByProj { get; set; }
        public short BindID { get; set; }

        [JsonIgnore]
        public Skill Skill => ID.HasValue ? MainPlugin.Instance.Skills[(byte)ID] : null;

        #region Release
        public void Release(TSPlayer player, double angle)
        {
            CD += Skill.CD;
            Skill.Release(player, angle);
        }
        #endregion
        #region IsBindTo
        public bool IsBindTo(Item item)
        {
            return !BindByProj && BindID == item.type;
        }
        public bool IsBindTo(Projectile proj)
        {
            return BindByProj && BindID == proj.type;
        }
        #endregion
        #region ToString
        public override string ToString()
        {
            if (ID == null)
            {
                return string.Empty;
            }
            if (CD > 0)
            {
                return $"{Skill}({CD / 60})";
            }
            return $"{Skill}";
        }
        #endregion
    }
    #endregion
}

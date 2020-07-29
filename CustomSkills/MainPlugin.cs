using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

using Microsoft.Xna.Framework;

namespace CustomSkills
{
	[ApiVersion(2, 1)]
	public class MainPlugin : TerrariaPlugin
	{
		private const string configPath = "tshock/CustomSkills.json";

		private Queue<DelayOperation> delayOperations;
		private Queue<ProjShiftOperation> psOperations;
		private Queue<ProjTask> pTasks;

		public static MainPlugin Instance
		{
			get;
			private set;
		}

		public override string Name => "CustomSkills";
		public override string Author => "1413";

		public PlayerInfo[] Players { get; }
		public SkillManager Skills { get; }
		public PlayerDataManager PlayerDatas { get; }
		public Config Config
		{
			get;
			private set;
		}

		public MainPlugin(Main game) : base(game)
		{
			Players = new PlayerInfo[byte.MaxValue];
			Skills = new SkillManager();
			PlayerDatas = new PlayerDataManager();
		}

		public override void Initialize()
		{
			Instance = this;

			delayOperations = new Queue<DelayOperation>();
			psOperations = new Queue<ProjShiftOperation>();
			pTasks = new Queue<ProjTask>();

			Config = Config.ReadConfig(configPath);

			Skills.LoadSkills(Config.SkillsDirectory);



			Commands.ChatCommands.Add(new Command("customskill.reload", ReloadSkills, "reloadskill"));
			Commands.ChatCommands.Add(new Command("customskill.use", SkillCommand, "skill"));


			ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);

			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += OnPostLogin;

			GetDataHandlers.NewProjectile += OnNewProjectile;
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);

				TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= OnPostLogin;

				GetDataHandlers.NewProjectile -= OnNewProjectile;
			}
			base.Dispose(disposing);
		}

		private void ReloadSkills(object obj)
		{
			Skills.LoadSkills(Config.SkillsDirectory);
			foreach (var player in Players)
			{
				if (player == null)
				{
					continue;
				}
				player.Data.ResetIDs();
			}
		}

		#region Updates
		private void UpdateDOs()
		{
			int count = delayOperations.Count;
			while (count-- != 0)
			{
				var operation = delayOperations.Dequeue();
				operation.Update();
				if (!operation.Ended)
				{
					delayOperations.Enqueue(operation);
				}
			}
		}
		private void UpdatePSs()
		{
			int count = psOperations.Count;
			while (count-- != 0)
			{
				var operation = psOperations.Dequeue();
				operation.Update();
				if (!operation.Ended)
				{
					psOperations.Enqueue(operation);
				}
			}
		}
		private void UpdatePTs()
		{
			int count = pTasks.Count;
			while (count-- != 0)
			{
				var operation = pTasks.Dequeue();
				operation.Update();
				if (!operation.Ended)
				{
					pTasks.Enqueue(operation);
				}
			}
		}
		#endregion
		#region Adds
		public void Add(DelayOperation item)
		{
			delayOperations.Enqueue(item);
		}
		public void Add(ProjShiftOperation item)
		{
			psOperations.Enqueue(item);
		}
		public void Add(ProjTask item)
		{
			pTasks.Enqueue(item);
		}
		#endregion
		#region Events
		private void OnUpdate(object args)
		{
			UpdateDOs();
			UpdatePSs();
			UpdatePTs();
		}
		#region NewProj
		private void OnNewProjectile(object sender, GetDataHandlers.NewProjectileEventArgs args)
		{
			var player = Players[args.Player.Index];
			player.OnNewProj(args);
		}
		#endregion
		#region PostLogin
		private void OnPostLogin(TShockAPI.Hooks.PlayerPostLoginEventArgs args)
		{
			Players[args.Player.Index] = new PlayerInfo(args.Player.Index);
		}
		#endregion
		private void OnGetData(GetDataEventArgs args)
		{
			var player = Players[args.Msg.whoAmI];
			if (player != null && player.TSPlayer.IsLoggedIn)
			{
				player.OnGetData(args);
			}
		}
		private void OnLeave(LeaveEventArgs args)
		{
			Players[args.Who]?.OnLeave();
			Players[args.Who] = null;
		}
		#endregion
		#region Commands
		private void SkillCommand(CommandArgs args)
		{
			var option = string.Empty;
			if (args.Parameters.Count > 0)
			{
				option = args.Parameters[0].ToLower();
			}
			#region FindSkill
			Skill FindSkill(string nameOrID)
			{
				Skill skill = null;
				if (int.TryParse(nameOrID, out int skillID))
				{
					if (!skillID.InRange(0, Skills.Count - 1))
					{
						args.Player.SendErrorMessage($"无效的技能ID: {skillID}");
						return null;
					}
					skill = Skills[skillID];
				}
				else
				{
					skill = Skills.FirstOrDefault(skill => skill.Name.StartsWith(nameOrID, StringComparison.OrdinalIgnoreCase));
					if (skill == null)
					{
						args.Player.SendErrorMessage($"找不到技能: {nameOrID}");
						args.Player.SendErrorMessage("使用/skill list [页码] 以查看技能列表");
						return null;
					}
				}
				return skill;
			}
			#endregion
			switch (option)
			{
				case "unbind":
					{
						#region CheckParamsCount
						if (args.Parameters.Count < 2)
						{
							goto default;
						}
						#endregion
						#region CheckSlotValid
						if (!int.TryParse(args.Parameters[1], out int slot) || !slot.InRange(1, Skill.MaxSlot))
						{
							args.Player.SendErrorMessage($"无效的slot: {args.Parameters[1]}");
							args.Player.SendErrorMessage("应为1/2/3/4/5");
							break;
						}
						slot--;
						#endregion
						var player = Players[args.Player.Index];
						player.UnBind(slot);
					}
					break;
				case "bind":
					{
						#region CheckParamsCount
						if (args.Parameters.Count < 3)
						{
							goto default;
						}
						#endregion
						#region CheckSlotValid
						if (!int.TryParse(args.Parameters[1], out int slot) || !slot.InRange(1, Skill.MaxSlot))
						{
							args.Player.SendErrorMessage($"无效的slot: {args.Parameters[1]}");
							args.Player.SendErrorMessage("应为1/2/3/4/5");
							break;
						}
						slot--;
						#endregion
						#region FindSkill
						var skill = FindSkill(args.Parameters[2]);
						if (skill == null)
						{
							break;
						}
						#endregion
						#region CheckSkill
						var player = Players[args.Player.Index];
						if (skill.ID != player.Skills[slot].ID && player.Skills.Count(skl => skl.ID == skill.ID) != 0)
						{
							player.TSPlayer.SendErrorMessage("你已经绑定了该技能");
							return;
						}
						#endregion
						#region CheckBinding
						switch (player.HeldItem.useStyle)
						{
							// 长矛
							case ItemUseStyleID.Shoot when player.HeldItem.noUseGraphic:
								player.BindSkill(slot, skill, true, player.HeldItem.shoot);
								break;
							// 射出类
							case ItemUseStyleID.Shoot:
								player.BindSkill(slot, skill, false, player.HeldItem.type);
								break;
							// 短剑
							case ItemUseStyleID.Rapier:
								player.BindSkill(slot, skill, false, player.HeldItem.type);
								break;
							// 剑气啊火花魔杖啊什么的
							case ItemUseStyleID.Swing when player.HeldItem.shoot != 0:
								player.BindSkill(slot, skill, true, player.HeldItem.shoot);
								break;
							default:
								args.Player.SendErrorMessage("该武器无法绑定技能");
								return;
						}
						args.Player.SendMessage(skill.Name, Color.Aqua);
						args.Player.SendMessage(skill.Introduction, Color.Aqua);
						#endregion
					}
					break;
				case "list":
					#region ListSkill
					if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out int page))
					{
						args.Player.SendInfoMessage("用法:  /skill list [页码]");
						break;
					}
					page = Math.Max(1, page);
					page = Math.Min(page, Skills.SkillLists.Length);
					args.Player.SendSuccessMessage("当前页: {0}/{1}", page, Skills.SkillLists.Length);
					args.Player.SendInfoMessage(Skills.SkillLists[page - 1]);
					break;
				#endregion
				#region Sounds
				//case "sounds":
				//	Task.Run(() =>
				//	{
				//		try
				//		{
				//			for (int i = 0; i < SoundID.TrackableLegacySoundCount; i++)
				//			{
				//				var sound = new NetMessage.NetSoundInfo(args.TPlayer.Center, (ushort)i);
				//				NetMessage.PlayNetSound(sound);
				//				args.Player.SendInfoMessage($"soundid: {i}");
				//				Thread.Sleep(5000);
				//			}
				//		}
				//		catch (Exception)
				//		{

				//		}
				//	});
				//	break;
				#endregion
				case "help":
					#region Help
					{
						if (args.Parameters.Count < 2)
						{
							goto default;
						}
						var skill = FindSkill(args.Parameters[1]);
						if (skill == null)
						{
							goto default;
						}
						args.Player.SendMessage(skill.Introduction, Color.Aqua);
					}
					break;
				#endregion
				default:
					args.Player.SendInfoMessage("bind <slot> <skill> 将制定技能绑定到手中的武器上");
					args.Player.SendInfoMessage("unbind <slot> 技能解绑");
					args.Player.SendInfoMessage("list  查看技能列表");
					args.Player.SendInfoMessage("help [技能id]  查看技能介绍");
					args.Player.SendInfoMessage("help  帮助");
					break;
			}
		}
		#endregion
	}
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace CustomSkills
{
	public class PlayerDataManager
	{
		private string SavePath
		{
			get => MainPlugin.Instance.Config.PlayerDataPath;
		}
		public PlayerData GetData(TSPlayer player)
		{
			var path = GetDataPath(player.Account.ID);
			PlayerData data;
			if (File.Exists(path))
			{
				var text = File.ReadAllText(path);
				data = JsonConvert.DeserializeObject<PlayerData>(text);
			}
			else
			{
				data = new PlayerData()
				{
					UserID = player.Account.ID
				};
			}
			data.Load();
			return data;
		}
		public void SaveData(PlayerData data)
		{
			var text = JsonConvert.SerializeObject(data);
			var path = GetDataPath(data.UserID);
			File.WriteAllText(path, text);
		}

		private string GetDataPath(int id)
		{
			return Path.Combine(SavePath, id + ".json");
		}
	}
}

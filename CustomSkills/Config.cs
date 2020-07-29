using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CustomSkills
{
	public class Config
	{
		public string StoragePath
		{
			get;
			set;
		}
		[JsonIgnore]
		public string PlayerDataPath
		{
			get;
			private set;
		}
		[JsonIgnore]
		public string SkillsDirectory
		{
			get;
			private set;
		}
		public Config()
		{
			StoragePath = "CustomSkill";
		}

		private void Load()
		{
			PlayerDataPath = Path.Combine(StoragePath, "Players");
			SkillsDirectory = Path.Combine(StoragePath, "Skills");
			if (!Directory.Exists(StoragePath))
			{
				Directory.CreateDirectory(StoragePath);
			}
			if (!Directory.Exists(PlayerDataPath))
			{
				Directory.CreateDirectory(PlayerDataPath);
			}
			if (!Directory.Exists(SkillsDirectory))
			{
				Directory.CreateDirectory(SkillsDirectory);
			}
		}

		public void Write(string path)
		{
			var text = JsonConvert.SerializeObject(this);
			File.WriteAllText(path, text);
		}

		public static Config ReadConfig(string path)
		{
			if (File.Exists(path))
			{
				var text = File.ReadAllText(path);
				var config = JsonConvert.DeserializeObject<Config>(text);
				config.Load();
				return config;
			}
			else
			{
				var config = new Config();
				config.Load();
				config.Write(path);
				return config;
			}
		}
	}
}

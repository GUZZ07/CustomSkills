using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CustomSkills
{
	public class SkillManager : IEnumerable<Skill>
	{
		private Skill[] skills;
		private Dictionary<string, int> skillNameToID;

		public Skill this[int id]
		{
			get => skills[id];
		}

		public string[] SkillLists
		{
			get;
			private set;
		}

		public int Count
		{
			get => skills.Length;
		}

		public SkillManager()
		{
			skills = new Skill[0];
			skillNameToID = new Dictionary<string, int>();
		}

		public void LoadSkills(string directory)
		{
			try
			{
				var folder = new DirectoryInfo(directory);
				var files = folder.GetFiles("*.json");
				Skill[] skills = new Skill[files.Length];
				for (int i = 0; i < files.Length; i++)
				{
					var file = files[i];
					var skillData = JsonConvert.DeserializeObject<SkillData>(File.ReadAllText(file.FullName));
					skills[i] = new Skill(skillData);
				}
				this.skills = skills;
				skillNameToID.Clear();
				for (int i = 0; i < skills.Length; i++)
				{
					skills[i].ID = i;
					skillNameToID.Add(skills[i].Name, i);
				}
				#region LoadSkillList
				if (Count == 0)
				{
					SkillLists = new[] { string.Empty };
				}
				else
				{
					SkillLists = new string[(int)Math.Ceiling(Count / 4.0 / 4.0)];
					int page = 0;
					var sb = new StringBuilder(skills.Length * 10);
					for (int i = 0; i < skills.Length; i++)
					{
						var skill = skills[i];
						sb.Append(skill.Name);
						sb.Append("   ");
						if (i != Count - 1 && i % 4 == 3)
						{
							sb.AppendLine();
						}
						if (i % (4 * 4) == 4 * 4 - 1 && i != Count - 1)
						{
							SkillLists[page] = sb.ToString();
							sb.Clear();
							page++;
						}
					}
					SkillLists[page] = sb.ToString();
				}
				#endregion
			}
			catch (Exception ex)
			{
				Console.WriteLine("技能加载失败");
				Console.WriteLine(ex);
			}
		}

		public int? GetIDByName(string skillName)
		{
			if (skillNameToID.TryGetValue(skillName, out int id))
			{
				return id;
			}
			return null;
		}
		#region IEnumerable
		public IEnumerator<Skill> GetEnumerator()
		{
			return ((IEnumerable<Skill>)skills).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}

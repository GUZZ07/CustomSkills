using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CustomSkills
{
	public class SkillData
	{
		public ProjData[] ProjDatas
		{
			get;
			set;
		}
		public int CoolDown
		{
			get;
			set;
		}
		public string SkillName
		{
			get;
			set;
		}
		public string Author
		{
			get;
			set;
		}
		public string SkillDescription
		{
			get;
			set;
		}
		public SkillData()
		{
			SkillName = string.Empty;
			Author = string.Empty;
			SkillDescription = string.Empty;
		}
	}
}

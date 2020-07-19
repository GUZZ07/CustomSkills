using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace SkillDesigner.Libs
{
	public partial class ProjView
	{
		private static Dictionary<int, Vector> ProjSizes;

		public ProjData Data { get; }
		public Vector ProjSize { get; }
		public ProjView(ProjData data, CoordinateSystem coordinateSystem)
		{
			#region Initialize Sizes
			if (ProjSizes == null)
			{
				using var stream = new MemoryStream(Resource.ProjSizes);
				var reader = new StreamReader(stream);
				var text = reader.ReadToEnd();
				ProjSizes = JsonConvert.DeserializeObject<Dictionary<int, Vector>>(text);
			}
			#endregion
			Data = data;
			ProjSize = ProjSizes[data.ProjType];
			Visible = false;
			MouseDown += (sender, args) =>
			{
				MessageBox.Show("hahaha");
			};
			Update(coordinateSystem);
		}
		public void Update(CoordinateSystem coordinateSystem)
		{
			Width = (int)(coordinateSystem.PixelPerPoint * ProjSize.X);
			Height = (int)(coordinateSystem.PixelPerPoint * ProjSize.Y);
			Location = coordinateSystem.Transform(Data.Position) - (Vector)Size / 2;
		}
	}
}

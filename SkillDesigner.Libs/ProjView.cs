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
using System.Drawing.Drawing2D;

namespace SkillDesigner.Libs
{
	public partial class ProjView
	{
		private static Dictionary<int, Vector> ProjSizes;

		public ProjData Data { get; }
		public Vector ProjSize { get; }
		public bool NoRotation { get; set; }
		public ProjView(ProjData data)
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
		}

		public RectangleF GetRect(CoordinateSystem coordinateSystem)
		{
			var pos = coordinateSystem.Transform(Data.Position);
			var size = coordinateSystem.PixelPerPoint * ProjSize;

			pos -= size / 2;
			return new RectangleF(pos, size);
		}

		public void Draw(Graphics graphics, CoordinateSystem coordinateSystem, Brush brush = null, Bitmap texture = null)
		{
			var ppp = coordinateSystem.PixelPerPoint;
			var matrix = graphics.Transform;
			matrix.RotateAt(90, coordinateSystem.Transform(Data.Position));
			matrix.RotateAt(-Data.SpeedAngle / MathF.PI * 180, coordinateSystem.Transform(Data.Position));
			graphics.Transform = matrix;
			graphics.FillRectangle(brush, GetRect(coordinateSystem));
			if (texture != null)
			{
				var pos = coordinateSystem.Transform(Data.Position) - new Vector(texture.Width, ProjSize.Y) * ppp / 2;
				var srcRect = new RectangleF(0, 0, texture.Width, texture.Height);
				var destRect = new RectangleF(pos + new Vector(ppp, 0) / 2, srcRect.Size * ppp);
				graphics.DrawImage(texture, destRect, srcRect, GraphicsUnit.Pixel);
			}
			graphics.ResetTransform();
			if (!NoRotation)
			{
				Data.SpeedAngle += MathF.PI / 180;
			}
		}
	}
}

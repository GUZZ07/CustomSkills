using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SkillDesigner.Libs
{
	public partial class CoordinateSystem : UserControl
	{
		public int LowX
		{
			get;
			private set;
		}
		public int LowY
		{
			get;
			private set;
		}
		public int HighX
		{
			get;
			private set;
		}
		public int HighY
		{
			get;
			private set;
		}
		public int PixelPerPoint
		{
			get;
			private set;
		}

		private List<ProjView> projViews;

		private const int ProjCount = 950;

		private Bitmap xyAxis;
		private Bitmap texture;
		private Bitmap[] projTextures;

		private Pen pen;
		private SolidBrush brush;

		public CoordinateSystem(int lowX = -160, int lowY = -160, int highX = 160, int highY = 160, int pixelPerPoint = 2)
		{
			LowX = lowX;
			LowY = lowY;
			HighX = highX;
			HighY = highY;
			PixelPerPoint = pixelPerPoint;

			InitializeComponent();
			Load += CoordinateSystem_Load;

			Width = PixelPerPoint * (HighX - LowX);
			Height = PixelPerPoint * (HighY - LowY);

			pen = new Pen(Color.FromArgb(255 / 3, Color.Blue));
			brush = new SolidBrush(Color.Purple);

			LoadBitmaps();
			LoadViews();

			CoordinateSystemTransformed();
		}
		#region Loads
		private void LoadBitmaps()
		{
			xyAxis = new Bitmap(Width, Height);
			texture = new Bitmap(Width, Height);

			projTextures = new Bitmap[ProjCount];

			projTextures[636] = Resource.Projectile636;
		}
		private void LoadViews()
		{
			var data = new ProjData
			{
				Position = Vector.FromPolar(Math.PI * 1.5, 16 * 3.5f),
				ProjType = 636,
				Speed = 14,
				SpeedAngle = MathF.PI / 2
			};
			projViews = new List<ProjView>()
			{
				new ProjView(data)
			};
		}
		#endregion
		#region Transform
		public Vector Transform(Vector point)
		{
			return Transform(point.X, point.Y);
		}

		/// <summary>
		/// 变换到像素坐标上
		/// </summary>
		public Vector Transform(float x, float y)
		{
			return new Vector
			{
				X = (x - LowX) * PixelPerPoint,
				Y = (HighY - y) * PixelPerPoint
			};
		}

		public Vector ReversedTransform(float x, float y)
		{
			return new Vector
			{
				X = x / PixelPerPoint + LowX,
				Y = HighY - y / PixelPerPoint
			};
		}
		#endregion
		public bool InRange(Vector point)
		{
			return
				LowX <= point.X && point.X <= HighX &&
				LowY <= point.Y && point.Y <= HighY;
		}
		#region ModifyCoordinate
		public void Transport(int Δx, int Δy)
		{
			LowX += Δx;
			HighX += Δx;
			LowY += Δy;
			HighY += Δy;
			CoordinateSystemTransformed();
		}

		public void ZoomUp2()
		{
			var width = HighX - LowX;
			var height = HighY - LowY;


			var cx = HighX + LowX >> 1;
			var cy = HighY + LowY >> 1;

			PixelPerPoint *= 2;

			width /= 2;
			height /= 2;

			LowX = cx - width / 2;
			HighX = cx + width / 2;

			LowY = cy - height / 2;
			HighY = cy + height / 2;

			CoordinateSystemTransformed();
		}

		public void ZoomDown2()
		{
			if (PixelPerPoint == 1)
			{
				return;
			}
			var width = HighX - LowX;
			var height = HighY - LowY;


			var cx = HighX + LowX >> 1;
			var cy = HighY + LowY >> 1;

			PixelPerPoint /= 2;

			width *= 2;
			height *= 2;

			LowX = cx - width / 2;
			HighX = cx + width / 2;

			LowY = cy - height / 2;
			HighY = cy + height / 2;

			CoordinateSystemTransformed();
		}

		private void CoordinateSystemTransformed()
		{
			PrepareXYAxisTexture();
		}
		#endregion
		#region Events

		private void CoordinateSystem_Load(object sender, EventArgs args)
		{
		}

		#endregion
		#region Draw
		private void CoordinateSystem_Paint(object sender, PaintEventArgs args)
		{
			var graphics = args.Graphics;
			Draw(graphics);
		}

		private void PrepareXYAxisTexture()
		{
			using (var graphics = Graphics.FromImage(xyAxis))
			{
				graphics.Clear(Color.Transparent);
				PrepareXYAxis(graphics);
				DrawBorder(graphics);
			}
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Draw(Graphics graphics)
		{
			using var textureGraphics = Graphics.FromImage(texture);
			textureGraphics.Clear(Color.FromArgb(0xA5, 255, 255));
			DrawProjViews(textureGraphics);
			DrawXYAxis(textureGraphics);
			DrawMouseAxis(textureGraphics);
			graphics.DrawImage(texture, 0, 0);
		}

		private void DrawBorder(Graphics graphics)
		{
			graphics.DrawRectangle(Pens.Yellow, 0, 0, Width - 1, Height - 1);
		}

		private void DrawMouseText(Graphics graphics, PointF mousePos, string text)
		{
			var font = Font;
			var size = graphics.MeasureString(text, font);
			var rect = new RectangleF(mousePos, size);
			rect.X -= size.Width;
			rect.Y -= size.Height;
			graphics.DrawString(text, font, SystemBrushes.InfoText, rect);
		}

		private void DrawMouseAxis(Graphics graphics)
		{
			var relative = PointToScreen(Location);
			var mousePos = MousePosition;
			mousePos.X -= relative.X;
			mousePos.Y -= relative.Y;
			var pos = ReversedTransform(mousePos.X, mousePos.Y);
			pen.Color = Color.FromArgb(200, 255, 0, 0);
			if (InRange(pos))
			{
				graphics.DrawLine(pen, Transform(pos.X, 0), Transform(pos));
				graphics.DrawLine(pen, Transform(0, pos.Y), Transform(pos));
				DrawMouseText(graphics, mousePos, $"({pos.X / 16: 0.0},{pos.Y / 16: 0.0})");
			}
		}

		private void DrawXYAxis(Graphics graphics)
		{
			graphics.DrawImage(xyAxis, 0, 0);
		}

		private void PrepareXYAxis(Graphics graphics)
		{
			graphics.DrawLine(Pens.Black, Transform(LowX, 0), Transform(HighX, 0));
			graphics.DrawLine(Pens.Black, Transform(0, LowY), Transform(0, HighY));
			pen.Color = Color.FromArgb(255 / 3, 0, 0, 255);
			for (int i = LowX; i < HighX; i++)
			{
				if (i % 16 == 0)
				{
					graphics.DrawLine(pen, Transform(i, LowY), Transform(i, HighY));
				}
			}
			for (int i = LowY; i < HighY; i++)
			{
				if (i % 16 == 0)
				{
					graphics.DrawLine(pen, Transform(LowX, i), Transform(HighX, i));
				}
			}
		}

		private void DrawProjViews(Graphics graphics)
		{
			foreach (var view in projViews)
			{
				view.Draw(graphics, this, brush, projTextures[view.Data.ProjType]);
			}
		}
		#endregion
	}
}

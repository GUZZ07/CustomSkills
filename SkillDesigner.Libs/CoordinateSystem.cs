using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkillDesigner.Libs
{
	public partial class CoordinateSystem : UserControl
	{
		public int LowX { get; }
		public int LowY { get; }
		public int HighX { get; }
		public int HighY { get; }
		public int PixelPerPoint { get; }

		private Bitmap background;
		private Bitmap texture;

		private Pen pen;

		public CoordinateSystem(int lowX = -160, int lowY = -160, int highX = 160, int highY = 160, int pixelPerPoint = 2)
		{
			LowX = lowX;
			LowY = lowY;
			HighX = highX;
			HighY = highY;
			PixelPerPoint = pixelPerPoint;
			InitializeComponent();
			Width = PixelPerPoint * (HighX - lowX);
			Height = PixelPerPoint * (HighY - lowY);
			pen = new Pen(Color.FromArgb(255 / 3, Color.Blue));
			background = new Bitmap(Width, Height);
			using (var graphics = Graphics.FromImage(background))
			{
				Clear(graphics);
				DrawXYAxis(graphics);
				DrawBorder(graphics);
			}
			texture = new Bitmap(Width, Height);
		}

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

		public bool InRange(Vector point)
		{
			return
				LowX <= point.X && point.X <= HighX &&
				LowY <= point.Y && point.Y <= HighY;
		}

		private void CoordinateSystem_Paint(object sender, PaintEventArgs args)
		{
			var graphics = args.Graphics;
			Draw(graphics);
		}

		public void Draw(Graphics graphics)
		{
			using var textureGraphics = Graphics.FromImage(texture);
			textureGraphics.DrawImage(background, 0, 0);
			DrawMouseAxis(textureGraphics);
			graphics.DrawImage(texture, 0, 0);
		}

		public void Clear(Graphics graphics)
		{
			graphics.Clear(Color.Aqua);
		}

		private void DrawBorder(Graphics graphics)
		{
			graphics.DrawRectangle(Pens.Yellow, 0, 0, Width - 1, Height - 1);
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
			}
		}

		private void DrawXYAxis(Graphics graphics)
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
	}
}

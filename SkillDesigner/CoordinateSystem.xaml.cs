using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

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


		// private List<ProjView> projViews;

		private const int ProjCount = 950;

		private const int gridLineCount = 100;

		private List<Line> horLines;
		private List<Line> verLines;


		public CoordinateSystem()
		{
			int lowX = -160; int lowY = -160; int highX = 160; int highY = 160; int pixelPerPoint = 2;

			InitializeComponent();

			LowX = lowX;
			LowY = lowY;
			HighX = highX;
			HighY = highY;
			PixelPerPoint = pixelPerPoint;

			horLines = new List<Line>(gridLineCount);
			verLines = new List<Line>(gridLineCount);

			for (int i = 1; i <= gridLineCount / 2; i++)
			{
				horLines.Add(new Line() { Style = (Style)Resources["HorAxis"], Y1 = 320 + 16 * i, Y2 = 320 + 16 * i });
				horLines.Add(new Line() { Style = (Style)Resources["HorAxis"], Y1 = 320 - 16 * i, Y2 = 320 - 16 * i });

				verLines.Add(new Line() { Style = (Style)Resources["VerAxis"], X1 = 320 + 16 * i, X2 = 320 + 16 * i });
				verLines.Add(new Line() { Style = (Style)Resources["VerAxis"], X1 = 320 - 16 * i, X2 = 320 - 16 * i });
			}
			foreach (var line in horLines)
			{
				Grid.Children.Add(line);
			}
			foreach (var line in verLines)
			{
				Grid.Children.Add(line);
			}

			Loaded += CoordinateSystem_Loaded;

			LoadBitmaps();
			LoadViews();
		}
		#region Loads
		private void LoadBitmaps()
		{

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
			var data2 = new ProjData
			{
				Position = Vector.FromPolar(Math.PI * 1, 16 * 7.5f),
				ProjType = 48,
				Speed = 14,
			};
			var data3 = new ProjData
			{
				Position = Vector.FromPolar(Math.PI * 0.5, 16 * 7.5f),
				ProjType = 238,
				Speed = 14,
			};
			var data4 = new ProjData
			{
				Position = Vector.FromPolar(Math.PI * 0, 16 * 7.5f),
				ProjType = 254,
				Speed = 14,
			};
			var data5 = new ProjData
			{
				Position = Vector.FromPolar(Math.PI * 0.75, 16 * 7.5f),
				ProjType = 465,
				Speed = 14,
			};
			//projViews = new List<ProjView>()
			//{
			//	new ProjView(data),
			//	new ProjView(data2),
			//	new ProjView(data3),
			//	new ProjView(data4),
			//	new ProjView(data5)
			//};
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
			CoordinateSystemTransformed(Δx, Δy);
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

			//CoordinateSystemTransformed();
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

			//CoordinateSystemTransformed();
		}

		private void CoordinateSystemTransformed(int Δx, int Δy)
		{
			void Modify(Line line)
			{
				line.X1 += Δx * PixelPerPoint;
				line.Y1 -= Δy * PixelPerPoint;
				line.X2 += Δx * PixelPerPoint;
				line.Y2 -= Δy * PixelPerPoint;
			}
			foreach (var line in horLines)
			{
				Modify(line);
			}
			foreach (var line in verLines)
			{
				Modify(line);
			}
			Modify(XAxis);
			Modify(YAxis);
		}
		#endregion
		#region Events

		private void CoordinateSystem_MouseWheel(object sender, MouseWheelEventArgs args)
		{
			var pos = args.MouseDevice.GetPosition(this);
			if (0 <= pos.X && pos.Y < Width && 0 <= pos.Y && pos.Y < Height)
			{
				var Δ = args.Delta / 120 * 16 / 2;
				if (!Keyboard.IsKeyDown(Key.LeftCtrl))
				{
					Transport(0, -Δ);
				}
				else
				{
					Transport(-Δ, 0);
				}
			}
		}

		private void CoordinateSystem_Loaded(object sender, EventArgs args)
		{
			// ProjView.LoadResources();
		}

		#endregion

		#region Draw
#if false
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Draw()
		{
			using var textureGraphics = Graphics.FromImage(texture);
			textureGraphics.Clear(Color.FromArgb(0xA5, 255, 255));
			// textureGraphics.DrawImage(Parent.BackgroundImage, 0, 0);
			// textureGraphics.Clear(Color.FromArgb(80, 0xA5, 255, 255));
			DrawProjViews(textureGraphics);
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
			brush.Color = Color.Purple;
			foreach (var view in projViews)
			{
				view.Draw(graphics, this, brush);
			}
		}
#endif
		#endregion

		private void UserControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{

		}
	}
}

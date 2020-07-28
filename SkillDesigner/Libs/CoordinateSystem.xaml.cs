using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

using WVector = System.Windows.Vector;
using System.Timers;

namespace SkillDesigner.Libs
{
	public partial class CoordinateSystem : UserControl, INotifyPropertyChanged
	{
		#region CSystem
		public static int LowX
		{
			get;
			private set;
		}
		public static int LowY
		{
			get;
			private set;
		}
		public static int HighX
		{
			get;
			private set;
		}
		public static int HighY
		{
			get;
			private set;
		}
		public static int PixelPerPoint
		{
			get;
			private set;
		}
		#endregion
		#region MouseAxis
		public double MouseAxisXX1
		{
			get => YAxis.X1;
		}

		public double MouseAxisXX2
		{
			get => Mouse.GetPosition(this).X;
		}

		public double MouseAxisXY
		{
			get => Mouse.GetPosition(this).Y;
		}

		public double MouseAxisYY1
		{
			get => XAxis.Y1;
		}

		public double MouseAxisYY2
		{
			get => Mouse.GetPosition(this).Y;
		}

		public double MouseAxisYX
		{
			get => Mouse.GetPosition(this).X;
		}


		public Thickness MouseAxisTextMargin
		{
			get
			{
				var pos = MouseAxisTextPosition;
				return new Thickness(pos.X, pos.Y, 0, 0);
			}
		}

		private Point MouseAxisTextPosition
		{
			get => Point.Subtract(Mouse.GetPosition(this), new WVector(MouseAxisTip.ActualWidth, MouseAxisTip.ActualHeight));
		}

		public string MouseAxisText
		{
			get => (ReversedTransform((Vector)Mouse.GetPosition(this)) / 16).ToString();
		}

		public Visibility MouseAxisVisibility
		{
			get => MouseAxisX.Visibility;
			set => MouseAxisX.Visibility = MouseAxisY.Visibility = MouseAxisTip.Visibility = value;
		}
		#endregion
		/// <summary>
		/// 在像素坐标系中
		/// </summary>
		public Vector OriginalOffset { get; private set; }


		// private List<ProjView> projViews;

		private const int ProjCount = 950;

		private const int gridLineCount = 100;

		private List<Line> horLines;
		private List<Line> verLines;
		private List<ProjView> projViews;
		private ProjView focusedView;
		private bool hideHitbox;

		public static int TickCount
		{
			get;
			private set;
		}

		public ProjView FocusedView
		{
			get
			{
				return focusedView;
			}
			set
			{
				focusedView = value;
				OnFocusedViewChanged();
			}
		}

		public bool HideHitbox
		{
			get => hideHitbox;
			set
			{
				hideHitbox = value;
				foreach(var view in projViews)
				{
					view.Hitbox.Visibility = value ? Visibility.Hidden : Visibility.Visible;
				}
			}
		}

		#region PropertyViews
		public int? FVType
		{
			get => FocusedView?.Data?.ProjType;
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.ProjType = (int)value;
					FocusedView.DataChanged(true);
				}
			}
		}
		public int? FVLD
		{
			get => FocusedView?.Data?.LaunchDelay;
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.LaunchDelay = (int)value;
					FocusedView.DataChanged(false);
				}
			}
		}
		public int? FVCD
		{
			get => FocusedView?.Data?.CreateDelay;
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.CreateDelay = (int)value;
					FocusedView.DataChanged(false);
				}
			}
		}
		public int? FVKnockback
		{
			get => FocusedView?.Data?.Knockback;
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.Knockback = (int)value;
					FocusedView.DataChanged(false);
				}
			}
		}
		public int? FVDamage
		{
			get => FocusedView?.Data?.Damage;
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.Damage = (int)value;
					FocusedView.DataChanged(false);
				}
			}
		}
		public Vector? FVPosition
		{
			get
			{
				if (FocusedView?.Data != null)
				{
					return FocusedView.Data.Position / 16;
				}
				return null;
			}
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.Position = (Vector)value * 16;
					FocusedView.DataChanged(false);
				}
			}
		}
		public float? FVSpeed
		{
			get
			{
				if (FocusedView?.Data != null)
				{
					return (int)(10 * FocusedView.Data.Speed * 60.0 / 16) / 10f;
				}
				return null;
			}
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.Speed = (float)(value * 16 / 60);
					FocusedView.DataChanged(false);
				}
			}
		}
		public float? FVSpeedAngle
		{
			get
			{
				if (FocusedView?.Data != null)
				{
					return (int)(10 * FocusedView.Data.SpeedAngle * 180 / Math.PI) / 10f % 360;
				}
				return null;
			}
			set
			{
				if (FocusedView?.Data != null)
				{
					FocusedView.Data.SpeedAngle = (float)value * MathF.PI / 180;
					FocusedView.DataChanged(false);
				}
			}
		}
		#endregion

		public CoordinateSystem()
		{
			int lowX = -160; int lowY = -160; int highX = 160; int highY = 160; int pixelPerPoint = 2;

			InitializeComponent();

			ProjView.LoadResources();

			LowX = lowX;
			LowY = lowY;
			HighX = highX;
			HighY = highY;
			PixelPerPoint = pixelPerPoint;

			Loaded += CoordinateSystem_Loaded;

			LoadGrid();

			projViews = new List<ProjView>();

			//ProjView proj = new ProjView(new ProjData { ProjType = 636, Position = (64, 0), SpeedAngle = MathF.PI / 2 });
			//projViews.Add(proj);
			//Grid.Children.Add(proj);

			LoadBitmaps();
			LoadViews();
		}
		#region Loads
		private void LoadGrid()
		{
			horLines = new List<Line>(gridLineCount);
			verLines = new List<Line>(gridLineCount);

			var unit = 16 * PixelPerPoint;

			for (int i = 1; i <= gridLineCount / 2; i++)
			{
				horLines.Add(new Line() { Style = (Style)Resources["HorAxis"], Y1 = 320 + unit * i, Y2 = 320 + unit * i });
				horLines.Add(new Line() { Style = (Style)Resources["HorAxis"], Y1 = 320 - unit * i, Y2 = 320 - unit * i });

				verLines.Add(new Line() { Style = (Style)Resources["VerAxis"], X1 = 320 + unit * i, X2 = 320 + unit * i });
				verLines.Add(new Line() { Style = (Style)Resources["VerAxis"], X1 = 320 - unit * i, X2 = 320 - unit * i });
			}
			foreach (var line in horLines)
			{
				Grid.Children.Add(line);
			}
			foreach (var line in verLines)
			{
				Grid.Children.Add(line);
			}

			Grid.Children.Remove(MouseAxisX);
			Grid.Children.Remove(MouseAxisY);

			Grid.Children.Add(MouseAxisX);
			Grid.Children.Add(MouseAxisY);
		}
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
		public static Vector Transform(Vector point)
		{
			return Transform(point.X, point.Y);
		}

		/// <summary>
		/// 变换到像素坐标上
		/// </summary>
		public static Vector Transform(float x, float y)
		{
			return new Vector
			{
				X = (x - LowX) * PixelPerPoint,
				Y = (HighY - y) * PixelPerPoint
			};
		}

		public static Vector ReversedTransform(Vector value)
		{
			return ReversedTransform(value.X, value.Y);
		}

		public static Vector ReversedTransform(float x, float y)
		{
			return new Vector
			{
				X = x / PixelPerPoint + LowX,
				Y = HighY - y / PixelPerPoint
			};
		}
		#endregion
		#region Views
		public void ImportDatas(ProjData[] datas)
		{
			ClearViews();
			foreach (var data in datas)
			{
				AddView(new ProjView(data));
			}
		}
		public ProjData[] ExportDatas()
		{
			var datas = new ProjData[projViews.Count];
			for (int i = 0; i < datas.Length; i++)
			{
				datas[i] = projViews[i].Data;
			}
			return datas;
		}
		public void AddNewView()
		{
			var view = new ProjView(new ProjData { ProjType = 636 });
			projViews.Add(view);
			Grid.Children.Add(view);
			FocusedView = view;
		}
		public void ClearViews()
		{
			FocusedView = null;
			foreach (var view in projViews)
			{
				Grid.Children.Remove(view);
			}
			projViews.Clear();
		}
		public void AddView(ProjView view)
		{
			projViews.Add(view);
			Grid.Children.Add(view);
		}
		public void RemoveView(ProjView view)
		{
			Grid.Children.Remove(view);
			projViews.Remove(view);
		}
		#endregion
		#region InRange
		public bool InRange(Vector point)
		{
			return
				LowX <= point.X && point.X <= HighX &&
				LowY <= point.Y && point.Y <= HighY;
		}
		#endregion
		#region ModifyCoordinate
		public void Transport(int Δx, int Δy)
		{
			LowX += Δx;
			HighX += Δx;
			LowY += Δy;
			HighY += Δy;
			TransportGrid(Δx, Δy);
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
			#region Modify
			static void ModifyHor(Line line)
			{
				var y = line.Y1;
				y = 320 + (y - 320) * 2;
				line.Y1 = y;
				line.Y2 = y;
			}
			static void ModifyVer(Line line)
			{
				var x = line.X1;
				x = 320 + (x - 320) * 2;
				line.X1 = x;
				line.X2 = x;
			}
			#endregion

			foreach (var line in horLines)
			{
				ModifyHor(line);
			}
			foreach (var line in verLines)
			{
				ModifyVer(line);
			}
			ModifyHor(XAxis);
			ModifyVer(YAxis);
			Transformed();
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
			#region Modify
			static void ModifyHor(Line line)
			{
				var y = line.Y1;
				y = 320 + (y - 320) / 2;
				line.Y1 = y;
				line.Y2 = y;
			}
			static void ModifyVer(Line line)
			{
				var x = line.X1;
				x = 320 + (x - 320) / 2;
				line.X1 = x;
				line.X2 = x;
			}
			#endregion

			foreach (var line in horLines)
			{
				ModifyHor(line);
			}
			foreach (var line in verLines)
			{
				ModifyVer(line);
			}
			ModifyHor(XAxis);
			ModifyVer(YAxis);
			Transformed();
		}

		private void Transformed()
		{
			MouseAxisPropChanged();
			foreach (var proj in projViews)
			{
				proj.DataChanged(false);
			}
		}

		private void TransportGrid(int Δx, int Δy)
		{
			void Modify(Line line)
			{
				line.X1 -= Δx * PixelPerPoint;
				line.Y1 += Δy * PixelPerPoint;
				line.X2 -= Δx * PixelPerPoint;
				line.Y2 += Δy * PixelPerPoint;
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
			Transformed();
		}
		#endregion
		#region Events
		private void CS_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Delete && FocusedView != null)
			{
				FocusedView = null;
				RemoveView(FocusedView);
			}
		}

		private void CSystem_MouseEnter(object sender, MouseEventArgs args)
		{
			MouseAxisVisibility = Visibility.Visible;
		}
		private void CSystem_MouseLeave(object sender, MouseEventArgs args)
		{
			MouseAxisVisibility = Visibility.Hidden;
		}
		private void CSystem_MouseMove(object sender, MouseEventArgs args)
		{
			MouseAxisPropChanged();
			var pos = (Vector)args.GetPosition(this);
			foreach(var view in projViews)
			{
				view.PView_MouseMoveEx(sender, pos);
			}
		}

		private void CS_MouseDown(object sender, MouseButtonEventArgs args)
		{
			if (FocusedView != null)
			{
				FocusedView.BorderBrush = null;
			}
			FocusedView = null;
			Keyboard.Focus(this);
		}
		private void CoordinateSystem_MouseWheel(object sender, MouseWheelEventArgs args)
		{
			var pos = args.MouseDevice.GetPosition(this);
			if (0 <= pos.X && pos.Y < Width && 0 <= pos.Y && pos.Y < Height)
			{
				var Δ = args.Delta / 120 * 16;
				if (!Keyboard.IsKeyDown(Key.LeftCtrl))
				{
					Transport(0, Δ);
				}
				else
				{
					Transport(Δ, 0);
				}
			}
		}

		private void CoordinateSystem_Loaded(object sender, EventArgs args)
		{
			// ProjView.LoadResources();
			var timer = new Timer(12);
			timer.Elapsed += (sender, args) =>
			{
				TickCount++;
				try
				{
					Dispatcher.Invoke(delegate
					{
						foreach (var view in projViews)
						{
							view.UpdateFrame(TickCount);
						}
					});
				}
				catch
				{

				}
			};
			timer.Start();
		}

		#endregion
		#region INotifyPropertyChanged
		private void OnFocusedViewChanged()
		{
			OnPropertyChanged(nameof(FVType));
			OnPropertyChanged(nameof(FVDamage));
			OnPropertyChanged(nameof(FVKnockback));
			OnPropertyChanged(nameof(FVPosition));
			OnPropertyChanged(nameof(FVSpeed));
			OnPropertyChanged(nameof(FVSpeedAngle));
			OnPropertyChanged(nameof(FVCD));
			OnPropertyChanged(nameof(FVLD));
		}
		private void MouseAxisPropChanged()
		{
			OnPropertyChanged(nameof(MouseAxisXX1));
			OnPropertyChanged(nameof(MouseAxisXX2));
			OnPropertyChanged(nameof(MouseAxisXY));
			OnPropertyChanged(nameof(MouseAxisYY1));
			OnPropertyChanged(nameof(MouseAxisYY2));
			OnPropertyChanged(nameof(MouseAxisYX));
			OnPropertyChanged(nameof(MouseAxisText));
			OnPropertyChanged(nameof(MouseAxisTextMargin));
		}
		public void OnPropertyChanged([CallerMemberName]string propName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
		public event PropertyChangedEventHandler PropertyChanged;
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
	}
}

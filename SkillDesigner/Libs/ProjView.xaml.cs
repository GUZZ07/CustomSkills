using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;

using WVector = System.Windows.Vector;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace SkillDesigner
{
	using Libs;
	public partial class ProjView : UserControl, INotifyPropertyChanged
	{
		private struct ProjViewData
		{
			public Vector Size;
			public int Frames;
			public double SpriteRotation;
			public bool NoRotation;
			public bool SpecialHeight;
		}
		private static ProjViewData[] ProjViewDatas;
		private static TextureManager textures;
		public static TextureManager Textures
		{
			get
			{
				LoadResources();
				return textures;
			}
		}

		public static bool LoadedResource
		{
			get;
			private set;
		}

		public static void LoadResources()
		{
			if (LoadedResource)
			{
				return;
			}
			textures = new TextureManager(Path.Combine(Environment.CurrentDirectory,"Projs"), 950);
			ProjViewDatas = JsonConvert.DeserializeObject<ProjViewData[]>(Resource.PDatas);

			ProjViewDatas[48].SpecialHeight = true;
			ProjViewDatas[636].SpecialHeight = true;

			int[] groupπover4 = { 21, 114, 115, 116, 132, 156, 157, 173, 263, 300, 301, 306, 451, 182, 697, 699, 707, 708, 938, 939, 940, 941, 942, 943, 944, 945 };
			int[] groupπover2 = { 1, 2, 4, 5, 7, 8, 14, 20, 23, 36, 41, 48, 54, 55, 57, 58, 59, 60, 61, 62, 81, 82, 83, 84, 88, 89, 91, 93, 98, 100, 103, 104, 107, 110, 117, 120, 150, 151, 152, 158, 159, 160, 161, 163, 167, 168, 169, 170, 172, 174, 176, 180, 184, 186, 195, 207, 213, 214, 216, 217, 219, 220, 223, 224, 225, 239, 242, 245, 246, 250, 251, 252, 257, 259, 262, 264, 265, 267, 271, 273, 275, 276, 278, 279, 282, 283, 284, 285, 286, 287, 302, 303, 304, 310, 311, 322, 336, 337, 338, 339, 340, 341, 343, 345, 348, 349, 350, 355, 369, 374, 389, 415, 416, 417, 418, 427, 428, 429, 430, 431, 432, 437, 438, 439, 440, 442, 445, 448, 452, 457, 458, 460, 461, 462, 474, 477, 478, 479, 480, 481, 491, 493, 494, 495, 497, 498, 502, 507, 508, 509, 514, 520, 536, 573, 576, 577, 583, 584, 591, 592, 598, 599, 600, 607, 609, 610, 636, 638, 639, 640, 661, 664, 666, 668, 671, 680, 684, 686, 710, 711, 715, 716, 717, 718, 719, 763, 776, 780, 784, 787, 790, 793, 796, 799, 802, 803, 804, 805, 806, 807, 808, 809, 810, 819, 864, 870, 876, 920, 930, 931, 933, 935 };
			int[] group3πover4 = { 46, 47, 49, 64, 66, 97, 105, 130, 153, 212, 342, 367, 368, 496, 662, 685, 730, 877, 878, 879 };
			int[] groupπ = { 227 };
			int[] group3πover2 = { 38, 631 };

			foreach (var value in groupπover4)
			{
				ProjViewDatas[value].SpriteRotation = -Math.PI / 4;
			}
			foreach (var value in groupπover2)
			{
				ProjViewDatas[value].SpriteRotation = -Math.PI / 2;
			}
			foreach (var value in group3πover4)
			{
				ProjViewDatas[value].SpriteRotation = -3 * Math.PI / 4;
			}
			foreach (var value in groupπ)
			{
				ProjViewDatas[value].SpriteRotation = -Math.PI;
			}
			foreach (var value in group3πover2)
			{
				ProjViewDatas[value].SpriteRotation = -3 * Math.PI / 2;
			}

			ProjViewDatas[238].NoRotation = true;
			ProjViewDatas[254].NoRotation = true;
			ProjViewDatas[384].NoRotation = true;
			ProjViewDatas[386].NoRotation = true;
			ProjViewDatas[423].NoRotation = true;
			ProjViewDatas[465].NoRotation = true;
			ProjViewDatas[696].NoRotation = true;

			#region Override Size
			static void OverrideSize(float? width, float? height, params int[] types)
			{
				foreach (var type in types)
				{
					ProjViewDatas[type].Size.X = width ?? ProjViewDatas[type].Size.X;
					ProjViewDatas[type].Size.Y = height ?? ProjViewDatas[type].Size.Y;
				}
			}
			static void ModifySize(Vector value, params int[] types)
			{
				foreach (var type in types)
				{
					ProjViewDatas[type].Size += value;
				}
			}

			OverrideSize(null, 34, 771, 822, 823, 843, 846, 845, 852);
			OverrideSize(null, 58, 824, 839, 840, 850, 853);
			OverrideSize(null, 38, 826, 830, 838);
			OverrideSize(null, 22, 828, 829, 827, 844);
			ModifySize((-8, -8), 28, 906, 903, 904, 910);
			OverrideSize(6, 6, 250, 267, 297, 323, 3, 711);
			OverrideSize(25, null, 308);
			OverrideSize(16, null, 663, 665, 667, 677, 678, 679, 691, 692, 693);
			OverrideSize(16, null, 688, 689, 690);
			OverrideSize(10, 10, 669, 706);
			OverrideSize(26, 26, 261, 277);
			OverrideSize(10, 10, 481, 491, 106, 262, 271, 270, 272, 273, 274, 280, 288, 301, 320, 333, 335, 343, 344, 497, 496, 6, 19, 113, 52, 520, 523, 585, 598, 599, 636, 837, 861, 867);
			OverrideSize(4, 4, 514);
			ModifySize((-12, -12), 248, 247, 507, 508, 662, 680, 685, 757, 928);
			ModifySize((-36, -36), 254);
			ModifySize((-20, -20), 182, 190, 33, 229, 237, 243, 866);
			ModifySize((6, 6), 533, 755);
			OverrideSize(8, 8, 582, 634, 635, 902);
			OverrideSize(20, 20, 617);
			OverrideSize(4, 4, 304);
			OverrideSize(4, 4, 931);
			#endregion
			LoadedResource = true;
		}

		public static void ReleaseResources()
		{
			Textures.Dispose();
		}

		public const int ProjCount = 950;


		private ProjData projData;
		private bool mouseDown;
		private bool canMove;
		private Vector? mouseDownPos;
		private Vector? mouseDownPosSelf;
		private int frame;

		public event PropertyChangedEventHandler PropertyChanged;

		public ProjData Data
		{
			get => projData;
			set => SetData(value);
		}
		public CoordinateSystem CSystem
		{
			get => (Parent as FrameworkElement)?.Parent as CoordinateSystem;
		}
		public Vector ProjSize => ProjViewDatas[Data.ProjType].Size;
		public ImageBrush Texture
		{
			get
			{
				if (Data == null)
				{
					return null;
				}
				var brush = new ImageBrush(Textures[Data.ProjType]);
				var viewbox = new Rect();

				viewbox.Width = 1;
				viewbox.Height = 1.0 / TextureData.Frames;
				viewbox.Y = 1.0 / TextureData.Frames * frame;
				brush.Stretch = Stretch.None;
				brush.Viewbox = viewbox;
				return brush;
			}
		}

		public double HitboxWidth
		{
			get => Data != null ? TextureData.Size.X : 0;
		}
		public double HitboxHeight
		{
			get => Data != null ? TextureData.Size.Y : 0;
		}

		public double TextureWidth
		{
			get => Data is null ? 0 : Textures[Data.ProjType].PixelWidth;
		}
		public double TextureHeight
		{
			get => Data is null ? 0 : Textures[Data.ProjType].PixelHeight / TextureData.Frames;
		}
		public Thickness ProjMargin
		{
			get
			{
				if (Data == null)
				{
					return default;
				}
				var offset = new Vector(TextureWidth, -TextureHeight) / 2;
				// offset.Angle += (TextureData.SpriteRotation + Data.SpeedAngle);
				var pos = CoordinateSystem.Transform(Data.Position - offset);
				return new Thickness(pos.X, pos.Y, 0, 0);
			}
		}

		private ProjViewData TextureData => ProjViewDatas[Data.ProjType];

		public bool NoRotation
		{
			get;
			set;
		}

		public ProjView() : this(null)
		{

		}
		public ProjView(ProjData data)
		{
			InitializeComponent();
			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;
			Data = data;
		}


		public void DataChanged(bool changeProjType)
		{
			var ppp = CoordinateSystem.PixelPerPoint;
			var handler = PropertyChanged;
			if (handler != null)
			{
				// handler(this, new PropertyChangedEventArgs(nameof(ProjTransform)));
				// handler(this, new PropertyChangedEventArgs(nameof(ProjRenderTransform)));
				if (changeProjType)
				{
					handler(this, new PropertyChangedEventArgs(nameof(TextureWidth)));
					handler(this, new PropertyChangedEventArgs(nameof(TextureHeight)));
					handler(this, new PropertyChangedEventArgs(nameof(Texture)));
					Hitbox.Width = HitboxWidth;
					Hitbox.Height = HitboxHeight;
					Hitbox.Margin = new Thickness(TextureWidth / 2 - HitboxWidth / 2, 0, 0, 0);
				}
				handler(this, new PropertyChangedEventArgs(nameof(ProjMargin)));

				Vector origin;
				if (TextureData.SpecialHeight)
				{
					origin = (0.5, (TextureData.Size.Y / TextureHeight) / 2);
				}
				else
				{
					origin = (0.5, 0.5);
				}
				Rotation.CenterX = origin.X * TextureWidth * ppp;
				Rotation.CenterY = origin.Y * TextureHeight * ppp;
				Translation.Y = (0.5 - origin.Y) * TextureHeight * ppp;

				double angle = TextureData.SpriteRotation;
				if (!TextureData.NoRotation)
				{
					angle += Data.SpeedAngle;
				}

				Scale.ScaleX = ppp;
				Scale.ScaleY = ppp;
				Rotation.Angle = -angle * 180 / Math.PI;
				// Background.RelativeTransform = ProjRenderTransform;
			}
		}
		public void SetData(ProjData data)
		{
			var oldData = projData;
			projData = data;
			DataChanged(oldData?.ProjType != data.ProjType);
		}
		public void UpdateFrame(in int timer)
		{
			if (timer % (30 / TextureData.Frames) == 0)
			{
				frame++;
				frame %= TextureData.Frames;
				var brush = Background as ImageBrush;
				var viewbox = new Rect();

				viewbox.Width = 1;
				viewbox.Height = 1.0 / TextureData.Frames;
				viewbox.Y = 1.0 / TextureData.Frames * frame;
				brush.Viewbox = viewbox;
			}
		}
		#region Events

		private void PView_LostFocus(object sender, RoutedEventArgs args)
		{
			if (CSystem?.FocusedView != this)
			{
				BorderBrush = null;
			}
		}

		private void PView_GotFocus(object sender, RoutedEventArgs args)
		{
			if (CSystem.FocusedView == this)
			{
				BorderBrush = Brushes.Yellow;
			}
		}
		public void PView_MouseMoveEx(object sender, Vector mousePos)
		{
			if (mouseDown && canMove && CSystem.FocusedView == this)
			{
				var delta = mousePos - (Vector)mouseDownPos;
				delta.Y *= -1;
				Data.Position = (Vector)mouseDownPosSelf + delta / CoordinateSystem.PixelPerPoint;
				DataChanged(false);
				CSystem.OnPropertyChanged(nameof(CSystem.FVPosition));
			}
		}
		private void PView_MouseDown(object sender, MouseEventArgs args)
		{
			if (CSystem.FocusedView == this)
			{
				canMove = true;
			}
			mouseDown = true;
			mouseDownPos = (Vector)args.GetPosition((IInputElement)Parent);
			mouseDownPosSelf = Data.Position;
			args.Handled = true;
		}
		private void PView_MouseUp(object sender, EventArgs args)
		{
			if (CSystem.FocusedView != null)
			{
				CSystem.FocusedView.BorderBrush = null;
			}
			CSystem.FocusedView = this;
			BorderBrush = Brushes.Red;
			canMove = false;
			mouseDown = false;
			mouseDownPos = null;
			mouseDownPosSelf = null;
		}
		private void PView_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			bool moved = false;
			if (Keyboard.IsKeyDown(Key.Up))
			{
				Data.Position += (0, 1);
				moved = true;
			}
			if (Keyboard.IsKeyDown(Key.Down))
			{
				Data.Position -= (0, 1);
				moved = true;
			}
			if (Keyboard.IsKeyDown(Key.Right))
			{
				Data.Position += (1, 0);
				moved = true;
			}
			if (Keyboard.IsKeyDown(Key.Left))
			{
				Data.Position -= (1, 0);
				moved = true;
			}
			if (moved)
			{
				DataChanged(false);
				CSystem.OnPropertyChanged(nameof(CSystem.FVPosition));
				args.Handled = true;
			}
			if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.C))
			{
				CSystem.ClipBoard = this;
			}
		}
		private void PView_PreviewKeyUp(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Delete)
			{
				CSystem.RemoveView(this);
				return;
			}
		}
		#endregion
	}
}

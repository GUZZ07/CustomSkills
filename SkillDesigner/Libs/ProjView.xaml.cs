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

namespace SkillDesigner.Libs
{
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
		private static TextureManager Textures;

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
			Textures = new TextureManager("Projs", 950);
			ProjViewDatas = JsonConvert.DeserializeObject<ProjViewData[]>(Resource.PDatas);

			ProjViewDatas[48].SpecialHeight = true;
			ProjViewDatas[636].SpecialHeight = true;

			ProjViewDatas[48].SpriteRotation = -Math.PI / 2;
			ProjViewDatas[348].SpriteRotation = -Math.PI / 2;
			ProjViewDatas[636].SpriteRotation = -Math.PI / 2;
			ProjViewDatas[639].SpriteRotation = -Math.PI / 2;

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
			get => (CoordinateSystem)((FrameworkElement)Parent).Parent;
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
			LoadResources();
			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;
			Data = data;
		}


		public void DataChanged(bool changeProjType)
		{
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
				Rotation.CenterX = origin.X * TextureWidth* CoordinateSystem.PixelPerPoint;
				Rotation.CenterY = origin.Y * TextureHeight* CoordinateSystem.PixelPerPoint;
				Translation.Y = (0.5 - origin.Y) * TextureHeight * CoordinateSystem.PixelPerPoint;

				double angle = TextureData.SpriteRotation;
				if (!TextureData.NoRotation)
				{
					angle += Data.SpeedAngle;
				}

				Scale.ScaleX = CoordinateSystem.PixelPerPoint;
				Scale.ScaleY = CoordinateSystem.PixelPerPoint;
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
			CSystem.FocusedView = this;
			BorderBrush = Brushes.Red;
			canMove = false;
			mouseDown = false;
			mouseDownPos = null;
			mouseDownPosSelf = null;
		}
		#endregion
	}
}

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
using System.Reflection.Metadata;

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

		public static bool HideHitbox
		{
			get;
			set;
		}
		public static bool HideTexture
		{
			get;
			set;
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
			Textures = new TextureManager("Projs", 950);
			ProjViewDatas = JsonConvert.DeserializeObject<ProjViewData[]>(Resource.PDatas);

			ProjViewDatas[48].SpecialHeight = true;
			ProjViewDatas[636].SpecialHeight = true;

			ProjViewDatas[48].SpriteRotation = -Math.PI / 2;
			ProjViewDatas[348].SpriteRotation = -Math.PI / 2;
			ProjViewDatas[636].SpriteRotation = -Math.PI / 2;

			ProjViewDatas[238].NoRotation = true;
			ProjViewDatas[254].NoRotation = true;
			ProjViewDatas[465].NoRotation = true;
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

		private int frame;
		private int t;
		private ProjData projData;

		public event PropertyChangedEventHandler PropertyChanged;

		public ProjData Data
		{
			get => projData;
			set => SetData(value);
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
				viewbox.Height = 1;
				viewbox.Width = 1 / TextureData.Frames;
				viewbox.Y = 1 / TextureData.Frames * frame;

				brush.Viewbox = viewbox;
				return brush;
			}
		}
		public double TextureWidth
		{
			get => Data is null ? 0 : Textures[Data.ProjType].PixelWidth;
		}
		public double TextureHeight
		{
			get => Data is null ? 0 : Textures[Data.ProjType].PixelHeight / TextureData.Frames;
		}
		public Transform ProjTransform
		{
			get
			{
				var matrix = Matrix.Identity;
				if (Data == null)
				{
					return new MatrixTransform(matrix);
				}
				if (!TextureData.NoRotation)
				{
					matrix.RotateAt(-TextureData.SpriteRotation * 180 / Math.PI, 0.5, 0.5);
					matrix.RotateAt(-Data.SpeedAngle * 180 / Math.PI, 0.5, 0.5);
					matrix.ScaleAt(CoordinateSystem.PixelPerPoint, CoordinateSystem.PixelPerPoint, 0.5, 0.5);
				}
				return new MatrixTransform(matrix);
			}
		}
		public Thickness ProjMargin
		{
			get
			{
				if (Data == null)
				{
					return default;
				}
				var size = ProjSize / 2; ;
				size.Y *= -1;
				var pos = Data.Position - size;
				pos = CoordinateSystem.Transform(pos);
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
			Data = data;
			VerticalAlignment = VerticalAlignment.Top;
			HorizontalAlignment = HorizontalAlignment.Left;
		}

		public void DataChanged(bool changeProjType)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(nameof(ProjTransform)));
				handler(this, new PropertyChangedEventArgs(nameof(ProjMargin)));
				handler(this, new PropertyChangedEventArgs(nameof(TextureWidth)));
				handler(this, new PropertyChangedEventArgs(nameof(TextureHeight)));
				if (changeProjType)
				{
					handler(this, new PropertyChangedEventArgs(nameof(Texture)));
				}
			}
		}
		public void SetData(ProjData data)
		{
			var oldData = projData;
			projData = data;
			DataChanged(oldData?.ProjType != data.ProjType);
		}


#if false
		public void Draw(Graphics graphics, CoordinateSystem coordinateSystem, Brush brush = null)
		{
			t++;
			if (t % (30 / TextureData.Frames) == 0)
			{
				frame++;
				frame %= TextureData.Frames;
			}
			var texture = Textures[Data.ProjType];
			var ppp = coordinateSystem.PixelPerPoint;
			if (!TextureData.NoRotation)
			{
				var matrix = graphics.Transform;
				var sr = -(float)(TextureData.SpriteRotation * 180 / Math.PI);
				matrix.RotateAt(sr, coordinateSystem.Transform(Data.Position));
				matrix.RotateAt(-Data.SpeedAngle / MathF.PI * 180, coordinateSystem.Transform(Data.Position));
				graphics.Transform = matrix;
			}
#region DrawHitbox
			if (!HideHitbox)
			{
				graphics.FillRectangle(brush, GetRect(coordinateSystem));
			}
#endregion
#region DrawTexture
			if (!HideTexture)
			{
				var pos = coordinateSystem.Transform(Data.Position) - new Vector(texture.Width, TextureData.SpecialHeight ? ProjSize.Y : texture.Height / TextureData.Frames) * ppp / 2f;
				if (TextureData.Frames == 1)
				{
					pos.X += ppp / 2f;
				}
				var srcRect = new RectangleF(0, frame * texture.Height / TextureData.Frames, texture.Width, texture.Height / TextureData.Frames);
				var destRect = new RectangleF(pos, srcRect.Size * ppp);
				graphics.DrawImage(texture, destRect, srcRect, GraphicsUnit.Pixel);
			}
#endregion
			graphics.ResetTransform();
			if (!NoRotation)
			{
				Data.SpeedAngle += MathF.PI / 180;
			}
		}
#endif
	}
}

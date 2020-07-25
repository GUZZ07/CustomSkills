using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkillDesigner.Libs
{
	public class TextureManager : IDisposable
	{
		private string directory;
		private ImageDrawing[] textures;
		public int Count
		{
			get => textures.Length;
		}
		public ImageDrawing this[int i]
		{
			get
			{
				if (textures[i] == null)
				{
					LoadProj(i);
				}
				return textures[i];
			}
		}
		public TextureManager(string path, int count)
		{
			textures = new ImageDrawing[count];
			directory = path;
		}
		private void LoadProj(int i)
		{
			var bmp = new BitmapImage(new Uri(Path.Combine(directory, i + ".png"), UriKind.Relative));
			textures[i] = new ImageDrawing(bmp, new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
		}

		public void Dispose()
		{
			foreach(var texture in textures)
			{

			}
		}
	}
}

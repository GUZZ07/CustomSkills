using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkillDesigner
{
	public class TextureManager : IDisposable
	{
		private string directory;
		private BitmapImage[] textures;
		private ImageSourceConverter converter;
		public int Count
		{
			get => textures.Length;
		}
		public BitmapImage this[int i]
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
			textures = new BitmapImage[count];
			converter = new ImageSourceConverter();
			directory = path;
		}
		private void LoadProj(int i)
		{
			textures[i] = new BitmapImage(new Uri(Path.Combine(directory, i.ToString() + ".png"), UriKind.Absolute));
		}

		public void Dispose()
		{
			foreach(var texture in textures)
			{

			}
		}
	}
}

using System;
using System.Drawing;
using System.IO;

namespace SkillDesigner.Libs
{
	public class TextureManager : IDisposable
	{
		private string directory;
		private Bitmap[] textures;
		public int Count
		{
			get => textures.Length;
		}
		public Bitmap this[int i]
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
			textures = new Bitmap[count];
			directory = path;
		}
		private void LoadProj(int i)
		{
			textures[i] = new Bitmap(Path.Combine(directory, i + ".png"));
		}

		public void Dispose()
		{
			foreach(var texture in textures)
			{
				texture?.Dispose();
			}
		}
	}
}

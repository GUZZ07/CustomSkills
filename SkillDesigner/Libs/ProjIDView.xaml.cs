using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkillDesigner
{
	/// <summary>
	/// ProjIDView.xaml 的交互逻辑
	/// </summary>
	public partial class ProjIDView : UserControl, INotifyPropertyChanged
	{
		private int projID;

		public event PropertyChangedEventHandler PropertyChanged;

		public int ProjID
		{
			get
			{
				return projID;
			}
			set
			{
				projID = value;
				OnPropertyChanged(nameof(ProjID));
				OnPropertyChanged(nameof(Texture));
			}
		}

		public ImageSource Texture
		{
			get => ProjView.Textures[projID];
		}

		public ProjIDView()
		{
			InitializeComponent();
		}

		private void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}

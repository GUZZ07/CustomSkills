using SkillDesigner.Libs;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SkillDesigner
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private ImageDrawing image;
		public static MainWindow Instance
		{
			get;
			private set;
		}
		//public CoordinateSystem CSystem { get; }
		public MainWindow()
		{
			Instance = this;
			InitializeComponent();
			//CSystem = new CoordinateSystem();
			//Grid.Children.Add(CSystem);
			//var bmp = new BitmapImage(new Uri("Background.png", UriKind.Relative));
			//image = new ImageDrawing(bmp, new Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
		}

		private void ZoomUp_Click(object sender, RoutedEventArgs args)
		{
			CSystem.ZoomUp2();
		}

		private void ZoomDown_Click(object sender, RoutedEventArgs args)
		{
			CSystem.ZoomDown2();
		}
	}
}

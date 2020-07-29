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
	/// ProjIDView.xaml 的交互逻辑
	/// </summary>
	public partial class ProjIDViewer : Window
	{
		public int? SelectedID
		{
			get;
			private set;
		}
		public ProjIDViewer()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();
			AddViews();
		}

		private void AddViews()
		{
			for (int i = 1; i < ProjView.ProjCount; i++)
			{
				var view = new ProjIDView();
				view.ProjID = i;
				view.Margin = new Thickness(0, view.Height * ((i - 1) / 3), 0, 0);
				view.MouseDoubleClick += View_MouseDoubleClick;
				Grid.Children.Add(view);
				Grid.SetColumn(view, (i - 1) % 3);
			}
		}

		private void View_MouseDoubleClick(object sender, MouseButtonEventArgs args)
		{
			SelectedID = (sender as ProjIDView).ProjID;
			Close();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}
	}
}

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
using System.Windows.Shapes;

namespace SkillDesigner.Libs
{
	/// <summary>
	/// AdvancedPropViewer.xaml 的交互逻辑
	/// </summary>
	public partial class AdvancedPropViewer : Window, INotifyPropertyChanged
	{
		public bool RotateWithPlayer
		{
			get
			{
				return MainWindow.Instance.CSystem.FocusedView?.Data.RotateWithPlayer ?? false;
			}
			set
			{
				var data = MainWindow.Instance.CSystem.FocusedView?.Data;
				if (data != null)
				{
					data.RotateWithPlayer = value;
				}
			}
		}
		public AdvancedPropViewer()
		{
			InitializeComponent();
			var view = MainWindow.Instance.CSystem.FocusedView;
			LoadPanel(view);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void Border_MouseDown(object sender, MouseButtonEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}

		private void AddNew_Click(object sender, RoutedEventArgs args)
		{
			var view = new ProjShiftView();
			Panel.Children.Add(view);
			view.Delay = 0;
		}

		private void Complete_Click(object sender, RoutedEventArgs args)
		{
			var view = MainWindow.Instance.CSystem.FocusedView;
			if (view != null)
			{
				Array.Resize(ref view.Data.Shifts, Panel.Children.Count);
				for (int i = 0; i < view.Data.Shifts.Length; i++)
				{
					var psView = (ProjShiftView)Panel.Children[i];
					view.Data.Shifts[i] = new ProjShift
					{
						Velocity = psView.Velocity,
						Delay = psView.Delay
					};
				}
			}
			Close();
		}

		private void LoadPanel(ProjView view)
		{
			Panel.Children.Clear();
			for (int i = 0; i < view.Data.Shifts.Length; i++)
			{
				var psView = new ProjShiftView();
				psView.Velocity = view.Data.Shifts[i].Velocity;
				Panel.Children.Add(psView);
				psView.Delay = view.Data.Shifts[i].Delay;
			}
		}

		public void ViewChanged(ProjView view)
		{
			if (view == null)
			{
				Panel.Children.Clear();
			}
			else
			{
				LoadPanel(view);
			}
			OnPropertyChanged(nameof(RotateWithPlayer));
		}

		private void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		private void APViewer_Closed(object sender, EventArgs e)
		{
			MainWindow.Instance.AdvancedViewer = null;
		}
	}
}

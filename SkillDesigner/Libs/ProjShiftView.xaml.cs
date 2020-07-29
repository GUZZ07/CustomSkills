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

namespace SkillDesigner.Libs
{
	/// <summary>
	/// ProjShiftView.xaml 的交互逻辑
	/// </summary>
	public partial class ProjShiftView : UserControl, INotifyPropertyChanged
	{
		private Vector velocity;
		private int delay;
		public Vector Velocity
		{
			get => velocity;
			set
			{
				velocity = value;
				OnPropertyChanged(nameof(VelocityView));
			}
		}
		public int Delay
		{
			get => delay;
			set
			{
				delay = value;
				OnPropertyChanged(nameof(DelayView));
				SortPanel((Panel)Parent);
			}
		}
		public Vector VelocityView
		{
			get
			{
				return Velocity * 60 / 16;
			}
			set
			{
				Velocity = value * 16 / 60;
			}
		}
		public int DelayView
		{
			get
			{
				return Delay;
			}
			set
			{
				Delay = value;
			}
		}
		public ProjShiftView()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		private static void SortPanel(Panel panel)
		{
			var arr = new UIElement[panel.Children.Count];
			panel.Children.CopyTo(arr, 0);
			Array.Sort(arr, (l, r) => (l as ProjShiftView).Delay - (r as ProjShiftView).Delay);
			panel.Children.Clear();
			foreach (var item in arr)
			{
				panel.Children.Add(item);
			}
#if false
			void Sort(int begin, int end)
			{
				if (begin >= end)
				{
					return;
				}
				var middle = (begin + end) / 2;
				var midValue = arr[middle] as ProjShiftView;
				int l = begin;
				int r = end;
				while (l < r)
				{
					while ((arr[l] as ProjShiftView).Delay < midValue.Delay)
					{
						l++;
					}
					while (midValue.Delay < (arr[r] as ProjShiftView).Delay)
					{
						r--;
					}
					if (l <= r)
					{
						var value = arr[l];
						arr[l] = arr[r];
						arr[r] = value;
						l++;
						r--;
					}
				}
				Sort(begin, r);
				Sort(l, end);
			}
			Sort(0, arr.Count - 1);
#endif
		}

		private void PSView_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Delete)
			{
				(Parent as Panel).Children.Remove(this);
			}
		}

		private void DVTextBox_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Enter)
			{
				VVTextBox.Focus();
				Keyboard.ClearFocus();
			}
		}

		private void VVTextBox_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Enter)
			{
				DVTextBox.Focus();
				Keyboard.ClearFocus();
			}
		}
	}
}

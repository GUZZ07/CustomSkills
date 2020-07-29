using SkillDesigner.Libs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
	/// SkillInfoDialog.xaml 的交互逻辑
	/// </summary>
	public partial class SkillInfoDialog : Window, INotifyPropertyChanged
	{
		public string SkillName
		{
			get;
			set;
		}
		public string Author
		{
			get;
			set;
		}
		public string SkillDescription
		{
			get;
			set;
		}
		public int CoolDown
		{
			get;
			set;
		}
		public bool Yes
		{
			get;
			private set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public SkillInfoDialog()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();
		}


		private void OK_Click(object sender, RoutedEventArgs args)
		{
			if (string.IsNullOrEmpty(SkillName))
			{
				MyMessageBox.Show("请填写技能名", "提示");
				return;
			}
			Yes = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs args)
		{
			Close();
		}

		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Enter)
			{
				Keyboard.Focus(Hahahaha);
			}
		}

		private void Dialog_MouseDown(object sender, MouseButtonEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}

		public void OnPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
}

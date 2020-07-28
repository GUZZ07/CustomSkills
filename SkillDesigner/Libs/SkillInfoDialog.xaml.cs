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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkillDesigner.Libs
{
	/// <summary>
	/// SkillInfoDialog.xaml 的交互逻辑
	/// </summary>
	public partial class SkillInfoDialog : Window
	{
		public string SkillName
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

		public SkillInfoDialog()
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			InitializeComponent();
		}

		private void OK_Click(object sender, RoutedEventArgs args)
		{
			if (string.IsNullOrEmpty(SkillName))
			{
				MessageBox.Show("请填写技能名");
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

		private void Dialog_MouseDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
	}
}

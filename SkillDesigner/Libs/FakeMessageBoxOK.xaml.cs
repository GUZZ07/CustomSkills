﻿using System;
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

namespace SkillDesigner.Libs
{
	/// <summary>
	/// FakeMessageBoxOK.xaml 的交互逻辑
	/// </summary>
	public partial class FakeMessageBoxOK : Window
	{
		public MessageBoxResult Result
		{
			get;
			private set;
		}
		public FakeMessageBoxOK()
		{
			Result = MessageBoxResult.None;
			InitializeComponent();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.OK;
			Close();
		}

		private void DragMove_MouseDown(object sender, MouseButtonEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}
	}
}

using Microsoft.Win32;
using Newtonsoft.Json;
using SkillDesigner.Libs;
using System;
using System.Collections.Generic;
using System.IO;
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
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

		private void AddProj_Click(object sender, RoutedEventArgs args)
		{
			CSystem.AddNewView();
		}

		private void ClearProj_Click(object sender, RoutedEventArgs args)
		{
			if (MessageBox.Show("确定要清除?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				CSystem.ClearViews();
			}
		}

		private void PropView_PreKeydown(object sender, KeyEventArgs args)
		{
			if (args.Key == Key.Enter)
			{
				Hahahaha.Focus();
			}
		}

		private void DumpProjs_Click(object sender, RoutedEventArgs args)
		{
			var dialog = new SkillInfoDialog();
			dialog.ShowDialog();
			if (dialog.Yes)
			{
				var dlg = new SaveFileDialog
				{
					CheckFileExists = false,
					Title = "导出技能",
					Filter = "json文件|*.json",
					FileName = dialog.SkillName + ".json",
					DefaultExt = ".json",
					AddExtension = true
				};
				if (dlg.ShowDialog() == true)
				{
					SkillData skilldata = new SkillData
					{
						SkillName = dialog.SkillName,
						CoolDown = dialog.CoolDown,
						ProjDatas = CSystem.ExportDatas()
					};
					var text = JsonConvert.SerializeObject(skilldata, Formatting.Indented);
					File.WriteAllText(dlg.FileName, text);
				}
			}
		}

		private void ImportProjs_Click(object sender, RoutedEventArgs args)
		{
			var dlg = new OpenFileDialog();
			dlg.CheckFileExists = true;
			dlg.Title = "导入技能";
			dlg.Filter = "json文件|*.json";
			dlg.FileName = "Skill.json";
			dlg.DefaultExt = ".json";
			dlg.AddExtension = true;
			dlg.Multiselect = false;
			if (dlg.ShowDialog() == true)
			{
				try
				{
					var text = File.ReadAllText(dlg.FileName);
					var skilldata = JsonConvert.DeserializeObject<SkillData>(text);
					CSystem.ImportDatas(skilldata.ProjDatas);
				}
				catch
				{
					MessageBox.Show("导入失败");
				}
			}
		}
	}
}

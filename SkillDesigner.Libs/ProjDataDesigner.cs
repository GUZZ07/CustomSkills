using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace SkillDesigner.Libs
{
	public partial class ProjDataDesigner : UserControl
	{
		#region Types
		private class PropertyView
		{
			private Label tip;
			private TextBox value;
			private Func<ProjData, string> updateText;
			public bool Locked { get; set; }
			public PropertyView(ProjDataDesigner owner, Point location, Size size, string name, Action<TextBox, string> onValueChange, Func<ProjData, string> updateText, string toolTip = "")
			{
				this.updateText = updateText;
				tip = new Label
				{
					AutoSize = false,
					Location = location + new Size(2, 0),
					Text = name,
					Size = new Size(size.Width / 3 - 4, size.Height),
					TextAlign = ContentAlignment.MiddleCenter,
					Font = new Font(FontFamily.GenericSansSerif, 9),
					BorderStyle = BorderStyle.None,
					ForeColor = Color.Blue,
				};
				tip.Paint += (sender, args) =>
				{
					args.Graphics.DrawRectangle(Pens.White, 1, 1, tip.Width - 2, tip.Height - 2);
				};
				owner.Controls.Add(tip);
				value = new TextBox()
				{
					Location = location + new Size(4 + 4 + tip.Width, 0),
					Size = new Size(size.Width * 2 / 3 - 4 * 2, size.Height),
					Multiline = true,
					TextAlign = HorizontalAlignment.Center,
					BackColor = Color.Aqua,
					BorderStyle = BorderStyle.Fixed3D
				};
				value.TextChanged += (sender, args) =>
				{
					if (!Locked)
					{
						Locked = true;
						onValueChange(value, value.Text);
						Locked = false;
					}
				};
				owner.Controls.Add(value);
			}
			public void Update(ProjData newValue)
			{
				value.Text = updateText(newValue);
			}
		}
		#endregion
		private ProjData data;
		private List<PropertyView> views;
		public ProjData Data
		{
			get
			{
				return data;
			}
			set
			{
				data = null;
				foreach (var view in views)
				{
					view.Update(value);
				}
				data = value;
			}
		}
		public ProjDataDesigner()
		{
			InitializeComponent();
			var size = new Size(Size.Width, (Size.Height - 2 * 9) / 8);
			var offset = new Size(0, (Size.Height - 2 * 9) / 8 + 2);
			var location = new Point(0, 2);
			views = new List<PropertyView>
			{
				new PropertyView(this, location + offset * 0, size, "生成延时", CDTextChange, value => value.CreateDelay.ToString()),
				new PropertyView(this, location + offset * 1, size, "发射延时", LDTextChange, value => value.LaunchDelay.ToString()),
				new PropertyView(this, location + offset * 2, size, "  击退  ", KBTextChange, value => value.Knockback.ToString()),
				new PropertyView(this, location + offset * 3, size, "  伤害  ", DamageTextChange, value => value.Damage.ToString()),
				new PropertyView(this, location + offset * 4, size, "弹幕类型", PTTextChange, value => value.ProjType.ToString()),
				new PropertyView(this, location + offset * 5, size, "生成位置", PosTextChange, value => value.Position.ToString("0.000")),
				new PropertyView(this, location + offset * 6, size, "  速率  ", SpeedTextChange, value => value.Speed.ToString("0.000")),
				new PropertyView(this, location + offset * 7, size, " 速度角 ", SATextChange, value => (value.SpeedAngle * 180 / MathF.PI).ToString("0.0"))
			};
		}

		#region XXChange
		private void CDTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (int.TryParse(text, out int newCD))
			{
				data.CreateDelay = newCD;
			}
			else
			{
				tb.Text = data.CreateDelay.ToString();
			}
		}
		private void LDTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (int.TryParse(text, out int newLD))
			{
				data.LaunchDelay = newLD;
			}
			else
			{
				tb.Text = data.LaunchDelay.ToString();
			}
		}
		private void KBTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (int.TryParse(text, out int newKN))
			{
				data.Knockback = newKN;
			}
			else
			{
				tb.Text = data.Knockback.ToString();
			}
		}
		private void DamageTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (int.TryParse(text, out int damage))
			{
				data.Damage = damage;
			}
			else
			{
				tb.Text = data.Damage.ToString();
			}
		}
		private void PTTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (int.TryParse(text, out int projType))
			{
				data.ProjType = projType;
			}
			else
			{
				tb.Text = data.ProjType.ToString();
			}
		}
		private void PosTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			var xy = text.Split(',');
			if (float.TryParse(xy[0], out var x) && float.TryParse(xy[1], out var y))
			{
				data.Position = new Vector(x, y);
			}
			else
			{
				tb.Text = data.Position.ToString("0.000");
			}
		}
		private void SpeedTextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (float.TryParse(text, out var speed))
			{
				data.Speed = speed;
			}
			else
			{
				tb.Text = data.Speed.ToString("0.000");
			}
		}
		private void SATextChange(TextBox tb, string text)
		{
			if (data == null)
			{
				return;
			}
			if (float.TryParse(text, out var speedAngle))
			{
				data.SpeedAngle = speedAngle * MathF.PI / 180;
			}
			else
			{
				tb.Text = (data.SpeedAngle * 180 / MathF.PI).ToString("0.0");
			}
		}
		#endregion
	}
}

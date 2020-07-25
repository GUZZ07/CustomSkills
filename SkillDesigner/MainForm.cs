using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkillDesigner
{
    using Libs;
	using System.Runtime.InteropServices;

	public partial class MainForm : Form
    {
        private Timer timer;
        public ProjDataDesigner Designer { get; }
        public CoordinateSystem CoordinateSystem { get; }
        public MainForm()
        {
            InitializeComponent();
            #region Coordinate System
            CoordinateSystem = new CoordinateSystem();
            Controls.Add(CoordinateSystem);
            #endregion
            #region ZoomUp
            var zoomUp = ControlCreator.CreateButton();
            zoomUp.Text = "+";
            zoomUp.Location = new Point()
            {
                X = CoordinateSystem.Width + (Width - CoordinateSystem.Width) / 2 * 3 / 2 - zoomUp.Width / 2,
                Y = ClientSize.Height - zoomUp.Height - 25 - zoomUp.Height - 1
            };
            zoomUp.MouseClick += (sender, args) =>
            {
                CoordinateSystem.ZoomUp2();
            };
            Controls.Add(zoomUp);
            #endregion
            #region ZoomDown
            var zoomDown = ControlCreator.CreateButton();
            zoomDown.Text = "-";
            zoomDown.Location = new Point()
            {
                X = CoordinateSystem.Width + (Width - CoordinateSystem.Width) / 2 * 3 / 2 - zoomDown.Width / 2,
                Y = ClientSize.Height  - zoomDown.Height - 25
            };
            zoomDown.MouseClick += (sender, args) =>
            {
                CoordinateSystem.ZoomDown2();
            };
            Controls.Add(zoomDown);
            #endregion
            #region HideHitbox
            var hideHitbox = ControlCreator.CreateCheckBox();
            hideHitbox.Text = "隐藏碰撞箱";
            hideHitbox.CheckedChanged += (sender, args) => ProjView.HideHitbox = hideHitbox.Checked;
            hideHitbox.Location = new Point()
            {
                X = CoordinateSystem.Width + (Width - CoordinateSystem.Width) / 2 / 2 - zoomUp.Width / 2,
                Y = zoomUp.Location.Y + (zoomUp.Height - hideHitbox.Height) / 2
            };
            Controls.Add(hideHitbox);
            #endregion
            #region HideTexture
            var hideTexture = ControlCreator.CreateCheckBox();
            hideTexture.Text = "隐藏贴图";
            hideTexture.CheckedChanged += (sender, args) => ProjView.HideTexture = hideTexture.Checked;
            hideTexture.Location = new Point()
            {
                X = CoordinateSystem.Width + (Width - CoordinateSystem.Width) / 2 / 2 - zoomDown.Width / 2,
                Y = zoomDown.Location.Y + (zoomDown.Height - hideTexture.Height) / 2
            };
            Controls.Add(hideTexture);
            #endregion
            #region ProjDataDesigner
            Designer = new ProjDataDesigner();
            Designer.Location = new Point(320 * 2, 0);
            Controls.Add(Designer);
			#endregion
			#region Self
			MouseWheel += MouseWheelHandler;
            Load += LoadHandler;
            var screen = Screen.PrimaryScreen.WorkingArea;
            DesktopLocation = new Point(screen.Width / 2 - Width / 2, screen.Height / 2 - Height / 2);
            #endregion
        }

        #region Events
        private void LoadHandler(object sender, EventArgs args)
        {
            #region Timer
            timer = new Timer()
            {
                Interval = 1000 / 60
            };
            timer.Tick += Timer_Tick;
            timer.Start();
            #endregion
        }

        private void MouseWheelHandler(object sender, MouseEventArgs args)
		{
            var pos = MousePosition;
            var relative = PointToScreen(CoordinateSystem.Location);
            pos.X -= relative.X;
            pos.Y -= relative.Y;
            var point = CoordinateSystem.ReversedTransform(pos.X, pos.Y);
            if (CoordinateSystem.InRange(point))
            {
                var Δ = args.Delta / 120 * 16;
                if (!NativeMethods.IsKeyDown(Keys.LControlKey))
                {
                    CoordinateSystem.Transport(0, Δ);
                }
                else
				{
                    CoordinateSystem.Transport(-Δ, 0);

                }
            }
		}

		private void Timer_Tick(object sender, EventArgs args)
        {
            using (var graphics = CoordinateSystem.CreateGraphics())
            {
                CoordinateSystem.Draw(graphics);
            }
        }
        #endregion
        #region Controls
        private static class ControlCreator
		{
            public static CheckBox CreateCheckBox()
			{
                var cb = new CheckBox();
                cb.Size = new Size(100, 40);
                cb.CheckAlign = ContentAlignment.MiddleLeft;
                return cb;
			}
            public static Button CreateButton()
			{
                var button = new Button
                {
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "emmm",
                    Size = new Size(88, 44),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(170, 0, 255, 255),
                };
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(130, Color.White);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, Color.Cyan);
                button.FlatAppearance.BorderColor = Color.White;
                button.FlatAppearance.BorderSize = 1;
                return button;
            }
		}
		#endregion
		#region Natives
		private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern short GetKeyState(Keys key);
            public static bool IsKeyDown(Keys key)
            {
                return (GetKeyState(key) & 0b100000000000000000000000000000000) != 0;
            }
        }
        #endregion
    }
}

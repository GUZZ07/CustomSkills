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
        public CoordinateSystem CoordinateSystem { get; }
        public MainForm()
        {
            InitializeComponent();
            CoordinateSystem = new CoordinateSystem();
            Controls.Add(CoordinateSystem);

            timer = new Timer()
            {
                Interval = 1000 / 60
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            MouseWheel += MouseWheenHandler;

        }

		private void MouseWheenHandler(object sender, MouseEventArgs args)
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
                    CoordinateSystem.Transport(Δ, 0);

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

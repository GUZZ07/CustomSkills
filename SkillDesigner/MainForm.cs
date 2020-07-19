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
    public partial class MainForm : Form
    {
        private Timer timer;
        public CoordinateSystem CoordinateSystem { get; }
        public MainForm()
        {
            InitializeComponent();
            CoordinateSystem = new CoordinateSystem();
            Controls.Add(CoordinateSystem);

            var timer = new Timer()
            {
                Interval = 1000 / 60
            };
            timer.Tick += (sender, args) =>
            {
                using var graphics = CoordinateSystem.CreateGraphics();
                CoordinateSystem.Draw(graphics);
            };
            timer.Start();
        }

    }
}

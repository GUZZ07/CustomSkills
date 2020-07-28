using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Microsoft.Windows.Themes
{
    internal class Emmmmmmmmmmmmmm { }
}
namespace SkillDesigner
{
	public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern short GetKeyState(Key key);
        public static bool IsKeyDown(Key key)
        {
            return (GetKeyState(key) & 0b100000000000000000000000000000000) != 0;
        }
    }
}

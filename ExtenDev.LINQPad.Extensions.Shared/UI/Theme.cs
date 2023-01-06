using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.UI
{
    // TODO: Add XML comments to all members
    public static class Theme
    {
        // TODO: Implement theme mechanism and use colors from theme throughout extensions instead of hardcoded strings
        public static ITheme Current { get; set; } = Util.IsDarkThemeEnabled ? new DarkTheme() : new LightTheme();
    }
}

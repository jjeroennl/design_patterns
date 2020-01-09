using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace vs_plugin.Code
{
    public static class UIHandler
    {
        public static ToolWindow1Control ToolWindow1Control { get; set; }
        public static SinglePattern SinglePattern { get; set; }

        public static void SummarySelection(Expander expander)
        {
            ToolWindow1Control.UpdateSummary(expander.Name);
        }
    }
}

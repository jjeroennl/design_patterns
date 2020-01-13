using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using idetector.Models;

namespace vs_plugin.Code
{
    public static class UIHandler
    {
        public static ToolWindow1Control ToolWindow1Control { get; set; }
        public static SinglePattern SinglePattern { get; set; }

        /// <summary>
        /// Calls main page to update summary based on selected item.
        /// </summary>
        /// <param name="expander"></param>
        public static void SummarySelection(string pattern, PatternRequirement req, bool passed)
        {
            ToolWindow1Control.UpdateSummary(pattern, req, passed);
        }
    }
}

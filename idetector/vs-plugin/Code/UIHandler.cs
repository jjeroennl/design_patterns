using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace vs_plugin.Code
{
    public static class UIHandler
    {
        public static bool IsLight { get; set; } = true;
        public static List<Control> ControlItems = new List<Control>();

        public static void SwitchMode()
        {
            SolidColorBrush color = (IsLight) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);

            foreach (var item in ControlItems)
            {
                if (item is Label)
                {
                    Label label = (Label)item;
                    if (label.Name != "ConditionIcon")
                        item.Foreground = color;
                }
                else
                    item.Foreground = color;
            }

            IsLight = !IsLight;
        }
    }
}

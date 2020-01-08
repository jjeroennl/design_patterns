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
        public static List<StackPanel> StackPanels = new List<StackPanel>();

        public static void SwitchMode()
        {
            SolidColorBrush color = (IsLight) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
            
            foreach (StackPanel stackPanel in StackPanels)
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is Control)
                    {
                        Control control = (Control)item;
                        if (control is Label)
                        {
                            Label label = (Label)control;
                            if (label.Name != "ConditionIcon")
                                control.Foreground = color;
                        }
                        else
                            control.Foreground = color;
                    }
                }
            }

            IsLight = !IsLight;
        }
    }
}

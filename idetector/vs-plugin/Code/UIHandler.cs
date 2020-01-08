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
        public static bool IsLight { get; set; } = true;
        public static List<UIElement> ControlItems = new List<UIElement>();

        public static void SwitchMode()
        {
            SolidColorBrush color = (IsLight) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);

            foreach (var item in ControlItems)
            {
                if (item is Control)
                {
                    Control control = (Control)item;
                    if (control is Label)
                    {
                        Label label = (Label)control;
                        if (label.Name != "ConditionIcon")
                            label.Foreground = color;
                    }
                    else
                        control.Foreground = color;
                }
                else
                {
                    if (item is TextBlock)
                    {
                        TextBlock textBlock = (TextBlock)item;
                        textBlock.Foreground = color;
                    }
                }

                
            }

            IsLight = !IsLight;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using vs_plugin.Code;

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for SingleClass.xaml
    /// </summary>
    public partial class SingleClass : UserControl
    {
        public SingleClass()
        {
            InitializeComponent();
        }

        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;
            UIHandler.ControlItems.Add(textBlock);
            ClassName.Header = textBlock;
        }
    }
}

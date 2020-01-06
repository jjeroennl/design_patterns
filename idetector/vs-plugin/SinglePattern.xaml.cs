using idetector.Models;
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

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for SinglePattern.xaml
    /// </summary>
    public partial class SinglePattern : UserControl
    {
        private string Test;

        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;

            PatternName1.Header = textBlock;
        }

        public SinglePattern()
        {
            InitializeComponent();
        }

        private void Collapse(object sender, RoutedEventArgs e)
        {

        }

        private void Expand(object sender, RoutedEventArgs e)
        {
        }

        internal void AddClass(ClassModel classobj)
        {
            var singleclass = new SingleClass();
            singleclass.SetHandle(classobj.Identifier);
            ClassList.Children.Add(singleclass);
        }
    }
}

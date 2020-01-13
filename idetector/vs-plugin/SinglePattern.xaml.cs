using idetector.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for SinglePattern.xaml
    /// </summary>
    public partial class SinglePattern : UserControl
    {
        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;
            PatternName1.Name = handle;
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

        public void AddPattern(Pattern pattern)
        {
            var singleclass = new UIPattern();
            singleclass.SetHandle(pattern.Class.Identifier);
            singleclass.SetScore(pattern.Score);
            singleclass.SetRequirements(pattern.RequirementResults);

            FoundList.Children.Add(singleclass);
        }
    }
}

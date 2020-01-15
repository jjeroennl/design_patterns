using idetector.Models;
using System.Windows;
using System.Windows.Controls;

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

        public void AddPattern(string patternName, Pattern pattern)
        {
            var singleclass = new UIPattern();
            singleclass.SetHandle(pattern.Class.Identifier);
            singleclass.SetScore(pattern.Score);
            singleclass.SetRequirements(patternName, pattern.RequirementResults);

            FoundList.Children.Add(singleclass);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using idetector.Models;
using vs_plugin.Code;

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for UIPattern.xaml
    /// </summary>
    public partial class UIPattern : UserControl
    {
        public UIPattern()
        {
            InitializeComponent();
            PatternName.PreviewMouseLeftButtonDown += pattern_PreviewMouseDown;
        }

        private void pattern_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHandler.SummarySelection((sender as Expander));
        }

        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;
            PatternName.Header = textBlock;
        }

        internal void SetScore(int score)
        {
            this.ScoreBlock.Text = score.ToString();
        }

        internal void SetRequirements(List<RequirementResult> requirementResults)
        {
            foreach (var result in requirementResults)
            {
                var req = new Requirement();
                req.SetRequirement(result);
                this.RequirementList.Children.Add(req);
            }
        }
    }
}

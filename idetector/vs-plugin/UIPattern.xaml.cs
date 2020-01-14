using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        }


        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;
            PatternName.Header = textBlock;
        }

        internal void SetScore(int score)
        {
            this.ScoreBlock.Text = score.ToString() +"%";
            if (score > 70)
            {
                this.ScoreBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            }
            else if(score > 50)
            {
                this.ScoreBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 127, 80));
            }
            else
            {
                this.ScoreBlock.Foreground = new SolidColorBrush(Color.FromRgb(128, 0, 0));
            }
        }

        internal void SetScore(bool score)
        {
            this.ScoreBlock.Text = "";
        }

        internal void SetRequirements(string pattern, List<RequirementResult> requirementResults)
        {
            foreach (var result in requirementResults)
            {
                var req = new Requirement();
                req.SetRequirement(pattern, result);
                this.RequirementList.Children.Add(req);

            }
        }
    }
}

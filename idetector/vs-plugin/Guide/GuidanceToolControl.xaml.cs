using System.Windows.Media;
using idetector.Collections;
using idetector.Patterns;

namespace vs_plugin.Guide
{
    using idetector;
    using idetector.Models;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for GuidanceToolControl.
    /// </summary>
    public partial class GuidanceToolControl : UserControl
    {
        private string _pattern;
        private string _typename;
        private Dictionary<string, List<PatternRequirement>> _results;
        private ScoreCalculator _calculator;

        private Dictionary<string, string> patterns = new Dictionary<string, string>()
        {
            {"abs", "ABSTRACT-FACTORY" },
            {"dec", "DECORATOR"},
            {"fac", "FACADE"},
            {"fcy", "FACTORY"},
            {"sin", "SINGLETON"},
            {"sta", "STATE"},
            {"str", "STRATEGY"},
            {"ite", "ITERATOR"},
            {"obs", "OBSERVER"},
            {"cmd", "COMMAND"},

        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidanceToolControl"/> class.
        /// </summary>
        public GuidanceToolControl()
        {
            this.InitializeComponent();

            NewGuidance.StatusChange += this.StartGuidance;
        }

        private void StartGuidance(bool guided, EventArgs e)
        {
            if (guided)
            {
                this._pattern = NewGuidance.GuidancePattern;
                this._typename = NewGuidance.TypeName;
                this._results = ToolWindow1Control.Patterns;
                this._calculator = ToolWindow1Control.Calc;
                this.NoGuidance.Visibility = Visibility.Collapsed;
                this.ResultsGrid.Visibility = Visibility.Visible;

                this.Scan();
            }
            else
            {
                this._pattern = null;
                this.NoGuidance.Visibility = Visibility.Visible;
                this.ResultsGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Scan()
        {
            var collection = ToolWindow1Control.ReadProjectCode();

            if (collection == null)
            {
                // No code to scan!
                return;
            }

            ClassModel cls = collection.GetClass(this._typename);
            ClassCollection nameSpaceClassCollection = collection.GetNamespace(this._typename);

            IPattern pattern = null;

            switch (this._pattern)
            {
                case "sin":
                    if(cls == null) { return; }
                    pattern = new Singleton(cls);
                    break;
                case "ite":
                    if (cls == null) { return; }
                    //pattern = new Iterator(cls);
                    break;
                case "dec":
                    if (nameSpaceClassCollection == null) { return; }
                    pattern = new idetector.Patterns.Decorator(nameSpaceClassCollection);
                    break;
                case "fac":
                    if (nameSpaceClassCollection == null) { return; }
                    pattern = new idetector.Patterns.Decorator(nameSpaceClassCollection);
                    break;
                case "abs":
                    if (nameSpaceClassCollection == null) { return; }
                    pattern = new AbstractFactoryMethod(nameSpaceClassCollection, false);
                    break;
                case "fcy":
                    if (nameSpaceClassCollection == null) { return; }
                    pattern = new AbstractFactoryMethod(nameSpaceClassCollection, true);
                    break;
                case "sta":
                    if (nameSpaceClassCollection == null) { return; }
                    pattern = new StateStrategy(nameSpaceClassCollection, true);
                    break;
                case "str":
                    if (nameSpaceClassCollection == null) { return; }
                    pattern = new StateStrategy(nameSpaceClassCollection, false);
                    break;
                case "obs":
                    if (nameSpaceClassCollection == null) { return; }
                    //pattern = new Observer(nameSpaceClassCollection, false);
                    break;
                case "cmd":
                    if (nameSpaceClassCollection == null) { return; }
                    //pattern = new Command(nameSpaceClassCollection, false);
                    break;
            }

            if (pattern == null)
            {
                return;
            }

            pattern.Scan();

            var results = pattern.GetResults();

            foreach (var result in results)
            { 
                TextBlock t = new TextBlock();
                t.FontWeight = FontWeights.Bold;
                t.FontSize = 15.0;
                t.Text = this._typename;
                this.Results.Children.Add(t);

                foreach (var requirement in result.Value)
                {
                    this.SetRequirements(requirement);
                    this.ShowInfo(requirement);
                    this.DrawBorder();
                }
            }


        }

        private void DrawBorder()
        {
            var border = new Border();
            border.Height = 1;
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(220,220,220));
            border.Margin = new Thickness(0,10,0,10);
            this.Results.Children.Add(border);
        }

        private void ShowInfo(RequirementResult requirement)
        {
            var requirements = ToolWindow1Control.Patterns[this.patterns[_pattern]];
            var patternRequirement = requirements.Find(e => e.Id.Equals(requirement.Id));

            string message;
            if (requirement.Passed)
                message = patternRequirement.Description;
            else
                message = patternRequirement.ErrorMessage;

            TextBlock t = new TextBlock();
            t.Text = message;
            this.Results.Children.Add(t);
        }

        private void SetRequirements(RequirementResult resultValue)
        {
            var requirement = new Requirement();
            requirement.SetRequirement(this.patterns[_pattern], resultValue);
            requirement.openInfoScreen = false;

            this.Results.Children.Add(requirement);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Results.Children.Clear();
            this.Scan();
        }
    }
}

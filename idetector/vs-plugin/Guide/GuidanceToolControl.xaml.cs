using System.Linq;
using System.Windows.Media;
using EnvDTE;
using idetector.Collections;
using idetector.Patterns;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Command = idetector.Patterns.Command;

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
        private Events events;
        private DocumentEvents documentEvents;

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
                var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(SDTE));

                events = dte.Events;
                documentEvents = events.DocumentEvents;
                documentEvents.DocumentSaved += SaveAction;

                this.Scan();
            }
            else
            {
                this._pattern = null;
                this.NoGuidance.Visibility = Visibility.Visible;
                this.ResultsGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveAction(Document Document)
        {
            this.Results.Children.Clear();
            this.Scan();
        }

        private void Scan()
        {
            this.Results.Children.Clear();

            var collection = ToolWindow1Control.ReadProjectCode();

            if (collection == null)
            {
                // No code to scan!
                this.NoProject();
                return;
            }

            ClassModel cls = collection.GetClass(this._typename);
            ClassCollection nameSpaceClassCollection = collection.GetNamespace(this._typename);

            IPattern pattern = null;

            switch (this._pattern)
            {
                case "sin":
                    if (cls == null) { this.NoClassFound(); return; }
                    pattern = new Singleton(cls);
                    break;
                case "ite":
                    if (cls == null) { this.NoClassFound(); return; }
                    //pattern = new Iterator(cls);
                    break;
                case "dec":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new idetector.Patterns.Decorator(nameSpaceClassCollection);
                    break;
                case "fac":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new idetector.Patterns.Decorator(nameSpaceClassCollection);
                    break;
                case "abs":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new AbstractFactoryMethod(nameSpaceClassCollection, false);
                    break;
                case "fcy":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new AbstractFactoryMethod(nameSpaceClassCollection, true);
                    break;
                case "sta":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new StateStrategy(nameSpaceClassCollection, true);
                    break;
                case "str":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new StateStrategy(nameSpaceClassCollection, false);
                    break;
                case "obs":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    //pattern = new Observer(nameSpaceClassCollection, false);
                    break;
                case "cmd":
                    if (nameSpaceClassCollection == null) { this.NoClassFound(); return; }
                    pattern = new Command(nameSpaceClassCollection);
                    break;
            }

            if (pattern == null)
            {
                return;
            }

            pattern.Scan();

            var results = pattern.GetResults().Values;
            var requirements = ToolWindow1Control.Patterns[this.patterns[this._pattern]];

            foreach (var classResultPair in results)
            {
                TextBlock t = new TextBlock();
                t.FontWeight = FontWeights.Bold;
                t.FontSize = 15.0;
                t.Text = this._typename;
                this.Results.Children.Add(t);

                foreach (var requirement in requirements)
                {
                    var hasResult = classResultPair.Any(rr => rr.Id == requirement.Id);
                    RequirementResult result = null;
                    if (hasResult)
                    {
                        result = classResultPair.First(rr => rr.Id == requirement.Id);
                    
                    }
                    else
                    {
                        result = new RequirementResult(requirement.Id, false, collection.GetClass(this._typename));
                    }

                    this.SetRequirements(result);
                    this.ShowInfo(result);
                    this.DrawBorder();
                }
            }


        }

        private void NoProject()
        {
            var message = new TextBlock();
            message.Margin = new Thickness(10);
            message.Text = "Please open a project before running the guidance tool";
            this.Results.Children.Add(message);
        }

        private void NoClassFound()
        {
            var message = new TextBlock();
            message.Margin = new Thickness(10);
            message.Text = "Class " + this._typename + " not found yet, have you created it?";
            this.Results.Children.Add(message);
        }

        private void DrawBorder()
        {
            var border = new Border();
            border.Height = 1;
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            border.Margin = new Thickness(0, 10, 0, 10);
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
            t.TextWrapping = TextWrapping.WrapWithOverflow;
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
            this.Scan();
        }
    }
}

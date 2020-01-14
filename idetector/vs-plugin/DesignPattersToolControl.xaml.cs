using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EnvDTE;
using idetector;
using idetector.CodeLoader;
using idetector.Collections;
using idetector.Data;
using idetector.Models;
using idetector.Patterns;
using idetector.Patterns.Facade;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using vs_plugin.Code;
using Decorator = idetector.Patterns.Decorator;

namespace vs_plugin
{
    /// <summary>
    ///     Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        public static Dictionary<string, List<PatternRequirement>> Patterns;
        public static ScoreCalculator Calc;
        private int cutoff = 50;
        private ClassCollection collection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolWindow1Control" /> class.
        /// </summary>
        public ToolWindow1Control()
        {
            InitializeComponent();
            var req = new Requirements();
            Patterns = req.GetRequirements();
            Calc = new ScoreCalculator(Patterns);
            UIHandler.ToolWindow1Control = this;

            this.CreateSettings();
        }

        private void CreateSettings()
        {
            foreach (var pattern in ToolWindow1Control.Patterns)
            {
                var patternWeight = new PatternWeight();
                patternWeight.Title.Text = pattern.Key;
                foreach (var requirement in pattern.Value)
                {
                    var weight = patternWeight.AddWeight(pattern.Key, requirement.Id, requirement.Title, requirement.Weight);

                    var rangeEventHandler = new RangeChangeEventHandler(pattern.Key, requirement.Id, weight);
                    weight.RangeSlider.ValueChanged += rangeEventHandler.UpdateValue;
                }

                this.Ranges.Children.Add(patternWeight);
            }
            this.updateScoreCalculator();
        }
        private void updateScoreCalculator()
        {
            Calc = new ScoreCalculator(Patterns);
        }
        public ClassCollection GetOpenWindowText()
        {
            var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(SDTE));
            if (dte.ActiveDocument == null)
            {
                MessageBox.Show("Please open a file before scanning it.");
                return null;
            }

            var filename = dte.ActiveDocument.FullName;

            return FileReader.ReadSingleFile(filename);
        }

        public ClassCollection ReadProjectCode()
        {
            var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(SDTE));
            if (dte.Solution.Projects == null)
            {
                MessageBox.Show("Please open a project before scanning it.");
                return null;
            }

            foreach (var project in dte.Solution.Projects)
            {
                var p = (Project)project;

                var fileName = p.FullName;
                var arrpath = fileName.Split('\\');
                arrpath[arrpath.Length - 1] = null;
                var path = string.Join("\\", arrpath);

                return FileReader.ReadDirectory(path);
            }

            return null;
        }

        private void Scan_Current_file(object sender, RoutedEventArgs e)
        {
            PatternList.Children.Clear();

            //Scan file
            collection = GetOpenWindowText();
            if (collection == null) return;

            AddClasses();
        }

        private void AddClasses()
        {
            Calc = new ScoreCalculator(Patterns);

            var stateList = new List<Pattern>();
            var strategyList = new List<Pattern>();
            var facadeList = new List<Pattern>();
            var factoryList = new List<Pattern>();
            var abstractFactoryList = new List<Pattern>();
            var singletonList = new List<Pattern>();
            var decoratorList = new List<Pattern>();

            var f = new Facade(collection);
            f.Scan();

            var state = new StateStrategy(collection, true);
            state.Scan();


            var strat = new StateStrategy(collection, false);
            strat.Scan();

            var fm = new AbstractFactoryMethod(collection, false);
            fm.Scan();

            var am = new AbstractFactoryMethod(collection, true);
            am.Scan();

            var d = new Decorator(collection);
            d.Scan();


            foreach (var item in collection.GetClasses())
            {
                var s = new Singleton(item.Value);
                s.Scan();

                singletonList = this.HandleResults("SINGLETON", singletonList, s.GetResults());

            }
            decoratorList = this.HandleResults("DECORATOR", decoratorList, d.GetResults());
            facadeList = this.HandleResults("FACADE", facadeList, f.GetResults());
            abstractFactoryList = this.HandleResults("ABSTRACT-FACTORY", abstractFactoryList, am.GetResults());
            factoryList = this.HandleResults("FACTORY", factoryList, fm.GetResults());


            PatternList.Children.Clear();


            PopulatePattern("Singleton", singletonList);
            PopulatePattern("Decorator", decoratorList);
            PopulatePattern("Facade", facadeList);
            PopulatePattern("Factory", factoryList);
            PopulatePattern("Abstract Factory", abstractFactoryList);
            // this.PopulatePattern("factory", factoryList);
            // this.PopulatePattern("singleton", singletonList);
            // this.PopulatePattern("state", stateList);
            // this.PopulatePattern("strategory", strategyList);    
        }

        private List<Pattern> HandleResults(string pattern, List<Pattern> patternList, Dictionary<string, List<RequirementResult>> results)
        {
            foreach (var patternResult in results)
            {
                var patternScore = Calc.GetScore(pattern, patternResult.Value);
                if (patternScore >= cutoff)
                    patternList.Add(new Pattern(collection.GetClass(patternResult.Key), patternScore,
                        patternResult.Value));
            }

            return patternList;
        }

        private void PopulatePattern(string patternName, List<Pattern> patternList)
        {
            if (patternList.Any(e => e.Score > 50))
            {
                var p = new SinglePattern();
                p.SetHandle(patternName?.First().ToString().ToUpper() + patternName?.Substring(1).ToLower());
                foreach (var pattern in patternList)
                    if (pattern.Score >= 50)
                        p.AddPattern(patternName, pattern);
                PatternList.Children.Add(p);
            }

        }

        private void Scan_Current_project(object sender, RoutedEventArgs e)
        {
            PatternList.Children.Clear();

            //Scan file
            collection = ReadProjectCode();
            if (collection == null) return;

            AddClasses();
        }

        /// <summary>
        ///     Method to replace summary's information and reset the text wrapping.
        /// </summary>
        /// <param name="text">Text to be placed inside TextBlock.</param>
        public void UpdateSummary(string pattern, PatternRequirement req, bool passed)
        {

            if (passed)
            {
                this.ConditionIcon.Content = "✔️";
                this.ConditionIcon.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            }
            else
            {
                this.ConditionIcon.Content = "❌";
                this.ConditionIcon.Foreground = new SolidColorBrush(Color.FromRgb(128, 0, 0));
            }
            Summary.Visibility = Visibility.Visible;
            PatternName.Content = pattern;
            ConditionTitle.Content = req.Title;
            if (passed)
            {
                ConditionText.Text = req.Description;
            }
            else
            {
                ConditionText.Text = req.ErrorMessage;
            }

            ConditionText.TextWrapping = TextWrapping.Wrap;
        }

        private Dictionary<ClassModel, List<RequirementResult>> GroupResults(List<RequirementResult> unGroupedList)
        {
            var returnVal = new Dictionary<ClassModel, List<RequirementResult>>();
            foreach (var reqResults in unGroupedList)
            {
                if (!returnVal.ContainsKey(reqResults.Class))
                    returnVal.Add(reqResults.Class, new List<RequirementResult>());

                returnVal[reqResults.Class].Add(reqResults);
            }

            return returnVal;
        }

        private void OpenSettingsPanel(object sender, RoutedEventArgs e)
        {
            Default.Visibility = Visibility.Collapsed;
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsGrid.Visibility = Visibility.Collapsed;
            this.Default.Visibility = Visibility.Visible;
        }
    }
}
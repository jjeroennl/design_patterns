using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
using vs_plugin.Guide;
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
        public static string WikiLink;

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

        public static ClassCollection ReadProjectCode()
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

        private void Scan_Current_project(object sender, RoutedEventArgs e)
        {
            PatternList.Children.Clear();

            //Scan file
            collection = ReadProjectCode();
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
            var commandList = new List<Pattern>();
            var observerList = new List<Pattern>();

            var f = new Facade(collection);
            f.Scan();

            var state = new StateStrategy(collection, true);
            state.Scan();


            var strat = new StateStrategy(collection, false);
            strat.Scan();

            var c = new idetector.Patterns.Command(collection);
            c.Scan();

            var fm = new AbstractFactoryMethod(collection, true);
            fm.Scan();

            var am = new AbstractFactoryMethod(collection, false);
            am.Scan();

            var d = new Decorator(collection);
            d.Scan();

            var o = new Observer(collection);
            o.Scan();

            foreach (var item in collection.GetClasses())
            {
                var s = new Singleton(item.Value);
                s.Scan();

                singletonList = this.HandleResults("SINGLETON", singletonList, s.GetResults());

            }
            decoratorList = this.HandleResults("DECORATOR", decoratorList, d.GetResults());
            facadeList = this.HandleResults("FACADE", facadeList, f.GetResults());
            commandList = this.HandleResults("COMMAND", commandList, c.GetResults());
            abstractFactoryList = this.HandleResults("ABSTRACT-FACTORY", abstractFactoryList, am.GetResults());
            factoryList = this.HandleResults("FACTORY", factoryList, fm.GetResults());
            stateList = this.HandleResults("STATE", stateList, state.GetResults());
            strategyList = this.HandleResults("STRATEGY", stateList, strat.GetResults());
            observerList = this.HandleResults("OBSERVER", observerList, o.GetResults());


            PatternList.Children.Clear();


            PopulatePattern("Singleton", singletonList);
            PopulatePattern("Decorator", decoratorList);
            PopulatePattern("Facade", facadeList);
            PopulatePattern("Factory", factoryList);
            PopulatePattern("Abstract Factory", abstractFactoryList);
            PopulatePattern("State", stateList);
            PopulatePattern("Strategy", strategyList);
            PopulatePattern("Command", commandList);
            PopulatePattern("Observer", observerList);

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

       

        /// <summary>
        ///     Method to replace summary's information and reset the text wrapping.
        /// </summary>
        /// <param name="text">Text to be placed inside TextBlock.</param>
        public void UpdateSummary(string pattern, PatternRequirement req, bool passed, IEnumerable<RequirementResult> results)
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
            PatternName.Text = pattern;
            ConditionTitle.Text = req.Title;
            if (passed)
            {
                ConditionText.Text = req.Description;
            }
            else
            {
                ConditionText.Text = req.ErrorMessage;
            }

            PatternName.TextWrapping = TextWrapping.Wrap;
            ConditionTitle.TextWrapping = TextWrapping.Wrap;
            ConditionText.TextWrapping = TextWrapping.Wrap;
            this.ClassList.Children.Clear();

            var ht = new TextBlock();
            ht.Text = "Found in: ";
            ht.FontWeight = FontWeights.Bold;
            this.ClassList.Children.Add(ht);

            WikiLink = req.WikipediaURL;

            foreach (var res in results)
            {
                if (res.Class == null)
                {
                    continue;
                }

                var t = new TextBlock();
                t.Inlines.Add(res.Class.Identifier);

                this.ClassList.Children.Add(t);
            }
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

        private void Pattern_Guide_Click(object sender, RoutedEventArgs e)
        {
            new NewGuidance();
        }

        private void MoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(WikiLink.ToString());
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using idetector;
using idetector.Data;
using idetector.Models;

namespace vs_plugin
{
    using EnvDTE;
    using idetector.CodeLoader;
    using idetector.Collections;
    using idetector.Parser;
    using idetector.Patterns;
    using idetector.Patterns.Facade;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using vs_plugin.Code;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
            UIHandler.ToolWindow1Control = this;
        }
        
        public ClassCollection GetOpenWindowText()
        {
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(SDTE));
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
            DTE dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(SDTE));
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
            var collection = GetOpenWindowText();
            if(collection == null)
            {
                return;
            }

            this.AddClasses(collection);
        }

        private void AddClasses(ClassCollection collection)
        {
            var req = new Requirements();
            ScoreCalculator calc = new ScoreCalculator(req.GetRequirements());

            List<Pattern> stateList = new List<Pattern>();
            List<Pattern> strategyList = new List<Pattern>();
            List<Pattern> facadeList = new List<Pattern>();
            List<Pattern> factoryList = new List<Pattern>();
            List<Pattern> singletonList = new List<Pattern>();
            List<Pattern> decoratorList = new List<Pattern>();

            Facade f = new Facade(collection);
            f.Scan();

            StateStrategy state = new StateStrategy(collection, true);
            state.Scan();


            StateStrategy strat = new StateStrategy(collection, false);
            strat.Scan();

            FactoryMethod fm = new FactoryMethod(collection);
            fm.Scan();

            idetector.Patterns.Decorator d = new idetector.Patterns.Decorator(collection);
            d.Scan();

            var cutoff = 50;

            foreach (var item in collection.GetClasses())
            {

                Singleton s = new Singleton(item.Value);
                s.Scan();
                var singletonResults = s.GetResults();

                foreach (var singletonResult in singletonResults)
                {
                    var singletonScore = calc.GetScore("SINGLETON", singletonResult.Value);
                    if (singletonScore >= cutoff)
                    {
                        singletonList.Add(new Pattern(collection.GetClass(singletonResult.Key), singletonScore, singletonResult.Value));
                    }
                }

                foreach (var decoratorResult in d.GetResults())
                {
                    var decoratorScore = calc.GetScore("DECORATOR", decoratorResult.Value);
                    if (decoratorScore >= cutoff)
                    {
                        decoratorList.Add(new Pattern(collection.GetClass(decoratorResult.Key), decoratorScore, decoratorResult.Value));
                    }
                }

            }

            PatternList.Children.Clear();

  
            this.PopulatePattern("singleton", singletonList);
            this.PopulatePattern("decorator", decoratorList);
            
            // this.PopulatePattern("facade", facadeList);
            // this.PopulatePattern("factory", factoryList);
            // this.PopulatePattern("singleton", singletonList);
            // this.PopulatePattern("state", stateList);
            // this.PopulatePattern("strategory", strategyList);
        }

        private void PopulatePattern(string pattern, List<Pattern> classList)
        {
            SinglePattern p = new SinglePattern();
            p.SetHandle(pattern?.First().ToString().ToUpper() + pattern?.Substring(1).ToLower());
            foreach (var cls in classList)
            {
                p.AddPattern(cls);
            }
            this.PatternList.Children.Add(p);
        }

        private void Scan_Current_project(object sender, RoutedEventArgs e)
        {
            PatternList.Children.Clear();

            //Scan file
            var collection = ReadProjectCode();
            if (collection == null)
            {
                return;
            }

            this.AddClasses(collection);
        }

        /// <summary>
        /// Method to replace summary's information and reset the text wrapping.
        /// </summary>
        /// <param name="text">Text to be placed inside TextBlock.</param>
        public void UpdateSummary(string title)
        {
            Summary.Visibility = Visibility.Visible;
            PatternName.Content = title;
            ConditionNumber.Content = "Condition #";
            ConditionText.TextWrapping = TextWrapping.Wrap;
        }

        private Dictionary<ClassModel, List<RequirementResult>> GroupResults(List<RequirementResult> unGroupedList)
        {
            var returnVal = new Dictionary<ClassModel, List<RequirementResult>>();
            foreach (var reqResults in unGroupedList)
            {
                if (!returnVal.ContainsKey(reqResults.Class))
                {
                    returnVal.Add(reqResults.Class, new List<RequirementResult>());
                }

                returnVal[reqResults.Class].Add(reqResults);
            }

            return returnVal;
        }
    }
}
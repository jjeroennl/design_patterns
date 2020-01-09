using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
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

            InitializeCustomUI();
        }

        public void InitializeCustomUI()
        {
            List<StackPanel> StackPanels = new List<StackPanel>();

            StackPanels.Add(ScanButtons);
            StackPanels.Add(Summary);
            StackPanels.Add(SummaryCondition);
            StackPanels.Add(MoreInfo);

            foreach (StackPanel stackPanel in StackPanels)
            {
                foreach (var item in stackPanel.Children)
                {
                    if (item is Control)
                        UIHandler.ControlItems.Add((Control)item);
                    if (item is TextBlock)
                        UIHandler.ControlItems.Add((TextBlock)item);
                }
            }
            UIHandler.ControlItems.Add(ConditionText);
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

            UIHandler.UpdateColors();
        }

        private void AddClasses(ClassCollection collection)
        {
            List<ClassModel> stateList = new List<ClassModel>();
            List<ClassModel> strategyList = new List<ClassModel>();
            List<ClassModel> facadeList = new List<ClassModel>();
            List<ClassModel> factoryList = new List<ClassModel>();
            List<ClassModel> singletonList = new List<ClassModel>();
            List<ClassModel> decoratorList = new List<ClassModel>();

            Facade f = new Facade(collection);
            f.Scan();

            StateStrategy state = new StateStrategy(collection, true);
            state.Scan();


            StateStrategy strat = new StateStrategy(collection, false);
            strat.Scan();

            FactoryMethod fm = new FactoryMethod(collection);
            fm.Scan();

            foreach (var item in collection.GetClasses())
            {

                Singleton s = new Singleton(item.Value);
                s.Scan();
                idetector.Patterns.Decorator d = new idetector.Patterns.Decorator(item.Value, collection.GetClasses());
                d.Scan();

                if (f.Score(item.Value) >= 50)
                {
                    facadeList.Add(item.Value);
                }
                if (strat.Score(item.Key) >= 50)
                {
                    strategyList.Add(item.Value);
                }
                if (state.Score(item.Key) >= 50)
                {
                    stateList.Add(item.Value);
                }

                if (s.Score() >= 50)
                {
                    singletonList.Add(item.Value);
                }

                if (d.Score() >= 50)
                {
                    decoratorList.Add(item.Value);
                }
            }

            PatternList.Children.Clear();

  
            this.PopulatePattern("decorator", decoratorList);
            
            this.PopulatePattern("facade", facadeList);
            this.PopulatePattern("factory", factoryList);
            this.PopulatePattern("singleton", singletonList);
            this.PopulatePattern("state", stateList);
            this.PopulatePattern("strategory", strategyList);
        }

        private void PopulatePattern(string pattern, List<ClassModel> classList)
        {
            SinglePattern p = new SinglePattern();
            p.SetHandle(pattern?.First().ToString().ToUpper() + pattern?.Substring(1).ToLower()); ;
            foreach (var cls in classList)
            {
                p.AddClass(cls);
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
            UIHandler.UpdateColors();
        }

        /// <summary>
        /// Changes plugin colors.
        /// </summary>
        private void SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            UIHandler.SwitchMode();
            UIHandler.UpdateColors();
            SolidColorBrush otherTheme = (UIHandler.IsLight) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
            SwitchMode.Background = otherTheme;

            UpdateConditionText("yeaaaaaaaaa this is my aaaaaaaaaaaa man");
        }

        private void UpdateConditionText(string text)
        {
            ConditionText.Text = text;
            ConditionText.TextWrapping = TextWrapping.Wrap;
        }
    }
}
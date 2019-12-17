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

            this.AddClasses(collection); ;


        }

        private void AddClasses(ClassCollection collection)
        {
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

                SinglePattern p = new SinglePattern();
                p.SetFacade(f.Score(item.Key.ToString()));
                p.SetStrategy(strat.Score(item.Key.ToString()));
                p.SetState(state.Score(item.Key.ToString()));
                p.SetSingleton(s.Score());
                p.SetDecorator(d.Score());
                p.SetFactoryMethod(0);


                p.SetHandle(item.Key.ToString());
                PatternList.Children.Add(p);
            }
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

            this.AddClasses(collection); ;
        }
    }
}
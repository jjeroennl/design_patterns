namespace vs_plugin
{
    using idetector.CodeLoader;
    using idetector.Parser;
    using idetector.Patterns;
    using idetector.Patterns.Facade;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PatternList.Children.Clear();
            
            //Scan file
            var file = "C:\\Users\\jjero\\strat.cs"; //replace with VS magic

            var collection = FileReader.ReadSingleFile(file);

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
                p.SetHandle(item.Key.ToString());
                PatternList.Children.Add(p);
            }


        }
    }
}
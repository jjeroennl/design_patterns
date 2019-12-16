namespace vs_plugin
{
    using idetector.CodeLoader;
    using idetector.Parser;
    using idetector.Patterns;
    using idetector.Patterns.Facade;
    using System.Diagnostics.CodeAnalysis;
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
            var file = "C:\\Users\\jjero\\singleton.cs"; //replace with VS magic

            var collection = FileReader.ReadSingleFile(file);
            
            Facade f = new Facade(collection);
            f.Scan();

            foreach (var item in collection.GetClasses())
            {
                Singleton s = new Singleton(item.Value);
                s.Scan();
                Decorator d = new System.Windows.Controls.Decorator(item.Value, collection.GetClasses());
                d.Scan();
             

                this.printBar(item.Value, "Singleton", s.Score());
                this.printBar(item.Value,"Decorator", d.Score());
                this.printBar(item.Value,"Facade", f.Score(item.Value));
            }
           
        }
    }
}
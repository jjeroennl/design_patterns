using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for SinglePattern.xaml
    /// </summary>
    public partial class SinglePattern : UserControl
    {
        private string Test;

        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;

            PatternName1.Header = textBlock;
        }

        public SinglePattern()
        {
            InitializeComponent();
        }

        private void Collapse(object sender, RoutedEventArgs e)
        {

        }

        private void Expand(object sender, RoutedEventArgs e)
        {
        }

        internal void SetFacade(int v)
        {
            this.IsFacade.IsChecked = v > 50;
            this.FacadePercentage.Content = v.ToString() + "%";
        }

        internal void SetStrategy(int v)
        {
            this.IsStrategy.IsChecked = v > 50;
            this.StrategyPercentage.Content = v.ToString() + "%";
        }

        internal void SetState(int v)
        {
            this.IsState.IsChecked = v > 50;
            this.StatePercentage.Content = v.ToString() + "%";
        }

        internal void SetSingleton(int v)
        {
            this.IsSingleton.IsChecked = v > 50;
            this.SingletonPercentage.Content = v.ToString() + "%";
        }

        internal void SetDecorator(int v)
        {
            this.IsDecorator.IsChecked = v > 50;
            this.DecoratorPercentage.Content = v.ToString() + "%";
        }

        internal void SetFactoryMethod(int v)
        {
            this.IsFactory.IsChecked = v > 50;
            this.FactoryPercentage.Content = v.ToString() + "%";
        }
    }
}

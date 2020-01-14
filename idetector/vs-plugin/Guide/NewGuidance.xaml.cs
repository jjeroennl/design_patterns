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

namespace vs_plugin.Guide
{
    /// <summary>
    /// Interaction logic for NewGuidance.xaml
    /// </summary>
    public partial class NewGuidance : Window
    {
        public static string GuidancePattern;
        public static string TypeName;
        public static event GuidanceStatusChange StatusChange;
        public delegate void GuidanceStatusChange(bool guided, EventArgs e);
        private string _currentSelected;

        public NewGuidance()
        {
            InitializeComponent();
            this.Show();
            this.BringIntoView();

            this.Title = "New pattern guide";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox) sender;
            var value = box.Text;

            this._currentSelected = value;

            if (value.Equals("sin") || value.Equals("ite"))
            {
                typeLabel.Text = "Class name";
            }
            else
            {
                typeLabel.Text = "Namespace";
            }

            typePanel.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GuidancePattern = this._currentSelected;
            TypeName = typeName.Text;
            StatusChange?.Invoke(true, e);
        }
    }
}

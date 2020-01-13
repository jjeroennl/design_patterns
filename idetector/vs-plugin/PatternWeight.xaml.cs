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
    /// Interaction logic for PatternWeight.xaml
    /// </summary>
    public partial class PatternWeight : UserControl
    {
        public PatternWeight()
        {
            InitializeComponent();
        }


        public WeightRange AddWeight(string pattern, string ID, string requirementTitle, float weight)
        {
            var range = new WeightRange();
            range.SetPattern(pattern);
            range.SetID(ID);
            range.SetTitle(requirementTitle);
            range.SetWeight(weight);

            this.Weights.Children.Add(range);

            return range;
        }
    }

}

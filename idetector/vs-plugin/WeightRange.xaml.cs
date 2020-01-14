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
    /// Interaction logic for WeightRange.xaml
    /// </summary>
    public partial class WeightRange : UserControl
    {
        private string _pattern;
        private string _id;

        public WeightRange()
        {
            InitializeComponent();
        }

        internal void SetPattern(string pattern)
        {
            this._pattern = pattern;
        }

        internal void SetID(string ID)
        {
            this._id = ID;
        }

        internal void SetTitle(string requirementTitle)
        {
            this.RequirementName.Content = requirementTitle;
        }

        internal void SetWeight(float weight)
        {
            this.RangeSlider.Value = weight * 50.0;
        }
    }
}

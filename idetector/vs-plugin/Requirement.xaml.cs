using System.Windows.Controls;
using idetector.Models;

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for Requirement.xaml
    /// </summary>
    public partial class Requirement : UserControl
    {
        private RequirementResult Result;
        public Requirement()
        {
            InitializeComponent();
        }

        public void SetRequirement(RequirementResult result)
        {
            this.Result = result;

            if (result.Passed)
            {
                this.Check.Text = "✔️";
            }
            else
            {
                this.Check.Text = "❌";
            }

            this.RequirementText.Text = result.Id;
        }
    }
}

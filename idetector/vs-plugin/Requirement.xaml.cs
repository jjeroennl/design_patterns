using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using idetector.Models;
using vs_plugin.Code;

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for Requirement.xaml
    /// </summary>
    public partial class Requirement : UserControl
    {
        private RequirementResult _result;
        private PatternRequirement _patternRequirement;
        public bool openInfoScreen = true;
        private string _pattern;
        private IEnumerable<RequirementResult> _classes;

        public Requirement()
        {
            InitializeComponent();
            this.MouseLeftButtonUp += pattern_PreviewMouseDown;
        }



        private void pattern_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (openInfoScreen)
            {
                UIHandler.SummarySelection(this._pattern, _patternRequirement, this._result.Passed, this._classes);
            }
        }


        public void SetRequirement(string pattern, RequirementResult result, bool loadrequirement = true)
        {
            this._result = result;
            this._pattern = pattern;

            if (result.Passed)
            {
                this.Check.Text = "✔️";
                this.Check.Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            }
            else
            {
                this.Check.Text = "❌";
                this.Check.Foreground = new SolidColorBrush(Color.FromRgb(128, 0, 0));
            }

            if (loadrequirement)
            {
                var requirements = ToolWindow1Control.Patterns[pattern.ToUpper().Replace(" ", "-")];
                _patternRequirement = requirements.Find(e => e.Id.Equals(result.Id));
                var errormessage = _patternRequirement.Title;
                this.RequirementText.Text = errormessage;
            }
        }

        internal void SetClasses(IEnumerable<RequirementResult> enumerable)
        {
            this._classes = enumerable;
        }

        internal void SetRequirement(object result)
        {
            throw new NotImplementedException();
        }
    }
}

using idetector.Collections;
using idetector.Patterns;

namespace vs_plugin.Guide
{
    using idetector;
    using idetector.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for GuidanceToolControl.
    /// </summary>
    public partial class GuidanceToolControl : UserControl
    {
        private string _pattern;
        private string _typename;
        private Dictionary<string, List<PatternRequirement>> _results;
        private ScoreCalculator _calculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidanceToolControl"/> class.
        /// </summary>
        public GuidanceToolControl()
        {
            this.InitializeComponent();

            NewGuidance.StatusChange += this.StartGuidance;
        }

        private void StartGuidance(bool guided, EventArgs e)
        {
            if (guided)
            {
                this._pattern = NewGuidance.GuidancePattern;
                this._typename = NewGuidance.TypeName;
                this._results = ToolWindow1Control.Patterns;
                this._calculator = ToolWindow1Control.Calc;
                this.NoGuidance.Visibility = Visibility.Collapsed;

                this.Scan();
            }
            else
            {
                this._pattern = null;
                this.NoGuidance.Visibility = Visibility.Visible;
            }
        }

        private void Scan()
        {
            var collection = ToolWindow1Control.ReadProjectCode();

            if (collection == null)
            {
                // No code to scan!
                return;
            }

            ClassModel cls = collection.GetClass(this._typename);
            ClassCollection nameSpaceClassCollection = collection.GetNamespace(this._typename);

            IPattern pattern = null;

            switch (this._pattern)
            {
                case "sin":
                    if(cls == null) { return; }
                    pattern = new Singleton(cls);
                    break;
                case "ite":
                    if (cls == null) { return; }
                    //pattern = new Iterator(cls);
                    break;
                case "dec":
                    if (cls == null) { return; }
                    pattern = new idetector.Patterns.Decorator(nameSpaceClassCollection);
                    break;
                case "fac":
                    if (cls == null) { return; }
                    pattern = new idetector.Patterns.Decorator(nameSpaceClassCollection);
                    break;
                case "abs":
                    if (cls == null) { return; }
                    pattern = new AbstractFactoryMethod(nameSpaceClassCollection, false);
                    break;
                case "fcy":
                    if (cls == null) { return; }
                    pattern = new AbstractFactoryMethod(nameSpaceClassCollection, true);
                    break;
                case "sta":
                    if (cls == null) { return; }
                    pattern = new StateStrategy(nameSpaceClassCollection, true);
                    break;
                case "str":
                    if (cls == null) { return; }
                    pattern = new StateStrategy(nameSpaceClassCollection, false);
                    break;
                case "obs":
                    if (cls == null) { return; }
                    //pattern = new Observer(nameSpaceClassCollection, false);
                    break;
                case "cmd":
                    if (cls == null) { return; }
                    //pattern = new Command(nameSpaceClassCollection, false);
                    break;
            }

            if (pattern == null)
            {
                return;
            }

            var results = pattern.GetResults();
        }
    }
}
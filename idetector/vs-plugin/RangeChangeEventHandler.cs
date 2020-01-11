using System;
using System.Windows;

namespace vs_plugin
{
    internal class RangeChangeEventHandler
    {
        private string pattern;
        private string id;
        private WeightRange weight;

        public RangeChangeEventHandler(string pattern, string id, WeightRange weight)
        {
            this.pattern = pattern;
            this.id = id;
            this.weight = weight;

            weight.RangeSlider.ValueChanged += this.UpdateValue;
        }

        private void UpdateValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = (float) e.NewValue;
            var weight = newValue / 100;

            ToolWindow1Control.Patterns[this.pattern].Find(r => r.Id == this.id).Weight = weight;
        }
    }
}
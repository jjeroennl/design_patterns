using System.Windows.Controls;
using idetector.Models;

namespace vs_plugin
{
    /// <summary>
    /// Interaction logic for SingleClass.xaml
    /// </summary>
    public partial class SingleClass : UserControl
    {
        public SingleClass()
        {
            InitializeComponent();
        }

        public void SetHandle(string handle)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = handle;
            ClassName.Header = textBlock;
        }

        public void AddClass(ClassModel cls, int score)
        {

        }
    }
}

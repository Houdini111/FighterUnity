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
using System.Windows.Shapes;

namespace AttackMapEditor
{
    /// <summary>
    /// Interaction logic for AddDialog.xaml
    /// </summary>
    public partial class AddDialog : Window
    {
        public ComboBox directionBox;
        public ComboBox classifcationBox;
        public bool save;

        public AddDialog(List<Direction> directions, List<Classification.Mode> dialog)
        {
            InitializeComponent();

            directionBox = (ComboBox)this.FindName("DirectionBox");
            classifcationBox = (ComboBox)this.FindName("ClassificationBox");

            foreach(Direction dir in directions) { directionBox.Items.Add(dir.ToString()); }
            foreach(Classification.Mode m in dialog) { classifcationBox.Items.Add(m); }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            save = true;
            this.Close();
        }
    }
}

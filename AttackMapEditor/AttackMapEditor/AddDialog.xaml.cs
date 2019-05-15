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

        //All directions and modes available
        public AddDialog(bool includeMiddle = false)
        {
            List<Direction> directions = new List<Direction>();
            directions.AddRange((Direction[])Enum.GetValues(typeof(Direction)));
            if(!includeMiddle) { directions.Remove(Direction.Middle); }
            List<Mode> modes = new List<Mode>();
            modes.AddRange((Mode[])Enum.GetValues(typeof(Mode)));
            
            init(directions, modes);
        }

        public AddDialog(List<Direction> directions, List<Mode> modes)
        {
            init(directions, modes);
        }

        private void init(List<Direction> directions, List<Mode> modes)
        {
            InitializeComponent();

            directionBox = (ComboBox)this.FindName("DirectionBox");
            classifcationBox = (ComboBox)this.FindName("ClassificationBox");

            foreach (Direction dir in directions) { directionBox.Items.Add(dir.ToString()); }
            foreach (Mode m in modes) { classifcationBox.Items.Add(m); }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            save = true;
            this.Close();
        }
    }
}

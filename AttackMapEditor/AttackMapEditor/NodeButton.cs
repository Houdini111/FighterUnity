using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace AttackMapEditor
{
    public class NodeButton
    {
        public Canvas canvas;
        public Rectangle square;
        public Rectangle diamond;
        public Ellipse circle;
        public Rectangle selection;
        public NodeAssociation association;

        public NodeButton(Canvas c)
        {
            canvas = c;

            string name = c.Name;
            if (!name.EndsWith("Button"))
            {
                throw new Exception("Canvas does not match format");
            }
            string dir = name.Substring(0, name.Length - "Button".Length);
            square = (Rectangle)c.FindName(dir + "Square");
            diamond = (Rectangle)c.FindName(dir + "Diamond");
            circle = (Ellipse)c.FindName(dir + "Circle");
            selection = (Rectangle)c.FindName(dir + "Select");
        }

        public void setVisibleType(Shape shape)
        {
            if(shape != Shape.Square) { square.Visibility = Visibility.Hidden; }
            else { square.Visibility = Visibility.Visible; }
            if(shape != Shape.Diamond) { diamond.Visibility = Visibility.Hidden; }
            else { diamond.Visibility = Visibility.Visible; }
            if(shape != Shape.Circle) { circle.Visibility = Visibility.Hidden; }
            else { circle.Visibility = Visibility.Visible; }
        }

        /*
        public void setVisibleType(string shape)
        {
            if (!shape.Equals("square", StringComparison.InvariantCultureIgnoreCase)) { square.Visibility = Visibility.Hidden; }
            else { square.Visibility = Visibility.Visible; }
            if (!shape.Equals("diamond", StringComparison.InvariantCultureIgnoreCase)) { diamond.Visibility = Visibility.Hidden; }
            else { diamond.Visibility = Visibility.Visible; }
            if (!shape.Equals("circle", StringComparison.InvariantCultureIgnoreCase)) { circle.Visibility = Visibility.Hidden; }
            else { circle.Visibility = Visibility.Visible; }
        }
        */

        public Visibility Visibility
        {
            get { return canvas.Visibility; }
            set { canvas.Visibility = value; }
        }

        public bool select
        {
            get { return selection.Visibility == Visibility.Visible; }
            set { selection.Visibility = (value ? Visibility.Visible : Visibility.Hidden); }
        }
    }
}

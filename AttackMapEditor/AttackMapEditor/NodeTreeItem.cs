using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace AttackMapEditor
{
    public partial class NodeTreeItem : TreeViewItem
    {
        public NodeTreeItem parent;
        public NodeAssociation association;

        public void setAssociation(NodeAssociation na)
        {
            this.association = na;
            updateHeader();
        }

        public void updateHeader()
        {
            this.Header = association.node.direction.ToString() + ": " + association.node.classification.mode.ToString();
        }

        public NodeTreeItem()
        {
            this.Header = "ERROR: NOT YET INITIALIZED";
            //this.Header = node.direction.ToString() + ": " + node.classification.mode.ToString();

            ContextMenu cm = new ContextMenu();
            this.ContextMenu = cm;
            MenuItem cmi = new MenuItem();
            cmi.Header = "Add child action";
            cmi.Click += (s, e) =>
            {
                //Prepare list of all direction to be filtered for possible options to be created
                List<Direction> directions = new List<Direction>();
                directions.AddRange((Direction[])Enum.GetValues(typeof(Direction)));

                //Get the NodeTreeItem (and thus the NodeAssociation) to find what options are available to be added
                MenuItem menuItem = s as MenuItem;
                NodeTreeItem toAddTo = (NodeTreeItem)(((ContextMenu)menuItem.Parent).PlacementTarget);

                //Filter out existing directions
                foreach (NodeTreeItem child in toAddTo.Items)
                {
                    directions.Remove(child.association.node.direction);
                }
                //Remove "Middle", as it should only ever be used in this context for the root node
                directions.Remove(Direction.Middle);

                //Get all modes a node can be
                List<Mode> modes = new List<Mode>();
                modes.AddRange((Mode[])Enum.GetValues(typeof(Mode)));

                //Display add dialog
                AddDialog dialog = new AddDialog(directions, modes);
                dialog.ShowDialog(); //ShowDialog will prevent interacting with the main window until the dialog is clsed
                if (dialog.save)
                {
                    //Get data back for use
                    Direction dir;
                    bool success = Enum.TryParse(dialog.directionBox.SelectedItem.ToString(), out dir);
                    if (!success) { MessageBox.Show($"Could not parse direction {dialog.directionBox.SelectedItem.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    Mode mode;
                    success = Enum.TryParse(dialog.classifcationBox.SelectedItem.ToString(), out mode);
                    if (!success) { MessageBox.Show($"Could not parse mode {dialog.classifcationBox.SelectedItem.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    Classification classification = Classification.GetClassificationByMode(mode);

                    Node newNode = new Node(classification, dir);
                    NodeTreeItem newItem = new NodeTreeItem();
                    newItem.parent = this;
                    NodeAssociation newAssociation = new NodeAssociation(newNode, newItem);
                    toAddTo.association.addChild(dir, newAssociation);

                    newItem.BringIntoView(); //Show element in tree without selecting it
                    toAddTo.OnSelected(new RoutedEventArgs(null, toAddTo)); //Create a fake click event on the parent so that the details (NodeButtons and the like) update
                }
            };
            cm.Items.Add(cmi);
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            //base.OnSelected(e);

            MainWindow main = (MainWindow)System.Windows.Application.Current.MainWindow;

            NodeTreeItem item = (NodeTreeItem)e.Source;
            item.IsExpanded = true;
            main.centerButton.select = true;
            main.centerButton.association = item.association;

            main.updateNodeButtons(item.association);

            main.loadNodeDetails(item.association.node);

            item.Focus(); //Make sure it's visible
        }
    }
}

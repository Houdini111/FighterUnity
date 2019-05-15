using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace AttackMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TreeView nodeListView;
        public Grid propertiesGrid;
        public ComboBox classificationBox;
        public Dictionary<Direction, NodeButton> nodeButtons;
        public NodeButton topButton;
        public NodeButton rightButton;
        public NodeButton bottomButton;
        public NodeButton leftButton;
        public NodeButton centerButton;
        public Button upButton;
        NodeAssociation root;

        FileStream attackMapFile;
        StreamReader attackMapReader;
        StreamWriter attackMapWriter;

        private static string fileName = "AttackMap.json";

        public MainWindow()
        {
            InitializeComponent();

            //Find items and pre-fill if applicable
            #region Find UI Elements
            nodeListView = (TreeView)this.FindName("NodeTreeView");
            propertiesGrid = (Grid)this.FindName("PropertiesGrid");

            classificationBox = (ComboBox)propertiesGrid.FindName("ClassificationBox");
            foreach (Mode mode in (Mode[])Enum.GetValues(typeof(Mode)))
            {
                classificationBox.Items.Add(mode.ToString());
            }

            topButton = new NodeButton((Canvas)this.FindName("TopButton"));
            rightButton = new NodeButton((Canvas)this.FindName("RightButton"));
            bottomButton = new NodeButton((Canvas)this.FindName("BottomButton"));
            leftButton = new NodeButton((Canvas)this.FindName("LeftButton"));
            centerButton = new NodeButton((Canvas)this.FindName("CenterButton"));
            upButton = (Button)this.FindName("UpButton");

            nodeButtons = new Dictionary<Direction, NodeButton>();
            nodeButtons[Direction.Up] = topButton;
            nodeButtons[Direction.Right] = rightButton;
            nodeButtons[Direction.Down] = bottomButton;
            nodeButtons[Direction.Left] = leftButton;
            nodeButtons[Direction.Middle] = centerButton;

            foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
            {
                Classification[] classificationArr = new Classification[] { Classification.getNormal(), Classification.getIntermediary(), Classification.getFinal() };
                foreach (Classification temp in classificationArr)
                {
                    if (temp.shape == Shape.Circle) { nodeButton.Value.circle.Fill = new SolidColorBrush(temp.color); }
                    if (temp.shape == Shape.Square) { nodeButton.Value.square.Fill = new SolidColorBrush(temp.color); }
                    if (temp.shape == Shape.Diamond) { nodeButton.Value.diamond.Fill = new SolidColorBrush(temp.color); }
                }

                nodeButton.Value.Visibility = Visibility.Hidden;
                nodeButton.Value.select = false;

                nodeButton.Value.canvas.MouseLeftButtonDown += new MouseButtonEventHandler(NodeButton_Click);
            }
            #endregion

            upButton.Click += (s, e) => { NavigateUp(); };

            Load();
            if(root == null)
            {
                Node rootNode = new Node(Classification.getNormal(), Direction.Middle);
                NodeTreeItem rootItem = new NodeTreeItem();
                root = new NodeAssociation(rootNode, rootItem);
            }

            propertiesGrid.IsEnabled = false;

            refreshList();
            NodeTreeItem rootListItem = (NodeTreeItem)nodeListView.Items[0];
            rootListItem.IsSelected = true; //This should trigger a refresh
        }

        #region Node Button Functionality
        public void updateNodeButtons(NodeAssociation current)
        {
            Console.WriteLine("CALLED");
            centerButton.Visibility = Visibility.Visible;
            centerButton.setVisibleType(current.node.classification.shape);

            foreach (KeyValuePair<Direction, NodeAssociation> pair in current.childAssociations)
            {
                Direction d = pair.Key;
                NodeAssociation a = pair.Value;
                
                nodeButtons[d].select = false; //Reset all selections

                //No child in that direction, make invisible
                if (a == null)
                {
                    nodeButtons[d].Visibility = Visibility.Hidden;
                    continue;
                }

                Node n = a.node;

                //Ignore any middle directions (because it shouldn't happen)
                if (d == Direction.Middle) { continue; }
                
                nodeButtons[d].association = a;
                nodeButtons[d].setVisibleType(n.classification.shape);
                nodeButtons[d].Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handle clicking on the main UI buttons that represent nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NodeButton_Click(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            //Reset all selections
            foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
            {
                nodeButton.Value.select = false;
            }

            //If it's a single click, just select the item
            if (e.ClickCount == 1)
            {
                foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
                {
                    if (nodeButton.Value.canvas == canvas)
                    {
                        nodeButton.Value.select = true;
                        main.loadNodeDetails(nodeButton.Value.association.node);
                    }
                }
            }
            //Otherwise, for double clicks, navigate to that node
            else if (e.ClickCount == 2)
            {
                NodeTreeItem item = (NodeTreeItem)nodeListView.SelectedItem;
                if (item != null)
                {
                    foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
                    {
                        if (nodeButton.Value.canvas == canvas)
                        {
                            if (!item.HasItems) { continue; }
                            //Found the clicked canvas' button
                            foreach (NodeTreeItem i in item.Items)
                            {
                                if (i.association.node.direction == nodeButton.Key)
                                {
                                    //Found the associated node item in the tree, select it
                                    //triggering the OnSelected for the NodeTreeItem event
                                    i.IsSelected = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        
        /// <summary>
        /// Load Properties section with the given's nodes details
        /// </summary>
        /// <param name="node"></param>
        public void loadNodeDetails(Node node)
        {
            propertiesGrid.IsEnabled = true;
            classificationBox.SelectedIndex = classificationBox.Items.IndexOf(node.classification.ToString());
        }

        /// <summary>
        /// Populate the NodeTreeView with the attack map, starting from the root
        /// </summary>
        public void refreshList()
        {
            nodeListView.Items.Clear();
            //NodeTreeItem item = new NodeTreeItem(root);
            NodeTreeItem item = root.treeItem;
            item.Header = "Root";
            addChildTreeItems(item);
            nodeListView.Items.Add(item);
        }

        /// <summary>
        /// Recursively add all child NodeTreeItems to the propvided NodeTreeItem
        /// </summary>
        /// <param name="item"></param>
        private void addChildTreeItems(NodeTreeItem item)
        {
            foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                //Ignore "Middle" as it should only ever be used in this context for the root, which should never be a child of anything
                if(dir == Direction.Middle) { continue; }
                NodeAssociation childNode = item.association.childAssociations[dir];
                if (childNode != null && childNode.node != null)
                {
                    //NodeTreeItem childItem = new NodeTreeItem(childNode);
                    addChildTreeItems(childNode.treeItem);
                    //addChildTreeItems(childItem);
                    //childItem.parent = item;
                    item.Items.Add(childNode.treeItem);
                }
            }
        }

        /// <summary>
        /// Traverse up from the current root
        /// </summary>
        public void NavigateUp()
        {
            NodeTreeItem current;
            NodeTreeItem parent;
            current = (NodeTreeItem)nodeListView.SelectedValue;
            if (current == null)
            {
                NodeButton nb = getSelectedNodeButton();
                NodeAssociation association = nb.association;
                Node n = association.node;
                current = association.treeItem;
            }
            parent = current.parent;
            if (parent != null) { parent.IsSelected = true; }
        }

        /// <summary>
        /// Return the single NodeButton that is currently selected
        /// </summary>
        /// <returns></returns>
        public NodeButton getSelectedNodeButton()
        {
            if (topButton.select) { return topButton; }
            if (rightButton.select) { return rightButton; }
            if (bottomButton.select) { return bottomButton; }
            if (leftButton.select) { return leftButton; }
            if (centerButton.select) { return centerButton; }

            return null;
        }

        #region Save/Load
        #region File Stream Utilities
        private void openFileStreams(bool clearFile = false)
        {
            if (clearFile) { File.WriteAllText(fileName, string.Empty); }
            attackMapFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            attackMapReader = new StreamReader(attackMapFile);
            attackMapWriter = new StreamWriter(attackMapFile);
        }

        private void closeFileStreams()
        {
            attackMapFile.Close(); //This should automatically close the reader and writer
            //attackMapWriter.Close();
            //attackMapReader.Close();
        }
        #endregion

        public void Load_Click(object sender, RoutedEventArgs e) { Load(); }
        private void Load()
        {
            openFileStreams(false);

            string fileString = attackMapReader.ReadToEnd();
            Node rootNode = JsonConvert.DeserializeObject<Node>(fileString);
            NodeTreeItem rootItem = new NodeTreeItem();
            NodeAssociation rootAssociation = new NodeAssociation(rootNode, rootItem);
            root = rootAssociation;

            closeFileStreams();
            refreshList();
            NodeTreeItem rootListItem = (NodeTreeItem)nodeListView.Items[0];
            rootListItem.IsSelected = true; //This should trigger a refresh
        }

        public void Save_Click(object sender, RoutedEventArgs e) { Save(); }
        private void Save()
        {
            openFileStreams(true);

            string jsonString = JsonConvert.SerializeObject(root.node, Formatting.Indented);
            attackMapWriter.Write(jsonString);
            attackMapWriter.Flush(); //Otherwise files of >2048 chars won't finish
            closeFileStreams();

            MessageBox.Show("Save succeeded", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        private void ClassificationBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            NodeButton nb = getSelectedNodeButton();
            ComboBox source = (ComboBox)e.Source;
            Mode mode = (Mode)Enum.Parse(typeof(Mode), source.SelectedValue.ToString());
            nb.association.node.classification = Classification.GetClassificationByMode(mode);
            NodeButton selected = getSelectedNodeButton();
            updateNodeButtons(centerButton.association);
            selected.select = true;
            //refreshList();
            nb.association.treeItem.updateHeader();
        }
    }
}

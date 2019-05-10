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
        Node root;

        StreamWriter attackMapFile;

        private static string fileName = "AttackMap.json";

        public MainWindow()
        {
            InitializeComponent();
            
            if(!File.Exists(fileName))
            {
                FileStream fileStream = File.Create(fileName);
            }

            nodeListView = (TreeView)this.FindName("NodeTreeView");
            propertiesGrid = (Grid)this.FindName("PropertiesGrid");

            classificationBox = (ComboBox)propertiesGrid.FindName("ClassificationBox");
            foreach (Classification.Mode mode in (Classification.Mode[])Enum.GetValues(typeof(Classification.Mode)))
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

            upButton.Click += (s, e) => { NavigateUp(); };

            addTestData();

            propertiesGrid.IsEnabled = false;

            refreshList();
            NodeTreeItem rootListItem = (NodeTreeItem)nodeListView.Items[0];
            rootListItem.IsSelected = true;
        }

        private void addTestData()
        {
            root = new Node(Classification.getNormal(), Direction.Middle);
            Node temp = new Node(Classification.getNormal(), Direction.Up);
            root.addChild(temp);
            temp = new Node(Classification.getIntermediary(), Direction.Right);
            root.addChild(temp);
            temp = new Node(Classification.getNormal(), Direction.Down);
            root.addChild(temp);
            temp = new Node(Classification.getFinal(), Direction.Left);
            root.addChild(temp);

            temp = new Node(Classification.getFinal(), Direction.Up);
            root.childNodes[Direction.Right].addChild(temp);
        }

        public void refreshList()
        {
            nodeListView.Items.Clear();
            NodeTreeItem item = new NodeTreeItem(root);
            item.Header = "Root";
            addChildTreeItems(item);
            nodeListView.Items.Add(item);
        }

        private void addChildTreeItems(NodeTreeItem item)
        {
            foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                Node childNode = item.node.childNodes[dir];
                if (childNode != null)
                {
                    NodeTreeItem childItem = new NodeTreeItem(childNode);
                    addChildTreeItems(childItem);
                    childItem.parent = item;
                    item.Items.Add(childItem);
                }
            }
        }

        public void loadNodeDetails(Node n)
        {
            propertiesGrid.IsEnabled = true;
            classificationBox.SelectedIndex = classificationBox.Items.IndexOf(n.classification.ToString());
        }

        public void NodeButton_Click(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            MainWindow main = (MainWindow)Application.Current.MainWindow;

            //Reset all selections
            foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
            {
                nodeButton.Value.select = false;
            }

            if (e.ClickCount == 1)
            {
                foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
                {
                    if (nodeButton.Value.canvas == canvas)
                    {
                        nodeButton.Value.select = true;
                        main.loadNodeDetails(nodeButton.Value.AssociatedNode);
                    }
                }
            }
            else if (e.ClickCount == 2)
            {
                NodeTreeItem item = (NodeTreeItem)nodeListView.SelectedItem;
                foreach (KeyValuePair<Direction, NodeButton> nodeButton in nodeButtons)
                {
                    if (nodeButton.Value.canvas == canvas)
                    {
                        if(!item.HasItems) { continue; }
                        //Found the clicked canvas' button
                        foreach (NodeTreeItem i in item.Items)
                        {
                            if (i.node.direction == nodeButton.Key)
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

        public void updateNodeButtons(Node current)
        {
            centerButton.Visibility = Visibility.Visible;
            centerButton.setVisibleType(current.classification.shape.ToString());

            foreach (KeyValuePair<Direction, Node> pair in current.childNodes)
            {
                Direction d = pair.Key;
                Node n = pair.Value;
                if (d == Direction.Middle) { continue; }
                nodeButtons[d].select = false;
                if (n == null)
                {
                    nodeButtons[d].Visibility = Visibility.Hidden;
                }
                else
                {
                    nodeButtons[d].AssociatedNode = pair.Value;
                    nodeButtons[d].setVisibleType(n.classification.shape.ToString());
                    nodeButtons[d].Visibility = Visibility.Visible;
                }
            }
        }

        public void NavigateUp()
        {
            NodeTreeItem parent;
            parent = ((NodeTreeItem)nodeListView.SelectedValue).parent;
            if (parent != null) { parent.IsSelected = true; }
        }

        public void selectRootItem()
        {
            ((NodeTreeItem)nodeListView.Items[0]).IsSelected = true;
        }

        public void expandAllAncestors(NodeTreeItem item)
        {
            NodeTreeItem next = item;
            while(next.parent != null)
            {
                next = next.parent;
                next.IsExpanded = true;
            }
        }

        public void selectDirectionButton(Direction dir)
        {
            if(dir == Direction.Up) { topButton.select = true; }
            else { topButton.select = false; }
            if(dir == Direction.Right) { rightButton.select = true; }
            else { rightButton.select = false; }
            if(dir == Direction.Down) { bottomButton.select = true; }
            else { bottomButton.select = false; }
            if(dir == Direction.Left) { leftButton.select = true; }
            else { leftButton.select = false; }
            if(dir == Direction.Middle) { centerButton.select = true; }
            else { centerButton.select = false; }
        }

        public NodeButton getSelectedNodeButton()
        {
            if(topButton.select) { return topButton; }
            if(rightButton.select) { return rightButton; }
            if(bottomButton.select) { return bottomButton; }
            if(leftButton.select) { return leftButton; }
            if(centerButton.select) { return centerButton; }

            return null;
        }

        public NodeTreeItem findNodeTreeItem(NodeTreeItem item)
        {
            return findNodeTreeItem(item, (NodeTreeItem)NodeTreeView.Items.GetItemAt(0));
        }

        private NodeTreeItem findNodeTreeItem(NodeTreeItem item, NodeTreeItem start)
        {
            if(item.itemNodeEquals(start)) { return start; }
            NodeTreeItem found = null;
            foreach(NodeTreeItem i in start.Items)
            {
                found = findNodeTreeItem(item, i);
                if(found != null) { return found; }
            }
            return null;
        }

        private void ClassificationBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            NodeButton nb = getSelectedNodeButton();
            ComboBox source = (ComboBox)e.Source;
            Classification.Mode mode = (Classification.Mode)Enum.Parse(typeof(Classification.Mode), source.SelectedValue.ToString());
            nb.AssociatedNode.classification = Classification.GetClassificationByMode(mode);
            NodeButton selected = getSelectedNodeButton();
            updateNodeButtons(centerButton.AssociatedNode);
            selected.select = true;
        }

        private Dictionary<int, string> _nodeJsonStrings = null;
        public Dictionary<int, string> NodeJsonStrings
        {
            get
            {
                if(_nodeJsonStrings == null) { _nodeJsonStrings = new Dictionary<int, string>(); }
                return _nodeJsonStrings;
            }
        }
        private string nl = Environment.NewLine; //nl == new line

        private void Save(object sender, RoutedEventArgs e)
        {
            System.IO.File.WriteAllText(fileName, string.Empty);
            attackMapFile = new StreamWriter(fileName);

            CreateTreeNodeString(root, 0);
            string outStr = "{";

            bool first = true;
            foreach(KeyValuePair<int, string> node in NodeJsonStrings)
            {
                if(!first) { outStr += $",{nl}"; }
                outStr += $"\"{node.Key}\": {node.Value}";
                first = false;
            }
            outStr += "}";

            attackMapFile.Write(outStr);

            /*
            bool first = true;
            foreach(NodeTreeItem item in nodeListView.Items)
            {
                if(!first) { attackMapFile.WriteLine(","); }

                WriteTreeItem(item.node, attackMapFile);

                if (first) { first = false; }
            }
            */
            
            attackMapFile.Close();
        }
        /*
        private void WriteTreeItem(Node item, StreamWriter file)
        {
            
            file.Write("{");
            file.WriteLine("\"Classification\": \"" + item.classification + "\",");
            file.WriteLine("\"Children\": {");
            bool first = true;
            foreach(KeyValuePair<Direction, Node> pair in item.childNodes)
            {
                if(pair.Value == null) { continue; }

                if(!first) { attackMapFile.WriteLine(","); }
                
                file.Write("\"" + pair.Key + "\":");
                WriteTreeItem(pair.Value, file);

                if (first) { first = false; }
            }
            file.WriteLine("}}");
        }
        */

        private (int thisNodeId, int nextAvailableId) CreateTreeNodeString(Node item, int myId)
        {
            int id = ++myId;
            string node = "{";
            
            node += $"\"Classification\": \"{item.classification}\",{nl}";
            node += "\"Children\": {" + nl;
            bool first = true;
            int childId;
            foreach(KeyValuePair<Direction, Node> pair in item.childNodes)
            {
                if(pair.Value == null) { continue; }
                if(!first) { node += ", "; }
                (childId, id) = CreateTreeNodeString(pair.Value, id); //It will increment the ID itself and add itself to the list
                if(childId != -1) { node += $"\"{pair.Key}\": {childId}"; }
                first = false;
            }
            node += "}";

            node += "}";

            NodeJsonStrings.Add(myId, node);
            return (myId, id);
        }
    }

    public class NodeButton 
    {
        public Canvas canvas;
        public Rectangle square;
        public Rectangle diamond;
        public Ellipse circle;
        public Rectangle selection;
        public Node AssociatedNode;

        public NodeButton(Canvas c)
        {
            canvas = c;

            string name = c.Name;
            if(!name.EndsWith("Button"))
            {
                throw new Exception("Canvas does not match format");
            }
            string dir = name.Substring(0, name.Length - "Button".Length);
            square = (Rectangle)c.FindName(dir + "Square");
            diamond = (Rectangle)c.FindName(dir + "Diamond");
            circle = (Ellipse)c.FindName(dir + "Circle");
            selection = (Rectangle)c.FindName(dir + "Select");
        }

        public void setVisibleType(string shape)
        {
            if(!shape.Equals("square", StringComparison.InvariantCultureIgnoreCase)) { square.Visibility = Visibility.Hidden; }
            else { square.Visibility = Visibility.Visible; }
            if(!shape.Equals("diamond", StringComparison.InvariantCultureIgnoreCase)) { diamond.Visibility = Visibility.Hidden; }
            else { diamond.Visibility = Visibility.Visible; }
            if(!shape.Equals("circle", StringComparison.InvariantCultureIgnoreCase)) { circle.Visibility = Visibility.Hidden; }
            else { circle.Visibility = Visibility.Visible; }
        }

        internal void RaiseEvent(RoutedEventArgs routedEventArgs)
        {
            throw new NotImplementedException();
        }

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

    public partial class NodeTreeItem : TreeViewItem
    {
        public NodeTreeItem parent;
        public Node node;

        public NodeTreeItem(Node n)
        {
            this.node = n;

            this.Header = node.direction.ToString() + ": " + node.classification.mode.ToString();

            ContextMenu cm = new ContextMenu();
            this.ContextMenu = cm;
            MenuItem cmi = new MenuItem();
            cmi.Header = "Add child action";
            cmi.Click += (s, e) => 
            {

                MainWindow main = (MainWindow)Application.Current.MainWindow;

                List<Direction> directions = new List<Direction>();
                directions.AddRange( (Direction[])Enum.GetValues(typeof(Direction)) );
                MenuItem menuItem = s as MenuItem;
                NodeTreeItem toAddTo = (NodeTreeItem)(((ContextMenu)menuItem.Parent).PlacementTarget);
                foreach(NodeTreeItem child in toAddTo.Items)
                {
                    directions.Remove(child.node.direction);
                }
                directions.Remove(Direction.Middle);

                List<Classification.Mode> classifications = new List<Classification.Mode>();
                classifications.AddRange((Classification.Mode[])Enum.GetValues(typeof(Classification.Mode)));

                AddDialog dialog = new AddDialog(directions, classifications);
                dialog.ShowDialog();
                if(dialog.save)
                {
                    Direction dir;
                    Enum.TryParse(dialog.directionBox.SelectedItem.ToString(), out dir);
                    Classification.Mode m;
                    Enum.TryParse(dialog.classifcationBox.SelectedItem.ToString(), out m);
                    Classification cla = Classification.GetClassificationByMode(m);
                    Node newNode = new Node(cla, dir);
                    NodeTreeItem newItem = new NodeTreeItem(newNode);
                    toAddTo.node.childNodes[dir] = newNode;
                    newItem.parent = toAddTo;
                    main.refreshList();

                    NodeTreeItem found = main.findNodeTreeItem(toAddTo);
                    found.IsSelected = true;
                }
            };
            cm.Items.Add(cmi);
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);

            MainWindow main = (MainWindow)Application.Current.MainWindow;
            
            NodeTreeItem item = (NodeTreeItem)e.Source;
            item.IsExpanded = true;
            main.centerButton.select = true;
            main.centerButton.AssociatedNode = node;

            main.updateNodeButtons(item.node);

            main.loadNodeDetails(item.node);

            

            main.expandAllAncestors(item);
        }
        
        public bool itemNodeEquals(NodeTreeItem o)
        {
            if(o == null) { return false; }
            if((this.parent == null) != (o.parent == null)) { return false; }
            if(this.parent != null)
            {
                bool parentsEqual = this.parent.singluarNodeEquals(o.parent);
                if (!parentsEqual) { return false; }
            }
            bool valuesEqual = this.node.Equals(o.node);
            if(!valuesEqual) { return false; }
            bool childrenEqual = true;
            foreach(NodeTreeItem child1 in this.Items)
            {
                bool found = false;
                foreach (NodeTreeItem child2 in o.Items)
                {
                    if(child1.singluarNodeEquals(child2))
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    childrenEqual = false;
                    break;
                }
            }
            if(!childrenEqual) { return false; }

            return true;
        }

        public bool singluarNodeEquals(NodeTreeItem o)
        {
            return this.node.Equals(o.node);
        }
    }

    public class Node
    {
        public Dictionary<Direction, Node> childNodes;
        public Classification classification;
        public Direction direction;
        public IActionType preAction;
        public IActionType action;
        public IActionType postAction;
        

        public Node(Classification c, Direction dir)
        {
            childNodes = new Dictionary<Direction, Node>();
            childNodes.Add(Direction.Up, null);
            childNodes.Add(Direction.Right, null);
            childNodes.Add(Direction.Down, null);
            childNodes.Add(Direction.Left, null);
            childNodes.Add(Direction.Middle, null);

            this.classification = c;
            this.direction = dir;
        }

        public void addChild(Node child)
        {
            if(childNodes[child.direction] != null)
            {
                throw new Exception("Child already exists in direction");
            }

            childNodes[child.direction] = child;
        }
    }
    
    public enum Direction { Up, Right, Down, Left, Middle };
    public enum Shape { Circle, Diamond, Square }
    
    public class Classification
    {
        public Color color;
        public Mode mode;
        public Shape shape;

        public enum Mode { Normal, Inter, Final };

        public static Classification getNormal()
        {
            return new Classification()
            {
                color = Color.FromRgb(152, 152, 152),
                mode = Mode.Normal,
                shape = Shape.Circle,
            };
        }

        public static Classification getIntermediary()
        {
            return new Classification()
            {
                color = Color.FromRgb(246, 161, 66),
                mode = Mode.Inter,
                shape = Shape.Diamond,
            };
        }

        public static Classification getFinal()
        {
            return new Classification()
            {
                color = Color.FromRgb(245, 73, 66),
                mode = Mode.Final,
                shape = Shape.Square,
            };
        }

        public static Classification GetClassificationByMode(Mode m)
        {
            if(m == Mode.Normal) { return getNormal(); }
            if(m == Mode.Inter) { return getIntermediary(); }
            if(m == Mode.Final) { return getFinal(); }
            return getNormal();
        }

        public override string ToString()
        {
            return mode.ToString();
        }
    }

    #region Placeholders for things to be used in Unity
    public interface IActionType
    {
        bool shouldExecute();
        bool execute();
    }

    public class AttackAction : IActionType
    {
        public bool execute()
        {
            throw new NotImplementedException();
        }

        public bool shouldExecute()
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}

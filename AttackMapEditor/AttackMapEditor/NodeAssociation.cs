using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttackMapEditor
{
    public class NodeAssociation
    {
        public Node node;
        public NodeTreeItem treeItem;
        public Dictionary<Direction, NodeAssociation> childAssociations;

        public NodeAssociation(Node node, NodeTreeItem treeItem)
        {
            this.node = node;
            this.treeItem = treeItem;
            this.treeItem.setAssociation(this);
            this.childAssociations = new Dictionary<Direction, NodeAssociation>();
            this.childAssociations.Add(Direction.Up, null);
            this.childAssociations.Add(Direction.Right, null);
            this.childAssociations.Add(Direction.Down, null);
            this.childAssociations.Add(Direction.Left, null);
        }

        public void addChild(Direction dir, NodeAssociation child)
        {
            node.childNodes[dir] = child.node;
            treeItem.Items.Add(child.treeItem);
            childAssociations[dir] = child;
        }
    }
}

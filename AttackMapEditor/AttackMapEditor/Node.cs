using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttackMapEditor
{
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
            if (childNodes[child.direction] != null)
            {
                throw new Exception("Child already exists in direction");
            }

            childNodes[child.direction] = child;
        }
    }
}

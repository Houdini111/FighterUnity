using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLogic : MonoBehaviour, AttackUI.IAttackUICallback
{
    [SerializeField]
    private GameObject playerObj;
    private Player player;
    
    [SerializeField]
    private Master master;
    [SerializeField]
    private ObjectInteraction interaction;
    
    [SerializeField]
    private AttackUI _attackUI;
    public AttackUI attackUI { get { return _attackUI; } }

    private Node root;
    private Node current;
    private int depth;

    public bool ready;

    // Start is called before the first frame update
    void Start()
    {
        if(playerObj == null)
        {
            playerObj = GameObject.Find("Player");
        }
        player = playerObj.GetComponent<Player>();
        
        initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void initialize()
    {
        ChoiceGroup children = new ChoiceGroup();
        Node n = new Node(Classification.getIntermediary(), Direction.up, attackUI);
        n.action = new AttackAction(this, interaction);
        children.up = n;
        n = new Node(Classification.getFinal(), Direction.right, attackUI);
        n.action = new AttackAction(this, interaction);
        children.right = n;
        n = new Node(Classification.getFinal(), Direction.down, attackUI);
        n.action = new AttackAction(this, interaction);
        children.down = n;
        n = new Node(Classification.getFinal(), Direction.left, attackUI);
        n.action = new AttackAction(this, interaction);
        children.left = n;

        root = new Node(Classification.getNormal(), Direction.middle, attackUI);
        root.children = children;

        children = new ChoiceGroup();
        n = new Node(Classification.getFinal(), Direction.up, attackUI);
        n.action = new AttackAction(this, interaction);
        children.up = n;
        n = new Node(Classification.getFinal(), Direction.down, attackUI);
        n.action = new AttackAction(this, interaction);
        children.down = n;
        root.children.up.children = children;


        current = root;

        //Initialize colors
        attackUI.setColors(root.children.getColors());
        //Initialize shapes
        attackUI.setShapes(root.children.getShapes());

        ready = true;
        _attackMode = true;
    }

    public void attackAnimationDone()
    {
        ready = true;
    }

    private bool _attackMode;
    public bool attackMode
    {
        get { return _attackMode; }
        set
        {
            _attackMode = value;
            attackUI.attackMode = value;
        }
    }

    #region Direction handlers
    public bool up()
    {
        if (current.children.up != null)
        {
            transition(current.children.up);
            return true;
        }
        return false;
    }

    public bool right()
    {
        if (current.children.right != null)
        {
            transition(current.children.right);
            return true;
        }
        return false;
    }

    public bool down()
    {
        if (current.children.down != null)
        {
            transition(current.children.down);
            return true;
        }
        return false;
    }

    public bool left()
    {
        if (current.children.left != null)
        {
            transition(current.children.left);
            return true;
        }
        return false;
    }
    #endregion

    private void transition(Node n)
    {
        //Precheck to try to prevent issues with partial execution
        if( (current.postAction != null && !current.postAction.shouldExecute()) ||
            (n.preAction != null && !n.preAction.shouldExecute()) ||
            (n.action != null && !n.action.shouldExecute()))
        {
            //Does not seem like it will successfully execute
            //return;
        }


        if (current.postAction != null)
        {
            if (!current.postAction.execute())
            {
                return;
            }
        }
        if (n.preAction != null)
        {
            if(!n.preAction.execute())
            {
                return;
            }
        }
        if (n.action != null)
        {
            if (!n.action.execute())
            {
                //return;
            }
        }

        ready = false;

        char dirCode = '\0';
        if (n.dir == Direction.up) { dirCode = 'U'; }
        else if(n.dir == Direction.right) { dirCode = 'R'; }
        else if(n.dir == Direction.down) { dirCode = 'D'; }
        else if(n.dir == Direction.left) { dirCode = 'L'; }
        if(n == root) { attackUI.queueReset(200, n.children.getColors(), n.children.getShapes()); }
        else { attackUI.selectCircle(dirCode, 200, n.children.getColors(), n.children.getShapes(), ++depth); }
        
        current = n;
        if (n.classification.mode == Classification.Mode.final)
        {
            depth = 0;
            transition(root);
        }
    }

    public class Node
    {
        public Classification classification;
        public IActionType preAction;
        public IActionType action;
        public IActionType postAction;
        public ChoiceGroup children;
        public Direction dir;
        public readonly AttackUI ui;

        public Color color { get { return classification.color; }  }

        public Node(Classification classification, IActionType preAction, IActionType action, IActionType postAction,
                ChoiceGroup children, Direction dir, AttackUI ui)
        {
            this.classification = classification;
            this.preAction = preAction;
            this.action = action;
            this.postAction = postAction;
            if (children != null) { this.children = children; }
            else { this.children = new ChoiceGroup(); }
            this.dir = dir;
            this.ui = ui;
        }

        public Node(Classification classification, Direction dir, AttackUI ui)
        {
            this.classification = classification;
            this.children = new ChoiceGroup();
            this.dir = dir;
            this.ui = ui;
        }
    }

    public class Classification
    {
        public Color color;
        public Mode mode;
        public char shapeCode;
        
        public enum Mode { normal, inter, final };

        public static Classification getNormal()
        {
            return new Classification()
            {
                color = new Color(0.596f, 0.596f, 0.596f),
                mode = Mode.normal,
                shapeCode = 'c',
            };
        }

        public static Classification getIntermediary()
        {
            return new Classification()
            {
                color = new Color(0.965f, 0.631f, 0.259f),
                mode = Mode.inter,
                shapeCode = 'd',
            };
        }

        public static Classification getFinal()
        {
            return new Classification()
            {
                color = new Color(0.961f, 0.286f, 0.259f),
                mode = Mode.final,
                shapeCode = 's',
            };
        }
    }

    public interface IActionType
    {
        bool shouldExecute();
        bool execute();
    }

    public class AttackAction : IActionType
    {
        private readonly AttackLogic logic;
        private readonly ObjectInteraction interaction;

        public AttackAction(AttackLogic al, ObjectInteraction i)
        {
            logic = al;
            interaction = i;
        }

        public bool shouldExecute()
        {
            GameObject raw = logic.master.entityMap[logic.player.ahead];
            if (raw != null)
            {
                MonoBehaviour[] scripts = raw.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour mb in scripts)
                {
                    IAttackable target = mb as IAttackable;
                    if (target != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool execute()
        {
            GameObject raw = logic.master.entityMap[logic.player.ahead];
            if(raw != null)
            {
                MonoBehaviour[] scripts = raw.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour mb in scripts)
                {
                    IAttackable target = mb as IAttackable;
                    if (target != null)
                    {
                        interaction.attack(logic.player, target);
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class ChoiceGroup
    {
        public Node up { get; set; }
        public Node right { get; set; }
        public Node down { get; set; }
        public Node left { get; set; }

        public Dictionary<char, Color> getColors()
        {
            Dictionary<char, Color> ret = new Dictionary<char, Color>();

            if(up != null) { ret['U'] = up.color; }
            if (right != null) { ret['R'] = right.color; }
            if (down != null) { ret['D'] = down.color; }
            if (left != null) { ret['L'] = left.color; }

            return ret;
        }

        public Dictionary<char, char> getShapes()
        {
            Dictionary<char, char> ret = new Dictionary<char, char>();

            if (up != null) { ret['U'] = up.classification.shapeCode; }
            if (right != null) { ret['R'] = right.classification.shapeCode; }
            if (down != null) { ret['D'] = down.classification.shapeCode; }
            if (left != null) { ret['L'] = left.classification.shapeCode; }

            return ret;
        }
    }
}

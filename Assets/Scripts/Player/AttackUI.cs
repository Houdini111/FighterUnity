using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class AttackUI : MonoBehaviour {
    
    [SerializeField]
    UIMaster top;
    [SerializeField]
    UIMaster right;
    [SerializeField]
    UIMaster bottom;
    [SerializeField]
    UIMaster left;
    [SerializeField]
    UIMaster middle;

    private Vector2 center;

    private List<Change> changeQueue;

    [SerializeField]
    private GameObject attackUICallbackObj;
    private IAttackUICallback attackUICallback;
    public interface IAttackUICallback
    {
        void attackAnimationDone();
    }

    [SerializeField]
    private UnityEngine.UI.Text depthText;
    public int depth
    {
        set
        {
            depthText.text = value.ToString();
        }
    }

    private bool _attackMode;
    public bool attackMode
    {
        get { return _attackMode; }
        set
        {
            _attackMode = value;
            if(value)
            {
                transitionQueue.Add(new AttackModeChange(top, AttackModeChange.Type.ToAttack));
                transitionQueue.Add(new AttackModeChange(right, AttackModeChange.Type.ToAttack));
                transitionQueue.Add(new AttackModeChange(bottom, AttackModeChange.Type.ToAttack));
                transitionQueue.Add(new AttackModeChange(left, AttackModeChange.Type.ToAttack));
                transitionQueue.Add(new AttackModeChange(middle, AttackModeChange.Type.ToAttack));
            }
            else
            {
                transitionQueue.Add(new AttackModeChange(top, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(right, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(bottom, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(left, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(middle, AttackModeChange.Type.FromAttack));
            }
        }
    }
    public static float inactiveScaleFactor = 0.6f;
    public static float transitionTime = 250; //ms

    private List<AttackModeChange> transitionQueue;

    class AttackModeChange
    {
        public float totalTime;
        public float timeLeft;
        public UIMaster shape;
        public Type type;

        public enum Type { ToAttack, FromAttack };

        public AttackModeChange(UIMaster shape, Type type)
        {
            this.totalTime = transitionTime;
            this.timeLeft = totalTime;
            this.shape = shape;
            this.type = type;
        }

        public AttackModeChange(float totalTime, UIMaster shape, Type type)
        {
            this.totalTime = totalTime;
            this.timeLeft = totalTime;
            this.shape = shape;
            this.type = type;
        }
    }
    
    class Change
    {
        public Vector2 startPos;
        //public float maxSize;
        public float totalTime;
        public float timeLeft;
        public UIMaster shape;
        public ChangeMode mode;
        public Direction? direction;
        public Change next;
        public Color? color;
        public Shape? form;
        public int? depth;

        public enum ChangeMode { Shrink, Grow, MoveToCenter, MoveToOriginalPosition, Hide, Show, Color, Shape, Depth }
        

        public Change(UIMaster shape, float msTime, ChangeMode mode, Change next = null)
        {
            //this.maxSize = shape.maxSize;
            this.totalTime = msTime;
            this.timeLeft = msTime;
            this.shape = shape;
            this.mode = mode;
            this.next = next;
        }

        public Change() { } //Here to make the compiler fine with the DelayChange that doesn't call the above constructor

        public override string ToString()
        {
            return shape.name + "." + mode.ToString() + " : " + timeLeft + "/" + totalTime;
        }
    }

    class DelayedChange : Change
    {
        public Change change;
        public bool ready;

        public DelayedChange(Change change)
        {
            this.change = change;
            ready = false;
        }

        public DelayedChange(UIMaster shape, float msTime, ChangeMode mode, Change next = null) : base(shape, msTime, mode, next)
        {
            ready = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        changeQueue = new List<Change>();
        transitionQueue = new List<AttackModeChange>();

        depthText = transform.Find("DepthText").GetComponent<UnityEngine.UI.Text>();

        attackUICallback = attackUICallbackObj.GetComponent<AttackLogic>();

        findShapes();
        center = middle.animationCenter;
        if (attackUICallback != null) { attackUICallback.attackAnimationDone(); }
    }

    // Update is called once per frame
    void Update()
    {
        bool foundDelay = false;
        float timePassed = Time.deltaTime * 1000; //Seconds to miliseconds
        for (int i = 0; i < changeQueue.Count; i++)
        {
            Change change = changeQueue[i];
            if (change.GetType() == typeof(DelayedChange) && ((DelayedChange)change).ready) { change = ((DelayedChange)change).change; }

            if (change.GetType() != typeof(DelayedChange))
            {
                if (timePassed > change.timeLeft)
                {
                    if (change.mode == Change.ChangeMode.Shrink)
                    {
                        change.shape.animationSizePercent = 0;
                    }
                    else if (change.mode == Change.ChangeMode.Grow)
                    {
                        change.shape.animationSizePercent = 1;
                    }
                    else if (change.mode == Change.ChangeMode.MoveToCenter)
                    {
                        change.shape.animationCenter = center;
                    }
                    else if(change.mode == Change.ChangeMode.MoveToOriginalPosition)
                    {
                        //change.shape.center = change.shape.startPos;
                        change.shape.animationCenter = new Vector2(0, 0);
                    }
                    else if(change.mode == Change.ChangeMode.Hide) //No transition equivilent
                    {
                        change.shape.visible = false;
                    }
                    else if(change.mode == Change.ChangeMode.Show) //No transition equivilent
                    {
                        change.shape.visible = true;
                    }
                    else if (change.mode == Change.ChangeMode.Color) //No transition equivilent
                    {
                        change.shape.fillColor = change.color != null ? (Color)change.color : Color.black;
                    }
                    else if(change.mode == Change.ChangeMode.Shape) //No transition equivilent
                    {
                        change.shape.shape = change.form != null ? (Shape)change.form : Shape.Circle;
                    }
                    else if(change.mode == Change.ChangeMode.Depth)
                    {
                        depth = change.depth ?? 0;
                    }
                    change.timeLeft = 0;
                    changeQueue.RemoveAt(i);
                    i--;
                    if (change.next != null)
                    {
                        changeQueue.Add(change.next);
                    }
                    continue;
                }
                change.timeLeft -= timePassed;
                if (change.mode == Change.ChangeMode.Shrink)
                {
                    float percent = change.timeLeft / change.totalTime;
                    change.shape.animationSizePercent = percent;
                }
                else if (change.mode == Change.ChangeMode.Grow)
                {
                    float percent = 1 - (change.timeLeft / change.totalTime);
                    change.shape.animationSizePercent = percent;
                }
                else if (change.mode == Change.ChangeMode.MoveToCenter)
                {
                    if (change.direction == null)
                    {
                        if (change.shape == top) { change.direction = Direction.Up; }
                        else if (change.shape == right) { change.direction = Direction.Right; }
                        else if (change.shape == bottom) { change.direction = Direction.Down; }
                        else if (change.shape == left) { change.direction = Direction.Left; }
                    }
                    float percent = change.timeLeft / change.totalTime;
                    percent -= 1;
                    switch (change.direction)
                    {
                        case Direction.Up:
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case Direction.Right:
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                        case Direction.Down:
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case Direction.Left:
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                    }
                }
                else if (change.mode == Change.ChangeMode.MoveToOriginalPosition)
                {
                    if (change.direction == null)
                    {
                        if (change.shape == top) { change.direction = Direction.Up; }
                        else if (change.shape == right) { change.direction = Direction.Right; }
                        else if (change.shape == bottom) { change.direction = Direction.Down; }
                        else if (change.shape == left) { change.direction = Direction.Left; }
                    }
                    float percent = 1 - (change.timeLeft / change.totalTime);
                    percent -= 1;
                    switch (change.direction)
                    {
                        case Direction.Up:
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case Direction.Right:
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                        case Direction.Down:
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case Direction.Left:
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                    }
                }
                else if (change.mode == Change.ChangeMode.Hide)
                {
                    change.shape.visible = false;
                }
                else if (change.mode == Change.ChangeMode.Show)
                {
                    change.shape.visible = true;
                }
            }

            if (change.GetType() == typeof(DelayedChange) && !((DelayedChange)change).ready)
            {
                //I'm not sure if I want this functionality.
                //I'm leaving it for now in case I do
                //foundDelay = true; 
                ((DelayedChange)change).ready = true;
            }
        }
        if(changeQueue.Count == 0)
        {
            attackUICallback.attackAnimationDone();
        }

        foreach(AttackModeChange amc in transitionQueue)
        {
            if(amc.timeLeft < timePassed)
            {
                if(amc.type == AttackModeChange.Type.FromAttack)
                {
                    amc.shape.transitionPositionScale = inactiveScaleFactor;
                    amc.shape.activeScaleFactor = inactiveScaleFactor;
                }
                else if(amc.type == AttackModeChange.Type.ToAttack)
                {
                    amc.shape.transitionPositionScale = 1;
                    amc.shape.activeScaleFactor = 1;
                }
                amc.timeLeft = 0;
                transitionQueue.Remove(amc);
            }
            else
            {
                amc.timeLeft -= timePassed;
                float percent = amc.timeLeft / amc.totalTime;
                float scaleRange = 1 - inactiveScaleFactor;
                Debug.Log(percent);
                if (amc.type == AttackModeChange.Type.FromAttack)
                {
                    amc.shape.transitionPositionScale = percent * scaleRange + inactiveScaleFactor;
                    amc.shape.activeScaleFactor = percent * scaleRange + inactiveScaleFactor;
                }
                else if (amc.type == AttackModeChange.Type.ToAttack)
                {
                    amc.shape.transitionPositionScale = (1 - percent) * scaleRange + inactiveScaleFactor;
                    amc.shape.activeScaleFactor = (1-percent)*scaleRange + inactiveScaleFactor;
                }
            }
        }
    }

    private void confirmCircles()
    {
        if( top == null || right == null || bottom == null || left == null || middle == null )
        {
            findShapes();
        }
    }

    private void findShapes()
    {
        top = GameObject.Find("Top").gameObject.GetComponent<UIMaster>();
        right = GameObject.Find("Right").gameObject.GetComponent<UIMaster>();
        bottom = GameObject.Find("Bottom").gameObject.GetComponent<UIMaster>();
        left = GameObject.Find("Left").gameObject.GetComponent<UIMaster>();
        middle = GameObject.Find("Middle").gameObject.GetComponent<UIMaster>();
    }

    #region Shrink Shape
    private void shrinkShape(Direction dir, float msTime)
    {
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Shrink));
    }

    private void shrinkShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Shrink));
    }

    private Change getShrinkShapeChange(Direction dir, float msTime)
    {
        if (msTime < 0) { return null; }
        return new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Shrink);
    }

    private Change getShrinkShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Shrink);
    }
    #endregion

    #region Grow Shape
    private void growShape(Direction dir, float msTime)
    {
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Grow));
    }

    private void growShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Grow));
    }

    private Change getGrowShapeChange(Direction dir, float msTime)
    {
        if (msTime < 0) { return null; }
        return new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Grow);
    }

    private Change getGrowShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Grow);
    }
    #endregion

    #region Move Shape To Center
    private void moveShapeToCenter(Direction dir, float msTime)
    {
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.MoveToCenter));
    }

    private void moveShapeToCenter(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.MoveToCenter));
    }

    private Change getMoveShapeToCenterChange(Direction dir, float msTime)
    {
        if (msTime < 0) { return null; }
        return new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.MoveToCenter);
    }

    private Change getMoveShapeToCenterChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.MoveToCenter);
    }
    #endregion

    #region Move Shape To Original Position
    private void moveShapeToOriginalPosition(Direction dir, float msTime)
    {
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.MoveToOriginalPosition));
    }

    private void moveShapeToOriginalPosition(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.MoveToOriginalPosition));
    }

    private Change getMoveShapeToOriginalPositionChange(Direction dir, float msTime)
    {
        if (msTime < 0) { return null; }
        return new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.MoveToOriginalPosition);
    }

    private Change getMoveShapeToOriginalPositionChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.MoveToOriginalPosition);
    }
    #endregion

    #region Hide Shape
    private void hideShape(Direction dir, float msTime)
    {
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Hide));
    }

    private void hideShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Hide));
    }

    private Change getHideShapeChange(Direction dir, float msTime)
    {
        if (msTime < 0) { return null; }
        return new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Hide);
    }

    private Change getHideShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Hide);
    }
    #endregion

    #region Show Shape
    private void showShape(Direction dir, float msTime)
    {
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Show));
    }

    private void showShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Show));
    }

    private Change getShowShapeChange(Direction dir, float msTime)
    {
        if (msTime < 0) { return null; }
        return new Change(getMasterFromDir(dir), msTime, Change.ChangeMode.Show);
    }

    private Change getShowShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Show);
    }
    #endregion

    #region Set Shape Color
    private void setShapeColor(Direction dir, Color color)
    {
        if (color == null) { return; }
        Change c = new Change(getMasterFromDir(dir), 0, Change.ChangeMode.Color);
        c.color = color;
        changeQueue.Add(c);
    }

    private void setShapeColor(UIMaster shape, Color color)
    {
        if (shape == null) { return; }
        if (color == null) { return; }
        Change c = new Change(shape, 0, Change.ChangeMode.Color);
        c.color = color;
        changeQueue.Add(c);
    }

    private Change getSetShapeColorChange(Direction dir,  Color color)
    {
        if (color == null) { return null; }
        Change c = new Change(getMasterFromDir(dir), 0, Change.ChangeMode.Color);
        c.color = color;
        return c;
    }

    private Change getSetShapeColorChange(UIMaster shape, Color color)
    {
        if (shape == null) { return null; }
        if (color == null) { return null; }
        Change c = new Change(shape, 0, Change.ChangeMode.Color);
        c.color = color;
        return c;
    }
    #endregion

    #region Set Shape Shape
    private void setShapeShape(Direction dir, Shape newShape)
    {
        Change c = new Change(getMasterFromDir(dir), 0, Change.ChangeMode.Shape);
        c.form = newShape;
        changeQueue.Add(c);
    }

    private void setShapeShape(UIMaster shape, Shape newShape)
    {
        if (shape == null) { return; }
        Change c = new Change(shape, 0, Change.ChangeMode.Shape);
        c.form = newShape;
        changeQueue.Add(c);
    }

    private Change getSetShapeShapeChange(Direction dir, Shape newShape)
    {
        Change c = new Change(getMasterFromDir(dir), 0, Change.ChangeMode.Shape);
        c.form = newShape;
        return c;
    }

    private Change getSetShapeShapeChange(UIMaster shape, Shape newShape)
    {
        if (shape == null) { return null; }
        Change c = new Change(shape, 0, Change.ChangeMode.Shape);
        c.form = newShape;
        return c;
    }
    #endregion

    #region Set Depth
    private void setDepth(int depth)
    {
        this.depth = depth;
    }

    private Change getSetDepthChange(int depth, float msTime)
    {
        Change c = new Change(middle, msTime, Change.ChangeMode.Depth);
        c.depth = depth;
        return c;
    }
    #endregion

    #region Select Circle
    public void selectCircle(Direction dir, float time, Dictionary<Direction, Color> nextOptions, Dictionary<Direction, Shape> shapes, int depth)
    {
        if(time < 0) { return; }
        List<Change> changes = new List<Change>();
        UIMaster selected = getMasterFromDir(dir);
        if (top != selected)
        {
            Change temp = getShrinkShapeChange(top, time / 2);
            if (nextOptions.ContainsKey(Direction.Up))
            {
                temp.next = getSetShapeColorChange(top, nextOptions[Direction.Up]);
                temp.next.next = getSetShapeShapeChange(top, shapes[Direction.Up]);
                temp.next.next.next = getShowShapeChange(top, 0);
                temp.next.next.next.next = getGrowShapeChange(top, time / 2);
            }
            else { temp.next = getHideShapeChange(top, 0); }
            changes.Add(temp);
        }
        if (right != selected)
        {
            Change temp = getShrinkShapeChange(right, time / 2);
            if (nextOptions.ContainsKey(Direction.Right))
            {
                temp.next = getSetShapeColorChange(right, nextOptions[Direction.Right]);
                temp.next.next = getSetShapeShapeChange(top, shapes[Direction.Right]);
                temp.next.next.next = getShowShapeChange(right, 0);
                temp.next.next.next.next = getGrowShapeChange(right, time / 2);
            }
            else { temp.next = getHideShapeChange(right, 0); }
            changes.Add(temp);
        }
        if (bottom != selected)
        {
            Change temp = getShrinkShapeChange(bottom, time / 2);
            if (nextOptions.ContainsKey(Direction.Down))
            {
                temp.next = getSetShapeColorChange(bottom, nextOptions[Direction.Down]);
                temp.next.next = getSetShapeShapeChange(top, shapes[Direction.Down]);
                temp.next.next.next = getShowShapeChange(bottom, 0);
                temp.next.next.next.next = getGrowShapeChange(bottom, time / 2);
            }
            else { temp.next = getHideShapeChange(bottom, 0); }
            changes.Add(temp);
        }
        if (left != selected)
        {
            Change temp = getShrinkShapeChange(left, time / 2);
            if (nextOptions.ContainsKey(Direction.Left))
            {
                temp.next = getSetShapeColorChange(left, nextOptions[Direction.Left]);
                temp.next.next = getSetShapeShapeChange(top, shapes[Direction.Left]);
                temp.next.next.next = getShowShapeChange(left, 0);
                temp.next.next.next.next = getGrowShapeChange(left, time / 2);
            }
            else { temp.next = getHideShapeChange(left, 0); }
            changes.Add(temp);
        }
        Change selectedChange = getMoveShapeToCenterChange(selected, time / 2);
        selectedChange.next = getMoveShapeToOriginalPositionChange(selected, 0);
        if(nextOptions.ContainsKey(getDirection(selected)))
        {
            selectedChange.next.next = getSetShapeColorChange(selected, nextOptions[dir]);
            selectedChange.next.next.next = getSetShapeShapeChange(selected, shapes[dir]);
            selectedChange.next.next.next.next = getShowShapeChange(selected, 0);
            selectedChange.next.next.next.next.next = getGrowShapeChange(selected, time / 2);
        }
        else { selectedChange.next.next = getHideShapeChange(selected, 0); }
        changes.Add(selectedChange);

        Change centerChange = getSetShapeShapeChange(middle, selected.shape);
        centerChange.next = getShrinkShapeChange(middle, time / 2);
        centerChange.next.next = getGrowShapeChange(middle, 0);
        changeQueue.Add(centerChange);

        changeQueue.Add(getSetDepthChange(depth, time/2));

        foreach (Change c1 in changes)
        {
            Change found = changeQueue.Find(c => c.shape == c1.shape);

            if(found == null)
            {
                changeQueue.Add(c1);
            }
            else
            {
                if(found.shape == middle && found.mode == Change.ChangeMode.Shrink) { break; }
                while (found.next != null)
                {
                    found = found.next;
                }
                found.next = c1;
            }
        }
    }
    #endregion

    private UIMaster getShape(Direction code)
    {
        UIMaster ret = null;

        switch(code)
        {
            case Direction.Up:
                ret = top;
                break;
            case Direction.Right:
                ret = right;
                break;
            case Direction.Down:
                ret = bottom;
                break;
            case Direction.Left:
                ret = left;
                break;
        }

        return ret;
    }

    private Direction getDirection(UIMaster shape)
    {
        if (shape == top) { return Direction.Up; }
        if (shape == right) { return Direction.Right; }
        if (shape == bottom) { return Direction.Down; }
        if (shape == left) { return Direction.Left; }
        return Direction.Middle;
    }

    private UIMaster getMasterFromDir(Direction dir)
    {
        if(dir == Direction.Up) { return top; }
        if(dir == Direction.Right) { return right; }
        if(dir == Direction.Down) { return bottom; }
        if(dir == Direction.Left) { return left; }
        return null;
    }

    private Shape getShapeFromCode(char code)
    {
        switch (code)
        {
            case 'C': //Circle
            case 'c':
                return Shape.Circle;
            case 'D':
            case 'd':
                return Shape.Diamond;
            case 'R': //Rectangle
            case 'r':
            case 'S': //Square
            case 's':
                return Shape.Rect;
        }
        return (Shape)(-1);
    }

    public void setColors(Dictionary<Direction, Color> colors)
    {
        if (colors.ContainsKey(Direction.Up)) { top.fillColor = colors[Direction.Up]; }
        if (colors.ContainsKey(Direction.Right)) { right.fillColor = colors[Direction.Right]; }
        if (colors.ContainsKey(Direction.Down)) { bottom.fillColor = colors[Direction.Down]; }
        if (colors.ContainsKey(Direction.Left)) { left.fillColor = colors[Direction.Left]; }
    }

    public void setShapes(Dictionary<Direction, Shape> shapes)
    {
        if (shapes.ContainsKey(Direction.Up)) { top.shape = shapes[Direction.Up]; }
        if (shapes.ContainsKey(Direction.Right)) { right.shape = shapes[Direction.Right]; }
        if (shapes.ContainsKey(Direction.Down)) { bottom.shape = shapes[Direction.Down]; }
        if (shapes.ContainsKey(Direction.Left)) { left.shape = shapes[Direction.Left]; }
    }

    public void queueReset(float msTime, Dictionary<Direction, Color> colors, Dictionary<Direction, Shape> shapes)
    {
        Change temp;
        UIMaster shape = getShape(Direction.Up);
        Change current = changeQueue.Find(change => change.shape == shape);
        while(current.next != null) { current = current.next; }
        temp = getSetShapeColorChange(Direction.Up, colors[Direction.Up]);
        temp.next = getMoveShapeToOriginalPositionChange(Direction.Up, 0);
        temp.next.next = getSetShapeShapeChange(Direction.Up, shapes[Direction.Up]);
        temp.next.next.next = getShowShapeChange(Direction.Up, 0);
        temp.next.next.next.next = getGrowShapeChange(Direction.Up, msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }

        shape = getShape(Direction.Right);
        current = changeQueue.Find(change => change.shape == shape);
        while (current.next != null) { current = current.next; }
        temp = getSetShapeColorChange(Direction.Right, colors[Direction.Right]);
        temp.next = getMoveShapeToOriginalPositionChange(Direction.Right, 0);
        temp.next.next = getSetShapeShapeChange(Direction.Right, shapes[Direction.Right]);
        temp.next.next.next = getShowShapeChange(Direction.Right, 0);
        temp.next.next.next.next = getGrowShapeChange(Direction.Right, msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }

        shape = getShape(Direction.Down);
        current = changeQueue.Find(change => change.shape == shape);
        while (current.next != null) { current = current.next; }
        temp = getSetShapeColorChange(Direction.Down, colors[Direction.Down]);
        temp.next = getMoveShapeToOriginalPositionChange(Direction.Down, 0);
        temp.next.next = getSetShapeShapeChange(Direction.Down, shapes[Direction.Down]);
        temp.next.next.next = getShowShapeChange(Direction.Down, 0);
        temp.next.next.next.next = getGrowShapeChange(Direction.Down, msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }

        shape = getShape(Direction.Left);
        current = changeQueue.Find(change => change.shape == shape);
        while (current.next != null) { current = current.next; }
        temp = getSetShapeColorChange(Direction.Left, colors[Direction.Left]);
        temp.next = getMoveShapeToOriginalPositionChange(Direction.Left, 0);
        temp.next.next = getSetShapeShapeChange(Direction.Left, shapes[Direction.Left]);
        temp.next.next.next = getShowShapeChange(Direction.Left, 0);
        temp.next.next.next.next = getGrowShapeChange(Direction.Left, msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }
        
        Change depthC = changeQueue.Find(c => c.mode == Change.ChangeMode.Depth);
        if(depthC != null)
        {
            depthC.depth = 0;
            return;
        }

        bool found = false;
        foreach(Change c in changeQueue)
        {
            Change ch = c;
            while(ch.next != null && ch.mode != Change.ChangeMode.Shrink)
            {
                ch = ch.next;
            }

            if(ch.mode == Change.ChangeMode.Shrink)
            {
                found = true;
                Change n = getSetDepthChange(0, 0);
                n.next = ch.next;
                ch.next = n;
                break;
            }
        }
        if(!found)
        {
            changeQueue.Add(getSetDepthChange(0, 0));
        }
    }
}

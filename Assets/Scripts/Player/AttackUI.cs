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
                //top.transitionPositionScale = 1f;
                //right.transitionPositionScale = 1f;
                //bottom.transitionPositionScale = 1f;
                //left.transitionPositionScale = 1f;
                //middle.transitionPositionScale = 1f;

                //top.activeScaleFactor = 1f;
                //right.activeScaleFactor = 1f;
                //bottom.activeScaleFactor = 1f;
                //left.activeScaleFactor = 1f;
                //middle.activeScaleFactor = 1f;
            }
            else
            {
                transitionQueue.Add(new AttackModeChange(top, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(right, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(bottom, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(left, AttackModeChange.Type.FromAttack));
                transitionQueue.Add(new AttackModeChange(middle, AttackModeChange.Type.FromAttack));
                //top.transitionPositionScale = inactiveScaleFactor;
                //right.transitionPositionScale = inactiveScaleFactor;
                //bottom.transitionPositionScale = inactiveScaleFactor;
                //left.transitionPositionScale = inactiveScaleFactor;
                //middle.transitionPositionScale = inactiveScaleFactor;

                //top.activeScaleFactor = inactiveScaleFactor;
                //right.activeScaleFactor = inactiveScaleFactor;
                //bottom.activeScaleFactor = inactiveScaleFactor;
                //left.activeScaleFactor = inactiveScaleFactor;
                //middle.activeScaleFactor = inactiveScaleFactor;
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
        public char? direction;
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
                        if (change.shape == top) { change.direction = 'U'; }
                        else if (change.shape == right) { change.direction = 'R'; }
                        else if (change.shape == bottom) { change.direction = 'B'; }
                        else if (change.shape == left) { change.direction = 'L'; }
                    }
                    float percent = change.timeLeft / change.totalTime;
                    percent -= 1;
                    switch (change.direction)
                    {
                        case 'U':
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case 'R':
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                        case 'B':
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case 'L':
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                    }
                }
                else if (change.mode == Change.ChangeMode.MoveToOriginalPosition)
                {
                    if (change.direction == null)
                    {
                        if (change.shape == top) { change.direction = 'U'; }
                        else if (change.shape == right) { change.direction = 'R'; }
                        else if (change.shape == bottom) { change.direction = 'B'; }
                        else if (change.shape == left) { change.direction = 'L'; }
                    }
                    float percent = 1 - (change.timeLeft / change.totalTime);
                    percent -= 1;
                    switch (change.direction)
                    {
                        case 'U':
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case 'R':
                            change.shape.animationCenter = new Vector2(change.shape.startPos.x * percent, change.shape.animationCenter.y);
                            break;
                        case 'B':
                            change.shape.animationCenter = new Vector2(change.shape.animationCenter.x, change.shape.startPos.y * percent);
                            break;
                        case 'L':
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
    private void shrinkShape(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getShape(code), msTime, Change.ChangeMode.Shrink));
    }

    private void shrinkShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Shrink));
    }

    private Change getShrinkShapeChange(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (msTime < 0) { return null; }
        return new Change(getShape(code), msTime, Change.ChangeMode.Shrink);
    }

    private Change getShrinkShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Shrink);
    }
    #endregion

    #region Grow Shape
    private void growShape(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getShape(code), msTime, Change.ChangeMode.Grow));
    }

    private void growShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Grow));
    }

    private Change getGrowShapeChange(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (msTime < 0) { return null; }
        return new Change(getShape(code), msTime, Change.ChangeMode.Grow);
    }

    private Change getGrowShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Grow);
    }
    #endregion

    #region Move Shape To Center
    private void moveShapeToCenter(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getShape(code), msTime, Change.ChangeMode.MoveToCenter));
    }

    private void moveShapeToCenter(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.MoveToCenter));
    }

    private Change getMoveShapeToCenterChange(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (msTime < 0) { return null; }
        return new Change(getShape(code), msTime, Change.ChangeMode.MoveToCenter);
    }

    private Change getMoveShapeToCenterChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.MoveToCenter);
    }
    #endregion

    #region Move Shape To Original Position
    private void moveShapeToOriginalPosition(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getShape(code), msTime, Change.ChangeMode.MoveToOriginalPosition));
    }

    private void moveShapeToOriginalPosition(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.MoveToOriginalPosition));
    }

    private Change getMoveShapeToOriginalPositionChange(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (msTime < 0) { return null; }
        return new Change(getShape(code), msTime, Change.ChangeMode.MoveToOriginalPosition);
    }

    private Change getMoveShapeToOriginalPositionChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.MoveToOriginalPosition);
    }
    #endregion

    #region Hide Shape
    private void hideShape(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getShape(code), msTime, Change.ChangeMode.Hide));
    }

    private void hideShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Hide));
    }

    private Change getHideShapeChange(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (msTime < 0) { return null; }
        return new Change(getShape(code), msTime, Change.ChangeMode.Hide);
    }

    private Change getHideShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Hide);
    }
    #endregion

    #region Show Shape
    private void showShape(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(getShape(code), msTime, Change.ChangeMode.Show));
    }

    private void showShape(UIMaster shape, float msTime)
    {
        if (shape == null) { return; }
        if (msTime < 0) { return; }
        changeQueue.Add(new Change(shape, msTime, Change.ChangeMode.Show));
    }

    private Change getShowShapeChange(char code, float msTime)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (msTime < 0) { return null; }
        return new Change(getShape(code), msTime, Change.ChangeMode.Show);
    }

    private Change getShowShapeChange(UIMaster shape, float msTime)
    {
        if (shape == null) { return null; }
        if (msTime < 0) { return null; }
        return new Change(shape, msTime, Change.ChangeMode.Show);
    }
    #endregion

    #region Set Shape Color
    private void setShapeColor(char code, Color color)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (color == null) { return; }
        Change c = new Change(getShape(code), 0, Change.ChangeMode.Color);
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

    private Change getSetShapeColorChange (char code,  Color color)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (color == null) { return null; }
        Change c = new Change(getShape(code), 0, Change.ChangeMode.Color);
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
    private void setShapeShape(char code, char shapeCode)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        if (shapeCode == '\0') { return; }
        Change c = new Change(getShape(code), 0, Change.ChangeMode.Shape);
        c.form = getShapeFromCode(shapeCode);
        changeQueue.Add(c);
    }

    private void setShapeShape(UIMaster shape, char shapeCode)
    {
        if (shape == null) { return; }
        if (shapeCode == '\0') { return; }
        Change c = new Change(shape, 0, Change.ChangeMode.Shape);
        c.form = getShapeFromCode(shapeCode);
        changeQueue.Add(c);
    }

    private Change getSetShapeShapeChange(char code, char shapeCode)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        if (shapeCode == '\0') { return null; }
        Change c = new Change(getShape(code), 0, Change.ChangeMode.Shape);
        c.form = getShapeFromCode(shapeCode);
        return c;
    }

    private Change getSetShapeShapeChange(UIMaster shape, char shapeCode)
    {
        if (shape == null) { return null; }
        if (shapeCode == '\0') { return null; }
        Change c = new Change(shape, 0, Change.ChangeMode.Shape);
        c.form = getShapeFromCode(shapeCode);
        return c;
    }

    private void setShapeShape(char code, Shape newShape)
    {
        code = unifyCode(code);
        if (code == '\0') { return; }
        Change c = new Change(getShape(code), 0, Change.ChangeMode.Shape);
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

    private Change getSetShapeShapeChange(char code, Shape newShape)
    {
        code = unifyCode(code);
        if (code == '\0') { return null; }
        Change c = new Change(getShape(code), 0, Change.ChangeMode.Shape);
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
    public void selectCircle(char code, float time, Dictionary<char, Color> nextOptions, Dictionary<char, char> shapes, int depth)
    {
        if(code == '\0') { return; }
        if(time < 0) { return; }
        List<Change> changes = new List<Change>();
        UIMaster selected = getShape(code);
        if (top != selected)
        {
            Change temp = getShrinkShapeChange(top, time / 2);
            if (nextOptions.ContainsKey('U'))
            {
                temp.next = getSetShapeColorChange(top, nextOptions['U']);
                temp.next.next = getSetShapeShapeChange(top, shapes['U']);
                temp.next.next.next = getShowShapeChange(top, 0);
                temp.next.next.next.next = getGrowShapeChange(top, time / 2);
            }
            else { temp.next = getHideShapeChange(top, 0); }
            changes.Add(temp);
        }
        if (right != selected)
        {
            Change temp = getShrinkShapeChange(right, time / 2);
            if (nextOptions.ContainsKey('R'))
            {
                temp.next = getSetShapeColorChange(right, nextOptions['R']);
                temp.next.next = getSetShapeShapeChange(top, shapes['R']);
                temp.next.next.next = getShowShapeChange(right, 0);
                temp.next.next.next.next = getGrowShapeChange(right, time / 2);
            }
            else { temp.next = getHideShapeChange(right, 0); }
            changes.Add(temp);
        }
        if (bottom != selected)
        {
            Change temp = getShrinkShapeChange(bottom, time / 2);
            if (nextOptions.ContainsKey('D'))
            {
                temp.next = getSetShapeColorChange(bottom, nextOptions['D']);
                temp.next.next = getSetShapeShapeChange(top, shapes['D']);
                temp.next.next.next = getShowShapeChange(bottom, 0);
                temp.next.next.next.next = getGrowShapeChange(bottom, time / 2);
            }
            else { temp.next = getHideShapeChange(bottom, 0); }
            changes.Add(temp);
        }
        if (left != selected)
        {
            Change temp = getShrinkShapeChange(left, time / 2);
            if (nextOptions.ContainsKey('L'))
            {
                temp.next = getSetShapeColorChange(left, nextOptions['L']);
                temp.next.next = getSetShapeShapeChange(top, shapes['L']);
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
            selectedChange.next.next = getSetShapeColorChange(selected, nextOptions[code]);
            selectedChange.next.next.next = getSetShapeShapeChange(selected, shapes[code]);
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

    private UIMaster getShape(char code)
    {
        UIMaster ret = null;

        switch(code)
        {
            case 'U':
                ret = top;
                break;
            case 'R':
                ret = right;
                break;
            case 'D':
                ret = bottom;
                break;
            case 'L':
                ret = left;
                break;
        }

        return ret;
    }

    private char getDirection(UIMaster shape)
    {
        if (shape == top) { return 'U'; }
        if (shape == right) { return 'R'; }
        if (shape == bottom) { return 'D'; }
        if (shape == left) { return 'L'; }
        return '\0';
    }

    private char unifyCode(char c)
    {
        switch(c)
        {
            case 'U':
            case 'u':
            case 'T':
            case 't':
            case 'N':
            case 'n':
                return 'U';
            case 'R':
            case 'r':
            case 'E':
            case 'e':
                return 'R';
            case 'D':
            case 'd':
            case 'B':
            case 'b':
            case 'S':
            case 's':
                return 'D';
            case 'L':
            case 'l':
            case 'W':
            case 'w':
                return 'L';
        }
        return '\0';
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

    public void setColors(Dictionary<char, Color> colors)
    {
        if (colors.ContainsKey('U')) { top.fillColor = colors['U']; }
        if (colors.ContainsKey('R')) { right.fillColor = colors['R']; }
        if (colors.ContainsKey('D')) { bottom.fillColor = colors['D']; }
        if (colors.ContainsKey('L')) { left.fillColor = colors['L']; }
    }

    public void setShapes(Dictionary<char, char> shapes)
    {
        if (shapes.ContainsKey('U')) { top.shape = getShapeFromCode(shapes['U']); }
        if (shapes.ContainsKey('R')) { right.shape = getShapeFromCode(shapes['R']); }
        if (shapes.ContainsKey('D')) { bottom.shape = getShapeFromCode(shapes['D']); }
        if (shapes.ContainsKey('L')) { left.shape = getShapeFromCode(shapes['L']); }
    }

    public void queueReset(float msTime, Dictionary<char, Color> colors, Dictionary<char, char> shapes)
    {
        Change temp;
        UIMaster shape = getShape('U');
        Change current = changeQueue.Find(change => change.shape == shape);
        while(current.next != null) { current = current.next; }
        temp = getSetShapeColorChange('U', colors['U']);
        temp.next = getMoveShapeToOriginalPositionChange('U', 0);
        temp.next.next = getSetShapeShapeChange('U', shapes['U']);
        temp.next.next.next = getShowShapeChange('U', 0);
        temp.next.next.next.next = getGrowShapeChange('U', msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }

        shape = getShape('R');
        current = changeQueue.Find(change => change.shape == shape);
        while (current.next != null) { current = current.next; }
        temp = getSetShapeColorChange('R', colors['R']);
        temp.next = getMoveShapeToOriginalPositionChange('R', 0);
        temp.next.next = getSetShapeShapeChange('R', shapes['R']);
        temp.next.next.next = getShowShapeChange('R', 0);
        temp.next.next.next.next = getGrowShapeChange('R', msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }

        shape = getShape('D');
        current = changeQueue.Find(change => change.shape == shape);
        while (current.next != null) { current = current.next; }
        temp = getSetShapeColorChange('D', colors['D']);
        temp.next = getMoveShapeToOriginalPositionChange('D', 0);
        temp.next.next = getSetShapeShapeChange('D', shapes['D']);
        temp.next.next.next = getShowShapeChange('D', 0);
        temp.next.next.next.next = getGrowShapeChange('D', msTime);
        if (current != null) { current.next = temp; }
        else { changeQueue.Add(temp); }

        shape = getShape('L');
        current = changeQueue.Find(change => change.shape == shape);
        while (current.next != null) { current = current.next; }
        temp = getSetShapeColorChange('L', colors['L']);
        temp.next = getMoveShapeToOriginalPositionChange('L', 0);
        temp.next.next = getSetShapeShapeChange('L', shapes['L']);
        temp.next.next.next = getShowShapeChange('L', 0);
        temp.next.next.next.next = getGrowShapeChange('L', msTime);
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

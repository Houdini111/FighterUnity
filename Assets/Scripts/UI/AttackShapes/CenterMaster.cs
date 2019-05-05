using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterMaster : UIMaster
{
    [SerializeField]
    private AttackShape circle;
    [SerializeField]
    private AttackShape diamond;
    [SerializeField]
    private AttackShape square;

    public override Color fillColor
    {
        get { return getCurrent().fillColor; }
        set { getCurrent().fillColor = value; }
    }

    /*
    public float size
    {
        get
        {
            return circle.size.x;
        }
        set
        {
            Vector2 n = new Vector2(value, value);
            circle.size = n;
            diamond.size = n;
        }
    }
    */

    private float _animationSizePercent;
    public override float animationSizePercent
    {
        get { return _animationSizePercent; }
        set
        {
            _animationSizePercent = value;
            recalculateSize();
        }
    }

    private float _activeScaleFactor;
    public override float activeScaleFactor
    {
        get
        {
            return _activeScaleFactor;
        }
        set
        {
            _activeScaleFactor = value;
            recalculateSize();
        }
    }

    private void recalculateSize()
    {
        float percent = _animationSizePercent * _activeScaleFactor;
        circle.size = new Vector2(circle.maxSize * percent, circle.maxSize * percent);
        diamond.size = new Vector2(diamond.maxSize * percent, diamond.maxSize * percent);
        square.size = new Vector2(square.maxSize * percent, square.maxSize * percent);
    }

    //private float _sizePercent;
    //public override float sizePercent
    //{
    //    get
    //    {
    //        return _sizePercent;
    //    }
    //    set
    //    {
    //        _sizePercent = value;
    //        AttackShape c = getCurrent();
    //        c.size = new Vector2(c.maxSize * _sizePercent, c.maxSize * _sizePercent);
    //    }
    //}


    public override float rotation
    {
        get { return getCurrent().rotation; }
        set { getCurrent().rotation = value; }
    }

    
    public override Vector2 animationCenter
    {
        get { return getCurrent().center; }
        set { getCurrent().center = value; }
    }

    private float _transitionPositionScale;
    public override float transitionPositionScale
    {
        get { return _transitionPositionScale; }
        set
        {
            _transitionPositionScale = value;
            //CHANGE MASTER POSITION
        }
    }

    public override Vector2 startPos
    {
        get { return circle.startPos; }
        set { }
    }

    public override bool visible
    {
        get { return getCurrent().visible; }
        set { getCurrent().visible = value; }
    }

    private Shape currentShape;
    public override Shape shape
    {
        get { return currentShape; }
        set { currentShape = value; }
    }

    void Start()
    {
        if (circle == null)
        {
            GameObject go = gameObject.transform.Find("Circle").gameObject;
            circle = go.GetComponent<AttackShape>();
        }
        if (diamond == null)
        {
            GameObject go = gameObject.transform.Find("Diamond").gameObject;
            diamond = go.GetComponent<AttackShape>();
        }
        if (diamond == null)
        {
            GameObject go = gameObject.transform.Find("Square").gameObject;
            square = go.GetComponent<AttackShape>();
        }

        _animationSizePercent = 1;
        _activeScaleFactor = 1;
    }

    private AttackShape getCurrent()
    {
        if(currentShape == Shape.Circle)
        {
            return circle;
        }
        else if(currentShape == Shape.Rect)
        {
            return square;
        }
        else if(currentShape == Shape.Diamond)
        {
            return diamond;
        }
        return null;
    }
}
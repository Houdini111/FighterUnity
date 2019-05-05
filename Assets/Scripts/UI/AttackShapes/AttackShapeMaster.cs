using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShapeMaster : UIMaster
{
    [SerializeField]
    private AttackShape circle;
    [SerializeField]
    private AttackShape diamond;
    [SerializeField]
    private AttackShape square;

    GameObject parent;
    RectTransform rt;
    Vector2 originalPos;
    private Vector2 panelMaxSize;

    /*
    private GameObject fillObj;
    private GameObject outlineObj;
    [SerializeField]
    private UICircle fill;
    [SerializeField]
    private UICircle outline;
    */

    public override Color fillColor
    {
        get { return circle.fillColor; }
        set
        {
            circle.fillColor = value;
            diamond.fillColor = value;
            square.fillColor = value;
        }
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
    //        circle.size = new Vector2(circle.maxSize * _sizePercent, circle.maxSize * _sizePercent);
    //        diamond.size = new Vector2(diamond.maxSize * _sizePercent, diamond.maxSize * _sizePercent);
    //        square.size = new Vector2(square.maxSize * _sizePercent, square.maxSize * _sizePercent);
    //    }
    //}

    public override Vector2 animationCenter
    {
        get { return circle.center; }
        set
        {
            circle.center = value;
            diamond.center = value;
            square.center = value;
        }
    }

    /*
    public float maxSize
    {
        get { return circle.maxSize; }
        set { }
    }
    */

    public override float rotation
    {
        get
        {
            return circle.rotation;
        }
        set
        {
            circle.rotation = value;
            diamond.rotation = value;
            square.rotation = value;
        }
    }

    private float _transitionPositionScale;
    public override float transitionPositionScale
    {
        get { return _transitionPositionScale; }
        set
        {
            _transitionPositionScale = value;
            //CHANGE MASTER POSITION
            if (value == 1f)
            {
                rt.anchoredPosition = originalPos;
            }
            else
            {
                float max = parent.GetComponent<RectTransform>().rect.width;
                //rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (max * value), rt.anchoredPosition.y - (max * value));
                rt.anchoredPosition = new Vector2(originalPos.x + (max * (1-value)), originalPos.y - (max * (1-value)));
            }
        }
    }

    

    public override Vector2 startPos
    {
        get { return circle.startPos; }
        set { }
    }

    public override bool visible
    {
        get
        {
            return circle.visible || diamond.visible || square.visible;
        }
        set
        {
            if(value)
            {
                if (currentShape == Shape.Circle)
                {
                    circle.visible = true;
                    diamond.visible = false;
                    square.visible = false;
                }
                else if (currentShape == Shape.Diamond)
                {
                    circle.visible = false;
                    diamond.visible = true;
                    square.visible = false;
                }
                else if(currentShape == Shape.Rect)
                {
                    circle.visible = false;
                    diamond.visible = false;
                    square.visible = true;
                }
            }
            else
            {
                circle.visible = false;
                diamond.visible = false;
                square.visible = false;
            }
        }
    }

    private Shape currentShape;
    public override Shape shape
    {
        get
        {
            return currentShape;
        }
        set
        {
            if(value == Shape.Circle)
            {
                circle.visible = true;
                diamond.visible = false;
                square.visible = false;
                currentShape = Shape.Circle;
            }
            else if (value == Shape.Diamond)
            {
                circle.visible = false;
                diamond.visible = true;
                square.visible = false;
                currentShape = Shape.Diamond;
            }
            else if (value == Shape.Rect)
            {
                circle.visible = false;
                diamond.visible = false;
                square.visible = true;
                currentShape = Shape.Rect;
            }
        }
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

        rt = GetComponent<RectTransform>();
        if (rt.position.x > 0) { direction = Direction.Right; }
        else if (rt.position.x < 0) { direction = Direction.Left; }
        else if (rt.position.y > 0) { direction = Direction.Up; }
        else if (rt.position.y < 0) { direction = Direction.Down; }
        else { direction = Direction.Middle; }

        panelMaxSize = rt.sizeDelta;

        parent = transform.parent.gameObject;
        
        originalPos = rt.anchoredPosition;

        _animationSizePercent = 1;
        _activeScaleFactor = 1;
    }
}
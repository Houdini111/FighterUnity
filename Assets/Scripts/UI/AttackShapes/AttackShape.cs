using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShape : MonoBehaviour {
    
    private GameObject fillObj;
    private GameObject outlineObj;
    [SerializeField]
    private UIShape fill;
    [SerializeField]
    private UIShape outline;

    public Color fillColor
    {
        get { return fill.color; }
        set { fill.color = value; }
    }
    
    public Vector2 size
    {
        get
        {
            return fill.size;
        }
        set
        {
            fill.size = value;
            outline.size = value;
        }
    }
    private float MAX_SIZE;
    public float maxSize
    {
        get { return MAX_SIZE; }
        set { return; }
    }

    public float rotation
    {
        get
        {
            return fill.rotation;
        }
        set
        {
            fill.rotation = value;
            outline.rotation = value;
        }
    }

    private RectTransform rt;
    public Vector2 center
    {
        get
        {
            return rt.anchoredPosition;
        }
        set
        {
            rt.anchoredPosition = value;
        }
    }

    [SerializeField]
    private Vector2 START_POS;
    public Vector2 startPos
    {
        get
        {
            return START_POS;
        }
        set { }
    }

    public bool visible
    {
        get
        {
            //return gameObject.activeSelf;
            return fill.visible || outline.visible;
        }
        set
        {
            //gameObject.SetActive(value);
            fill.visible = value;
            outline.visible = value;
        }
    }

	// Use this for initialization
	void Start ()
    {
        initalize();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnEnable()
    {
        initalize();
    }

    private void initalize()
    {
        //fillObj = gameObject.transform.Find("Fill").gameObject;
        //outlineObj = gameObject.transform.Find("Outline").gameObject;

        //fill = fillObj.GetComponent<UICircle>();
        //outline = outlineObj.GetComponent<UICircle>();

        rt = gameObject.GetComponent<RectTransform>();

        MAX_SIZE = rt.rect.width;
        fill.size = new Vector2(MAX_SIZE, MAX_SIZE);
        outline.size = new Vector2(MAX_SIZE, MAX_SIZE); ;
        //START_POS = rt.anchoredPosition;
        START_POS = transform.parent.GetComponent<RectTransform>().anchoredPosition;
    }
}

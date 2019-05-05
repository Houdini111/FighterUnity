using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UICircle : UIShape
{
    [SerializeField]
    Texture m_Texture;

    [Range(0, 1)]
    [SerializeField]
    private float fillAmount;
    
    //private RectTransform rectTransform;

    public float FillAmount
    {
        get { return fillAmount; }
        set
        {
            fillAmount = value;

            // This detects a change and sets the vertices dirty so it gets updated
            SetVerticesDirty();
        }
    }

    [SerializeField]
    private bool _fill = true;
    public override bool fill { get { return _fill; } set { _fill = value; } }
    [SerializeField]
    private int _thickness = 5;
    public override int thickness { get { return _thickness; } set { _thickness = value; } }

    [Range(0, 360)]
    public int segments = 360;

    public override bool visible
    {
        get { return enabled; }
        set { enabled = value; }
    }

    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }

    public override Texture texture
    {
        get { return m_Texture; }

        set
        {
            if (m_Texture == value)
                return;
            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    // Using arrays is a bit more efficient
    UIVertex[] uiVertices = new UIVertex[4];
    Vector2[] uvs = new Vector2[4];
    Vector2[] pos = new Vector2[4];

    protected override void Start()
    {
        uvs[0] = new Vector2(0, 1);
        uvs[1] = new Vector2(1, 1);
        uvs[2] = new Vector2(1, 0);
        uvs[3] = new Vector2(0, 0);

        //rectTransform = gameObject.GetComponent<RectTransform>();
    }


    // Updated OnPopulateMesh to user VertexHelper instead of mesh
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // There's really no need to clamp the thickness
        //thickness = (int)Mathf.Clamp(thickness, 0, (rectTransform.rect.width / 2));
        float outer = -rectTransform.pivot.x * rectTransform.rect.width;
        float inner = -rectTransform.pivot.x * rectTransform.rect.width + thickness;

        float degrees = 360f / segments;
        int fa = (int)((segments + 1) * this.fillAmount);

        // Updated to new vertexhelper
        vh.Clear();
        

        // Changed initial values so the first polygon is correct when circle isn't filled
        float x = outer * Mathf.Cos(0);
        float y = outer * Mathf.Sin(0);
        Vector2 prevX = new Vector2(x, y);

        // Changed initial values so the first polygon is correct when circle isn't filled
        x = inner * Mathf.Cos(0);
        y = inner * Mathf.Sin(0);
        Vector2 prevY = new Vector2(x, y);

        for (int i = 0; i < fa - 1; i++)
        {
            // Changed so there isn't a stray polygon at the beginning of the arc
            float rad = Mathf.Deg2Rad * ((i + 1) * degrees);
            float c = Mathf.Cos(rad);
            float s = Mathf.Sin(rad);

            pos[0] = prevX;
            pos[1] = new Vector2(outer * c, outer * s);

            if (fill)
            {
                pos[2] = Vector2.zero;
                pos[3] = Vector2.zero;
            }
            else
            {
                pos[2] = new Vector2(inner * c, inner * s);
                pos[3] = prevY;
            }

            // Set values for uiVertices
            for (int j = 0; j < 4; j++)
            {
                uiVertices[j].color = color;
                uiVertices[j].position = pos[j];
                uiVertices[j].uv0 = uvs[j];
            }

            // Get the current vertex count
            int vCount = vh.currentVertCount;

            // If filled, we only need to create one triangle
            vh.AddVert(uiVertices[0]);
            vh.AddVert(uiVertices[1]);
            vh.AddVert(uiVertices[2]);

            // Create triangle from added vertices
            vh.AddTriangle(vCount, vCount + 2, vCount + 1);

            // If not filled we need to add the 4th vertex and another triangle
            if (!fill)
            {
                vh.AddVert(uiVertices[3]);
                vh.AddTriangle(vCount, vCount + 3, vCount + 2);
            }

            prevX = pos[1];
            prevY = pos[2];
        }
    }
    
    public override Vector2 size
    {
        get
        {
            return new Vector2(rectTransform.rect.height, rectTransform.rect.width);
        }
        set
        {
            rectTransform.sizeDelta = value;
        }
    }
    public override float rotation
    {
        get
        {
            return rectTransform.rotation.z;
        }
        set
        {
            rectTransform.Rotate(new Vector3(rectTransform.rotation.x, rectTransform.rotation.y, value));
        }
    }

    public override Vector2 center
    {
        get
        {
            return gameObject.GetComponent<RectTransform>().anchoredPosition;
        }
        set
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = value;
        }
    }
}

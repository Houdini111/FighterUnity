using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public abstract class UIShape : MaskableGraphic // Changed to maskableGraphic so it can be masked with RectMask2D
{
    public abstract bool fill { get; set; }
    public abstract int thickness { get; set; }
    public abstract override Texture mainTexture { get; }
    public abstract Texture texture { get; set; }
    protected abstract override void Start();
    protected abstract override void OnPopulateMesh(VertexHelper vh);
    public abstract Vector2 size { get; set; }
    public abstract float rotation { get; set; }
    public abstract Vector2 center { get; set; }
    public abstract bool visible { get; set; }
}

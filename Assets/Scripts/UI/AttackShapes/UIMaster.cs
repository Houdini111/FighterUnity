using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIMaster : MonoBehaviour
{
    public Direction direction;
    public abstract Color fillColor { get; set; }
    public abstract float animationSizePercent { get; set; }
    public abstract float activeScaleFactor { get; set; }
    public abstract float rotation { get; set; }
    public abstract Vector2 animationCenter { get; set; }
    public abstract float transitionPositionScale { get; set; }
    public abstract Vector2 startPos { get; set; }
    public abstract bool visible { get; set; }
    public abstract Shape shape { get; set; }
}

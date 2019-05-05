using UnityEngine;

public class Rotation
{
    private readonly Transform transform;

    public int _rot;
    public int rot
    {
        get { return _rot; }
        set
        {
            int r = value;
            do
            {
                r -= 360;
            } while (r >= 360);
            do
            {
                r += 360;
            } while (r < 0);
            _rot = r;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, r);
        }
    }

    public Rotation(Transform t, int r)
    {
        transform = t;
        rot = r;
    }

    public Rotation(Transform t)
    {
        transform = t;
    }

    public void rotateCW() { rot -= 90; }
    public void rotateCCW() { rot += 90; }
    public void flip() { rot += 180; }
    public void setUp() { rot = 0; }
    public void setRight() { rot = 270; }
    public void setDown() { rot = 180; }
    public void setLeft() { rot = 90; }

    public bool up { get { return rot == 0; } }
    public bool right { get { return rot == 270; } }
    public bool down { get { return rot == 180; } }
    public bool left { get { return rot == 90; } }
}

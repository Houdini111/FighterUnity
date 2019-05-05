using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class MoveMap<T>
{
    private int WIDTH;
    private int HEIGHT;
    
    public T[,] map;
    
    public MoveMap(int w, int h)
    {
        WIDTH = w;
        HEIGHT = h;
        map = new T[w, h];
    }
    
    public T this[int x, int y]
    {
        get { return map[x, y]; }
        set { map[x, y] = value; }
    }

    public T this[Vector2Int pos]
    {
        get { return this[pos.x, pos.y]; }
        set { this[pos.x, pos.y] = value; }
    }
    
    public int getWidth() { return WIDTH; }

    public int getHeight() { return HEIGHT; }
}

public class Master : MonoBehaviour
{
    [SerializeField]
    private int MAP_WIDTH;
    [SerializeField]
    private int MAP_HEIGHT;

    public MoveMap<GameObject> staticMap;
    public MoveMap<GameObject> entityMap;

	// Use this for initialization
	void Start ()
	{
        staticMap = new MoveMap<GameObject>(MAP_WIDTH, MAP_HEIGHT);
        entityMap = new MoveMap<GameObject>(MAP_WIDTH, MAP_HEIGHT);
    }
	
	// Update is called once per frame
	void Update () {

	}
}

public enum Shape { Circle, Diamond, Rect }
public enum Direction { up, right, down, left, middle };
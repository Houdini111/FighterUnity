using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class MoveMap
{
    private int WIDTH;
    private int HEIGHT;
    
    public bool[,] map;
    
    public MoveMap(int w, int h)
    {
        map = new bool[w, h];
    }
    
    public bool this[int x, int y]
    {
        get { return map[x, y]; }
        set { map[x, y] = value; }
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

    public MoveMap moveMap;

	// Use this for initialization
	void Start ()
	{
        moveMap = new MoveMap(MAP_WIDTH, MAP_HEIGHT);
    }
	
	// Update is called once per frame
	void Update () {

	}
}

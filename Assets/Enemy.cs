using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float MOVE_SPEED; //Units moved per second
    public float SIGHT_RANGE;

    private Vector2Int position;
    private Player player;
    private float timeTilNextMove;
    private float TIME_BETWEEN_MOVES; //Static variable that holds MOVE_SPEED as something more usable;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();
        position = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));
        timeTilNextMove = 0;
        TIME_BETWEEN_MOVES = 1/MOVE_SPEED;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("DIST: " + Vector2.Distance(position, player.position));
        if (Vector2.Distance(position, player.position) <= SIGHT_RANGE)
        {
            //Debug.Log("SEEN");
            timeTilNextMove -= Time.deltaTime;
            if (timeTilNextMove <= 0)
            {
                timeTilNextMove = TIME_BETWEEN_MOVES;
                Debug.Log("MOVE");
                moveToPosition(player.position);
            }
        }
    }

    void moveToPosition(Vector2Int pos)
    {

    }
}

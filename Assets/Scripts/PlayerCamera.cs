using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    
    public Player player;
    public float cameraHeight = 20.0f;
    public float MAX_SPEED;

	// Use this for initialization
	void Start () {
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, p.y, p.z + cameraHeight);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos1 = player.transform.position;
        Vector3 pos2 = transform.position;
        int yDir = 0;
        int xDir = 0;
        if(pos1.y > pos2.y) { yDir = 1; }
        else if(pos1.y < pos2.y) { yDir = -1; }
        if (pos1.x > pos2.x) { xDir = 1; }
        else if (pos1.x < pos2.x) { xDir = -1; }
        float x = Mathf.Min(Mathf.Abs(pos1.x - pos2.x)/2, MAX_SPEED * Time.deltaTime);
        float y = Mathf.Min(Mathf.Abs(pos1.y - pos2.y)/2, MAX_SPEED * Time.deltaTime);
        transform.position = new Vector3(pos2.x + x*xDir, pos2.y + y*yDir, pos2.z);
	}
}

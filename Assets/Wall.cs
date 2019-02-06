using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("Master").GetComponent<Master>().moveMap[(int)transform.position.x, (int)transform.position.y] = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

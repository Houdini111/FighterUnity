using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    static Master master;

	// Use this for initialization
	void Start () {
        if(master == null) { master = GameObject.Find("Master").GetComponent<Master>(); }
        master.staticMap[(int)transform.position.x, (int)transform.position.y] = this.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

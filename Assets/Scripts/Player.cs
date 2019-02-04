using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Master master;
    private Controller controller;

    public float MAX_SPEED;
    private int rot;
    private int xDir;
    private int yDir;
    private float xLeft;
    private float yLeft;
    public bool canMove;

    private static float RAYCAST_LEN = 0.75f;

	// Use this for initialization
	void Start ()
	{
	    rot = 90;
	    xDir = 0;
	    yDir = 0;
        canMove = true;

	    GameObject masterObj = GameObject.Find("Master");
	    master = masterObj.GetComponent<Master>();

        GameObject controllerObj = GameObject.Find("Controller");
	    controller = controllerObj.GetComponent<Controller>();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (master.isPlayerTurn)
        {
            if(canMove)
            {
                int oldRot = rot;
                if (controller.lookU.down) { rot = 90; }
                else if (controller.lookR.down) { rot = 0; }
                else if (controller.lookD.down) { rot = 270; }
                else if (controller.lookL.down) { rot = 180; }
                int rotDiff = rot - oldRot;
                if (rotDiff != 0) { transform.Rotate(Vector3.forward * (rotDiff)); }

                xDir = 0;
                yDir = 0;
                if (controller.moveU.down) { yDir = 1; }
                else if (controller.moveR.down) { xDir = 1; }
                else if (controller.moveD.down) { yDir = -1; }
                else if (controller.moveL.down) { xDir = -1; }

                if(xDir != 0 || yDir != 0)
                {
                    RaycastHit2D result = Physics2D.Raycast(transform.position, new Vector3(xDir, yDir, 0), RAYCAST_LEN);
                    Debug.DrawRay(transform.position, new Vector3(xDir * RAYCAST_LEN, yDir * RAYCAST_LEN, 0), Color.cyan, 0.25f, false);
                    if (result.collider == null)
                    {
                        //transform.Translate(new Vector3(xDir, yDir)); //Movement based on orientation
                        //transform.position = new Vector3(transform.position.x + xDir, transform.position.y + yDir);
                        xLeft = xDir;
                        yLeft = yDir;
                        moveTowards();
                        canMove = false;
                    }
                }
            }
            else if(xLeft != 0 || yLeft != 0)
            {
                moveTowards();
            }
            else
            {
                canMove = true;
            }
        }
    }

    void moveTowards()
    {
        float xMove = Mathf.Min(MAX_SPEED * Time.deltaTime, Mathf.Abs(xLeft)) * Mathf.Sign(xLeft);
        float yMove = Mathf.Min(MAX_SPEED * Time.deltaTime, Mathf.Abs(yLeft)) * Mathf.Sign(yLeft);
        xLeft -= xMove;
        yLeft -= yMove;
        /*
        if(xLeft > 0) { xLeft -= xMove; }
        else { xLeft += xMove; }
        if (yLeft > 0) { yLeft -= yMove; }
        else { yLeft += yMove; }
        */
        transform.position = new Vector3(transform.position.x + xMove, transform.position.y + yMove);
    }
}

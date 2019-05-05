using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IAttackable, IAttacker
{
    private Master master;
    public Controller controller;
    private AttackLogic attackLogic;

    public float MAX_SPEED;
    //Since Unity's built in numbers are floats, I won't get the precision I want
    //So I manually track it as well
    public Vector2Int position;

    [SerializeField]
    private decimal health;

    private decimal _damage = 10;
    public decimal damage { get { return _damage; } }
    [SerializeField]
    private float ATTACK_COOLDOWN;
    
    private int xDir;
    private int yDir;
    private float xLeft;
    private float yLeft;

    public bool canMove; //Public for the debug text
    public Rotation rot;
    
    public Vector2Int ahead
    {
        get
        {
            Vector2Int r = position;
            if (rot.up) { r.y++; }
            else if (rot.left) { r.x--; }
            else if (rot.down) { r.y--; }
            else if (rot.right) { r.x++; }
            return r;
        }
    }
    
    // Use this for initialization
    void Start ()
	{
        rot = new Rotation(transform);
        rot.setRight();

        position = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));
        xDir = 0;
	    yDir = 0;
        canMove = true;

	    GameObject masterObj = GameObject.Find("Master");
	    master = masterObj.GetComponent<Master>();

        master.entityMap[position.x, position.y] = this.gameObject;

        GameObject controllerObj = GameObject.Find("Controller");
	    controller = controllerObj.GetComponent<Controller>();

        attackLogic = GameObject.Find("AttackLogic").GetComponent<AttackLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (controller.lookU.down) { rot.setUp(); }
            else if (controller.lookR.down) { rot.setRight(); }
            else if (controller.lookD.down) { rot.setDown(); }
            else if (controller.lookL.down) { rot.setLeft(); }

            xDir = 0;
            yDir = 0;
            if (controller.moveU.down) { yDir = 1; }
            else if (controller.moveR.down) { xDir = 1; }
            else if (controller.moveD.down) { yDir = -1; }
            else if (controller.moveL.down) { xDir = -1; }

            //Only move if the player isn't already
            if(xDir != 0 || yDir != 0)
            {
                int newX = position.x + xDir;
                int newY = position.y + yDir;
                //RaycastHit2D result = Physics2D.Raycast(transform.position, new Vector3(xDir, yDir, 0), RAYCAST_LEN);
                //Debug.DrawRay(transform.position, new Vector3(xDir * RAYCAST_LEN, yDir * RAYCAST_LEN, 0), Color.cyan, 0.25f, false);
                //if (result.collider == null)
                if(master.staticMap[newX, newY] == null && master.entityMap[newX, newY] == null)
                {
                    master.entityMap[position.x, position.y] = null;
                    master.entityMap[newX, newY] = this.gameObject;
                    xLeft = xDir;
                    yLeft = yDir;
                    position.x += (int)xLeft; //xLeft and yLeft SHOULD ALWAYS BE WHOLE AT THIS POINT
                    position.y += (int)yLeft;
                    moveTowards();
                    canMove = false;
                }
            }

            
            if(attackLogic.ready)
            {
                if (controller.attackU.down) { attackLogic.up(); }
                if (controller.attackR.down) { attackLogic.right(); }
                if (controller.attackD.down) { attackLogic.down(); }
                if (controller.attackL.down) { attackLogic.left(); }
            }
            if(controller.attackMod.down) { attackLogic.attackMode = false; }
            else if(controller.attackMod.up) { attackLogic.attackMode = true; }
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

    void moveTowards()
    {
        float xMove = Mathf.Min(MAX_SPEED * Time.deltaTime, Mathf.Abs(xLeft)) * Mathf.Sign(xLeft);
        float yMove = Mathf.Min(MAX_SPEED * Time.deltaTime, Mathf.Abs(yLeft)) * Mathf.Sign(yLeft);
        xLeft -= xMove;
        yLeft -= yMove;
        if(xLeft == 0 && yLeft == 0)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + xMove, transform.position.y + yMove);
        }
    }

    public bool getAttacked(decimal damage)
    {
        health -= damage;
        return true;
    }

    public void die()
    {
        master.entityMap[position] = null;
        gameObject.SetActive(false);
    }
}

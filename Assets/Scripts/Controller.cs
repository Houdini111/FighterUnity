using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class Controller : MonoBehaviour
{
    public struct Map
    {
        public bool down;
        public bool hold;
        public bool up;

        public void setFalse()
        {
            down = false;
            hold = false;
            up = false;
        }

        public void press()
        {
            if (hold)
            {
                down = false;
                up = false;
            }
            else
            {
                down = true;
                up = false;
            }
            hold = true;
        }

        public void release()
        {
            if (hold)
            {
                up = true;
            }
            else
            {
                up = false;
            }
            down = false;
            hold = false;
        }

        public override String ToString()
        {
            String ret = "";

            if (down) { ret += "1 "; }
            else { ret += "0 "; }

            if (hold) { ret += "1 "; }
            else { ret += "0 "; }

            if (up) { ret += "1"; }
            else { ret += "0"; }

            return ret;
        }
    }

    public List<Map> inputMaps;

    public Map moveU;
    public Map moveR;
    public Map moveD;
    public Map moveL;

    public Map lookU;
    public Map lookR;
    public Map lookD;
    public Map lookL;

    public Map moveMod;
    public Map lookMod;

    public Map defendU;
    public Map defendR;
    public Map defendD;
    public Map defendL;

    public Map attackU;
    public Map attackR;
    public Map attackD;
    public Map attackL;

    void setAllFalse()
    {
        for(int i = 0; i < inputMaps.Count; i++)
        {
            inputMaps[i].setFalse();
        }
    }

    void setFalse(ref Map map)
    {
        map.down = false;
        map.hold = false;
        map.up = false;
    }

	// Use this for initialization
	void Start ()
	{
        inputMaps = new List<Map>();

	    moveU = new Map();
	    moveR = new Map();
	    moveD = new Map();
	    moveL = new Map();

	    lookU = new Map();
	    lookR = new Map();
	    lookD = new Map();
	    lookL = new Map();

	    moveMod = new Map();
	    lookMod = new Map();

	    defendU = new Map();
	    defendR = new Map();
	    defendD = new Map();
	    defendL = new Map();

	    attackU = new Map();
	    attackR = new Map();
	    attackD = new Map();
	    attackL = new Map();


        inputMaps.Add(moveU);
	    inputMaps.Add(moveR);
	    inputMaps.Add(moveD);
	    inputMaps.Add(moveL);

	    inputMaps.Add(lookU);
	    inputMaps.Add(lookR);
	    inputMaps.Add(lookD);
	    inputMaps.Add(lookL);

	    inputMaps.Add(moveMod);
	    inputMaps.Add(lookMod);

	    inputMaps.Add(defendU);
	    inputMaps.Add(defendR);
	    inputMaps.Add(defendD);
	    inputMaps.Add(defendL);

	    inputMaps.Add(attackU);
	    inputMaps.Add(attackR);
	    inputMaps.Add(attackD);
	    inputMaps.Add(attackL);

	    setAllFalse();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(Input.GetAxisRaw("moveModifier") > 0) { moveMod.press(); }
        else { moveMod.release(); }

	    if (Input.GetAxisRaw("lookModifier") > 0) { lookMod.press(); }
	    else { lookMod.release(); }

        if (moveMod.hold)
        {
            moveU.release();
            moveR.release();
            moveD.release();
            moveL.release();

	        if (Input.GetAxisRaw("moveVertically") > 0)
	        {
                defendU.press();
                defendD.release();
	        }
            else if (Input.GetAxisRaw("moveVertically") < 0)
	        {
	            defendU.release();
	            defendD.press();
	        }
            else
            {
                defendU.release();
                defendD.release();
            }

	        if (Input.GetAxisRaw("moveHorizontally") > 0)
	        {
	            defendR.press();
	            defendL.release();
	        }
	        else if (Input.GetAxisRaw("moveHorizontally") < 0)
	        {
	            defendR.release();
	            defendL.press();
	        }
	        else
	        {
	            defendR.release();
	            defendL.release();
	        }
        }
	    else
        {
            defendU.release();
            defendR.release();
	        defendD.release();
	        defendL.release();

	        if (Input.GetAxisRaw("moveVertically") > 0)
	        {
	            moveU.press();
	            moveD.release();
	        }
	        else if (Input.GetAxisRaw("moveVertically") < 0)
	        {
	            moveU.release();
	            moveD.press();
	        }
	        else
	        {
	            moveU.release();
	            moveD.release();
	        }

	        if (Input.GetAxisRaw("moveHorizontally") > 0)
	        {
	            moveR.press();
	            moveL.release();
	        }
	        else if (Input.GetAxisRaw("moveHorizontally") < 0)
	        {
	            moveR.release();
	            moveL.press();
	        }
	        else
	        {
	            moveR.release();
	            moveL.release();
	        }
        }


	    if (lookMod.hold)
	    {
	        lookU.release();
	        lookR.release();
	        lookD.release();
	        lookL.release();

	        if (Input.GetAxisRaw("lookVertically") > 0)
	        {
	            attackU.press();
	            attackD.release();
	        }
	        else if (Input.GetAxisRaw("lookVertically") < 0)
	        {
	            attackU.release();
	            attackD.press();
	        }
	        else
	        {
	            attackU.release();
	            attackD.release();
	        }

	        if (Input.GetAxisRaw("lookHorizontally") > 0)
	        {
	            attackR.press();
	            attackL.release();
	        }
	        else if (Input.GetAxisRaw("lookHorizontally") < 0)
	        {
	            attackR.release();
                attackL.press();
	        }
	        else
	        {
	            attackR.release();
	            attackL.release();
	        }
	    }
	    else
	    {
	        attackU.release();
	        attackR.release();
	        attackD.release();
	        attackL.release();

	        if (Input.GetAxisRaw("lookVertically") > 0)
	        {
	            lookU.press();
	            lookD.release();
	        }
	        else if (Input.GetAxisRaw("lookVertically") < 0)
	        {
	            lookU.release();
	            lookD.press();
	        }
	        else
	        {
	            lookU.release();
	            lookD.release();
	        }

	        if (Input.GetAxisRaw("lookHorizontally") > 0)
	        {
	            lookR.press();
	            lookL.release();
	        }
	        else if (Input.GetAxisRaw("lookHorizontally") < 0)
	        {
	            lookR.release();
	            lookL.press();
	        }
	        else
	        {
	            lookR.release();
	            lookL.release();
	        }
	    }
    }
}

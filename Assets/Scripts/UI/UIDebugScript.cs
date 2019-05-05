using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugScript : MonoBehaviour
{
    private Player player;
    private Controller controller;

    private Text text;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();
	    controller = GameObject.Find("Controller").GetComponent<Controller>();

	    text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    text.text = "";

        text.text += "canMove: " + (player.canMove ? "1" : "0") + "\n";

	    if (controller.moveMod.hold)
        {
            text.text += "Look U: " + controller.lookU.ToString() + "\n";
            text.text += "Look R: " + controller.lookR.ToString() + "\n";
            text.text += "Look D: " + controller.lookD.ToString() + "\n";
            text.text += "Look L: " + controller.lookL.ToString() + "\n";
	    }
        else
        {
            text.text += "Move U: " + controller.moveU.ToString() + "\n";
	        text.text += "Move R: " + controller.moveR.ToString() + "\n";
	        text.text += "Move D: " + controller.moveD.ToString() + "\n";
	        text.text += "Move L: " + controller.moveL.ToString() + "\n";
        }
	    
	    text.text += "\n";

	    if (controller.attackMod.hold)
        {
            text.text += "Defend U: " + controller.defendU.ToString() + "\n";
            text.text += "Defend R: " + controller.defendR.ToString() + "\n";
            text.text += "Defend D: " + controller.defendD.ToString() + "\n";
            text.text += "Defend L: " + controller.defendL.ToString() + "\n";
        }
	    else
	    {
            text.text += "Attack U: " + controller.attackU.ToString() + "\n";
            text.text += "Attack R: " + controller.attackR.ToString() + "\n";
            text.text += "Attack D: " + controller.attackD.ToString() + "\n";
            text.text += "Attack L: " + controller.attackL.ToString() + "\n";
        }
        //text.text += "Attack Ready: " + (player.attackUI.ready ? "1" : "0");
    }
}

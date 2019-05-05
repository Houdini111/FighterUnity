using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScript : MonoBehaviour {

    //[SerializeField]
    //GameObject player;

	// Use this for initialization
	void Start () {
        Vector2[] vertices = new Vector2[] { new Vector2(1, 1), new Vector2(.05f, 1.3f), new Vector2(1, 2), new Vector2(1.95f, 1.3f), new Vector2(1.58f, 0.2f), new Vector2(.4f, .2f) };
        ushort[] triangles = new ushort[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 1 };
        DrawPolygon2D(vertices, triangles, Color.red);
    }

    void DrawPolygon2D(Vector2[] vertices, ushort[] triangles, Color color)
    {
        GameObject polygon = new GameObject(); //create a new game object
        SpriteRenderer sr = polygon.AddComponent<SpriteRenderer>(); // add a sprite renderer
        Texture2D texture = new Texture2D(1025, 1025); // create a texture larger than your maximum polygon size

        // create an array and fill the texture with your color
        List<Color> cols = new List<Color>();
        for (int i = 0; i < (texture.width * texture.height); i++)
            cols.Add(color);
        texture.SetPixels(cols.ToArray());
        texture.Apply();

        sr.color = color; //you can also add that color to the sprite renderer

        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 1024, 1024), Vector2.zero, 1); //create a sprite with the texture we just created and colored in

        sr.sprite.OverrideGeometry(vertices, triangles); // set the vertices and triangles

        /*
        Vector2 pointA;
        Vector2 pointB;
        int lineWidth = 5;
        pointA = new Vector2(0, 0);
        pointB = new Vector2(1, 1);
        Vector2 differenceVector = pointB - pointA;

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        imageRectTransfom.pivot = new Vector2(0, 0.5f);
        imageRectTransform.position = pointA;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
        */
        /*
        GameObject polygon = new GameObject(); //create a new game object
        SpriteRenderer sr = polygon.AddComponent<SpriteRenderer>(); // add a sprite renderer
        Texture2D texture = new Texture2D(1025, 1025); // create a texture larger than your maximum polygon size

        // create an array and fill the texture with your color
        List<Color> cols = new List<Color>();
        for (int i = 0; i < (texture.width * texture.height); i++)
            cols.Add(color);
        texture.SetPixels(cols.ToArray());
        texture.Apply();

        sr.color = color; //you can also add that color to the sprite renderer

        sr.sprite = Sprite.Create(texture, new Rect(0, 0, 1024, 1024), Vector2.zero, 1); //create a sprite with the texture we just created and colored in

        
        //convert coordinates to local space
        float lx = Mathf.Infinity, ly = Mathf.Infinity;
        foreach (Vector2 vi in vertices)
        {
            if (vi.x < lx)
                lx = vi.x;
            if (vi.y < ly)
                ly = vi.y;
        }
        Vector2[] localv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            localv[i] = vertices[i] - new Vector2(lx, ly);
        }
        
        sr.sprite.OverrideGeometry(localv, triangles); // set the vertices and triangles
        
        // new Vector2(player.transform.position.x, player.transform.position.y);
        //polygon.transform.position = (Vector2)transform.InverseTransformPoint(transform.position);// + new Vector2(lx, ly); // return to world space
        */
    }

    // Update is called once per frame
    void Update () {
		
	}
}
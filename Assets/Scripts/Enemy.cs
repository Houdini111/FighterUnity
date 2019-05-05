using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, IAttackable
{

    public float MOVE_SPEED; //Units moved per second
    public float SIGHT_RANGE;

    [SerializeField]
    private decimal health;

    private Vector2Int position;
    private Rotation rot;

    private float timeTilNextMove;
    private float TIME_BETWEEN_MOVES; //Static variable that holds MOVE_SPEED as something more usable;

    private Player player;
    private Master master;
    
    void Start () {
        rot = new Rotation(transform);

        player = GameObject.Find("Player").GetComponent<Player>();
        master = GameObject.Find("Master").GetComponent<Master>();

        position = new Vector2Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y));
        timeTilNextMove = 0;
        TIME_BETWEEN_MOVES = 1/MOVE_SPEED;
        
        master.entityMap[position.x, position.y] = this.gameObject;
    }
	
	void Update () {
        float dist = Vector2.Distance(position, player.position);
        if (dist <= 1) //Don't bother checking if they're already right by
        {
            timeTilNextMove = TIME_BETWEEN_MOVES;
        }
        else if (dist <= SIGHT_RANGE)
        {
            timeTilNextMove -= Time.deltaTime;
            if (timeTilNextMove <= 0)
            {
                timeTilNextMove = TIME_BETWEEN_MOVES;
                List<Vector2Int> path = moveTowardsPosition(player.position);
                if(path != null && path.Count > 0)
                {
                    Vector2Int next = path[0];
                    if (next != null)
                    {
                        if(next.x < position.x) { rot.setLeft(); }
                        else if(next.x > position.x) { rot.setRight(); }
                        else if(next.y > position.y) { rot.setUp(); }
                        else if(next.y < position.y) { rot.setDown(); }
                        master.entityMap[position.x, position.y] = null;
                        master.entityMap[next.x, next.y] = this.gameObject;
                        position = next;
                        transform.position = new Vector3(next.x, next.y, transform.position.z);
                    }
                }
            }
        }
        else //If the player moves out of range, make them "blind" (as if they couldn't see them at all)
        {
            timeTilNextMove = TIME_BETWEEN_MOVES;
        }
    }

    #region Pathfinding
    protected class Location
    {
        public Vector2Int point { get; set; }
        public int x { get { return point.x; } set { point = new Vector2Int(value, point.y); } }
        public int y { get { return point.y; } set { point = new Vector2Int(point.x, value); } }
        public int g { get; set; } //Distance traveled to node, with 10 for a straight line
        public int h { get; set; } //Distance in a straight line to goal  
        public int f { get { return g + h; } } //G+H
        public State state { get; set; }
        public Location prev; //If my C# is right, this should be a reference

        public enum State { Untested, Open, Closed };

        public Location(int x = 0, int y = 0, int g = 0, int h = 0, State state = State.Open, Location location = null)
        {
            this.point = new Vector2Int(x, y);
            this.g = g;
            this.h = h;
            this.state = state;
            prev = location;
        }

        public Location(Vector2Int pos, int g = 0, State state = State.Open)
        {
            this.point = pos;
            this.g = g;
            this.state = state;
        }

        public static bool operator ==(Location l, object obj)
        {
            if(ReferenceEquals(l, obj)) { return true; }
            if(ReferenceEquals(l, null)) { return false; }
            if(ReferenceEquals(obj, null)) { return false; }

            if (obj is Location)
            {
                Location o = (Location)obj;
                if (l.point.x == o.point.x && l.point.y == o.point.y) { return true; }
            }
            if(obj is Vector2Int)
            {
                Vector2Int o = (Vector2Int)obj;
                if(l.point.x == o.x && l.point.y == o.y) { return true; }
            }
            if(obj is int[])
            {
                try
                {
                    int[] o = (int[])obj;
                    if(l.x == o[0] && l.y == o[1]) { return true; }
                }
                catch(IndexOutOfRangeException)
                {
                    //Just here to makes sure it doesn't crash if it wasn't a large enough array
                }
            }
            return false;
        }

        public static bool operator !=(Location l, object obj)
        {
            return !(l == obj);
        }

        public override bool Equals(object obj)
        {
            return this == obj;
            //return base.Equals(obj);
        }
    }

    private List<Vector2Int> moveTowardsPosition(Vector2Int pos)
    {
        MoveMap<GameObject> mm = master.staticMap;
        int w = mm.getWidth();
        int h = mm.getHeight();
        Location[,] walkMap = new Location[w, h];
        transposeMoveMapOntoWalkMap(ref mm, ref walkMap);
        transposeMoveMapOntoWalkMap(ref master.entityMap, ref walkMap);

        List<Location> nextToCheck = new List<Location>();
        Location start = new Location(position);
        start.state = Location.State.Closed;
        nextToCheck.Add(start);
        nextToCheck.AddRange(getAdjacentWalkable(ref walkMap, ref start, pos));
        nextToCheck.Sort((loc1, loc2) => loc1.f.CompareTo(loc2.f));
        for (int i = 0; i < nextToCheck.Count; i++)
        {
            Location loc = nextToCheck[i];
            if(loc.point == pos)
            {
                //TODO THIS RETURN ISN'T RETURNING A WHOLE LIST
                List<Vector2Int> path = new List<Vector2Int>();
                Location curr = loc;
                while(curr.prev != null)
                {
                    path.Add(curr.point);
                    curr = curr.prev;
                }
                path.Reverse();
                return path;
            }
            else
            {
                if (search(ref walkMap, ref loc, pos))
                {
                    //TODO THIS RETURN ISN'T RETURNING A WHOLE LIST
                    List<Vector2Int> path = new List<Vector2Int>();
                    Location curr = loc;
                    while (curr.prev != null)
                    {
                        path.Add(curr.point);
                        curr = curr.prev;
                    }
                    path.Reverse();
                    return path;
                }
            }
        }
        return null;
    }

    private bool search(ref Location[,] walkMap, ref Location location, Vector2Int goal)
    {
        location.state = Location.State.Closed;
        List<Location> nextToCheck = getAdjacentWalkable(ref walkMap, ref location, goal);
        nextToCheck.Sort((loc1, loc2) => loc1.f.CompareTo(loc2.f));
        for(int i = 0; i < nextToCheck.Count; i++)
        {
            Location loc = nextToCheck[i];
            if(loc.point == goal)
            {
                return true;
            }
            else
            {
                if (search(ref walkMap, ref loc, goal))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private List<Location> getAdjacentWalkable(ref Location[,] walkMap, ref Location fromLocation, Vector2Int goal)
    {
        //TODO This function will need to also take into account the variable count of squares, when those are implemented
        List<Location> walkList = new List<Location>();
        List<Vector2Int> points = new List<Vector2Int>
            {
                new Vector2Int(fromLocation.point.x, fromLocation.point.y+1),
                new Vector2Int(fromLocation.point.x+1, fromLocation.point.y),
                new Vector2Int(fromLocation.point.x, fromLocation.point.y-1),
                new Vector2Int(fromLocation.point.x-1, fromLocation.point.y)
            };

        for(int i = 0; i < points.Count; i++)
        {
            Vector2Int point = points[i];
            int x = point.x;
            int y = point.y;

            //Ignore squares outside the map
            //if (x > w || x < 0) { continue; }
            //if (y > h || y < 0) { continue; }

            Location loc;
            try
            {
                loc = walkMap[x, y];
            }
            catch(IndexOutOfRangeException)
            {
                continue; //If it's out of range, don't bother trying
            }
            //If it doesn't exist yet, add it
            if (loc == null)
            {
                loc = new Location(x, y, fromLocation.g + 10, (int)Math.Round(Vector2Int.Distance(new Vector2Int(x, y), goal)), Location.State.Open, fromLocation);
                walkMap[x, y] = loc;
                walkList.Add(loc);
            }
            //If it's already been checked, but is still open
            else if(loc != null && loc.state == Location.State.Open)
            {
                int gCost = (int)Math.Round(Vector2Int.Distance(new Vector2Int(x, y), goal)) + 10;
                if(gCost < loc.g)
                {
                    loc.prev = fromLocation;
                    walkList.Add(loc);
                }
            }
            //If it's actually the goal, add it anyway
            else if(point == goal)
            {
                loc.prev = fromLocation;
                walkList.Add(loc);
            }
        }
        return walkList;
    }

    private void transposeMoveMapOntoWalkMap(ref MoveMap<GameObject> mm, ref Location[,] walkMap)
    {
        for(int x = 0; x < mm.getWidth(); x++)
        {
            for(int y = 0; y < mm.getHeight(); y++)
            {
                if(mm[x, y] != null)
                {
                    if(walkMap[x, y] == null)
                    {
                        walkMap[x, y] = new Location(x, y);
                    }
                    walkMap[x, y].state = Location.State.Closed;
                }
            }
        }
    }
    #endregion

    public bool getAttacked(decimal damage)
    {
        health -= damage;
        die();
        return true;
    }

    public void die()
    {
        master.entityMap[position] = null;
        gameObject.SetActive(false);
    }
}

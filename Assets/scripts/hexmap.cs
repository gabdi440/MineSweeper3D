using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hexmap : MonoBehaviour
{
    public enum tiletype { free = 0, mine = 1, obstacle = 2, revealed = 3 };


    public GameObject hex;
    Vector2 size = new Vector2(5, 5);
    public Dictionary<Vector2, tile> mappositions = new Dictionary<Vector2, tile>();
    public Dictionary<Vector2, GameObject> mapbutton = new Dictionary<Vector2, GameObject>();



    public struct tile
    {
        public tiletype type;
        public tile(tiletype tip)
        {
            type = tip;
        }
    }







    void Start()
    {
        definemap((int)size.x,(int) size.y); 
    }



    void definemap(int x, int y)
    {

        for (int Xi = 0; Xi < x; Xi++)
        {
            for (int Yi = 0; Yi < y; Yi++)
            {
                Vector2 pos = new Vector2(Xi, Yi);
                int angle = 0;
                int offsetX = 0;
                if (Yi % 2 != 0)
                { angle = 60; offsetX = 1; }

                GameObject Hexagon = Instantiate(hex) as GameObject;
                if (Yi % 2 != 0)
                { angle = 60; offsetX = 1; Hexagon.transform.GetChild(0).eulerAngles = new Vector3(Hexagon.transform.GetChild(0).eulerAngles.x, Hexagon.transform.GetChild(0).eulerAngles.y, -60); }
                Hexagon.transform.eulerAngles = new Vector3(-90, angle, 0);
                Hexagon.transform.position = new Vector3((Xi * 2) + offsetX, 0, Yi * 1.65f);
                mapbutton.Add(pos, Hexagon);
            }

        }

      
    }



}

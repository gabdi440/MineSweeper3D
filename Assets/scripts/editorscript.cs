using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static MapData;
using static MapData.TileData;

public class editorscript : MonoBehaviour
{

    [SerializeField] EditorMapData mapData;

    Tiletype RC;
    public GameObject field;
    public GameObject background;
    public GameObject button;
    public Font mainfont;
    public Camera cam;

    [Header("prefabs")]
    public GameObject flag;

    public Dictionary<int, GameObject[]> dic_obstacles = new Dictionary<int, GameObject[]>();
    int theme_id = 0;
    public GameObject[] forest_obstacles_list;
    public GameObject[] beach_obstacles_list;
    public GameObject[] space_obstacles_list;
    public GameObject[] snow_obstacles_list;


    public Texture[] floors;



    [Header("UI texts")]
    public Text t_size;
    public Text t_mines;
    public Text t_obsacles;
    public Text t_percentcovered;
    public Text t_percentmine;
    public Text t_percentobstacles;


    Vector2 size;
    int mines;
    int obstacles;
    Vector2 start = -Vector2.one;
    Vector2 finish = -Vector2.one;


    Dictionary<Vector2, tile> map = new Dictionary<Vector2, tile>();

    [SerializeField] GameObject CopyToClipboardPanel;
    [SerializeField] TMPro.TMP_InputField inputFieldName;


    class tile
    {
        public Tiletype type = Tiletype.free;
        public int itemid = -1;
        public GameObject topgo;
    }

    void update_texts()
    {

        t_size.text = "" + size.x + " X " + size.y;
        t_mines.text = "" + mines;
        t_obsacles.text = "" + obstacles;
        t_percentcovered.text = "" + Mathf.RoundToInt(((mines + obstacles) * 100) / (size.x * size.y)) + "%";
        t_percentmine.text = "" + Mathf.RoundToInt(((mines) * 100) / (size.x * size.y)) + "%";
        t_percentobstacles.text = "" + Mathf.RoundToInt(((obstacles) * 100) / (size.x * size.y)) + "%";


    }
    void set_obstacle_dictionary()
    {
        dic_obstacles.Add(0, forest_obstacles_list);
        dic_obstacles.Add(1, beach_obstacles_list);
        dic_obstacles.Add(2, space_obstacles_list);
        dic_obstacles.Add(3, snow_obstacles_list);

    }

    public void set_theme(int i)
    {
        theme_id = i;

        background.GetComponent<RawImage>().texture = floors[theme_id];
        populate_map();
    }

    void Start()
    {
        set_obstacle_dictionary();
        size = new Vector2(3, 3);
        set_theme(0);
        update_texts();
        update_cam();
        RC = Tiletype.mine;

    }




    void populate_map()
    {
        foreach(Transform tr in field.transform)
        {
            Destroy(tr.gameObject);
        }
        for(int Xi = 0; Xi < size.x; Xi++)
        {
            for(int Yi = 0; Yi < size.y; Yi++)
            {
                Vector2 pos = new Vector2(Xi, Yi);
                if(!map.ContainsKey(pos))
                {
                    map.Add(pos, new tile());
                }

                GameObject newbtn = Instantiate(button) as GameObject;
                newbtn.transform.SetParent(field.transform);
                newbtn.transform.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
                newbtn.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                Destroy(newbtn.GetComponent<buttonclichandler>());
                // Destroy(newbtn.GetComponent<Button>());
                newbtn.AddComponent<editormapbtn>().pos = pos;
                newbtn.GetComponent<editormapbtn>().editor = this;
                if(map[pos].topgo != null)
                {

                    map[pos].topgo = Instantiate(spawnproperitem(map[pos].type, pos, map[pos].itemid)) as GameObject;



                    map[pos].topgo.transform.parent = newbtn.transform;
                    // map[pos].topgo.transform.localEulerAngles = Vector3.zero;
                    map[pos].topgo.transform.localPosition = Vector3.zero;

                }
            }
        }



    }
    void adjust_arrows_pos()
    {
        field.transform.parent.GetComponent<RectTransform>().sizeDelta = field.GetComponent<RectTransform>().sizeDelta;
        update_cam();
        update_texts();
    }


    public void set_pos_in_dictionary(GameObject btn, Vector2 pos)
    {
        // same type should toggle off unless its obstacle
        if(map[pos].type == RC)
        {
            if(map[pos].type == Tiletype.mine)
            {
                if(map[pos].topgo != null)
                {
                    Destroy(map[pos].topgo);
                }
                mines--;
                map[pos].type = Tiletype.free;
                map[pos].itemid = -1;
                update_texts();
                return;
            }


            if(map[pos].type == Tiletype.obstacle)
            {

                if(map[pos].itemid < dic_obstacles[theme_id].Length - 1)
                {
                    map[pos].itemid++;
                    obstacles--;
                    spwan_item(btn, pos);
                    return;

                }
                else
                {
                    if(map[pos].topgo != null)
                    {
                        Destroy(map[pos].topgo);
                    }
                    obstacles--;
                    map[pos].itemid = -1;
                    map[pos].type = Tiletype.free;
                    update_texts();
                    return;

                }

            }

            //should be start and finish tiles
            map[pos].type = Tiletype.free;
            if(map[pos].topgo != null)
            {
                Destroy(map[pos].topgo);
            }

            return;
        }

        //when  not using the same type or a free space
        else
        {
            //one of the 2 counter has go down because item will be replaced
            if(map[pos].type != Tiletype.free)
            {
                if(map[pos].type == Tiletype.mine) { mines--; }
                if(map[pos].type == Tiletype.obstacle) { obstacles--; }

            }
            if(RC == Tiletype.obstacle) { map[pos].itemid++; }
            spwan_item(btn, pos);
        }
    }

    void spwan_item(GameObject btn, Vector2 pos)
    {
        map[pos].type = RC;
        //when replacing with another type of RC
        if(map[pos].topgo != null)
        {
            Destroy(map[pos].topgo);

        }

        map[pos].topgo = Instantiate(spawnproperitem(RC, pos, map[pos].itemid)) as GameObject;
        map[pos].topgo.transform.SetParent(btn.transform);
        // map[pos].topgo.transform.localEulerAngles = Vector3.zero;
        map[pos].topgo.transform.localPosition = Vector3.zero;
        //rem-spawn item;
        if(map[pos].type == Tiletype.mine) { mines++; }
        if(map[pos].type == Tiletype.obstacle) { obstacles++; }
        update_texts();
    }



    GameObject spawnproperitem(Tiletype type, Vector2 pos, int id)
    {
        GameObject x = null;
        switch(type)
        {
            // case tiletype.free:
            //  break;
            case Tiletype.mine:
                x = flag;
                break;
            case Tiletype.obstacle:
                if(id == -1)
                {
                    int i = 0;
                    x = dic_obstacles[theme_id][i];
                }
                else
                {
                    if(id < dic_obstacles[theme_id].Length)
                    {
                        x = dic_obstacles[theme_id][id];
                    }
                    else
                    {
                        x = dic_obstacles[theme_id][0];
                    }
                }
                break;
            //  case tiletype.revealed:
            //      break;
            case Tiletype.start:
                x = new GameObject();
                x.AddComponent<Text>().text = "START";
                x.GetComponent<Text>().color = Color.green;
                x.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                x.GetComponent<Text>().raycastTarget = false;
                x.GetComponent<Text>().fontSize = 4;
                x.GetComponent<Text>().font = mainfont;
                break;
            case Tiletype.finish:
                x = new GameObject();
                x.AddComponent<Text>().text = "Finish";
                x.GetComponent<Text>().color = Color.red;
                x.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                x.GetComponent<Text>().raycastTarget = false;
                x.GetComponent<Text>().fontSize = 4;
                x.GetComponent<Text>().font = mainfont;
                break;
        }

        return x;
    }


    public void set_RC(int i)
    {
        RC = (Tiletype)i;
    }

    void update_cam()
    {
        int x;
        if(size.x >= size.y) { x = (int)size.x; } else { x = (int)size.y; }
        cam.orthographicSize = (x * 15) + size.x;
        if(cam.orthographicSize < 125) { cam.orthographicSize = 125; }
    }




    public void add_row(bool bot)
    {
        if(Input.GetKey(KeyCode.LeftShift)) { rem_row(bot); return; }
        if(size.y >= 20) { return; }
        size = new Vector2(size.x, size.y + 1);
        print(size);
        if(!bot)
        {

            for(int x = 0; x < size.x; x++)
            {
                if(map.ContainsKey(new Vector2(x, size.y)))
                { map.Remove(new Vector2(x, size.y)); }
                map.Add(new Vector2(x, size.y), new tile());
            }
        }
        else
        {


            for(int y = ((int)size.y - 1); y > 0; y--)
            {

                for(int x = 0; x < size.x; x++)
                {
                    if(map.ContainsKey(new Vector2(x, y)))
                    { map.Remove(new Vector2(x, y)); }

                    map.Add(new Vector2(x, y), map[new Vector2(x, y - 1)]);
                }
            }
            for(int x = 0; x < size.x; x++)
            {
                if(map.ContainsKey(new Vector2(x, 0)))
                { map.Remove(new Vector2(x, 0)); }
                map.Add(new Vector2(x, 0), new tile());
            }
        }
        populate_map();
        adjust_arrows_pos();
    }

    public void add_column(bool left)
    {
        if(Input.GetKey(KeyCode.LeftShift)) { rem_col(left); return; }
        if(size.x >= 20) { return; }
        size = new Vector2(size.x + 1, size.y);
        field.GetComponent<GridLayoutGroup>().constraintCount = (int)size.x;
        print(size);
        if(!left)
        {

            for(int y = 0; y < size.y; y++)
            {
                if(map.ContainsKey(new Vector2(size.x, y)))
                { map.Remove(new Vector2(size.x, y)); }
                map.Add(new Vector2(size.x, y), new tile());
            }
        }
        else
        {


            for(int x = ((int)size.x - 1); x > 0; x--)
            {

                for(int y = 0; y < size.y; y++)
                {
                    if(map.ContainsKey(new Vector2(x, y)))
                    { map.Remove(new Vector2(x, y)); }

                    map.Add(new Vector2(x, y), map[new Vector2(x - 1, y)]);
                }
            }
            for(int y = 0; y < size.y; y++)
            {
                if(map.ContainsKey(new Vector2(0, y)))
                { map.Remove(new Vector2(0, y)); }
                map.Add(new Vector2(0, y), new tile());
            }
        }
        populate_map();
        adjust_arrows_pos();

    }




    public void rem_row(bool bot)
    {
        if(size.y <= 3) { return; }

        if(!bot)
        {

            for(int x = 0; x < size.x; x++)
            {
                if(map.ContainsKey(new Vector2(x, size.y - 1)))
                {
                    if(map[new Vector2(x, size.y - 1)].type == Tiletype.mine) { mines--; }
                    if(map[new Vector2(x, size.y - 1)].type == Tiletype.obstacle) { obstacles--; }
                    map.Remove(new Vector2(x, size.y - 1));
                }
            }
            size = new Vector2(size.x, size.y - 1);
        }
        else
        {

            for(int y = 0; y < ((int)size.y - 1); y++)
            {

                for(int x = 0; x < size.x; x++)
                {

                    if(map.ContainsKey(new Vector2(x, y)))
                    {
                        if(y == 0)
                        {
                            if(map[new Vector2(x, y)].type == Tiletype.mine) { mines--; }
                            if(map[new Vector2(x, y)].type == Tiletype.obstacle) { obstacles--; }
                        }
                        map.Remove(new Vector2(x, y));
                    }

                    map.Add(new Vector2(x, y), map[new Vector2(x, y + 1)]);
                }
            }
            for(int x = 0; x < size.x; x++)
            {
                if(map.ContainsKey(new Vector2(x, size.y - 1)))
                {
                    map.Remove(new Vector2(x, size.y - 1));
                }

            }
            size = new Vector2(size.x, size.y - 1);
            print(size);
        }
        populate_map();
        adjust_arrows_pos();
    }

    public void rem_col(bool left)
    {
        if(size.x <= 3) { return; }

        if(!left)
        {

            for(int y = 0; y < size.y; y++)
            {
                if(map.ContainsKey(new Vector2(size.x - 1, y)))
                {
                    if(map[new Vector2(size.x - 1, y)].type == Tiletype.mine)
                    {
                        mines--;
                    }
                    if(map[new Vector2(size.x - 1, y)].type == Tiletype.obstacle)
                    {
                        obstacles--;
                    }
                    map.Remove(new Vector2(size.x - 1, y));
                }
            }
            size = new Vector2(size.x - 1, size.y);
            field.GetComponent<GridLayoutGroup>().constraintCount = (int)size.x;
            print(size);
        }
        else
        {


            for(int x = 0; x < ((int)size.x - 1); x++)
            {

                for(int y = 0; y < size.y; y++)
                {
                    if(map.ContainsKey(new Vector2(x, y)))
                    {
                        if(x == 0)
                        {
                            if(map[new Vector2(x, y)].type == Tiletype.mine) { mines--; }
                            if(map[new Vector2(x, y)].type == Tiletype.obstacle) { obstacles--; }
                        }
                        map.Remove(new Vector2(x, y));
                    }

                    map.Add(new Vector2(x, y), map[new Vector2(x + 1, y)]);
                }
            }
            for(int y = 0; y < size.y; y++)
            {
                if(map.ContainsKey(new Vector2(size.x - 1, y)))
                {
                    map.Remove(new Vector2(size.x - 1, y));
                }
            }
            size = new Vector2(size.x - 1, size.y);
            field.GetComponent<GridLayoutGroup>().constraintCount = (int)size.x;
            print(size);
        }
        populate_map();
        adjust_arrows_pos();
    }



    string GetInputFieldText()
    {
        string s = inputFieldName.text;
        if(s == null || s.Length < 1)
        {
            s = "NewMap";
        }
        return s;
    }

    public GameObject seedtext;
    public void generate_map_seed(bool asFile = false)
    {
        string seed = seeding();

        if(asFile)
        {

            string path = Application.persistentDataPath + "/Maps/" + GetInputFieldText() + ".MsMap";
            FileStream fs = new FileStream(path, FileMode.Create);
           
            StreamWriter writer = new StreamWriter(fs);

            writer.WriteLine(seed);
            writer.Close();
        }
        else
        {
            ClipboardExtension.CopyToClipboard(seed);
        }

    }
    string seeding()
    {
        string s;

        s = GetInputFieldText() + "#";
        if(start.x != -1 && finish.x != -1)
        {
            s += "" + 1 + "#";
        }
        else
        {
            s += "" + 0 + "#";
        }


        s += size.x + "#" + size.y + "#" + theme_id + "#";
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                string k = "0#"; ;
                switch(map[new Vector2(x, y)].type)

                {
                    case Tiletype.free:
                        k = "0#";
                        break;
                    case Tiletype.mine:
                        k = "1#";
                        break;
                    case Tiletype.obstacle:
                        k = "" + (map[new Vector2(x, y)].itemid + 5) + "#";
                        break;
                    case Tiletype.start:
                        k = "2#";
                        break;
                    case Tiletype.finish:
                        k = "3#";
                        break;
                    case Tiletype.tresaure:
                        k = "4#";
                        break;
                }
                s += k;

            }
        }



        return s;



    }
}
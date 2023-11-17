using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class map : MonoBehaviour
{
    public enum tiletype { free = 0, mine = 1, obstacle = 2, revealed = 3, start = 4, finish = 5, tresaure = 6 };
    public enum state { none = 0, flag = 1, questionmark = 2, revealed = 3, obstacle = 4 }
    public bool android;
    public bool game_on = false;
    public bool clic_on = false;
    public bool cam_on = false;
    public enum GameType { freeplay, mission }
    public GameType gametype = GameType.freeplay;
    public int level;
    public bool inquiry_clic;
    public Camera cam;
    public Color[] hintcolors;
    public GameObject[] flags;
    public Texture[] rewardicon;
    public GameObject[] explosions;
    pathfinding PF;
    List<GameObject> bestpathmarks = new List<GameObject>();
    public GameObject Walker;
    public GameObject walkerprefab;
    bool bonus_coin;
    bool game_started = false;
    //game levels
    Dictionary<int, string> levels = new Dictionary<int, string>();

    // instance of the gamestate
    public loadsave.gamestate GameState;

    // themes
    public Dictionary<int, GameObject[]> dic_obstacles = new Dictionary<int, GameObject[]>();
    MapData.Themes theme = 0;
    public GameObject[] forest_obstacles_list;
    public GameObject[] beach_obstacles_list;
    public GameObject[] space_obstacles_list;
    public GameObject[] snow_obstacles_list;

    public Texture[] floor_textures;
    public Color32[] tilecolors;
    public Color32[] bg_color;


    public GameObject Background;
    public Font gamefont;
    public Texture crater;
    public GameObject blankbutton;
    public GameObject levelbutton;
    public GameObject map_panel;
    public GameObject Walk_button;
    public Text txtflagcount;
    public Text txtlivescount;
    public GameObject end_panel;
    public Text endtext;
    public Text txttimer;
    public GameObject free_play_panel;
    public GameObject game_canvas;
    public GameObject inquiry_btn;
    public GameObject Tresaurepanel;
    public GameObject Main_panel;
    public GameObject level_panel;
    public Text coin_text;
    public Text lives_text;
    public GameObject chest_icon;
    public GameObject shop_panel;



    public float clic_lenght = 1.2f;
    public float orthoZoomSpeed = 0.5f;
    public Vector2Int size = new Vector2Int(5, 5);
    public Vector2Int map_center = new Vector2Int(450, 450);
    int mines = 7;
    public int obstacles;
    int flagcount = 0;
    bool first_clic;
    public float clic_timer;
    public Vector2 startpos;
    public Vector2 finishpos;
    public GameObject current_button;
    public Dictionary<Vector2, tile> mappositions = new Dictionary<Vector2, tile>();
    public Dictionary<Vector2, bool> bombcheck = new Dictionary<Vector2, bool>();
    public Dictionary<Vector2, GameObject> mapbutton = new Dictionary<Vector2, GameObject>();
    public Dictionary<GameObject, Vector2> buttonmap = new Dictionary<GameObject, Vector2>();

    Coroutine lastRoutine = null;


    public class tile
    {
        public tiletype Type = tiletype.free;
        public int itemid = -1;
        public GameObject Go_on_top;
        public state State = state.none;

        public tile()
        {
        }

        public tile(int tile)
        {
            if(tile < 100) { Type = (tiletype)tile; }
            else
            {
                Type = tiletype.obstacle;
                itemid = tile - 100;
            }
            Go_on_top = null;
            State = state.none;
        }
    }

    void Awake()
    {
        set_obstacle_dictionary();
        set_levels_dictionary();
        GameState = loadsave.loadgame();
        game_bonusees_check();
        update_main_info();
        PF = new pathfinding();
        PF.Map = this;
        toggle_shop();
    }


    void Start()
    {
        load_free_map_info();
        map_panel.GetComponent<GridLayoutGroup>().constraintCount = (int)size.y;
        if(gametype == GameType.mission)
        {
            defineMissionMap(levels[level]);
        }
        else
        {
            defineRandomMap((int)size.x, (int)size.y, mines, true);
        }
        populatemapbuttons();
    }


    public void try_find_shortest_path()
    {
        if(!game_on) { return; }
        if(gametype != GameType.mission) { return; }
        foreach(GameObject go in bestpathmarks)
        {
            Destroy(go);
        }
        bestpathmarks.Clear();
        PF.populatenodes();
        PF.try_find_shortest_path(out Vector2[] path);
        foreach(Vector2 pos in path)
        {
            GameObject id = Instantiate(new GameObject()) as GameObject;
            id.AddComponent<RawImage>().color = Color.red;
            id.GetComponent<RawImage>().raycastTarget = false;
            id.transform.SetParent(mapbutton[pos].transform);
            id.transform.localPosition = Vector3.zero;
            id.transform.localEulerAngles = Vector3.zero;
            id.transform.localScale = new Vector3(.1f, .1f, .1f);
            bestpathmarks.Add(id);
        }
        game_on = false;
        Walker.GetComponent<walker>().walk(path);
        /*  
         *  
          if (PF.pathcheck8(startpos)) { Debug.Log("PATH FOUND!!!!!!");
          Vector2[] path  = PF.path();
              foreach (Vector2 pos  in path)
              {
                  print(pos);
              }


          }
          else { Debug.Log("pathcehck didn'T worked"); }*/
    }

    public void update_main_info()
    {
        coin_text.text = GameState.coins.ToString("#,#", System.Globalization.CultureInfo.InvariantCulture);
        set_text_lives(lives_text);
        if(GameState.tresaures > 0)
        {
            chest_icon.SetActive(true);
        }
        else
        {
            chest_icon.SetActive(false);
        }
    }
    void game_bonusees_check()
    {
        if(PlayerPrefs.GetString("lastbonustimestamp").Length > 1)
        {

            if(System.DateTime.Compare(System.DateTime.Parse(PlayerPrefs.GetString("lastbonustimestamp")), System.DateTime.UtcNow) < 0)
            {
                GameState.tresaures++;
                GameState.lives += 10;
                PlayerPrefs.SetString("lastbonustimestamp", System.DateTime.UtcNow.AddDays(1).ToString());
            }
        }
        else
        {
            //first timer reward sets str...
            GameState.lives += 10;
            PlayerPrefs.SetString("lastbonustimestamp", System.DateTime.Now.ToString());
        }
        //will open the tresaure panel if  1 or more is owned
        if(GameState.tresaures > 0)
        {
            Tresaurepanel.GetComponent<treasaurepanel>().toggleMainPanel();
        }


    }


    void FixedUpdate()
    {
        if(!game_on) { return; }
        if(!bonus_coin) { bonus_coin = true; }
        timer();
        android_longpress();

        if(clic_on)
        {
            if(current_button != null)
            {
                if(clic_timer >= clic_lenght)
                {
                  //  Handheld.Vibrate();
                  //  StartCoroutine(void_vibration());
                    clic_handler(current_button, false);
                    clic_void();
                }
                if(Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    clic_handler(current_button, true);
                    clic_void();
                }
            }
        }
        if(Input.touchCount < 1) { clic_on = true; clic_timer = 0; }

        //if (Input.touchCount >= 2) { clic_void(); };

    }



    #region map_funct



    public void clic_void()
    {
        clic_on = false;
        clic_timer = 0;
        current_button = null;
    }
    void defineRandomMap(int x, int y, int minestock, bool init)
    {
        mappositions.Clear();
        for(int Xi = 0; Xi < x; Xi++)
        {
            for(int Yi = 0; Yi < y; Yi++)
            {
                tile tt = new tile();
                tt.Type = tiletype.free;
                tt.State = state.none;
                mappositions.Add(new Vector2(Xi, Yi), new tile());
            }
        }
        for(int i = 0; i < mines; i++)
        {
            randomize_mine_pos(new Vector2(-1, -1));
        }
        for(int i = 0; i < obstacles; i++)
        {
            randomize_obstacle_pos();
        }

        //  print("map sucessfully defined!");
    }

    void defineMissionMap(string s)
    {
        mappositions.Clear();
        string[] seedarray = s.Split(char.Parse("#"));
        mines = 0;
        size = new Vector2Int(int.Parse(seedarray[1]), int.Parse(seedarray[2]));

        theme = (MapData.Themes)int.Parse(seedarray[3]);
        set_terrain((int)theme);
        int i = 4;
        for(int Xi = 0; Xi < size.x; Xi++)
        {
            for(int Yi = 0; Yi < size.y; Yi++)
            {
                tile tt = new tile();
                tt.State = state.none;
                // print(seedarray[i]);

                if(int.Parse(seedarray[i]) == 0) { tt.Type = tiletype.free; }
                if(int.Parse(seedarray[i]) == 1) { tt.Type = tiletype.mine; mines++; }
                if(int.Parse(seedarray[i]) == 2) { tt.Type = tiletype.start; startpos = new Vector2(Xi, Yi); }
                if(int.Parse(seedarray[i]) == 3) { tt.Type = tiletype.finish; finishpos = new Vector2(Xi, Yi); }
                if(int.Parse(seedarray[i]) == 4) { tt.Type = tiletype.tresaure; }
                if(int.Parse(seedarray[i]) > 4)
                {
                    //  print("seeding itemmmm");
                    tt.Type = tiletype.obstacle;
                    tt.State = state.obstacle;
                    tt.itemid = int.Parse(seedarray[i]) - 5;
                }

                mappositions.Add(new Vector2(Xi, Yi), tt);
                //   print("pos:" + new Vector2(Xi, Yi) + " with id : " + mappositions[new Vector2(Xi, Yi)].itemid);
                i++;
            }
        }

        ui_flags_count(0);
    }

    void SetMap2(string seed)
    {

        MapData data = Seed.GetMapData(seed);
        string name = data.GetName();
        mines = 0; //???
        size = data.GetSize();
        theme = data.GetTheme();
        int[] tiles = data.GetTiles();
        for(int y = 0; y < size.y; y++)
        {
            for(int x = 0; x < size.x; x++)
            {
                int tile = tiles[y * size.x + x];
                mappositions.Add(new Vector2(x, y), new map.tile(tile));

            }
        }
        ui_flags_count(0);
    }


    void populatemapbuttons()
    {
        map_panel.GetComponent<GridLayoutGroup>().constraintCount = (int)size.y;
        foreach(KeyValuePair<Vector2, tile> kvp in mappositions)
        {
            GameObject newbtn = Instantiate(blankbutton) as GameObject;
            newbtn.transform.SetParent(map_panel.transform);
            newbtn.transform.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
            newbtn.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
            newbtn.GetComponent<buttonclichandler>().Map = this;
            newbtn.GetComponent<RawImage>().color = tilecolors[(int)theme];
            mapbutton.Add(kvp.Key, newbtn);
            buttonmap.Add(newbtn, kvp.Key);
            Text txt;
            switch(kvp.Value.Type)
            {
                case tiletype.free:
                    break;
                case tiletype.mine:
                    break;
                case tiletype.obstacle:
                    newbtn.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);
                    Destroy(newbtn.GetComponent<buttonclichandler>());
                    Destroy(newbtn.GetComponent<Button>());
                    instantiate_obstacles(kvp.Key, kvp.Value.itemid);
                    break;
                case tiletype.revealed:
                    reveal(kvp.Key);
                    break;

                case tiletype.start:
                    DestroyImmediate(newbtn.GetComponent<RawImage>());
                    Destroy(newbtn.GetComponent<buttonclichandler>());
                    Destroy(newbtn.GetComponent<Button>());
                    txt = newbtn.AddComponent<Text>();
                    newbtn.AddComponent<Outline>();
                    txt.alignment = TextAnchor.MiddleCenter;
                    txt.fontSize = 4;
                    txt.font = gamefont;
                    txt.color = new Color(0, 209, 0, 255);
                    txt.text = "Start";
                    spawncharacter(kvp.Key);
                    break;

                case tiletype.finish:
                    DestroyImmediate(newbtn.GetComponent<RawImage>());
                    Destroy(newbtn.GetComponent<buttonclichandler>());
                    Destroy(newbtn.GetComponent<Button>());
                    txt = newbtn.AddComponent<Text>();
                    newbtn.AddComponent<Outline>();
                    txt.alignment = TextAnchor.MiddleCenter;
                    txt.fontSize = 4;
                    txt.font = gamefont;
                    txt.color = new Color(209, 0, 0, 255);
                    txt.text = "Finish";
                    break;
                default:
                    break;
            }


        }
    }


    void spawncharacter(Vector2 pos)
    {
        if(mapbutton.TryGetValue(pos, out GameObject value))
        {
            GameObject character = Instantiate(walkerprefab) as GameObject;

            character.transform.SetParent(value.transform);
            character.transform.localPosition = new Vector3(0, 0, 0);
            transform.eulerAngles = Vector3.zero;
            character.transform.position += new Vector3(0, 0, -5);



            character.GetComponent<walker>().Map = this;
            mappositions[pos].Go_on_top = character;
            Walker = character;
        }
    }

    void randomize_mine_pos(Vector2 current_pos)
    {
        Vector2 pos = new Vector2((int)Random.Range(0, size.x), (int)Random.Range(0, size.y));
        // print("mine pos" + pos);
        if(mappositions[pos].Type != tiletype.free) { randomize_mine_pos(current_pos); ; }
        else
        {
            if(pos != current_pos)
            {
                mappositions[pos].Type = tiletype.mine;
            }
            else { randomize_mine_pos(current_pos); }
        }
    }
    void randomize_obstacle_pos()
    {
        Vector2 pos = new Vector2((int)Random.Range(0, size.x), (int)Random.Range(0, size.y));
        // print(" obstacle" + pos);
        if(mappositions[pos].Type != tiletype.free) { randomize_obstacle_pos(); }
        else { mappositions[pos].Type = tiletype.obstacle; }
    }

    #endregion map





    public void zero_reveal_nearby(Vector2 pos)
    {
        for(int x = (int)pos.x - 1; x <= pos.x + 1; x++)
        {
            if(x < 0 || x >= size.x) { continue; }
            for(int y = (int)pos.y - 1; y <= pos.y + 1; y++)
            {
                if(y < 0 || y >= size.y) { continue; }
                if(x == pos.x && y == pos.y) { continue; }
                if(mappositions[new Vector2(x, y)].Type == tiletype.revealed) { continue; }

                reveal(new Vector2(x, y));
            }
        }

    }


    public int get_nearby_mines_count(Vector2 pos)
    {
        int count = 0;
        for(int x = (int)pos.x - 1; x <= pos.x + 1; x++)
        {
            if(x < 0 || x >= size.x) { continue; }
            for(int y = (int)pos.y - 1; y <= pos.y + 1; y++)
            {
                if(y < 0 || y >= size.y) { continue; }
                if(x == pos.x && y == pos.y) { continue; }
                if(mappositions[new Vector2(x, y)].Type == tiletype.revealed) { continue; }
                if(mappositions[new Vector2(x, y)].Type == tiletype.mine) { count++; }
            }
        }
        if(count == 0 && (mappositions[pos].Type != tiletype.mine)) { zero_reveal_nearby(pos); }
        return count;
    }

    void update_nearby_mines_count(Vector2 pos)
    {


        for(int x = (int)pos.x - 1; x <= pos.x + 1; x++)
        {
            if(x < 0 || x >= size.x) { continue; }
            for(int y = (int)pos.y - 1; y <= pos.y + 1; y++)
            {
                if(y < 0 || y >= size.y) { continue; }
                Vector2 currentpos = new Vector2(x, y);
                GameObject btn = mapbutton[currentpos];
                if(mappositions[currentpos].Type != tiletype.revealed) { continue; }
                set_tile_number(btn.GetComponent<Text>(), currentpos);
            }
        }

    }

    public void clic_handler(GameObject btn, bool breveal)
    {

        if(!game_on) { return; }
        if(!game_started) { game_started = true; }
        if(android && !clic_on) { clic_void(); return; }
        Vector2 pos = new Vector2(buttonmap[btn].x, buttonmap[btn].y);
        current_button = null;
        // if (breveal) { reveal(pos); }

        if(breveal)
        {
            reveal(pos);
        }
        else
        {
            if(mappositions[pos].Type == tiletype.revealed)
            {
                zero_reveal_nearby(pos);
            }
            else
            {
                flag(pos);
            }
        }



    }


    public void flag(Vector2 pos)
    {
        if(mappositions[pos].Type != tiletype.revealed)
        {
            switch(mappositions[pos].State)
            {
                case state.none:
                    if(flagcount >= mines) { mappositions[pos].State = state.questionmark; break; }
                    mappositions[pos].State = state.flag;
                    if(bombcheck.ContainsKey(pos)) { bombcheck[pos] = true; }
                    ui_flags_count(1);
                    break;
                case state.flag:
                    mappositions[pos].State = state.questionmark;
                    if(bombcheck.ContainsKey(pos)) { bombcheck[pos] = false; }
                    ui_flags_count(-1);
                    break;
                case state.questionmark:
                    mappositions[pos].State = state.none;
                    break;
            }
            if((mappositions[pos].Go_on_top) != null)
            {
                Destroy(mappositions[pos].Go_on_top);
            }
            if(mappositions[pos].State != state.none)
            {
                GameObject flag = Instantiate(flags[(int)mappositions[pos].State - 1]);
                mappositions[pos].Go_on_top = flag;
                flag.transform.position = mapbutton[pos].transform.position;
            }


        }

    }

    void reveal(Vector2 pos)
    {
        if(mappositions[pos].State != state.none) { clic_void(); return; }
        if(mappositions[pos].Type == tiletype.revealed) { clic_void(); return; }
        if(mappositions[pos].Type == tiletype.obstacle) { clic_void(); return; }
        if(mappositions[pos].Type == tiletype.finish) { clic_void(); return; }
        if(mappositions[pos].Type == tiletype.start) { clic_void(); return; }


        GameObject btn = mapbutton[pos];
        if(mappositions[pos].Type == tiletype.mine)
        {
            if(first_clic)

            {
                randomize_mine_pos(pos);
            }
            else
            {
                if(!inquiry_clic)
                {
                    explosion(pos);

                    return;
                }
                else { bombcheck[pos] = true; ui_flags_count(1); }

            }

        }

        first_clic = false;


        Text txt;

        if(mappositions[pos].Type != tiletype.revealed)
        {
            DestroyImmediate(btn.GetComponent<RawImage>());
            DestroyImmediate(btn.GetComponent<Button>());
            txt = btn.AddComponent<Text>();
            btn.AddComponent<Outline>();
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontSize = 20;
            txt.font = gamefont;
        }
        else { txt = btn.GetComponent<Text>(); }

        mappositions[pos].Type = tiletype.revealed;
        if(inquiry_clic) { GameState.inquiries--; bonus_coin = false; Toggle_inquiry(); update_nearby_mines_count(pos); }
        else { bonus_coin = true; }
        set_tile_number(txt, pos);
        clic_void();
        flagcheck();

    }

    void set_tile_number(Text txt, Vector2 pos)
    {
        int i = get_nearby_mines_count(pos);
        txt.text = "" + i;
        txt.color = hintcolors[int.Parse(txt.text)];
        if(bonus_coin)
        {
            coin_chance(pos, i);
        }
    }

    void ui_flags_count(int i)
    {
        flagcount += i;
        txtflagcount.text = "" + flagcount + "/" + mines;
        flagcheck();
    }

    void flagcheck()
    {
        if(!game_on) { return; }
        if(gametype == GameType.mission)
        {
            //hereee
            if(pathcheck(startpos, virtualmap) == true) { try_find_shortest_path(); return; }
        }
        else
        {
            if(flagcount != mines) { return; }

            foreach(KeyValuePair<Vector2, bool> kvp in bombcheck)
            {
                if(kvp.Value == false) { print("nop!!"); return; }
            }
            foreach(KeyValuePair<Vector2, tile> kvp in mappositions)
            {
                if(kvp.Value.Type == tiletype.free) { print("nop!!"); return; }
            }

            big_winner();
        }
    }
    public void explosion(Vector2 pos)
    {
        game_on = false;
      //  Handheld.Vibrate();
        StartCoroutine(void_vibration());
        mapbutton[pos].GetComponent<RawImage>().texture = crater;
        GameObject expl = Instantiate(explosions[0]);
        mappositions[pos].Go_on_top = expl;
        expl.transform.position = mapbutton[pos].transform.position;
        if(lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = StartCoroutine(endgameanimations(true));

    }

    Color32 levelcolorgrad(int i)
    {
        Color32 color = new Color32(0, 209, 0, 255);
        if(i <= 100)
        {
            int k = Mathf.RoundToInt((i * 255) / 100);
            color = new Color32((byte)k, 255, 0, 255);
        }
        else if(i > 100 && i < 200)
        {
            int k = Mathf.RoundToInt((i * 255) / 100);
            color = new Color32(255, (byte)(255 - k), 0, 255);
        }
        else { color = new Color32(255, 0, 0, 255); }

        return color;
    }
    void populate_mission_panel()
    {
        foreach(Transform tr in level_panel.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0))
        {
            Destroy(tr.gameObject);
        }
        foreach(KeyValuePair<int, string> plevel in levels)
        {
            GameObject btn = Instantiate(levelbutton) as GameObject;
            btn.transform.SetParent(level_panel.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0));
            btn.transform.GetChild(0).GetComponent<Text>().text = "" + plevel.Key;

            if(GameState.completedlevels.ContainsKey(plevel.Key))
            { btn.GetComponent<RawImage>().color = new Color32(218, 165, 32, 255); }
            else
            { btn.GetComponent<RawImage>().color = levelcolorgrad(plevel.Key); }

            btn.GetComponent<Button>().onClick.AddListener(delegate () { set_mission(plevel.Key); });
        }

        set_text_lives(level_panel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>());
    }
    public void set_mission(int i)
    {
        level = i;
        gametype = GameType.mission;
        toggle_mission(false);
        game_warm_up();
    }
    public void big_winner()
    {
        print("BIG BIG BIG WINNER!!!!!!!!");
        game_on = false;// isn't it already over? mayber for freemode 
        cam_on = false;
        if(gametype == GameType.mission)
        {
            if(GameState.completedlevels.ContainsKey(level)) { GameState.completedlevels[level] = true; }
            else { GameState.completedlevels.Add(level, true); }
            loadsave.savegame(GameState);

        }
        if(lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = StartCoroutine(endgameanimations(false));


    }



    IEnumerator endgameanimations(bool fail)
    {

        yield return new WaitForSeconds(1);
        if(fail)
        {
            if(gametype == GameType.mission)
            {
                GameState.lives--; loadsave.savegame(GameState); set_text_lives(txtlivescount);
            }
            endtext.color = new Color32(209, 0, 0, 0); endtext.text = "Failure!";
            current_game_coins = 0;
        }
        else { endtext.color = new Color32(0, 209, 0, 0); endtext.text = "SUCESS!!!"; check_FWOTD(); }
        float alpha = 0;

        if(current_game_coins > 0) { endtext.transform.GetChild(0).gameObject.SetActive(true); endtext.transform.GetChild(0).GetComponent<Text>().text = "+" + current_game_coins; }
        else { endtext.transform.GetChild(0).gameObject.SetActive(false); }
        GameState.coins += current_game_coins;
        loadsave.savegame(GameState);
        while(alpha < 1)
        {
            alpha += (Time.deltaTime * 0.5f);
            endtext.color = new Color(endtext.color.r, endtext.color.g, endtext.color.b, alpha);
            yield return new WaitForEndOfFrame();
        }
        end_panel.GetComponent<Animator>().Play("endgameanim");
    }



    void check_FWOTD()
    {
        if(PlayerPrefs.GetString("lastwotdtime").Length > 1)
        {

            if(System.DateTime.Compare(System.DateTime.Parse(PlayerPrefs.GetString("lastwotdtime")), System.DateTime.UtcNow) < 0)
            {
                GameState.tresaures++;
                PlayerPrefs.SetString("lastwotdtime", System.DateTime.UtcNow.AddDays(1).ToString());
            }
        }
        else
        {
            //first timer reward sets str...
            GameState.tresaures++;
            PlayerPrefs.SetString("lastwotdtime", System.DateTime.Now.ToString());
        }
        if(GameState.tresaures > 0 && game_on == false)
        {
            Tresaurepanel.GetComponent<treasaurepanel>().toggleMainPanel();
        }

    }
    private void set_text_lives(Text txt)
    {
        if(GameState.unlimitedLives)
        {
            txt.text = "(<)";// unlimited sign
            GameState.lives = 10;// reset count incase; unlimited  lives should altenate between 9 and 10

        }
        else
        {
            txt.text = "(" + GameState.lives + ")";
        }


    }

    public void RAZ(bool clicked = false)
    {
        if(clicked) { GameState.lives--; loadsave.savegame(GameState); }
        set_text_lives(txtlivescount);
        flagcount = 0;
        current_game_coins = 0;
        txtflagcount.text = "" + flagcount + "/" + mines;
        endtext.transform.GetChild(0).gameObject.SetActive(false);
        if(lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }
        foreach(Transform tr in map_panel.transform)
        {
            Destroy(tr.gameObject);
        }

        foreach(KeyValuePair<Vector2, tile> kvp in mappositions)
        {
            Destroy(kvp.Value.Go_on_top);
        }

        endtext.color = Color.clear;
        end_panel.GetComponent<Animator>().Play("New State");
        mappositions.Clear();
        bombcheck.Clear();
        mapbutton.Clear();
        buttonmap.Clear();
        hours = 0; min = 0; sec = 0; dec = 0;
        txttimer.text = "00:00:00";
        game_on = true;
        game_started = false;
        clic_on = true;
        cam_on = true;
        if(gametype == GameType.freeplay)
        {
            first_clic = true;
            Walk_button.SetActive(false);
        }
        if(gametype == GameType.mission)
        {
            first_clic = false;
            Walk_button.SetActive(true);
        }

        inquiry_clic = false;
        inquiry_btn.GetComponent<Outline>().effectColor = Color.black;
        inquiry_btn.transform.GetChild(1).GetComponent<Text>().text = "" + GameState.inquiries;
        if(gametype == GameType.freeplay)
        {
            change_environnement();
        }
        else
        {
            //might be remived
            setvirtualmap2(virtualmap2);
        }
        set_terrain((int)theme);
        Start();
    }
    int hours = 0; int min = 0; int sec = 0; float dec;
    void timer()
    {
        if(!game_on) { return; }
        if(!game_started) { return; }
        dec += Time.deltaTime;

        if(dec >= 1) { dec = 0; sec++; }
        if(sec >= 59) { sec = 0; min++; }
        if(min >= 59) { min = 0; hours++; }

        string s_min; string s_sec; string s_hour;
        if(sec < 10) { s_sec = "0" + sec; } else { s_sec = "" + sec; }
        if(min < 10) { s_min = "0" + min; } else { s_min = "" + min; }
        if(hours < 10) { s_hour = "0" + hours; } else { s_hour = "" + hours; }
        txttimer.text = "" + s_hour + ":" + s_min + ":" + s_sec;

    }

    public void set_map_info()
    {
        if(free_play_panel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text.Length < 1 || free_play_panel.transform.GetChild(1).GetChild(0).GetComponent<InputField>().text.Length < 1 || free_play_panel.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text.Length < 1)
        { print("text not good"); return; }
        size = new Vector2Int(int.Parse(free_play_panel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text), int.Parse(free_play_panel.transform.GetChild(1).GetChild(0).GetComponent<InputField>().text));
        mines = int.Parse(free_play_panel.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text);
        obstacles = int.Parse(free_play_panel.transform.GetChild(3).GetChild(0).GetComponent<InputField>().text);

        if(size.x > 20) { size = new Vector2Int(20, size.y); }
        if(size.y > 20) { size = new Vector2Int(size.x, 20); }
        if(size.x < 3) { size = new Vector2Int(3, size.y); }
        if(size.y < 3) { size = new Vector2Int(size.x, 3); }
        if(mines > (size.x * size.y) * 0.5f) { mines = (int)Mathf.Floor((size.x * size.y) * 0.5f); }
        if(mines < 0.1f * (size.x * size.y)) { mines = (int)Mathf.Floor(0.1f * (size.x * size.y)); }
        if(mines < 1) { mines = 1; }
        if(obstacles > 0.2f * (size.x * size.y)) { obstacles = (int)Mathf.Floor(0.2f * (size.x * size.y)); }

        PlayerPrefs.SetInt("sizeX", (int)size.x);
        PlayerPrefs.SetInt("sizeY", (int)size.y);
        PlayerPrefs.SetInt("mines", mines);
        PlayerPrefs.SetInt("obstacles", obstacles);
        gametype = GameType.freeplay;
        free_play_panel.transform.parent.gameObject.SetActive(false);

        game_warm_up();
    }
    void game_warm_up()
    {
        game_canvas.SetActive(true);
        RAZ();
        cam.orthographicSize = (size.x * 10) + size.x;
        if(cam.orthographicSize < 110) { cam.orthographicSize = 110; }

    }


    public void exitgame()
    { Application.Quit(); }
    public void show_main_menu(bool lifeloss)
    {
        if(lifeloss)
        {
            GameState.lives--;
            loadsave.savegame(GameState);
            quit_panel.transform.parent.gameObject.SetActive(false);

        }
        cam_on = false;
        game_on = false;
        end_panel.transform.localPosition = new Vector3(0, 451, 0);
        if(gametype == GameType.freeplay)
        {
            free_play_panel.transform.parent.gameObject.SetActive(true);
        }
        if(gametype == GameType.mission)
        {
            toggle_mission(false);
        }
        game_canvas.SetActive(false);
        update_main_info();
    }

    void android_longpress()
    {
        if(!android) { return; }
        if(Input.touchCount > 1) { clic_void(); return; }
        if(current_button == null) { return; }
        if(Input.GetMouseButton(0))
        {
            clic_timer += Time.deltaTime;
        }
    }





    public void Toggle_inquiry()
    {

        if(GameState.inquiries < 1)
        {
            inquiry_clic = false;
            inquiry_btn.GetComponent<Outline>().effectColor = Color.black;
            inquiry_btn.transform.GetChild(1).GetComponent<Text>().text = "" + GameState.inquiries;
            return;
        }




        inquiry_clic = !inquiry_clic;

        if(inquiry_clic) { inquiry_btn.GetComponent<Outline>().effectColor = Color.yellow; }
        else
        {
            inquiry_btn.GetComponent<Outline>().effectColor = Color.black;
            inquiry_btn.transform.GetChild(1).GetComponent<Text>().text = "" + GameState.inquiries;
        }

    }

    void instantiate_obstacles(Vector2 pos, int id)
    {
        if(id < 0)
        {
            id = Random.Range(0, dic_obstacles[(int)theme].Length);

        }
        GameObject obstacle = Instantiate(dic_obstacles[(int)theme][id]) as GameObject;
        obstacle.transform.SetParent(mapbutton[pos].transform);
        //obstacle.transform.localEulerAngles = Vector3.zero;
        obstacle.transform.localPosition = Vector3.zero;



    }

    IEnumerator void_vibration()
    {
        yield return new WaitForSeconds(.1f);
       // Handheld.Vibrate();
    }


    void load_free_map_info()
    {


        free_play_panel.transform.GetChild(0).GetChild(0).GetComponent<InputField>().text = PlayerPrefs.GetInt("sizeX").ToString();
        free_play_panel.transform.GetChild(1).GetChild(0).GetComponent<InputField>().text = PlayerPrefs.GetInt("sizeY").ToString();
        free_play_panel.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text = PlayerPrefs.GetInt("mines").ToString();
        free_play_panel.transform.GetChild(3).GetChild(0).GetComponent<InputField>().text = PlayerPrefs.GetInt("obstacles").ToString();


    }


    void set_obstacle_dictionary()
    {
        dic_obstacles.Add(0, forest_obstacles_list);
        dic_obstacles.Add(1, beach_obstacles_list);
        dic_obstacles.Add(2, space_obstacles_list);
        dic_obstacles.Add(3, snow_obstacles_list);

    }
    void change_environnement()
    {
        if(gametype == GameType.freeplay)
        {
            theme = (MapData.Themes)Random.Range(0, dic_obstacles.Count);
        }
        set_terrain((int)theme);



    }
    void set_terrain(int k)
    {
        Background.GetComponent<RawImage>().color = bg_color[k];
        Background.transform.GetChild(1).GetComponent<RawImage>().texture = floor_textures[k];

    }

    public void toggle_mission(bool main)
    {
        level_panel.SetActive(!level_panel.activeInHierarchy);
        if(main)
        {
            Main_panel.SetActive(!Main_panel.activeInHierarchy);
        }
        populate_mission_panel();
    }

    public void toggle_free_play(bool main)
    {
        free_play_panel.SetActive(!free_play_panel.activeInHierarchy);
        if(main)
        {
            Main_panel.SetActive(!Main_panel.activeInHierarchy);
        }

    }

    void set_levels_dictionary()
    {
        levels.Add(1, "0#3#3#0#4#0#0#0#1#0#0#0#5#");
        levels.Add(5, "1#5#5#0#6#7#7#0#3#1#0#0#1#0#0#0#7#0#0#0#0#0#0#1#2#0#1#5#0#");
        levels.Add(7, "1#6#3#1#10#0#2#9#0#0#8#0#1#7#0#1#6#0#5#5#0#3#");
        levels.Add(8, "1#5#5#2#6#5#5#0#0#0#1#0#1#7#2#1#7#1#3#0#1#0#1#0#7#0#1#0#1#");
        levels.Add(9, "1#6#6#0#2#0#0#0#0#0#0#1#0#1#0#0#0#0#0#6#0#1#0#1#1#5#5#0#6#0#0#5#3#1#0#1#1#0#0#1#");
        levels.Add(11, "1#10#4#3#1#0#2#0#6#0#5#0#0#1#6#0#0#7#0#1#0#7#0#6#0#5#1#0#0#1#0#0#5#0#1#5#0#1#1#6#6#0#0#3#");
        levels.Add(12, "1#8#8#0#2#0#0#0#0#0#0#5#0#0#0#1#0#0#0#1#0#0#5#1#0#7#1#0#1#0#1#0#6#0#0#0#0#0#0#0#5#1#0#0#1#1#0#1#6#0#1#0#5#0#0#1#0#0#0#0#5#1#0#0#1#0#0#3#");
        levels.Add(13, "1#10#4#3#1#1#0#2#1#1#0#5#0#5#0#1#1#0#0#0#0#0#5#0#0#1#0#0#0#6#1#1#0#0#0#0#7#1#0#1#3#0#0#1#");
        levels.Add(14, "1#19#8#0#0#0#0#0#0#5#0#2#0#5#0#0#0#5#0#0#0#5#5#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#5#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#5#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#7#0#0#5#5#0#0#0#5#0#0#0#5#0#0#0#5#5#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#0#0#0#5#5#0#0#0#0#0#0#3#5#5#5#5#0#0#");
        levels.Add(19, "1#6#11#1#0#1#0#0#0#1#0#0#0#5#3#2#0#1#0#6#0#0#1#5#1#0#0#7#0#0#1#0#0#5#0#0#0#1#0#1#1#1#10#1#8#1#0#1#0#0#6#0#0#0#10#0#1#1#0#0#1#0#1#0#1#0#0#0#0#0#");
        levels.Add(40, "1#8#10#2#2#0#1#0#0#0#0#5#5#0#1#0#0#5#1#0#1#1#0#0#1#1#0#0#0#1#0#0#1#0#5#6#5#6#0#5#5#1#1#5#0#0#0#0#0#0#0#0#0#0#0#1#7#1#1#1#5#1#0#1#0#6#7#0#0#0#5#1#1#1#1#0#0#1#1#1#0#0#0#3#");
        levels.Add(41, "1#8#9#3#5#0#0#0#5#0#0#0#0#1#0#0#1#0#0#0#0#0#0#5#2#1#0#0#5#0#0#1#0#0#0#1#5#0#1#0#0#1#0#1#5#0#1#0#0#5#6#0#5#0#1#1#5#1#1#1#0#1#1#1#0#3#0#0#0#1#0#0#0#0#0#5#");
        levels.Add(99, "1#20#20#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#3#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#1#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#1#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#5#0#5#5#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#2#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#");


    }

    void setvirtualmap2(Dictionary<Vector2, bool> virtmap)
    {
        bool firstgame = true;

        if(virtmap.TryGetValue(Vector2.zero, out firstgame))
        {
            firstgame = false;
        }

        for(int Xi = 0; Xi < size.x; Xi++)
        {
            for(int Yi = 0; Yi < size.y; Yi++)
            {
                if(firstgame) { virtmap.Add(new Vector2(Xi, Yi), false); }
                else { virtmap[new Vector2(Xi, Yi)] = false; }

            }
        }

    }

    Dictionary<Vector2, bool> virtualmap2 = new Dictionary<Vector2, bool>();
    Dictionary<Vector2, int> virtualmap = new Dictionary<Vector2, int>();
    bool pathcheck4(Vector2 pos)
    {
        // print("pathccheck : " + pos);
        if(pos.x < 0 || pos.y < 0) { return false; }
        if(pos.x >= size.x || pos.y >= size.y) { return false; }
        if(virtualmap2[pos]) { return false; }
        if(mappositions[pos].Type == tiletype.finish) { return true; }
        //switch for  if not obstacle and not  flagged if function return true then check for mines on the virtual map path. 
        if(mappositions[pos].Type != tiletype.revealed) { if(mappositions[pos].Type != tiletype.start) { return false; } }
        virtualmap2[pos] = true;
        if(pathcheck4(pos + (new Vector2(0, 1))) == true) { return true; }
        if(pathcheck4(pos + (new Vector2(1, 0))) == true) { return true; }
        if(pathcheck4(pos + (new Vector2(0, -1))) == true) { return true; }
        if(pathcheck4(pos + (new Vector2(-1, 0))) == true) { return true; }
        virtualmap2[pos] = false;
        return false;

    }



    void setvirtualmap(Dictionary<Vector2, int> virtmap)
    {
        bool firstgame = true;
        int gg;
        if(virtmap.TryGetValue(Vector2.zero, out gg))
        {
            firstgame = false;
        }

        for(int Xi = 0; Xi < size.x; Xi++)
        {
            for(int Yi = 0; Yi < size.y; Yi++)
            {
                if(firstgame) { virtmap.Add(new Vector2(Xi, Yi), -2); }
                else { virtmap[new Vector2(Xi, Yi)] = -2; }
            }
        }

    }


    bool pathcheck(Vector2 pos, Dictionary<Vector2, int> virmap)
    {
        Dictionary<Vector2, int> tempvirmap = new Dictionary<Vector2, int>();
        if(pathcheckfull(pos, tempvirmap) > -1)
        {
            foreach(KeyValuePair<Vector2, int> kvp in tempvirmap)
            {
                mapbutton[kvp.Key].GetComponent<RawImage>().color = Color.red;
            }

        }
        // print("virmap: " +tempvirmap.Count);



        return false;
    }


    int pathcheckfull(Vector2 pos, Dictionary<Vector2, int> virmap)
    {
        int i = 0;
        // print("pathccheck : " + pos);
        if(pos.x < 0 || pos.y < 0) { return -1; }
        if(pos.x >= size.x || pos.y >= size.y) { return -1; }
        if(virmap.ContainsKey(pos))
        {
            if(virmap[pos] > -2) { return -1; }
        }
        if(mappositions[pos].Type == tiletype.obstacle) { return -1; }

        if(mappositions[pos].Type == tiletype.finish) { print("reached finish"); return virmap.Count; }// switch all virmap value to lenght

        //switch for  if not obstacle and not  flagged if function return true then check for mines on the virtual map path. 

        if(mappositions[pos].Type != tiletype.revealed) { if(mappositions[pos].Type != tiletype.start) { i = 1; } }//!REVEALED SHOULD ADD + 1 TO THE  LINE

        virmap.Add(pos, i);
        // III  PATH CHECK  RETURN> -1 MEANS THAT PATH IS EITHER PERFECT OR FAISABLE. 
        //THEN DECIDE WHICH IS SHORTER THAN RETURN THE RIGHT PATH FOR NEXT COMPARAISONS.
        Dictionary<Vector2, int> tempvirmap = new Dictionary<Vector2, int>(virmap);
        Dictionary<int, Dictionary<Vector2, int>> dir_dic = new Dictionary<int, Dictionary<Vector2, int>>();
        for(int j = 0; i < 8; i++)
        {
            dir_dic = new Dictionary<int, Dictionary<Vector2, int>>();
            Vector2 newpos1 = Vector2.zero;
            switch(j)
            {
                case 0:
                    newpos1 = new Vector2(0, 1);
                    break;

                case 1:
                    newpos1 = new Vector2(1, 1);
                    break;

                case 2:
                    newpos1 = new Vector2(1, 0);
                    break;

                case 3:
                    newpos1 = new Vector2(1, -1);
                    break;

                case 4:
                    newpos1 = new Vector2(0, -1);
                    break;

                case 5:
                    newpos1 = new Vector2(-1, -1);
                    break;

                case 6:
                    newpos1 = new Vector2(-1, 0);
                    break;

                case 7:
                    newpos1 = new Vector2(-1, 1);
                    break;
            }
            pathcheckfull(newpos1, tempvirmap);
            dir_dic.Add(j, tempvirmap);
        }
        int w = -2;
        int dangermine = 999;
        int dirid = -2;
        for(int j = 0; j < 8; j++)
        {
            if(dir_dic.ContainsKey(j))
            {
                if(dir_dic[j].Count < w)
                {
                    if(dir_dic[j].Count < -1)
                    {
                        dirid = j; w = dir_dic[j].Count;
                    }
                    else
                    {
                        w = -1;
                    }
                }
                if(dir_dic[j].Count == w)
                {
                    int t = dangermine;
                    foreach(KeyValuePair<Vector2, int> kvp in dir_dic[j])
                    {
                        t += kvp.Value;
                    }
                    if(dangermine > t)
                    {
                        dangermine = t;
                        w = dir_dic[j].Count;
                    }
                }
            }
            virmap = new Dictionary<Vector2, int>(tempvirmap);
        }
        Vector2 newpos = Vector2.zero;
        switch(dirid)
        {
            case 0:
                newpos = new Vector2(0, 1);
                break;

            case 1:
                newpos = new Vector2(1, 1);
                break;

            case 2:
                newpos = new Vector2(1, 0);
                break;

            case 3:
                newpos = new Vector2(1, -1);
                break;

            case 4:
                newpos = new Vector2(0, -1);
                break;

            case 5:
                newpos = new Vector2(-1, -1);
                break;

            case 6:
                newpos = new Vector2(-1, 0);
                break;

            case 7:
                newpos = new Vector2(-1, 1);
                break;
            default:
                return -1;

        }
        return pathcheckfull(newpos, virmap);

    }



    bool pathcheck8(Vector2 pos, Dictionary<Vector2, int> virmap)
    {
        int i = 0;
        // print("pathccheck : " + pos);
        if(pos.x < 0 || pos.y < 0) { return false; }
        if(pos.x >= size.x || pos.y >= size.y) { return false; }
        if(virtualmap2[pos]) { return false; }
        if(mappositions[pos].Type == tiletype.finish) { return true; }// switch all virmap value to lenght 
                                                                      //switch for  if not obstacle and not  flagged if function return true then check for mines on the virtual map path. 
        if(mappositions[pos].Type != tiletype.revealed) { i = 1; if(mappositions[pos].Type != tiletype.start) { return false; } }//!REVEALED SHOULD ADD + 1 TO THE  LINE
        virmap.Add(pos, i);
        // III  PATH CHECK  RETURN> -1 MEANS THAT PATH IS EITHER PERFECT OR FAISABLE. 
        //THEN DECIDE WHICH IS SHORTER THAN RETURN THE RIGHT PATH FOR NEXT COMPARAISONS.
        if(pathcheck8(pos + (new Vector2(0, 1)), virmap) == true) { return true; }// --N
        if(pathcheck8(pos + (new Vector2(1, 1)), virmap) == true) { return true; }// --NE
        if(pathcheck8(pos + (new Vector2(1, 0)), virmap) == true) { return true; }// --E
        if(pathcheck8(pos + (new Vector2(1, -1)), virmap) == true) { return true; }// --SE
        if(pathcheck8(pos + (new Vector2(0, -1)), virmap) == true) { return true; }// --S
        if(pathcheck8(pos + (new Vector2(-1, -1)), virmap) == true) { return true; }// --SW
        if(pathcheck8(pos + (new Vector2(-1, 0)), virmap) == true) { return true; }// --W
        if(pathcheck8(pos + (new Vector2(-1, 1)), virmap) == true) { return true; }// --NW

        virmap.Remove(pos);
        return false;

    }

    public bool isInbound(Vector2 pos)
    {
        if(pos.x < 0 || pos.y < 0) { return false; }
        if(pos.x >= size.x || pos.y >= size.y) { return false; }
        return true;
    }


    int current_game_coins = 0;
    public GameObject coinanim;
    void coin_chance(Vector2 pos, int tilevalue)
    {
        // random chances part
        if(Random.Range(0, 100000) > 100000 - (tilevalue * 10000))
        {
            current_game_coins += 100;


            GameObject coin = Instantiate(coinanim) as GameObject;
            coin.transform.position = mapbutton[pos].transform.position;
            coin.transform.SetParent(mapbutton[pos].transform.parent);
            Destroy(coin, 2.5f);
        }


        // anim part using pos and button dictio

    }




    public void toggle_shop()
    {
        shop_panel.SetActive(!shop_panel.activeInHierarchy);
        if(!shop_panel.activeInHierarchy) { update_main_info(); }

    }
    public GameObject quit_panel;
    public void Toggle_Quit_panel(bool open)
    {
        if(open && !game_on) { return; }
        game_on = !open;
        if(!game_started) { show_main_menu(false); return; }

        if(open)
        {
            quit_panel.transform.parent.gameObject.SetActive(true);
            if(gametype == GameType.mission)
            {
                quit_panel.transform.GetChild(1).GetComponent<Text>().text = "You will lose Your <color=red>progrss</color> and a<color=red> recruit badge </color> if you leave.";
            }
            else if(gametype == GameType.freeplay)
            {
                quit_panel.transform.GetChild(1).GetComponent<Text>().text = "You will lose Your <color=red>progrss</color> if you leave.";
            }
        }
        else
        {
            quit_panel.transform.parent.gameObject.SetActive(false);
        }

    }

    public GameObject Option_panel;
    public void Toggle_options_menu(bool open)
    {
        Option_panel.transform.parent.parent.gameObject.SetActive(open);

        if(open)
        {
            set_option_panel_info();
        }
        else
        {
            save_options();
        }

    }

    public static void set_option_panel_info()
    {
        // bla bla  check according to saved settings.


    }

    public static void save_options()
    {
        // bla bla  check according to saved settings.


    }




}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public enum DataTypes {Name = 0, Mission = 1, SizeX = 2, SizeY=3, Theme = 4, }
    public enum Themes { forest = 0, beach = 1, space = 2, snow = 3 };

    [SerializeField] protected string name;
    [SerializeField] protected Themes theme;
    [SerializeField] protected Vector2Int size;
    [SerializeField] protected int[] tiles;
    [SerializeField] protected bool mission;


    public MapData(string seed)
    {
        string[] seeds = seed.Split("#");

        name = seeds[(int)DataTypes.Name];      
        theme = (Themes) Convert.ToInt32(seeds[(int)DataTypes.Theme]);

        int x = Convert.ToInt32(seeds[(int)DataTypes.SizeX]);
        int y =  Convert.ToInt32(seeds[(int)DataTypes.SizeY]);
        size = new Vector2Int(x, y);

        mission = 1 == Convert.ToInt32(seeds[(int)DataTypes.Mission]);
        
        tiles = seeds.AsSpan(5, size.x * size.y).ToArray().Select(i => Convert.ToInt32(i)).ToArray();
    }

    public MapData(string name, bool mission, Themes theme, Vector2Int size, int[] tiles)
    {
        this.name = name;
        this.theme = theme;
        this.mission = mission;
        this.size = size;
        this.tiles = tiles;
    }


    public string GetName() { return name; }
    public Themes GetTheme() { return theme; }
    public Vector2Int GetSize() { return size; }
    public int[] GetTiles() { return tiles; }
    public bool? GetMission() { return mission; }


    [System.Serializable]
    public class TileData
    {
        public enum Tiletype { free = 0, mine = 1, obstacle = 2, revealed = 3, start = 4, finish = 5, tresaure = 6 };


        public Tiletype type = Tiletype.free;
        public int itemid = -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static MapData.TileData;
using static MapData;
using System;

public static class Seed
{
    public static string GetSeed(MapData data)
    {
        string s;

        bool Mission = data.GetTiles().Any(i => (Tiletype)i == Tiletype.start || (Tiletype)i == Tiletype.finish);

        if(Mission)
        {
            s = "" + 1 + "#";
        }
        else
        {
            s = "" + 0 + "#";
        }

        Vector2Int size = data.GetSize();
        s += size.x + "#" + size.y + "#" + (int)data.GetTheme() + "#";
        int[] tiles = data.GetTiles();
        for(int y = 0; y < size.x; y++)
        {
            for(int x = 0; y < size.x; x++)
            {
                int id= tiles[y * size.x + x];
                Tiletype  tiletype = (Tiletype)id;
                string k = "0#"; ;
                switch(tiletype)

                {
                    case Tiletype.free:
                        k = "0#";
                        break;
                    case Tiletype.mine:
                        k = "1#";
                        break;
                    case Tiletype.obstacle:
                        k = "" + tiles + "#";
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

    public static MapData GetMapData(string seed) 
    {
        return new MapData(seed);
    }
}

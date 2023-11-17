using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static editorscript;

[System.Serializable]
public class EditorMapData : MapData
{
    public EditorMapData(string seed) : base(seed)
    {
    }

    public EditorMapData(string name, bool mission, Themes theme, Vector2Int size, int[] tiles) : base(name, mission, theme, size, tiles)
    {
    }

    public void SetTheme(Themes newTheme)
    {
        theme = newTheme;
    }
    public void SetSize(Vector2Int newSize)
    {
        size = newSize;
    }
    void SetTile(Vector2Int tile, int value) 
    {

        if((tile.x < 0 || tile.y < 0) || (tile.x >= size.x || tile.y >= size.y)) { return; }
        tiles[tile.y * size.x + tile.x] = value;

    }
  
}

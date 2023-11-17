using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MapData;

public class ButtonLayoutView : MonoBehaviour
{
    public class ButtonSetupData 
    {
        public TileButton prefab;
        public Color[] colors;
        public int fontsize = 20;
        public GameObject[] obstacles;
        public System.Action<TileButton, bool> callback = null;


    }

    [SerializeField] Transform tilesCenter;
    [SerializeField] GridLayoutGroup grid;


    TileButton[] tiles;

    public TileButton GetTile(int id)
    {
        if(tiles == null || id < 0 || id >= tiles.Length) { return null; }
        return tiles[id];
    }

    public void SetButtons(Vector2Int size, int[] tiles, ButtonSetupData buttonData)
    {
        ClearMap();

        this.tiles = new TileButton[size.x * size.y];

        grid.constraintCount = size.x;

        for(int i = 0; i < tiles.Length; i++)
        {
            TileButton tile = Instantiate(buttonData.prefab, tilesCenter) as TileButton;
            if(tiles[i] >= 100)
            {
                tile.SetTopObject(Instantiate(buttonData.obstacles[tiles[i] - 100]));
            }
            else 
            {
            
                tile.SetUp(buttonData.colors, buttonData.fontsize, buttonData.callback);
            }
            this.tiles[i] = tile;
        }

    }

    void ClearMap()
    {
        if(tiles == null) { return; }
        for(int i = 0; i < tiles.Length; i++)
        {
            Destroy(tiles[i]);
        }

    }
}

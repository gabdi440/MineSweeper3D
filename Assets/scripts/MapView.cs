using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    [SerializeField] RawImage floor;
    [SerializeField] MapCreationData mapCreationData;
    [SerializeField] ButtonLayoutView buttonLayoutView;



    MapData data;


    public void CreateMap(string seed, System.Action<TileButton,bool> callback) 
    {
      MapData data = Seed.GetMapData(seed);

        Theme theme = mapCreationData.GetTheme(data.GetTheme());

        floor.texture = theme.GetFloorTexture((int)data.GetTheme());
        Vector2Int size = data.GetSize();
        TileButton tilePrefab = theme.GetButton();
        if(tilePrefab == null)
        {
            tilePrefab = mapCreationData.GetDefaultTheme().GetButton();
        }
        int[] tiles = data.GetTiles();
        
        ButtonLayoutView.ButtonSetupData buttonData = new ButtonLayoutView.ButtonSetupData();
        buttonData.prefab = tilePrefab;
        buttonData.colors = mapCreationData.Colors();
        buttonData.fontsize = mapCreationData.FontSize();
        buttonData.callback = callback;
        buttonData.obstacles = theme.GetObstacles();

        buttonLayoutView.SetButtons(size, tiles, buttonData);

        for(int i = 0; i < tiles.Length; i++)
        {
            GameObject prefab;
            if(tiles[i] >= 100)
            {
                prefab = theme.GetObstacleId(tiles[i] - 100);

                if(prefab != null)
                {
                    buttonLayoutView.GetTile(i)?.SetTopObject(Instantiate(prefab));
                }
            }
        }
    }


}

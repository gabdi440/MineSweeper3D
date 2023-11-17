using System;
using UnityEngine;
using static MapData;

[System.Serializable]
[CreateAssetMenu(menuName = "Theme")]
public class Theme:ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] MapData.Themes theme;
    [SerializeField] Color buttonColor;
    [SerializeField] Texture2D floorTexture;
    [SerializeField] GameObject[] obstacles;
    [SerializeField] TileButton button;
    [SerializeField] GameObject flag;
    [SerializeField] GameObject questionMark;


    public bool IsOfTheme(Themes otherTheme)
    {
        return theme == otherTheme;
    }

    public Texture2D GetFloorTexture(int id)
    {
        return floorTexture;
    }

    internal TileButton GetButton()
    {
        return button;
    }
    public GameObject GetObstacleId(int id) 
    {
        if(obstacles == null || id < 0 || id >= obstacles.Length) { return null; }
        return obstacles[id];
      
    }

    internal GameObject[] GetObstacles()
    {
        return obstacles;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileNumber : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] Text text;

    public void SetupTileNumber(Color[] colors, int fontsize = 20) 
    {
       text.fontSize = fontsize;
       this.colors = colors;
    }

    public void SetNumber(int number) 
    {
        text.color = colors[number];
        text.text = ""+ number;
        gameObject.SetActive(true);
    }


}

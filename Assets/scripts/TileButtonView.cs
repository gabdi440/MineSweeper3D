using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TileButtonView : MonoBehaviour
{
    [SerializeField] TileNumber tileNumber;
    [SerializeField] GameObject tile;
    [SerializeField] Transform topObjectParent;
    
    GameObject topObject;

    public void SetupView(Color[] colors, int fontSize = 20) 
    {
    tileNumber.SetupTileNumber(colors, fontSize);
    }

    public void SetText(int count) 
    {
        ToggleTile(false);
        tileNumber.SetNumber(count);
    }


    void ToggleTile(bool toggle) 
    {
        tile.SetActive(toggle);
    }

    public void SetTopObject(GameObject objectToPair,bool hideTile = false) 
    {
        topObject?.SetActive(false);
        topObject = objectToPair;
        objectToPair.transform.SetParent(topObjectParent, false);
        topObject.transform.localPosition = Vector3.zero;

        if(hideTile) 
        {
            ToggleTile(false);
        }

    }
    public void Explode() 
    {
    
    }

}

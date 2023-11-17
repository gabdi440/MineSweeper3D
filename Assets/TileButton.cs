using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{

    [SerializeField] TileButtonView tilebuttonView;

    System.Action<TileButton, bool> callback;
    public void SetUp(Color[] colors, int fontsize = 20, System.Action<TileButton, bool> callback = null)
    {
        this.tilebuttonView.SetupView(colors, fontsize);
        this.callback = callback;
    }

    public void SetTopObject(GameObject topObject) 
    {
        tilebuttonView.SetTopObject(topObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // if(Map.android) { return; }

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            callback?.Invoke(this, true);
           // Map.clic_handler(this.gameObject, true);
        }

        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            callback?.Invoke(this, false);

        }
    }

    /*
    public void OnPointerUp(PointerEventData eventData)
    {      
        if (!Map.android) { return; }
        if (Input.touchCount >= 1) { return; }
    
        if (eventData.button != PointerEventData.InputButton.Left) { return; }          
        if (Map.clic_on == false) { Map.clic_on = true; return; }

        Map.current_button = this.gameObject;

         //   if (Map.clic_timer >= Map.clic_lenght)
       // {           
        //    Map.clic_handler(this.gameObject, false);
       // }
      //else 
       //
           // Map.clic_handler(this.gameObject, true);
      //}
      //  Map.clic_timer = 0;
    }

    */
    public void OnPointerDown(PointerEventData eventData) //???
    {
      //  if(!Map.android) { return; }

        if(Input.touchCount >= 2) { return; }
        if(eventData.button != PointerEventData.InputButton.Left) { return; }
       // Map.current_button = this.gameObject;
    }



}

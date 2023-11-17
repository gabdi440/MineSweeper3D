using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class buttonclichandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
   public  map Map;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Map.android){return;}
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Map.clic_handler(this.gameObject, true);
        }

        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Map.clic_handler(this.gameObject, false);
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
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Map.android) { return; }
        if (Input.touchCount >= 2) { return; }
        if (eventData.button != PointerEventData.InputButton.Left) { return; }
        Map.current_button = this.gameObject;
    }
}

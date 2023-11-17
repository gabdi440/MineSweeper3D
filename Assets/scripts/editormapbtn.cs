using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class editormapbtn : MonoBehaviour, IPointerClickHandler
{
    public editorscript editor;
    public Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.transform.GetComponent<EventTrigger>());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        { print("pos: " + pos);
        }
        else
        {
            editor.set_pos_in_dictionary(this.gameObject, pos);
        }
    }
}

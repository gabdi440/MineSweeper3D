using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walker : MonoBehaviour
{
    public map Map;

   public void walk(Vector2[] path)
    {

        StartCoroutine(walkco(path));

    }

    IEnumerator walkco(Vector2[] path)
    {

        foreach (Vector2 pos in path)
        {
            if (pos == path[0]) { continue; }
            transform.LookAt(Map.mapbutton[pos].transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            float magnitude = Vector3.Distance( transform.position, Map.mapbutton[pos].transform.position);

            

           // print("magnitude :  "+ magnitude);
            float speed = (magnitude / 60) ;
            for (int i = 0; i < 60 ; i++)
            {
                transform.Translate(Vector3.forward *speed); ;
       
                yield return new WaitForEndOfFrame();
            }
            if (Map.mappositions[pos].Type == map.tiletype.mine) { Map.explosion(pos); break; }
            if (Map.mappositions[pos].Type == map.tiletype.finish) { Map.big_winner(); break; }


        }

        yield return null;

    }

}

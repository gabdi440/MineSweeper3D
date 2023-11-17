using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class treasaurepanel : MonoBehaviour
{
    public Text tresauretxt;
    public Text wintxt;
    public GameObject winpanel;
    public map Map;
    public void toggleMainPanel()
    {

        if (!this.gameObject.activeInHierarchy)
        {
            update_tresaures_text();
        }
        else { Map.update_main_info(); }
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);
    }
    public void toggleWinPanel()
    {
        if (Map.GameState.tresaures < 1) { winpanel.SetActive(false); toggleMainPanel(); return; }
        if (!winpanel.activeInHierarchy)
        {
            Map.GameState.tresaures--;
            int y = Random.Range(1, 1000);

            int i = 0;
            if (y > 980)
            {

                if (y < 981) { i = 250; }
                if (y >= 981 && y < 990) { i = 500; }
                if (y >= 991 && y < 996) { i = 1000; }
                if (y >= 997 && y < 998) { i = 2500; }
                if (y >= 999) { i = 5000; }

                Map.GameState.coins += i;
                wintxt.text = "YOU Found <size=70>" + i + "</size> ";
                winpanel.transform.GetChild(0).GetComponent<RawImage>().texture = Map.rewardicon[0];
            }
            else if (y > 700 && y <= 980)
            {
                y = Random.Range(1, 1000);
                i = 1;
                if (y < 981) { i = 1; }
                if (y >= 981 && y <= 990) { i = 2; }
                if (y >= 991 && y <= 996) { i = 3; }
                if (y >= 997 && y <= 998) { i = 4; }
                if (y >= 999) { i = 5; }

               
                wintxt.text = "YOU Found <size=70>" + i + "</size> ";
                if (!Map.GameState.unlimitedLives)
                {
                    Map.GameState.lives += i;
                    winpanel.transform.GetChild(0).GetComponent<RawImage>().texture = Map.rewardicon[2];
                }
                else
                {
                    Map.GameState.inquiries += i;
                    winpanel.transform.GetChild(0).GetComponent<RawImage>().texture = Map.rewardicon[1];
                }


            }
            else
            {
                y = Random.Range(1, 1000);
                i = 1;
                if (y < 350) { i = 1; }
                if (y >= 350 && y <= 600) { i = 2; }
                if (y >= 600 && y <= 800) { i = 3; }
                if (y >= 800 && y <= 950) { i = 4; }
                if (y >= 950) { i = 5; }
                Map.GameState.inquiries += i;
                wintxt.text = "YOU unlocked <size=70>" + i + "</size>";
                winpanel.transform.GetChild(0).GetComponent<RawImage>().texture = Map.rewardicon[1];
            }
        }
        winpanel.SetActive(!winpanel.activeInHierarchy);
        update_tresaures_text();
        loadsave.savegame(Map.GameState);
    }
    void update_tresaures_text()
    {
        tresauretxt.text = "YOU HAVE <size=70> " + Map.GameState.tresaures + " </size> tresaure(s)!";
    }
}

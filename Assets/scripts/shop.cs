using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shop : MonoBehaviour
{
    public Text coin_text;
    public GameObject menu;
    public map Map;
    public GameObject list_item_prefab;
    public Texture[] currency_type;
    public Texture[] item_icons;
    struct store_item
    {
        public string name;
        public bool real_currency;
        public int price;
        public int count;
        public int store_icon_id;

        public store_item(string nam, bool curr, int pric, int coun, int id)
        {
            name = nam;
            real_currency = curr;
            price = pric;
            count = coun;
            store_icon_id = id;
        }

    }

    Dictionary<int, store_item> store_items = new Dictionary<int, store_item>();

    // Start is called before the first frame update
    void Awake()
    {
        set_store_dictionary();
        set_list();


    }
    void Start()
    {
        update_coins();
        //  set_list(); 
    }

    void set_store_dictionary()
    {// ids { 0 = lens, 1 =  badge, 2 = chest, 3 =coins, 4 =no ads,  5= bundle1,  6= 20000 coins, 7= 2x 50x + chest bundle,  8 total buyout bundle }
        //tiers 1 items
        store_items.Add(1, new store_item("inquiry lens", false, 500, 1, 0));
        if (!Map.GameState.unlimitedLives)
        {
            store_items.Add(2, new store_item("Recruit badge", false, 500, 1, 1));
        }
        store_items.Add(3, new store_item("Tresaure chest", false, 1000, 1, 2));
        store_items.Add(4, new store_item("5,000 coins", true, 99, 5000, 3));
        if (!Map.GameState.noAds)
        {
            store_items.Add(5, new store_item("no ads!", true, 299, 1, 4));
        }
        //tiers 2 items
        store_items.Add(6, new store_item("inquiry lens", false, 2000, 5, 0));
        if (!Map.GameState.unlimitedLives)
        {
            store_items.Add(7, new store_item("Recruit badge", false, 2000, 5, 1));
        }
        store_items.Add(8, new store_item("Lens & badge bundle", false, 3500, 5, 5));
        store_items.Add(9, new store_item("20,000 coins", true, 299, 20000, 6));
        //tiers 3 items
        store_items.Add(10, new store_item("inquiry lens", false, 18000, 50, 0));
        if (!Map.GameState.unlimitedLives)
        {
            store_items.Add(11, new store_item("Recruit badge", false, 18000, 50, 1));
        }
        store_items.Add(12, new store_item("Lens & badge & chests bundle", false, 35000, 50, 7));
        store_items.Add(14, new store_item("50,000 coins", true, 599, 50000, 6));
        if (!Map.GameState.unlimitedLives)
        { 
            if (!Map.GameState.noAds)
            {
                store_items.Add(13, new store_item("ultimate bundle", true, 1699, 1, 8));
            }
            else
            {
                store_items.Add(13, new store_item("ultimate bundle", true, 1399, 1, 8));
            }
        }
    }

    public void buy_item(int item)
    {
        print("buying stuff");
        store_item selected_item = store_items[item];
        if (!selected_item.real_currency)
        {
            if (Map.GameState.coins >= selected_item.price)
            {
                Map.GameState.coins -= selected_item.price;
                update_coins();
            }
            else
            {
                return;
            }
        }
        else
        {       // real currency exchange
            if (!true) { }
            else { return; }

        }

        switch (selected_item.store_icon_id)
        {
            case 0:
                Map.GameState.inquiries += selected_item.count;
                break;
            case 1:
                Map.GameState.lives += selected_item.count;
                break;
            case 2:
                Map.GameState.tresaures += selected_item.count;
                break;
            case 3:
                //  real money event
                Map.GameState.coins += selected_item.count;
                break;
            case 4:
                Map.GameState.noAds = true;
                break;
            case 5:
                Map.GameState.inquiries += selected_item.count;
                if (!Map.GameState.unlimitedLives)
                {
                    Map.GameState.lives += selected_item.count;
                }
                else
                {
                    Map.GameState.inquiries += selected_item.count;
                }
                break;
            case 6:
                // real money event
                Map.GameState.coins += selected_item.count;
                break;
            case 7:
                Map.GameState.inquiries += selected_item.count;
                if (!Map.GameState.unlimitedLives)
                {
                    Map.GameState.lives += selected_item.count;
                }
                else
                {
                    Map.GameState.inquiries += selected_item.count;
                }

                Map.GameState.tresaures += Mathf.FloorToInt(selected_item.count * 0.1f);

                break;
            case 8:
                Map.GameState.inquiries += 100;
                Map.GameState.coins += 100000;
                Map.GameState.lives += 0;
                Map.GameState.tresaures += 25;
                Map.GameState.noAds = true;
                Map.GameState.unlimitedLives = true;
                break;
        }
        loadsave.savegame(Map.GameState);
    }

    void set_list()
    {
        foreach (KeyValuePair<int, store_item> kvp in store_items)
        {
            GameObject list_item = Instantiate(list_item_prefab) as GameObject;
            list_item.transform.SetParent(menu.transform);
            list_item.transform.GetChild(0).GetComponent<RawImage>().texture = item_icons[kvp.Value.store_icon_id];
            list_item.transform.GetChild(1).GetComponent<Text>().text = kvp.Value.name;
            if (kvp.Value.store_icon_id == 4 || kvp.Value.store_icon_id == 8)
            {
                list_item.transform.GetChild(2).GetComponent<Text>().text = "";
            }
            else
            {
                list_item.transform.GetChild(2).GetComponent<Text>().text = "" + kvp.Value.count.ToString("#,#", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (kvp.Value.real_currency)
            {
                list_item.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "" + (kvp.Value.price * 0.01f).ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                list_item.transform.GetChild(3).GetChild(0).GetComponent<Text>().color = new Color32(148, 255, 13, 255);
                list_item.transform.GetChild(3).GetComponent<RawImage>().color = new Color32(17, 147, 65, 255);
                list_item.transform.GetChild(3).GetChild(1).GetComponent<RawImage>().texture = currency_type[1];
            }
            else
            {
                list_item.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "" + kvp.Value.price.ToString("#,#", System.Globalization.CultureInfo.InvariantCulture);
                list_item.transform.GetChild(3).GetChild(1).GetComponent<RawImage>().texture = currency_type[0];
            }
            list_item.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate () { buy_item(kvp.Key); });

        }
    }

    void update_coins()
    {       
        coin_text.text = Map.GameState.coins.ToString("#,#", System.Globalization.CultureInfo.InvariantCulture);
    }

}

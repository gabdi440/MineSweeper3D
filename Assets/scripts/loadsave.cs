using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
public static class loadsave
{
    [System.Serializable]
    public class gamestate
    {
        public int coins;
        public int lives;
        public int tresaures;
        // might change it for a dictio of toolquantity;
        public int inquiries;
        public Dictionary<int, bool> completedlevels;
        public bool noAds;
        public bool unlimitedLives;

        public gamestate()
        {
            coins = 250;
            lives = 10;
            tresaures = 1;
            inquiries = 3;
            completedlevels = new Dictionary<int, bool>();
            noAds = false;
            unlimitedLives = false;

        }
        public gamestate(gamestate gs)
        {
            coins = gs.coins;
            lives = gs.lives;
            tresaures = gs.tresaures;
            inquiries = gs.inquiries;
            completedlevels = gs.completedlevels;
            noAds = gs.noAds;
            unlimitedLives = gs.unlimitedLives;
        }

    }
    public static void savegame (gamestate game)
        {     
        BinaryFormatter formatter = new BinaryFormatter();
        string  path = Application.persistentDataPath + "/useful.dump";
        FileStream stream = new FileStream(path, FileMode.Create);

        gamestate data = new gamestate(game);

        formatter.Serialize(stream, data);
        stream.Close();
        }


    public static gamestate loadgame()
    {
        string path = Application.persistentDataPath + "/useful.dump";

        gamestate Gs =  new gamestate();
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Gs = formatter.Deserialize(stream) as gamestate;
            stream.Close();
            //if for some reasone the save exists and is fucked...
            if (Gs == null) { Gs = new gamestate(); }
        }


        return Gs;
    }
}

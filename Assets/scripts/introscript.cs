using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class introscript : MonoBehaviour
{
    float timer = 0;

    // Update is called once per frame
    void Start()
    {
        if (PlayerPrefs.GetInt("sizeX") < 1) { PlayerPrefs.SetInt("sizeX", 7); }
        if (PlayerPrefs.GetInt("sizeY") < 1) { PlayerPrefs.SetInt("sizeY", 7); }
        if (PlayerPrefs.GetInt("mines") < 1) { PlayerPrefs.SetInt("mines", 5); }
        if (PlayerPrefs.GetInt("obstacles") < 1) { PlayerPrefs.SetInt("obstacles", 5); }
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 3) { SceneManager.LoadScene(1); }
    }
}

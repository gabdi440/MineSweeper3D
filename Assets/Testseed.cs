using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testseed : MonoBehaviour
{
    [SerializeField] MapView view;
    void Start()
    {
        Seed.GetMapData("MichalLevel#0#3#3#0#0#0#0#0#1#0#0#102#5#");

        view.CreateMap("MichalLevel#0#3#3#3#0#0#0#0#1#0#0#102#5#", null);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

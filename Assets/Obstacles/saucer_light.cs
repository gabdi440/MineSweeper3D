using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saucer_light : MonoBehaviour
{
    float timer;
    void Start()
    {
        
        transform.localEulerAngles += new Vector3(0, Mathf.Floor((Random.Range(0,1200))/100) * 30, 0);

    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0.083333f)
        { 
        transform.localEulerAngles +=  new Vector3(0,30,0);
            timer = 0;
        }

    }
}

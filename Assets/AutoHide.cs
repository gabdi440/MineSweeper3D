using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField, Range(1,60)] int time;
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

}

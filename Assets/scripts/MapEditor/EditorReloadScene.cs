using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EditorReloadScene : MonoBehaviour
{
    [SerializeField] Button button;
    private void Awake()
    {
        button.onClick.AddListener(ReloadEditor);
    }
    void ReloadEditor() 
    {
        SceneManager.LoadScene(0);
    }
}

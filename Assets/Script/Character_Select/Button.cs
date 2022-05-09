using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public GameObject Select_Manager;

    public void OnClick()
    {   
        SceneManager.LoadScene("SampleScene");
        DontDestroyOnLoad(Select_Manager);
    }
}

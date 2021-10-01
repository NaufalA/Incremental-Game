using System;
using System.Collections;
using System.Collections.Generic;
using SaveGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    private void Start()
    {
        UserDataManager.Load();
        SceneManager.LoadScene(1);
    }
}

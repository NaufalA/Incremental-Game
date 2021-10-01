using System;
using System.Collections;
using System.Collections.Generic;
using SaveGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private Button localButton; 
    [SerializeField] private Button cloudButton; 
    
    private void Start()
    {
        localButton.onClick.AddListener((() =>
        {
            SetButtonInteractable(false);
            UserDataManager.LoadFromLocal();
            SceneManager.LoadScene(1);
        }));
        
        cloudButton.onClick.AddListener((() =>
        {
            SetButtonInteractable(false);
            StartCoroutine(UserDataManager.LoadFromCloud(() => SceneManager.LoadScene(1)));
        }));
    }
    
    private void SetButtonInteractable (bool interactable)

    {
        localButton.interactable = interactable;
        cloudButton.interactable = interactable;
    }
}

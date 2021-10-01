using System;
using System.Collections;
using System.Collections.Generic;
using SaveGame;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ResourceConfig
{
    public string name;
    public double unlockCost;
    public double upgradeCost;
    public double output;
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Range(0f, 1f)] public float autoCollectPercentage = 0.1f;
    public ResourceConfig[] resourceConfigs;

    public Transform resourceParent;
    public ResourceController resourcesPrefab;
    public TapText tapTextPrefab;

    public Transform coinIcon;
    public Text goldInfo;
    public Text goldSpentInfo;
    public Text autoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    public Sprite[] resourceSprites;
    private float _collectSecond;

    // Start is called before the first frame update
    private void Start()
    {
        AddAllResources();

        goldInfo.text = $"Gold : {UserDataManager.Progress.goldTotal:0}";
        goldSpentInfo.text = $"Gold Spent : {UserDataManager.Progress.goldSpent:0}";
    }

    // Update is called once per frame
    private void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }
        
        CheckResourceCost();

        coinIcon.transform.localScale = Vector3.LerpUnclamped(coinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
    }

    private void AddAllResources()
    {
        bool showResources = true;
        int index = 0;
        
        foreach (ResourceConfig config in resourceConfigs)
        {
            GameObject obj = Instantiate(resourcesPrefab.gameObject, resourceParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();
            
            resource.SetConfig(index, config);
            obj.gameObject.SetActive(showResources);

            if (showResources && !resource.isUnlocked)
            {
                showResources = false;
            }
            
            _activeResources.Add(resource);
            index++;
        }
    }

    public void ShowNextResource()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            bool isBuyable;
            if (resource.isUnlocked)
            {
                isBuyable = UserDataManager.Progress.goldTotal >= resource.GetUpgradeCost();
            }
            else
            {
                isBuyable = UserDataManager.Progress.goldTotal >= resource.GetUnlockCost();
            }
            resource.resourceImage.sprite = resourceSprites[isBuyable ? 1 : 0];
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;

        foreach (ResourceController resource in _activeResources)
        {
            if (resource.isUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        output *= autoCollectPercentage;
        autoCollectInfo.text = $"Auto Collect : {output:F1}/second";
        
        AddGold(output);
    }

    public void AddGold(double value)
    {
        UserDataManager.Progress.goldTotal += value;
        goldInfo.text = $"Gold : {UserDataManager.Progress.goldTotal:0}";
        
        AchievementController.Instance.CheckAchievement(AchievementType.AccumulateGold, UserDataManager.Progress.goldTotal);
        
        UserDataManager.Save();
    }

    public void AddSpend(double value)
    {
        UserDataManager.Progress.goldSpent += value;
        goldSpentInfo.text = $"Gold Spent : {UserDataManager.Progress.goldSpent:0}";
        
        AchievementController.Instance.CheckAchievement(AchievementType.SpendGold, UserDataManager.Progress.goldSpent);
        
        UserDataManager.Save();
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.isUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent,false);
        tapText.transform.position = tapPosition;

        tapText.textComponent.text = $"+{output.ToString("0")}";
        tapText.gameObject.SetActive(true);
        coinIcon.transform.localScale = Vector3.one*1.75f;
        
        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);

        if (tapText == null)
        {
            tapText = Instantiate(tapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }

        return tapText;
    }
}

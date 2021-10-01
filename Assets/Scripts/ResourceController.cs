using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using SaveGame;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button resourceButton;
    public Image resourceImage;

    public Text resourceDescription;
    public Text resourceUpgradeCost;
    public Text resourceUnlockCost;

    private ResourceConfig _config;

    public bool isUnlocked = false;
    
    private int _index;
    public int Level
    {
        set
        {
            UserDataManager.Progress.resourceLevels[_index] = value;
            UserDataManager.Save();
        }
        get
        {
            if (UserDataManager.HasResources(_index))
            {
                return UserDataManager.Progress.resourceLevels[_index];
            }
            return 1;
        }
    }

    private void Start()
    {
        resourceButton.onClick.AddListener(() =>
        {
            if (isUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }

    public void SetConfig(int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        resourceDescription.text = $"{_config.name} Lv.{Level}\n+{GetOutput().ToString("0")}";
        resourceUnlockCost.text = $"Unlock Cost\n{GetUnlockCost()}";
        resourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        
        SetUnlocked(config.unlockCost == 0 || UserDataManager.HasResources(index));
    }

    public double GetOutput()
    {
        return _config.output * Level;
    }

    public double GetUpgradeCost()
    {
        return _config.upgradeCost * Level;
    }

    public double GetUnlockCost()
    {
        return _config.unlockCost;
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();

        if (UserDataManager.Progress.gold < unlockCost)
        {
            return;
        }
        
        GameManager.Instance.AddGold(-unlockCost);
        GameManager.Instance.AddSpend(unlockCost);
        
        SetUnlocked(true);
        
        AchievementController.Instance.CheckAchievement(AchievementType.UnlockResource, _config.name);
        GameManager.Instance.ShowNextResource();
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        if (unlocked)
        {
            if (!UserDataManager.HasResources(_index))
            {
                UserDataManager.Progress.resourceLevels.Add(Level);
                UserDataManager.Save();
            }
        }
        
        resourceImage.color = isUnlocked ? Color.white : Color.grey;
        resourceUnlockCost.gameObject.SetActive(!unlocked);
        resourceUpgradeCost.gameObject.SetActive(unlocked);
    }
    
    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();

        if (UserDataManager.Progress.gold < upgradeCost)
        {
            return;
        }
        
        GameManager.Instance.AddGold(-upgradeCost);
        GameManager.Instance.AddSpend(upgradeCost);
        Level++;

        resourceUpgradeCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
        resourceDescription.text = $"{_config.name} Lv.{Level}\n+{GetOutput().ToString("0")}";
    }
}

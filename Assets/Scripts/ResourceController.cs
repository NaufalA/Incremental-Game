using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Text resourceDescription;
    public Text resourceUpgradeCost;
    public Text resourceUnlockCost;

    private ResourceConfig _config;

    private int _level = 1;

    public void SetConfig(ResourceConfig config)
    {
        _config = config;

        resourceDescription.text = $"{_config.name} Lv.{_level}\n+{GetOutput().ToString("0")}";
        resourceUnlockCost.text = $"Unlock Cost\n{GetUnlockCost()}";
        resourceUnlockCost.text = $"Upgrade Cost\n{GetUpgradeCost()}";
    }

    public double GetOutput()
    {
        return _config.output * _level;
    }

    public double GetUpgradeCost()
    {
        return _config.upgradeCost * _level;
    }

    public double GetUnlockCost()
    {
        return _config.unlockCost;
    }
}

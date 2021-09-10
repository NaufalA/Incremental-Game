using System.Collections;
using System.Collections.Generic;
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

    public static GameManager instance
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

    public Text goldInfo;
    public Text autoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private float _collectSecond;

    private double _totalGold;

    // Start is called before the first frame update
    void Start()
    {
        AddAllResources();
    }

    // Update is called once per frame
    void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }
    }

    private void AddAllResources()
    {
        foreach (ResourceConfig config in resourceConfigs)
        {
            GameObject obj = Instantiate(resourcesPrefab.gameObject, resourceParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();
            
            resource.SetConfig(config);
            _activeResources.Add(resource);
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;

        foreach (ResourceController resource in _activeResources)
        {
            output += resource.GetOutput();
        }

        output *= autoCollectPercentage;
        autoCollectInfo.text = $"Auto Collect : {output.ToString("F1")}/second";
        
        AddGold(output);
    }

    private void AddGold(double value)
    {
        _totalGold += value;
        goldInfo.text = $"Gold : {_totalGold.ToString("0")}";
    }
}

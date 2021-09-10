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
    public TapText tapTextPrefab;

    public Transform coinIcon;
    public Text goldInfo;
    public Text autoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    public Sprite[] resourceSprites;
    private float _collectSecond;

    private double _totalGold;
    public double TotalGold => _totalGold;

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
        
        CheckResourceCost();

        coinIcon.transform.localScale = Vector3.LerpUnclamped(coinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
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

    public void AddGold(double value)
    {
        _totalGold += value;
        goldInfo.text = $"Gold : {_totalGold.ToString("0")}";
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            output += resource.GetOutput();
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

    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            bool isBuyable = TotalGold >= resource.GetUpgradeCost();
            resource.resourceImage.sprite = resourceSprites[isBuyable ? 1 : 0];
        }
    }
}

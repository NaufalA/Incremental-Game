using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AchievementData
{
    public string title;
    public AchievementType type;
    public string value;
    public bool isUnlocked;
}

public enum AchievementType
{
    UnlockResource
}

public class AchievementController : MonoBehaviour
{
    private static AchievementController _instance = null;

    public static AchievementController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AchievementController>();
            }

            return _instance;
        }
    }

    [SerializeField] private Transform popUpTransform;
    [SerializeField] private Text popUpText;
    [SerializeField] private float popUpDuration = 3f;

    [SerializeField] private List<AchievementData> achievementList;

    private float _popUpDurationCounter;

    // Update is called once per frame
    private void Update()
    {
        if (_popUpDurationCounter > 0)
        {
            _popUpDurationCounter -= Time.unscaledDeltaTime;
            popUpTransform.localScale = Vector3.LerpUnclamped(popUpTransform.localScale, Vector3.one, 0.5f);
        }
        else
        {
            popUpTransform.localScale = Vector2.LerpUnclamped(popUpTransform.localScale, Vector3.right, 0.5f);
        }
    }

    public void UnlockAchievement(AchievementType type, string value)
    {
        AchievementData achievementData = achievementList.Find(a => a.type == type && a.value == value);

        if (achievementData != null && !achievementData.isUnlocked)
        {
            achievementData.isUnlocked = true;
            ShowAchievementPopUp(achievementData);
        }
    }

    private void ShowAchievementPopUp(AchievementData achievementData)
    {
        popUpText.text = achievementData.title;
        _popUpDurationCounter = popUpDuration;
        popUpTransform.localScale = Vector2.right;

    }
}
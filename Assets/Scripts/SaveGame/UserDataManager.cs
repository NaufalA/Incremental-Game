using System;
using UnityEngine;

namespace SaveGame
{
    public class UserDataManager
    {
        private const string PROGRESS_KEY = "Progress";

        public static UserProgressData Progress;

        public static void Load()
        {
            if (!PlayerPrefs.HasKey(PROGRESS_KEY))
            {
                Progress = new UserProgressData();
                Save();
            }
            else
            {
                string json = PlayerPrefs.GetString(PROGRESS_KEY);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
            }
        }

        public static void Save()
        {
            string json = JsonUtility.ToJson(Progress);
            PlayerPrefs.SetString(PROGRESS_KEY, json);
        }

        public static bool HasResources(int index)
        {
            return index + 1 <= Progress.resourceLevels.Count;
        }
    }
}
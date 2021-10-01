using System;
using System.Collections;
using System.Text;
using Firebase.Storage;
using UnityEngine;

namespace SaveGame
{
    public class UserDataManager
    {
        private const string PROGRESS_KEY = "Progress";

        public static UserProgressData Progress;

        public static void LoadFromLocal()
        {
            if (!PlayerPrefs.HasKey(PROGRESS_KEY))
            {
                Progress = new UserProgressData();
                Save(true);
            }
            else
            {
                string json = PlayerPrefs.GetString(PROGRESS_KEY);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
            }
        }

        public static IEnumerator LoadFromCloud(System.Action onComplete)
        {
            StorageReference targetStorage = GetTargetCloudStorage();

            bool isCompleted = false;
            bool isSuccessful = false;

            const long maxAllowedSize = 1024 * 1024;

            targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith(
                task =>
                {
                    if (!task.IsFaulted)
                    {
                        string json = Encoding.Default.GetString(task.Result);
                        Progress = JsonUtility.FromJson<UserProgressData>(json);
                        isSuccessful = true;
                    }

                    isCompleted = true;
                });

            while (!isCompleted)
            {
                yield return null;
            }

            if (isSuccessful)
            {
                Save();
            }
            else
            {
                LoadFromLocal();
            }
            
            onComplete?.Invoke();
        }

        public static void Save(bool saveToCloud = false)
        {
            string json = JsonUtility.ToJson(Progress);
            PlayerPrefs.SetString(PROGRESS_KEY, json);

            if (saveToCloud)
            {
                AnalyticsManager.SetUserProperties ("gold", Progress.goldTotal.ToString());

                byte[] data = Encoding.Default.GetBytes(json);
                StorageReference targetStorage = GetTargetCloudStorage();

                targetStorage.PutBytesAsync(data);

            }
        }

        public static bool HasResources(int index)
        {
            return index + 1 <= Progress.resourceLevels.Count;
        }

        private static StorageReference GetTargetCloudStorage()
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;

            return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceId}");
        }
    }
}
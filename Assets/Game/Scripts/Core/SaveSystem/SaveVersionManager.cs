using System;
using System.Collections.Generic;
using GamePush;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem
{
    public static class SaveVersionManager
    {
        private const string VERSION_KEY = "save_version_info";

        private static Dictionary<string, int> s_currentVersions = new Dictionary<string, int>
        {
            { "game_state", SaveVersion.GAME_STATE_CURRENT_VERSION }
        };

        public static void CheckAndUpgradeAllSaves()
        {
            var versionInfo = LoadVersionInfo();
            bool needsUpgrade = false;

            foreach (var kvp in s_currentVersions)
            {
                string saveKey = kvp.Key;
                int currentVersion = kvp.Value;
                
                if (!versionInfo.TryGetValue(saveKey, out int savedVersion) || savedVersion < currentVersion)
                {
                    Debug.Log($"Save '{saveKey}' needs upgrade from version {savedVersion} to {currentVersion}");
                    needsUpgrade = true;
                }
            }

            if (needsUpgrade)
            {
                SaveVersionInfo(s_currentVersions);
            }
        }

        public static bool NeedsMigration(string saveKey, int currentVersion)
        {
            var versionInfo = LoadVersionInfo();
            
            if (!versionInfo.TryGetValue(saveKey, out int savedVersion))
            {
                return true;
            }

            return savedVersion < currentVersion;
        }

        public static void MarkSaveAsUpgraded(string saveKey, int version)
        {
            var versionInfo = LoadVersionInfo();
            versionInfo[saveKey] = version;
            SaveVersionInfo(versionInfo);
        }

        public static void BackupAllSaves()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                
                foreach (var saveKey in s_currentVersions.Keys)
                {
                    string data = GP_Player.GetString(saveKey);
                    if (!string.IsNullOrEmpty(data))
                    {
                        var backupKey = $"{saveKey}_backup_{timestamp}";
                        GP_Player.Set(backupKey, data);
                    }
                }
                
                GP_Player.Sync();
                Debug.Log($"Backup completed with timestamp: {timestamp}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void RestoreFromBackup(string timestamp)
        {
            try
            {
                foreach (var saveKey in s_currentVersions.Keys)
                {
                    var backupKey = $"{saveKey}_backup_{timestamp}";
                    string backupData = GP_Player.GetString(backupKey);
                    
                    if (!string.IsNullOrEmpty(backupData))
                    {
                        GP_Player.Set(saveKey, backupData);
                    }
                }
                
                GP_Player.Sync();
                Debug.Log($"Restore completed from backup: {timestamp}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static Dictionary<string, int> LoadVersionInfo()
        {
            try
            {
                string json = GP_Player.GetString(VERSION_KEY);
                if (string.IsNullOrEmpty(json))
                    return new Dictionary<string, int>();

                var versionData = JsonUtility.FromJson<SaveVersionData>(json);
                var result = new Dictionary<string, int>();

                for (int i = 0; i < versionData.Keys.Length; i++)
                {
                    result[versionData.Keys[i]] = versionData.Versions[i];
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new Dictionary<string, int>();
            }
        }

        private static void SaveVersionInfo(Dictionary<string, int> versionInfo)
        {
            try
            {
                var keys = new string[versionInfo.Count];
                var versions = new int[versionInfo.Count];
                
                int index = 0;
                foreach (var kvp in versionInfo)
                {
                    keys[index] = kvp.Key;
                    versions[index] = kvp.Value;
                    index++;
                }

                var versionData = new SaveVersionData
                {
                    Keys = keys,
                    Versions = versions
                };

                string json = JsonUtility.ToJson(versionData);
                GP_Player.Set(VERSION_KEY, json);
                GP_Player.Sync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        [Serializable]
        private struct SaveVersionData
        {
            public string[] Keys;
            public int[] Versions;
        }
    }
}
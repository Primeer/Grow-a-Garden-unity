using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core.SaveSystem
{
    [Serializable]
    public struct VersionedSaveData
    {
        public int Version;
        public string JsonData;
    }

    public interface IVersionedSaver
    {
        int CurrentVersion { get; }
        string MigrateFromVersion(string jsonData, int fromVersion, int toVersion);
        string SerializeData(object data);
        object DeserializeData(string jsonData, int version);
    }

    public abstract class VersionedSaverBase<T> : IVersionedSaver
    {
        public abstract int CurrentVersion { get; }

        protected readonly Dictionary<int, Func<string, string>> m_migrators = new Dictionary<int, Func<string, string>>();

        protected VersionedSaverBase()
        {
            RegisterMigrators();
        }

        protected abstract void RegisterMigrators();

        protected void RegisterMigrator(int fromVersion, Func<string, string> migrator)
        {
            m_migrators[fromVersion] = migrator;
        }

        public string MigrateFromVersion(string jsonData, int fromVersion, int toVersion)
        {
            string currentData = jsonData;

            for (int version = fromVersion; version < toVersion; version++)
            {
                if (m_migrators.TryGetValue(version, out var migrator))
                {
                    currentData = migrator(currentData);
                }
                else
                {
                    Debug.LogWarning($"No migrator found for version {version} to {version + 1} in {GetType().Name}");
                }
            }

            return currentData;
        }

        public virtual string SerializeData(object data)
        {
            return JsonUtility.ToJson(data);
        }

        public virtual object DeserializeData(string jsonData, int version)
        {
            return JsonUtility.FromJson<T>(jsonData);
        }

        protected string WrapInVersionedData(string jsonData, int version)
        {
            var versionedData = new VersionedSaveData
            {
                Version = version,
                JsonData = jsonData
            };
            return JsonUtility.ToJson(versionedData);
        }

        protected (string jsonData, int version) UnwrapVersionedData(string rawData)
        {
            try
            {
                var versionedData = JsonUtility.FromJson<VersionedSaveData>(rawData);
                return (versionedData.JsonData, versionedData.Version);
            }
            catch
            {
                return (rawData, 1);
            }
        }

        protected string LoadAndMigrate(string saveKey, Func<string, string> loadFunction)
        {
            string rawData = loadFunction(saveKey);
            
            if (string.IsNullOrEmpty(rawData))
                return rawData;

            var (jsonData, saveVersion) = UnwrapVersionedData(rawData);

            if (saveVersion < CurrentVersion)
            {
                try
                {
                    jsonData = MigrateFromVersion(jsonData, saveVersion, CurrentVersion);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarning($"Migration failed for {GetType().Name}, using default values");
                    return null;
                }
            }

            return jsonData;
        }

        protected void SaveWithVersion(string saveKey, object data, Action<string, string> saveFunction)
        {
            string jsonData = SerializeData(data);
            string wrappedData = WrapInVersionedData(jsonData, CurrentVersion);
            saveFunction(saveKey, wrappedData);
        }
    }
}
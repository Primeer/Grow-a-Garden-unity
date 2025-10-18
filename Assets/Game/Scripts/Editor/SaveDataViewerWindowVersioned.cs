using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.Plants;
using Game.Scripts.Core.SaveSystem;
using Game.Scripts.Core.SaveSystem.Plants;
using GamePush;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Editor
{
    public class SaveDataViewerWindowVersioned : EditorWindow
    {
        private Vector2 m_scrollPosition;
        private bool m_showPlants = true;
        private bool m_showMoney = true;
        private bool m_showInventory = true;
        private bool m_showVersionInfo = true;
        private bool m_showRawData = false;
        
        private PlantsSaveData m_plantsPlantsSaveData;
        private int m_moneyValue;
        private InventorySaver.Data m_inventoryData;
        private PlantsConfig m_plantsConfig;
        
        // Version info
        private Dictionary<string, (int currentVersion, int saveVersion, bool needsMigration)> m_versionInfo;
        private string m_rawPlantsData = "";
        private string m_rawInventoryData = "";
        private string m_rawMoneyData = "";
        
        private string m_statusMessage = "";
        private MessageType m_messageType = MessageType.Info;

        [MenuItem("Tools/Save Data Viewer (Versioned)")]
        public static void ShowWindow()
        {
            GetWindow<SaveDataViewerWindowVersioned>("Save Data Viewer").Show();
        }

        private void OnEnable()
        {
            LoadPlantsConfig();
            RefreshData();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            DrawHeader();
            EditorGUILayout.EndVertical();

            if (!string.IsNullOrEmpty(m_statusMessage))
            {
                EditorGUILayout.HelpBox(m_statusMessage, m_messageType);
            }

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            if (m_showVersionInfo)
            {
                DrawVersionInfoSection();
            }

            if (m_showMoney)
            {
                DrawMoneySection();
            }

            if (m_showInventory)
            {
                DrawInventorySection();
            }

            if (m_showPlants)
            {
                DrawPlantsSection();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Save Data Viewer (With Versioning)", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Refresh Data", GUILayout.Width(100)))
            {
                LoadPlantsConfig();
                RefreshData();
            }

            if (GUILayout.Button("Clear All Saves", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Clear All Saves", 
                    "Are you sure you want to clear all save data? This action cannot be undone.", 
                    "Clear", "Cancel"))
                {
                    ClearAllSaves();
                }
            }
            
            if (GUILayout.Button("Backup Saves", GUILayout.Width(100)))
            {
                SaveVersionManager.BackupAllSaves();
                ShowStatus("Backup created", MessageType.Info);
            }

            GUILayout.FlexibleSpace();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // View options
            EditorGUILayout.BeginHorizontal();
            m_showRawData = EditorGUILayout.Toggle("Show Raw Data", m_showRawData, GUILayout.Width(120));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            // Fold toggles
            EditorGUILayout.BeginHorizontal();
            m_showVersionInfo = EditorGUILayout.Foldout(m_showVersionInfo, "Version Info", true);
            m_showMoney = EditorGUILayout.Foldout(m_showMoney, "Money", true);
            m_showInventory = EditorGUILayout.Foldout(m_showInventory, "Inventory", true);
            m_showPlants = EditorGUILayout.Foldout(m_showPlants, "Plants", true);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawVersionInfoSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Save Version Information", EditorStyles.boldLabel);
            
            if (m_versionInfo == null || m_versionInfo.Count == 0)
            {
                EditorGUILayout.LabelField("No version information available");
            }
            else
            {
                foreach (var kvp in m_versionInfo)
                {
                    string saveKey = kvp.Key;
                    var versionData = kvp.Value;
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{saveKey.ToUpper()}:", EditorStyles.boldLabel, GUILayout.Width(80));
                    
                    if (versionData.needsMigration)
                    {
                        var style = new GUIStyle(EditorStyles.label);
                        style.normal.textColor = Color.yellow;
                        EditorGUILayout.LabelField($"v{versionData.saveVersion} → v{versionData.currentVersion}", style, GUILayout.Width(80));
                        EditorGUILayout.LabelField("NEEDS MIGRATION", style);
                    }
                    else
                    {
                        var style = new GUIStyle(EditorStyles.label);
                        style.normal.textColor = Color.green;
                        EditorGUILayout.LabelField($"v{versionData.currentVersion}", style, GUILayout.Width(80));
                        EditorGUILayout.LabelField("UP TO DATE", style);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.Space();
            
            if (m_showRawData)
            {
                DrawRawDataSection();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawRawDataSection()
        {
            EditorGUILayout.LabelField("Raw Save Data", EditorStyles.boldLabel);
            
            // Money raw data
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Money (Raw):", EditorStyles.miniLabel);
            EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(m_rawMoneyData) ? "(empty)" : m_rawMoneyData, 
                EditorStyles.textArea, GUILayout.Height(20));
            EditorGUILayout.EndVertical();
            
            // Inventory raw data
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Inventory (Raw):", EditorStyles.miniLabel);
            EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(m_rawInventoryData) ? "(empty)" : 
                TruncateText(m_rawInventoryData, 200), EditorStyles.textArea, GUILayout.Height(40));
            EditorGUILayout.EndVertical();
            
            // Plants raw data
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Plants (Raw):", EditorStyles.miniLabel);
            EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(m_rawPlantsData) ? "(empty)" : 
                TruncateText(m_rawPlantsData, 200), EditorStyles.textArea, GUILayout.Height(40));
            EditorGUILayout.EndVertical();
        }

        private void DrawMoneySection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Money System", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Money:", GUILayout.Width(120));
            
            int newMoney = EditorGUILayout.IntField(m_moneyValue);
            if (newMoney != m_moneyValue)
            {
                m_moneyValue = newMoney;
                SetSaveValue("money", m_moneyValue);
                ShowStatus("Money updated successfully", MessageType.Info);
            }
            
            if (GUILayout.Button("Reset", GUILayout.Width(60)))
            {
                m_moneyValue = 0;
                SetSaveValue("money", m_moneyValue);
                ShowStatus("Money reset to 0", MessageType.Info);
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawInventorySection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Inventory System", EditorStyles.boldLabel);
            
            if (m_inventoryData.Seeds != null && m_inventoryData.Seeds.Length > 0)
            {
                EditorGUILayout.LabelField($"Seeds ({m_inventoryData.Seeds.Length}):");
                EditorGUI.indentLevel++;
                foreach (int seedId in m_inventoryData.Seeds)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Seed ID: {seedId}", GUILayout.Width(100));
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        RemoveSeedFromInventory(seedId);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("No seeds in inventory");
            }
            
            EditorGUILayout.Space();
            
            if (m_inventoryData.Plants != null && m_inventoryData.Plants.Length > 0)
            {
                EditorGUILayout.LabelField($"Plants ({m_inventoryData.Plants.Length}):");
                EditorGUI.indentLevel++;
                foreach (var plant in m_inventoryData.Plants)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Plant ID: {plant.Id}, Weight: {plant.WeightFactor:F2}", GUILayout.Width(200));
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        RemovePlantFromInventory(plant.Id, plant.WeightFactor);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("No plants in inventory");
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Seeds", GUILayout.Width(100)))
            {
                ClearInventorySeeds();
            }
            if (GUILayout.Button("Clear Plants", GUILayout.Width(100)))
            {
                ClearInventoryPlants();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        private void DrawPlantsSection()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Plants System", EditorStyles.boldLabel);
            
            if (m_plantsPlantsSaveData.Plants != null && m_plantsPlantsSaveData.Plants.Count > 0)
            {
                EditorGUILayout.LabelField($"Total Plants: {m_plantsPlantsSaveData.Plants.Count}");
                
                if (m_plantsPlantsSaveData.SaveTimeTicks > 0)
                {
                    var saveTime = new DateTime(m_plantsPlantsSaveData.SaveTimeTicks);
                    var timeSinceSave = DateTime.Now - saveTime;
                    EditorGUILayout.LabelField($"Last Save: {saveTime:yyyy-MM-dd HH:mm:ss}");
                    EditorGUILayout.LabelField($"Time Since Save: {FormatTimeSinceSave(timeSinceSave)}");
                    
                    // Validation of save time after versioning
                    if (IsValidSaveTime(saveTime))
                    {
                        var validStyle = new GUIStyle(EditorStyles.label);
                        validStyle.normal.textColor = Color.green;
                        EditorGUILayout.LabelField("✓ Save time is valid", validStyle);
                    }
                    else
                    {
                        var invalidStyle = new GUIStyle(EditorStyles.label);
                        invalidStyle.normal.textColor = Color.red;
                        EditorGUILayout.LabelField("⚠ Save time may be corrupted after migration", invalidStyle);
                    }
                }
                
                EditorGUILayout.Space();
                
                EditorGUI.indentLevel++;
                for (int i = 0; i < m_plantsPlantsSaveData.Plants.Count; i++)
                {
                    var plant = m_plantsPlantsSaveData.Plants[i];
                    DrawPlantData(plant, i);
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("No plants saved");
            }
            
            if (GUILayout.Button("Clear All Plants"))
            {
                if (EditorUtility.DisplayDialog("Clear Plants", 
                    "Are you sure you want to clear all plant save data?", 
                    "Clear", "Cancel"))
                {
                    ClearPlantsData();
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawPlantData(PlantSaveData plantSave, int index)
        {
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Plant {index + 1}", EditorStyles.boldLabel, GUILayout.Width(80));
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                RemovePlantData(index);
                return;
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField($"ID: {plantSave.PlantId}");
            EditorGUILayout.LabelField($"Position: {plantSave.TransformData.Position}");
            
            // Enhanced growth information with step ratios
            var growthInfo = GetPlantGrowthInfo(plantSave);
            EditorGUILayout.LabelField($"Growth Step: {plantSave.Step}/{growthInfo.maxSteps}");
            EditorGUILayout.LabelField($"Timer: {plantSave.Timer:F2}s/{growthInfo.stepTime:F2}s");
            
            bool isCompleted = plantSave.Step >= growthInfo.maxSteps;
            EditorGUILayout.LabelField($"Growth Completed: {isCompleted}");
            
            // Progress bars for visual feedback
            if (!isCompleted)
            {
                // Step progress bar
                var stepRect = GUILayoutUtility.GetRect(0, 16, GUILayout.ExpandWidth(true));
                var stepProgress = growthInfo.maxSteps > 0 ? (float)plantSave.Step / growthInfo.maxSteps : 0f;
                EditorGUI.ProgressBar(stepRect, stepProgress, $"Step Progress: {plantSave.Step}/{growthInfo.maxSteps}");
                
                // Timer progress bar
                var timerRect = GUILayoutUtility.GetRect(0, 16, GUILayout.ExpandWidth(true));
                var timerProgress = growthInfo.stepTime > 0 ? plantSave.Timer / growthInfo.stepTime : 0f;
                EditorGUI.ProgressBar(timerRect, timerProgress, $"Timer: {plantSave.Timer:F1}s/{growthInfo.stepTime:F1}s");
            }
            
            if (plantSave.Children != null && plantSave.Children.Length > 0)
            {
                EditorGUILayout.LabelField($"Children: {plantSave.Children.Length}", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                
                for (int i = 0; i < plantSave.Children.Length; i++)
                {
                    var child = plantSave.Children[i];
                    DrawChildStatus(child, i);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawChildStatus(PlantChildSaveData plantChildSave, int index)
        {
            EditorGUILayout.BeginVertical("box");
            
            // Header with child index and main status
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Child Spawner {index + 1}", EditorStyles.miniLabel, GUILayout.Width(100));
            
            // Status badge with color coding
            GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
            string statusText = "";
            
            if (plantChildSave.HasChild)
            {
                statusStyle.normal.textColor = Color.green;
                statusText = "HAS CHILD";
            }
            else
            {
                statusStyle.normal.textColor = Color.yellow;
                statusText = "WAITING";
            }
            
            EditorGUILayout.LabelField(statusText, statusStyle, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            
            // Detailed information
            EditorGUI.indentLevel++;
            
            if (plantChildSave.HasChild)
            {
                EditorGUILayout.LabelField("• Current child plant is active");
                EditorGUILayout.LabelField("• Ready for harvest when player interacts");
                
                // Child plant growth information (now stored directly in ChildData)
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Child Plant Growth:", EditorStyles.boldLabel);
                
                // Use fallback values since we don't have PlantId in ChildData
                int maxSteps = 4; // Fallback
                float stepTime = 15f; // Fallback
                
                EditorGUILayout.LabelField($"• Growth Step: {plantChildSave.Step}/{maxSteps}");
                EditorGUILayout.LabelField($"• Timer: {plantChildSave.Timer:F2}s/{stepTime:F2}s");
                
                // Progress bars for child plant - use stored data from ChildData
                if (plantChildSave.Step < maxSteps)
                {
                    // Step progress bar
                    var stepRect = GUILayoutUtility.GetRect(0, 14, GUILayout.ExpandWidth(true));
                    var stepProgress = maxSteps > 0 ? (float)plantChildSave.Step / maxSteps : 0f;
                    EditorGUI.ProgressBar(stepRect, stepProgress, $"Child Step: {plantChildSave.Step}/{maxSteps}");
                    
                    // Timer progress bar
                    var timerRect = GUILayoutUtility.GetRect(0, 14, GUILayout.ExpandWidth(true));
                    var timerProgress = stepTime > 0 ? plantChildSave.Timer / stepTime : 0f;
                    EditorGUI.ProgressBar(timerRect, timerProgress, $"Child Timer: {plantChildSave.Timer:F1}s/{stepTime:F1}s");
                }
            }
            else
            {
                EditorGUILayout.LabelField($"• Time to spawn: {plantChildSave.TimeToSpawn:F1}s");
                
                // Progress bar for spawn timing if time is set
                if (plantChildSave.TimeToSpawn > 0)
                {
                    var rect = GUILayoutUtility.GetRect(0, 18, GUILayout.ExpandWidth(true));
                    // Since we don't have current time info, show the spawn delay value
                    EditorGUI.ProgressBar(rect, 0f, $"Spawn in: {plantChildSave.TimeToSpawn:F1}s");
                }
            }
            
            // Additional timing information
            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status:", EditorStyles.miniLabel, GUILayout.Width(50));
            string statusInfo = plantChildSave.HasChild ? 
                "Child ready for harvest" : 
                $"Waiting {plantChildSave.TimeToSpawn:F1}s to spawn";
            EditorGUILayout.LabelField(statusInfo, EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void RefreshVersionInfo()
        {
            m_versionInfo = new Dictionary<string, (int currentVersion, int saveVersion, bool needsMigration)>();
            
            var gameStateRaw = GetRawSaveValue("game_state");
            var (stateJson, stateVersion) = TryUnwrapVersionedData(gameStateRaw);
            m_versionInfo["game_state"] = (SaveVersion.GAME_STATE_CURRENT_VERSION, stateVersion, 
                stateVersion < SaveVersion.GAME_STATE_CURRENT_VERSION);
        }
        
        private (string jsonData, int version) TryUnwrapVersionedData(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
                return ("", 1);
                
            try
            {
                var versionedData = JsonUtility.FromJson<VersionedSaveData>(rawData);
                if (versionedData.Version > 0)
                {
                    return (versionedData.JsonData, versionedData.Version);
                }
            }
            catch
            {
                // Not versioned data, treat as legacy version 1
            }
            
            return (rawData, 1);
        }
        
        private bool IsValidSaveTime(DateTime saveTime)
        {
            // Check if save time is reasonable (not too far in past/future)
            var now = DateTime.Now;
            var maxDaysBack = 365; // 1 year
            var maxDaysForward = 7; // 1 week
            
            return saveTime >= now.AddDays(-maxDaysBack) && saveTime <= now.AddDays(maxDaysForward);
        }
        
        private string GetRawSaveValue(string key)
        {
            if (EditorApplication.isPlaying)
            {
                return GP_Player.GetString(key);
            }
            else
            {
                string gpKey = "xGP_" + key;
                return PlayerPrefs.GetString(gpKey, "");
            }
        }
        
        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;
                
            return text.Substring(0, maxLength) + "...";
        }

        private void LoadPlantsConfig()
        {
            try
            {
                m_plantsConfig = Resources.Load<PlantsConfig>("PlantsConfig");
                if (m_plantsConfig == null)
                {
                    ShowStatus("Warning: PlantsConfig not found in Resources. Using fallback values.", MessageType.Warning);
                }
            }
            catch (Exception e)
            {
                ShowStatus($"Error loading PlantsConfig: {e.Message}", MessageType.Error);
            }
        }
        
        private void RefreshData()
        {
            try
            {
                RefreshVersionInfo();
                
                // Load Money (simple int, no versioning)
                m_moneyValue = GetSaveValue<int>("money");
                m_rawMoneyData = m_moneyValue.ToString();
                
                // Load Inventory - handle versioned data properly
                m_rawInventoryData = GetRawSaveValue("inventory");
                if (!string.IsNullOrEmpty(m_rawInventoryData))
                {
                    var (unwrappedJson, version) = TryUnwrapVersionedData(m_rawInventoryData);
                    if (!string.IsNullOrEmpty(unwrappedJson))
                    {
                        try
                        {
                            m_inventoryData = JsonUtility.FromJson<InventorySaver.Data>(unwrappedJson);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to parse inventory data: {ex.Message}");
                            Debug.LogError($"Raw data: {m_rawInventoryData}");
                            Debug.LogError($"Unwrapped data: {unwrappedJson}");
                            m_inventoryData = new InventorySaver.Data();
                        }
                    }
                    else
                    {
                        m_inventoryData = new InventorySaver.Data();
                    }
                }
                else
                {
                    m_inventoryData = new InventorySaver.Data();
                }
                
                // Load Plants - handle versioned data properly
                m_rawPlantsData = GetRawSaveValue("plants");
                if (!string.IsNullOrEmpty(m_rawPlantsData))
                {
                    var (unwrappedJson, version) = TryUnwrapVersionedData(m_rawPlantsData);
                    if (!string.IsNullOrEmpty(unwrappedJson))
                    {
                        try
                        {
                            m_plantsPlantsSaveData = JsonUtility.FromJson<PlantsSaveData>(unwrappedJson);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to parse plants data: {ex.Message}");
                            Debug.LogError($"Raw data: {m_rawPlantsData}");
                            Debug.LogError($"Unwrapped data: {unwrappedJson}");
                            m_plantsPlantsSaveData = new PlantsSaveData { Plants = new List<PlantSaveData>() };
                        }
                    }
                    else
                    {
                        m_plantsPlantsSaveData = new PlantsSaveData { Plants = new List<PlantSaveData>() };
                    }
                }
                else
                {
                    m_plantsPlantsSaveData = new PlantsSaveData { Plants = new List<PlantSaveData>() };
                }
                
                Debug.Log($"Loaded plants: {m_plantsPlantsSaveData.Plants?.Count ?? 0}, inventory seeds: {m_inventoryData.Seeds?.Length ?? 0}, inventory plants: {m_inventoryData.Plants?.Length ?? 0}");
                
                ShowStatus($"Data refreshed. Plants: {m_plantsPlantsSaveData.Plants?.Count ?? 0}, Seeds: {m_inventoryData.Seeds?.Length ?? 0}, Inventory Plants: {m_inventoryData.Plants?.Length ?? 0}", MessageType.Info);
            }
            catch (Exception e)
            {
                ShowStatus($"Error refreshing data: {e.Message}", MessageType.Error);
                Debug.LogException(e);
            }
            
            Repaint();
        }
        
        private (int maxSteps, float stepTime) GetPlantGrowthInfo(PlantSaveData plantSaveData)
        {
            if (m_plantsConfig == null)
                return (4, 15f); // Fallback values
                
            try
            {
                var configData = m_plantsConfig.GetPlantData(plantSaveData.PlantId);
                if (configData?.Prefab?.Growth != null)
                {
                    var growth = configData.Prefab.Growth;
                    var stepTime = growth.StepDuration;
                    var maxSteps = growth.StepsCount;
                    
                    return (maxSteps, stepTime);
                }
            }
            catch (System.Exception)
            {
                // Plant ID not found or other error, use fallback
            }
            
            return (4, 15f); // Fallback values
        }

        private T GetSaveValue<T>(string key) where T : IConvertible
        {
            if (EditorApplication.isPlaying)
            {
                // В Play Mode используем GamePush API
                if (typeof(T) == typeof(int))
                    return (T)(object)GP_Player.GetInt(key);
                if (typeof(T) == typeof(string))
                    return (T)(object)GP_Player.GetString(key);
                if (typeof(T) == typeof(float))
                    return (T)(object)GP_Player.GetFloat(key);
            }
            else
            {
                // В Editor Mode используем PlayerPrefs напрямую
                string gpKey = "xGP_" + key;
                if (typeof(T) == typeof(int))
                    return (T)(object)PlayerPrefs.GetInt(gpKey, 0);
                if (typeof(T) == typeof(string))
                    return (T)(object)PlayerPrefs.GetString(gpKey, "");
                if (typeof(T) == typeof(float))
                    return (T)(object)PlayerPrefs.GetFloat(gpKey, 0f);
            }
            
            return default(T);
        }

        private void SetSaveValue<T>(string key, T value)
        {
            if (EditorApplication.isPlaying)
            {
                // В Play Mode используем GamePush API
                if (typeof(T) == typeof(int))
                    GP_Player.Set(key, (int)(object)value);
                else if (typeof(T) == typeof(string))
                    GP_Player.Set(key, (string)(object)value);
                else if (typeof(T) == typeof(float))
                    GP_Player.Set(key, (float)(object)value);
                else if (typeof(T) == typeof(bool))
                    GP_Player.Set(key, (bool)(object)value);
                GP_Player.Sync();
            }
            else
            {
                // В Editor Mode используем PlayerPrefs напрямую
                string gpKey = "xGP_" + key;
                if (typeof(T) == typeof(int))
                    PlayerPrefs.SetInt(gpKey, (int)(object)value);
                else if (typeof(T) == typeof(string))
                    PlayerPrefs.SetString(gpKey, (string)(object)value);
                else if (typeof(T) == typeof(float))
                    PlayerPrefs.SetFloat(gpKey, (float)(object)value);
                PlayerPrefs.Save();
            }
        }

        private void ClearAllSaves()
        {
            SetSaveValue("money", 0);
            SetSaveValue("game_state", "");
            SetSaveValue("tutorial_movement", false);
            
            RefreshData();
            ShowStatus("All save data cleared", MessageType.Info);
        }

        private void ClearPlantsData()
        {
            SetSaveValue("plants", "");
            RefreshData();
            ShowStatus("Plants data cleared", MessageType.Info);
        }

        private void ClearInventorySeeds()
        {
            m_inventoryData.Seeds = new int[0];
            SaveInventoryData();
            ShowStatus("Inventory seeds cleared", MessageType.Info);
        }

        private void ClearInventoryPlants()
        {
            m_inventoryData.Plants = new InventorySaver.PlantData[0];
            SaveInventoryData();
            ShowStatus("Inventory plants cleared", MessageType.Info);
        }

        private void RemoveSeedFromInventory(int seedId)
        {
            var seedsList = m_inventoryData.Seeds.ToList();
            seedsList.Remove(seedId);
            m_inventoryData.Seeds = seedsList.ToArray();
            SaveInventoryData();
            ShowStatus($"Seed {seedId} removed from inventory", MessageType.Info);
        }

        private void RemovePlantFromInventory(int plantId, float weight)
        {
            var plantsList = m_inventoryData.Plants.ToList();
            plantsList.RemoveAll(p => p.Id == plantId && Mathf.Approximately(p.WeightFactor, weight));
            m_inventoryData.Plants = plantsList.ToArray();
            SaveInventoryData();
            ShowStatus($"Plant {plantId} removed from inventory", MessageType.Info);
        }

        private void RemovePlantData(int index)
        {
            m_plantsPlantsSaveData.Plants.RemoveAt(index);
            SavePlantsData();
            ShowStatus($"Plant {index + 1} removed", MessageType.Info);
        }

        private void SaveInventoryData()
        {
            string json = JsonUtility.ToJson(m_inventoryData);
            SetSaveValue("inventory", json);
        }

        private void SavePlantsData()
        {
            string json = JsonUtility.ToJson(m_plantsPlantsSaveData);
            SetSaveValue("plants", json);
        }

        private string FormatTimeSinceSave(TimeSpan timeSpan)
        {
            int minutes = (int)timeSpan.TotalMinutes;
            int seconds = timeSpan.Seconds;
            
            return $"{minutes}m {seconds}s";
        }

        private void ShowStatus(string message, MessageType type)
        {
            m_statusMessage = message;
            m_messageType = type;
            
            // Clear status after 3 seconds
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        m_statusMessage = "";
                        Repaint();
                    };
                };
            };
        }
    }
}
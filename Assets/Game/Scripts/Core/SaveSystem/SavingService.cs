using System;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core.InventorySystem;
using Game.Scripts.Core.SaveSystem.Migrators;
using Game.Scripts.Core.SaveSystem.Plants;
using GamePush;
using ObservableCollections;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Scripts.Core.SaveSystem
{
    public class SavingService : VersionedSaverBase<SavingService.Data>, IStartable, IDisposable
    {
        private const string SAVE_KEY = "game_state";
        
        private readonly PlantsSaver m_plantsSaver;
        private readonly InventorySaver m_inventorySaver;
        private readonly InventoryRepository m_inventoryRepository;
        
        private CancellationTokenSource m_cts = new();
        private IDisposable m_disposable;
        private bool m_hasChanges;
        private bool m_isQuitting;

        public event Action GameStateLoaded;

        public SavingService(PlantsSaver plantsSaver, InventorySaver inventorySaver, InventoryRepository inventoryRepository)
        {
            m_plantsSaver = plantsSaver;
            m_inventorySaver = inventorySaver;
            m_inventoryRepository = inventoryRepository;
        }

        public override int CurrentVersion => SaveVersion.GAME_STATE_CURRENT_VERSION;

        protected override void RegisterMigrators()
        {
            RegisterMigrator(1, SaveMigrator1To2.GetMigrator);
        }

        public void Start()
        {
            LoadSafely();

            Application.quitting += OnApplicationQuitting;

            var d1 = Observable.Merge(
                    m_inventoryRepository.Seeds.ObserveCountChanged(),
                    m_inventoryRepository.Plants.ObserveCountChanged())
                .Subscribe(_ => SaveGameState());

            var d2 = PlayerContext.Instance.PlantsSpawned
                .Subscribe(_ => SaveGameState());

            m_disposable = Disposable.Combine(d1, d2);
            
            GameStateLoaded?.Invoke();
        }

        public void Dispose()
        {
            Application.quitting -= OnApplicationQuitting;

            if (!m_isQuitting && m_hasChanges)
            {
                SaveImmediately();
            }

            m_cts?.Cancel();
            m_cts?.Dispose();
            m_cts = null;
            
            m_disposable?.Dispose();
        }

        private void SaveGameState()
        {
            if (m_hasChanges)
                return;

            m_hasChanges = true;
            SaveAsync().Forget();
        }

        private async UniTaskVoid SaveAsync()
        {
            try
            {
                await UniTask.WaitForEndOfFrame(cancellationToken: m_cts.Token);
            }
            catch (OperationCanceledException)
            {
                m_hasChanges = false;
                return;
            }

            if (m_isQuitting)
            {
                m_hasChanges = false;
                return;
            }
            
            var data = CollectSaveData();
            
            PersistSaveData(data);
            
            PlayerContext.Instance.SaveData = data;
            
            m_hasChanges = false;
        }

        private void LoadSafely()
        {
            try
            {
                Load();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Load()
        {
            string json = LoadAndMigrate(SAVE_KEY, GP_Player.GetString);
            
            if (string.IsNullOrEmpty(json))
                return;
                
            var saveData = DeserializeSaveData(json);
            
            // m_plantsSaver.LoadSaveData(saveData.Plants);
            m_inventorySaver.LoadSaveData(saveData.Inventory);

            PlayerContext.Instance.SaveData = saveData;

            Debug.Log(GetSaveLog(saveData));
        }

        private Data DeserializeSaveData(string json)
        {
            return JsonUtility.FromJson<Data>(json);
        }

        private Data CollectSaveData()
        {
            return new Data
            {
                Plants = PlayerContext.Instance.PlantsSpawned.CurrentValue ? 
                    m_plantsSaver.GetSaveData() : 
                    PlayerContext.Instance.SaveData.HasValue ? 
                        PlayerContext.Instance.SaveData.Value.Plants : 
                        new PlantsSaveData(),
                
                Inventory = m_inventorySaver.GetSaveData(),
            };
        }

        private void OnApplicationQuitting()
        {
            m_isQuitting = true;
            SaveImmediately();
        }

        private void SaveImmediately()
        {
            var data = CollectSaveData();
            PersistSaveData(data);
            m_hasChanges = false;
        }

        private void PersistSaveData(Data data)
        {
            SaveWithVersion(SAVE_KEY, data, (key, wrappedData) =>
            {
                GP_Player.Set(key, wrappedData);
                GP_Player.Sync();
            });
        }

        private string GetSaveLog(Data saveData)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Inventory seeds: ");
            sb.AppendJoin(", ", saveData.Inventory.Seeds);
            sb.AppendLine();
            
            sb.Append("Inventory plants: ");
            sb.AppendJoin(", ", saveData.Inventory.Plants.Select(plant => plant.Id));
            sb.AppendLine();
            
            sb.Append("Plants: ");
            sb.AppendJoin(", ", saveData.Plants.Plants.Select(plant => plant.PlantId));
            
            return sb.ToString();
        }

        [Serializable]
        public struct Data
        {
            public PlantsSaveData Plants;
            public InventorySaver.Data Inventory;
        }
    }
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core.MoneySystem;
using GamePush;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.SaveSystem
{
    public class MoneySaver : IInitializable, IDisposable
    {
        private const string SAVE_KEY = "money";
        
        private readonly MoneyRepository m_moneyRepository;

        private IDisposable m_disposable;
        private CancellationTokenSource m_cts = new CancellationTokenSource();
        private bool m_hasChanges;

        public MoneySaver(MoneyRepository moneyRepository)
        {
            m_moneyRepository = moneyRepository;
        }

        public void Initialize()
        {
            LoadMoney();
            m_disposable = m_moneyRepository.Money.Skip(1).Subscribe(OnMoneyChanged);
        }

        public void Dispose()
        {
            m_cts?.Cancel();
            m_cts?.Dispose();
            m_cts = null;
            
            m_disposable?.Dispose();
        }

        private void LoadMoney()
        {
            m_moneyRepository.Money.Value = GP_Player.GetInt(SAVE_KEY);

#if UNITY_EDITOR
            if (m_moneyRepository.Money.Value == 0)
                m_moneyRepository.Money.Value = 1000;
#endif
        }

        private void OnMoneyChanged(int money)
        {
            if (m_hasChanges)
                return;
            
            m_hasChanges = true;
            SaveAsync().Forget();
        }

        private async UniTaskVoid SaveAsync()
        {
            await UniTask.WaitForEndOfFrame(m_cts.Token);
            
            int money = m_moneyRepository.Money.CurrentValue;
            GP_Player.Set(SAVE_KEY, money);
            GP_Player.Sync();
            
            m_hasChanges = false;
        }

    }
}
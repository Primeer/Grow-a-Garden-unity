using System;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.MoneySystem
{
    public class MoneyController : IInitializable, IDisposable
    {
        private readonly MoneyView m_moneyView;
        private readonly MoneyRepository m_moneyRepository;

        private IDisposable m_disposable;

        public MoneyController(MoneyView moneyView, MoneyRepository moneyRepository)
        {
            m_moneyView = moneyView;
            m_moneyRepository = moneyRepository;
        }

        public void Initialize()
        {
            m_moneyView.SetMoney(m_moneyRepository.Money.Value);

            m_disposable = m_moneyRepository.Money.Subscribe(OnMoneyChanged);
        }

        public void Dispose()
        {
            m_disposable.Dispose();
        }

        private void OnMoneyChanged(int money) => m_moneyView.SetMoney(money);
    }
}
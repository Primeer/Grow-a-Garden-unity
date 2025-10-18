using System;
using System.Linq;
using System.Text;
using Game.Scripts.Common.Utilities;
using Game.Scripts.Core.InventorySystem;
using ObservableCollections;
using R3;
using VContainer.Unity;

namespace Game.Scripts.Core.ShopSystem.Selling
{
    public class ShopSellButtonsController : IInitializable, IDisposable
    {
        private readonly ShopSellWindow m_shopSellWindow;
        private readonly InventoryRepository m_inventoryRepository;
        private readonly ShopSellModel m_shopModel;

        private IDisposable m_disposable;

        public ShopSellButtonsController(ShopSellWindow shopSellWindow, InventoryRepository inventoryRepository, ShopSellModel shopModel)
        {
            m_shopSellWindow = shopSellWindow;
            m_inventoryRepository = inventoryRepository;
            m_shopModel = shopModel;
        }

        public void Initialize()
        {
            m_shopSellWindow.Showed += OnShopSellShowed;
            m_shopSellWindow.Hided += OnShopSellHided;
        }

        public void Dispose()
        {
            m_shopSellWindow.Showed -= OnShopSellShowed;
            m_shopSellWindow.Hided -= OnShopSellHided;
        }

        private void OnShopSellShowed()
        {
            SetButtons();
            
            var d1 = m_inventoryRepository.Plants.ObserveAdd().Subscribe(_ => OnPlantsChanged());
            var d2 = m_inventoryRepository.Plants.ObserveRemove().Subscribe(_ => OnPlantsChanged());

            var d3 = m_shopModel.SelectedSlot.Subscribe(_ => OnSelectedSlotChanged());

            m_disposable = Disposable.Combine(d1, d2, d3);
        }

        private void OnShopSellHided()
        {
            m_disposable.Dispose();
        }

        private void SetButtons()
        {
            SetSellButton();
            SetSellAllButton();
        }

        private void SetSellButton()
        {
            var slot = m_shopModel.SelectedSlot.CurrentValue;

            bool isSlotSelected = slot != null;
            
            var sb = new StringBuilder(LocalizationUtils.GetLocalized("Sell selected", "Продать выбранное"));

            if (isSlotSelected)
            {
                var item = m_shopModel.Slots[slot];
                sb.Append($" +{StringsUtils.FormatMoney(item.Cost)}");
            }
            
            m_shopSellWindow.SellButton.Button.interactable = isSlotSelected;
            m_shopSellWindow.SellButton.SetText(sb.ToString());
        }

        private void SetSellAllButton()
        {
            bool hasItems = m_inventoryRepository.Plants.Count > 0;
            
            var sb = new StringBuilder(LocalizationUtils.GetLocalized("Sell all", "Продать всё"));
            
            if (hasItems)
            {
                int sum = m_inventoryRepository.Plants.Sum(plant => plant.Cost);
                sb.Append($" +{StringsUtils.FormatMoney(sum)}");
            }
            
            m_shopSellWindow.SellAllButton.Button.interactable = hasItems;
            m_shopSellWindow.SellAllButton.SetText(sb.ToString());
        }

        private void OnPlantsChanged()
        {
            SetSellAllButton();
        }

        private void OnSelectedSlotChanged()
        {
            SetSellButton();
        }
    }
}
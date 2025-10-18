using R3;

namespace Game.Scripts.Core.MoneySystem
{
    public class MoneyRepository
    {
        public ReactiveProperty<int> Money { get; } = new();
    }
}
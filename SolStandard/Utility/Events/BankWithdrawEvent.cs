using SolStandard.Entity.General;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class BankWithdrawEvent : IEvent
    {
        private readonly GameUnit actingUnit;
        private readonly int goldToDeposit;

        public BankWithdrawEvent(GameUnit actingUnit, int goldToDeposit)
        {
            this.actingUnit = actingUnit;
            this.goldToDeposit = goldToDeposit;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            Bank.Withdraw(actingUnit, goldToDeposit);
            Complete = true;
        }
    }
}
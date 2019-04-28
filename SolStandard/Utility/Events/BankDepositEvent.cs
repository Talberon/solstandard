using SolStandard.Entity.General;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events
{
    public class BankDepositEvent : IEvent
    {
        private readonly GameUnit actingUnit;
        private readonly int goldToDeposit;

        public BankDepositEvent(GameUnit actingUnit, int goldToDeposit)
        {
            this.actingUnit = actingUnit;
            this.goldToDeposit = goldToDeposit;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            Bank.Deposit(actingUnit, goldToDeposit);
            Complete = true;
        }
    }
}
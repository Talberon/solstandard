using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class MoraleBrokenStatus : StatusEffect
    {
        private readonly UnitStatistics debuffStats;

        public MoraleBrokenStatus(int turnDuration, GameUnit unitToDebuff) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.MoraleBroken,
                new Vector2(GameDriver.CellSize)),
            name: "Morale Broken!",
            description: "Commander is defeated; statuses reduced.",
            turnDuration: turnDuration,
            hasNotification: false
        )
        {
            debuffStats = new UnitStatistics(
                hp: 0,
                armor: 0,
                atk: unitToDebuff.Stats.BaseAtk / 2,
                ret: unitToDebuff.Stats.BaseRet / 2,
                luck: unitToDebuff.Stats.BaseLuck / 2,
                mv: unitToDebuff.Stats.BaseMv / 2,
                atkRange: new int[0]
            );
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.AtkModifier -= debuffStats.Atk;
            target.Stats.RetModifier -= debuffStats.Ret;
            target.Stats.LuckModifier -= debuffStats.Luck;
            target.Stats.MvModifier -= debuffStats.Mv;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.AtkModifier += debuffStats.Atk;
            target.Stats.RetModifier += debuffStats.Ret;
            target.Stats.LuckModifier += debuffStats.Luck;
            target.Stats.MvModifier += debuffStats.Mv;
        }
    }
}
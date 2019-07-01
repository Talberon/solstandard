using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Statuses
{
    public class FocusStatus : StatusEffect, ITurnProc
    {
        private const int InitialDuration = 100;
        private const string StatusName = "Focusing!";
        public int FocusPoints { get; private set; }

        public FocusStatus(int focusPoints) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Focus, GameDriver.CellSizeVector),
            name: StatusName,
            description: "Allows acting again at the end of your turn.",
            turnDuration: InitialDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            FocusPoints = focusPoints;
            UpdateTitle();
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }

        public void OnTurnStart()
        {
            //Do nothing
        }

        public void OnTurnEnd()
        {
            if (FocusPoints > 0)
            {
                if (StatusWasAppliedThisTurn)
                {
                    GlobalEventQueue.QueueSingleEvent(new EndTurnEvent(skipProcs: true));
                }
                else
                {
                    GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                    FocusPoints--;
                    UpdateTitle();
                }
            }
            else
            {
                TurnDuration = 0;
            }
        }

        private bool StatusWasAppliedThisTurn => TurnDuration == InitialDuration;

        private void UpdateTitle()
        {
            Name = StatusName + " <" + FocusPoints + "pt>";
        }
    }
}
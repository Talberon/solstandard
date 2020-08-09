using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Statuses.Duelist
{
    public class FocusStatus : StatusEffect, ITurnProc
    {
        private readonly bool actImmediately;
        private const int InitialDuration = 100;
        private const string StatusName = "Focusing!";
        public int FocusPoints { get; private set; }

        public FocusStatus(int focusPoints, bool actImmediately) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Focus, GameDriver.CellSizeVector),
            name: StatusName,
            description: "Allows acting again at the end of your turn.",
            turnDuration: InitialDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.actImmediately = actImmediately;
            FocusPoints = focusPoints;
            UpdateTitle();
        }

        private bool StatusWasAppliedThisTurn => TurnDuration == InitialDuration;
        private bool StatusAppliedAndCanNotAct => StatusWasAppliedThisTurn && !actImmediately;

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
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
            if (FocusPoints <= 0)
            {
                TurnDuration = 0;
            }
        }

        public static bool ActiveDuelistHasFocusPoints =>
            GlobalContext.ActiveUnit.Role == Role.Duelist &&
            GlobalContext.ActiveUnit.IsAlive &&
            GlobalContext.ActiveUnit.StatusEffects.Any(status =>
                status is FocusStatus focusStatus &&
                focusStatus.FocusPoints > 0 &&
                !focusStatus.StatusAppliedAndCanNotAct
            );

        public void StartAdditionalAction()
        {
            AdditionalActionEvent.StartExtraAction("Extra Focus action!");
            FocusPoints--;
            UpdateTitle();
            GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
        }

        private void UpdateTitle()
        {
            Name = StatusName + " <" + FocusPoints + "pt>";
        }
    }
}
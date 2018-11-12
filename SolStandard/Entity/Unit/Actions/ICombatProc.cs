namespace SolStandard.Entity.Unit.Actions
{
    public interface ICombatProc
    {
        void OnCombatStart(GameUnit attacker, GameUnit defender);
        void OnBlock(GameUnit damageDealer, GameUnit target);
        void OnDamage(GameUnit damageDealer, GameUnit target);
        void OnCombatEnd(GameUnit attacker, GameUnit defender);
    }
}
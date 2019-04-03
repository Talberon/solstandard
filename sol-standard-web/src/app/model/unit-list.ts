import { Unit, Role } from './unit';

export const UNITS: Unit[] = [
    new Unit(1, 'Champion', Role.Tank, 'A bullying unit that moves his enemies around on the map.'),
    new Unit(2, 'Marauder', Role.Tank, 'An aggressive tank unit that gains strength as he takes damage.'),
    new Unit(3, 'Paladin', Role.Tank, 'An defensive tank that can rescue units and stun opponents.'),
    new Unit(4, 'Lancer', Role.Melee, 'A swift unit that chases down his target relentlessly.'),
    new Unit(5, 'Duelist', Role.Melee, 'A tempo-based unit that can store actions to use later.'),
    new Unit(6, 'Pugilist', Role.Melee, 'A focused unit that can brush off status effects and attack through armour.'),
    new Unit(7, 'Archer', Role.Ranged, 'A tricky unit that can extend its range and leave ensnaring traps on the ground.'),
    new Unit(8, 'Mage', Role.Ranged, 'A glass cannon unit that can apply status effects and leave fire traps on the ground.'),
    new Unit(9, 'Bard', Role.Support, 'A force-multiplying unit that can cast buffs on allied units.'),
    new Unit(10, 'Cleric', Role.Support, 'A recovery-focused unit that can cleanse statuses and grant armour bonuses.'),
];

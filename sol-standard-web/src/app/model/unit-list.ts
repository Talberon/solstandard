import { Unit } from './unit';

export const UNITS: Unit[] = [
    new Unit(1, 'Champion', 'Tank', 'A bullying unit that moves his enemies around on the map.'),
    new Unit(2, 'Marauder', 'Tank', 'An aggressive tank unit that gains strength as he takes damage.'),
    new Unit(3, 'Paladin', 'Tank', 'An defensive tank that can rescue units and stun opponents.'),
    new Unit(4, 'Lancer', 'Melee DPS', 'A swift unit that chases down his target relentlessly.'),
    new Unit(5, 'Duelist', 'Melee DPS', 'A tempo-based unit that can store actions to use later.'),
    new Unit(6, 'Pugilist', 'Melee DPS', 'A focused unit that can brush off status effects and attack through armour.'),
    new Unit(7, 'Archer', 'Ranged DPS', 'A tricky unit that can extend its range and leave ensnaring traps on the ground.'),
    new Unit(8, 'Mage', 'Ranged DPS', 'A glass cannon unit that can apply status effects and leave fire traps on the ground.'),
    new Unit(9, 'Bard', 'Support', 'A force-multiplying unit that can cast buffs on allied units.'),
    new Unit(10, 'Cleric', 'Support', 'A recovery-focused unit that can cleanse statuses and grant armour bonuses.'),
];

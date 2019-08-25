import { Unit, Role } from './unit';

export const UNITS: Unit[] = [
    new Unit(1, 'Champion', Role.Tank, 'A bullying tank that moves his enemies around on the map.'),
    new Unit(2, 'Marauder', Role.Tank, 'An aggressive tank unit that gains strength as he takes damage.'),
    new Unit(3, 'Paladin', Role.Tank, 'An defensive tank that can jump to allies and stun opponents.'),
    new Unit(4, 'Cavalier', Role.Tank, 'An maneuverable tank that can attack units from range and buff allies.'),
    new Unit(5, 'Lancer', Role.Melee, 'A swift unit that can jump to enemies and mitigate their counterattacks.'),
    new Unit(6, 'Duelist', Role.Melee, 'A tempo-based unit that can store actions to use later.'),
    new Unit(7, 'Pugilist', Role.Melee, 'A snowballing unit that deals increased damage the more she fights.'),
    new Unit(8, 'Rogue', Role.Melee, 'An tricky unit that can steal items and break doors easily.'),
    new Unit(9, 'Archer', Role.Ranged, 'A tricky unit that can extend its range and leave ensnaring traps on the ground.'),
    new Unit(10, 'Mage', Role.Ranged, 'A glass cannon unit that can manipulate terrain.'),
    new Unit(11, 'Bard', Role.Support, 'A force-multiplying unit that can buff allies within her auras.'),
    new Unit(12, 'Cleric', Role.Support, 'A recovery-focused unit that can cleanse statuses and grant armour bonuses.'),
];

import { GameTile } from './game-tile';

export const TILE_LIST: GameTile[] = [
    new GameTile('Artillery', 'An entity that allows attacking at an extended range.'),
    new GameTile('Breakable Obstacle', 'An obstacle that cannot be moved through. Has HP and can be destroyed and possibly drop items.'),
    new GameTile('Buff Tile', 'A tile that confers a unit attribute bonus.'),
    new GameTile('Chest', 'Contains gold or items. Can potentially be locked and only unlockable with a key.'),
    new GameTile('Deployment', 'Units drafted in the draft phase can be deployed to these tiles at the beginning of a match.'),
    new GameTile('Door', 'Can be opened/closed. Can be locked and unlocked via keys or switches.'),
    new GameTile('Drawbridge', 'A tile that is able to be walked upon when activated. Can be triggered by a switch.'),
    new GameTile('Portal', 'A tile that transports a unit to an associated target tile.'),
    new GameTile('Pressure Plate', 'A type of switch that is activated when a unit ends their turn standing on it.'),
    new GameTile('Push Block', 'A collision tile that can be moved as a free action.'),
    new GameTile('Railgun', 'An entity that allows attacking at an extended range along a cardinal axis.'),
    new GameTile('Seize', 'A tile that can be captured by one or both teams for an instant victory.'),
    new GameTile('Switch', 'Can be activated via a context action. Can trigger doors, traps, bridges, etc.'),
    new GameTile('Trap', 'A tile that deals damage at the beginning of a new battle round. May have additional effects.'),
];

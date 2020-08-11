# Sol Standard - How To Play

# Basic Controls (Default)

All inputs can be remapped in the Config menu. Below are the default inputs for game control.

## Keyboard Controls

The keyboard is used for both teams throughout the game when playing locally, even if a gamepad is 
connected. During netplay, it will be active only during your turn.

| Command | Default Input |
|---|---|
| Move Cursor | `WASD` |
| Move Camera | `Arrow Keys` |
| Zoom Camera | `Left Ctrl` / `Left Alt` |
| Confirm | `Space` |
| Cancel | `Left Shift` |
| Preview Unit | `Q` |
| Preview Item | `E` |
| Pause Menu | `Enter` |
| Status | `Esc` |
| Next/Previous Selection | `Tab` / `R` |

## Gamepad Controls

Gamepads will be assigned to specific players in the order they are turned on. If your gamepad does 
not work, try changing the team settings during map select, or confirm that your gamepads are plugged 
in and support X-Input.

| Command | WASD |
|---|---|
| Move Cursor | `Directional Pad` |
| Move Camera | `Right Analog Stick` |
| Zoom Camera | `Left Trigger` / `Right Trigger` |
| Confirm | `A` |
| Cancel | `B` |
| Preview Unit | `X` |
| Preview Item | `Y` |
| Pause Menu | `Start` |
| Status | `Select` |
| Next/Previous Selection | `Left Shoulder` / `Right Shoulder` |

# Objectives

Victory conditions are checked at the end of each unit's turn.

## Victory Conditions

| Objective | Description | 
| --- | --- |
| Assassinate | Defeat all of the opposing player's Commander units. |
| Rout Army | Defeat all of the opposing player's units. |
| Seize | Use the Seize entity's `Seize` context action. Seize entities can only be seized by designated teams. |
| Taxes | Deposit the target amount of gold into the bank. |
| Surrender | Always active. Can be selected via the Pause Menu. |
| Escape | Use the Escape entity's `Escape` action with your Commander. <br /> **Commander can only escape if all allies have also escaped or have been defeated.** |
| Solo Defeat Boss | Defeat all Creep Team Commander units to win. |
| Collect The Relics (VS) | The first team to collect the target number of relic items (by holding them in their units' inventories) wins. |
| Collect The Relics (Co-Op) | As long as both player teams are holding the target number of relic items (combined), both players will win. |


# Unit Attributes

Each unit has its own unique statline, and these statistics will differ based on the identity of the unit. 
Defensive units tend to have high `AMR` stats, aggressive units will have high `ATK` stats, and support 
units will have lower stats but will have unique skills that help boost the power of their team.

## Stats

### Health (`HP`)

A unit's primary health stat. If this stat reaches 0, the unit will die. This stat is difficult to 
regenerate and is often a permanent loss if taken!

### Armor (`AMR`)

A unit's secondary health stat. This health will be used when taking damage first. If a unit does not 
have any `AMR` remaining, they will start to lose `HP` instead. This stat is easily recoverable and it
is recommended to keep filled to prevent taking permanent `HP` damage.

### Movement (`MV`)

The number of steps a unit may take during the Movement Phase.

### Attack Range (`RNG`)

The basic attack range for a given unit. Various abilities may ignore a unit's base range and have their
own unique range. In combat, if an attacker is beyond the attack range of the defender, the defender will 
not be able to counter-attack.

### Attack Power (`ATK`)

The primary offensive stat. Each point of `ATK` counts towards one point of flat damage during combat 
when the unit is initiating combat.

### Retribution (`RET`)

Similar to `ATK`, `RET` counts towards one point of flat damage during combat when the unit is 
*defending* / being attacked.

### Block (`BLK`)

A defensive stat that will cancel out one point of damage dealt by an opponent during combat. This stat 
is rare and is usually only obtained via a status or terrain effect.

### Luck (`LCK`)

Each point of this stat will give a combatant a single die to roll. A die may land on one of 6 results, 
as detailed below:

| Value | Die Face | Description |
| --- | --- | --- |
| 1 | Blank | No effect; ignored during damage calculation. |
| 2 | Shield | Counts as 1 point of `BLK` during damage resolution. |
| 3 | Shield | Counts as 1 point of `BLK` during damage resolution. |
| 4 | Sword | Counts as 1 point of `ATK`/`RET` during damage resolution. |
| 5 | Sword | Counts as 1 point of `ATK`/`RET` during damage resolution. |
| 6 | Sword | Counts as 1 point of `ATK`/`RET` during damage resolution. |

### Command Points (`CP`)

A resource stat that regenerates by `1` every full Game Round. A Commander unit can spend this resource 
to use a unique Command Skill.

## Unit Actions / Skills

### Offensive Skills

Many Actions are attacks that have special modifiers and effects that trigger as part of, or before, the attack.

### Status Skills

Some skills can be used to cast status effects on the caster or another unit. These effects can be boons or 
debuffs depending on the target. Status effects will tick down by `1T` every full Game Round.

### Movement Skills

Some skills impact the positioning of the unit using the skill or another unit affected by the skill.

### Misc Skills

Other skills can interact with terrain, such as by unlocking doors, flipping switches, pushing blocks, and more.

# Game Phases

## Map Select

### Selecting a Team

Press the `Preview Unit` and `Preview Item` buttons to change which team will be controlled by player 1 (P1) 
and by player 2 (P2). During netplay, the Host player is considered `P1`.

### Selecting a Map

Use the `Cursor Move` inputs to move around the map and select one to play. Each map has a preview pane with 
a view of the map, the list of objectives and a rough time estimate for a given match. Use the `Next Unit` / 
`Previous Unit` buttons to jump to the next selectable option on the world map. Select a map by hovering over 
it and pressing `Confirm`.

## Unit Draft

If the selected map has Unit Draft enabled, then a round of alternating selection of units is done between all 
active players until the target number of units has been selected. Use this time to determine the unit composition 
and strategy you want to employ during your match, and pay attention to the units your opponent drafts as well.

You can preview the abilities and stats of any unit in the draft list by pressing `Unit Preview` on the unit 
you want to examine.

## Unit Deployment

After all units have been drafted, you can survey the map and determine where you want to place the units you 
have drafted. Take care to position your units well so that you can start the game with your best positioning!

## Game Start

After Draft/Deployment, the game begins in earnest. For the first round of a game, AI units will not act. On 
subsequent turns, AI units will act first in the round before player units activate. Units can act in any order, 
and are exhausted until the next round after they take a primary action.

### Unit Select

The active player has the freedom to survey the map, select a unit on their team to activate, or preview a unit's 
statistics by hovering over them and pressing the `Preview Unit` button. Units with items in their inventory, spoils, 
as well as vendors and some treasure chests, can have their inventories examined by hovering over them and pressing 
the `Preview Item` button.

### Unit Movement

Selecting a non-exhausted allied unit will begin the Movement Phase. Press `Cancel` to return to the Unit Select Phase 
at any time (unless this unit has already performed a free action this turn). Move the unit to the desired position and 
press `Confirm` to move to the next phase.

### Action Menu

#### Context Actions Submenu

This submenu will appear if the unit is within activation range of a contextual action. Various actions can take place 
when a unit is in range. Hover over different terrain entities to see the range within which they can be interacted with.

#### Basic Attack

A simple attack based on a unit's `ATK`, `LCK` and `RNG` statistics. All player units have this ability.

#### Skills Submenu

This submenu gives access to a unit type's unique skills. Examine these moves well, as they will be key to outmaneuvering 
and outperforming your opponents. Preview the skills a unit has available to them by using the `Preview Unit` button 
during the Unit Select phase or viewing the Codex in the Pause/Main menu.

#### Role Action

This action is a common action for a given unit's role. Tank units have access to `Shove` to move other units, Damage 
units and Support units can use `Sprint` to move some extra distance.

#### Inventory Submenu

This option appears when the active unit has at least one item in their inventory. Select this option to view the 
Use/Drop actions for the items available.

#### Guard Action

Recover a set amount of `AMR` and end your turn. This option is not available for Marauder since the Marauder class 
does not have any `AMR`.

#### Wait Action

End your turn without taking any action.

 ##### Focus Action
 
 Focus is a unique action for the Duelist class that allows the user to store an extra primary action that they can 
 activate on their next turn. Focus points cannot exceed a set maximum, so spend your points while you can!

### Targeting Phase

The targeting phase starts when an action has been selected. Spaces with highlighted square tiles can be selected to 
perform the selected action. If a given action is invalid, a tooltip will appear describing why the action cannot be 
performed.

### Action Resolution

After selecting a valid target during the Targeting phase, the action is resolved. Different actions have different 
effects in this phase, but one of the most common is an attack that will trigger the Combat subphase.

#### Combat

The combat phase determines which units will deal damage and whether they will survive.

Attackers will deal flat damage equal to their `ATK` stat, plus any bonuses from buffs or terrain effects. Defenders 
will deal flat damage equal to their `RET` stat (plus bonuses). In addition, both attackers and defenders will roll a 
number of die equal to their `LCK` stat (plus bonuses). Each point of `BLK` from an attacker or defender will cancel 
out one point of damage from the opposing unit.

Before damage resolution, both attacker and defender will roll all dice they have. The result of the dice will determine 
the final damage/block values before calculation.

All block values from both units are resolved first, followed by damage. Attackers will then resolve their damage first. 
If a defending unit is defeated after the attacker has resolved all of their damage, then they will not counter-attack, 
and their damage values will be ignored. If a defender is not within attack range (`RNG`), then their attack is also forfeit. 
If a defending unit is within range and is still alive after the attacker has resolved their damage, then the defender will 
counter-attack based on the damage remaining on their side of the table.

#### Other Effects

Some actions will cause status effects or manipulate the terrain. Read the description of each action to understand their effects.

### Resolving Unit Turn

After Action Resolution, various checks are made to the map state.

#### Effect Tiles (Turn-based)

Some effect tiles (such as Pressure Plates) will trigger at the end of every player turn. Their effects may vary, but you 
can see the type of trigger for a triggerable tile by hovering over it and viewing the Entity preview panel.

#### Victory Condition Check

At the end of every round, all victory conditions are checked for and the game will end if any conditions are satisfied.

#### Queue Next Routine (AI Units Only)

AI units will queue a new AI routine to follow at the end of their turn. This routine will be displayed over their head 
and in their status effect pane. You can preview the unit using the `Preview Unit` button to check what the routine will 
do on the unit's next turn.

### Resolving Game Round

When every unit has been exhausted, the round is over, all units are refreshed and various checks are made before starting 
a new round.

#### Status Effect Resolution

All status effects will tick down by `1T` at the end of the round, and statuses that have a proc effect will trigger now. 
Statuses with a turn timer of `0T` will be removed and any removal effects will happen at the same time.

#### Effect Tiles (Round-based)

Various traps and other effect tiles will trigger now. Some traps deal damage or will cause status effects, while some 
tiles may have regenerative effects or other ability triggers.

### AI Units

Units on the Creep team will act at the beginning of a round (except for the first round of a game). Some AI units will 
carry items or need to be defeated to achieve victory conditions, while others are a source of gold or just an obstacle 
to the player.

#### AI Routines

All AI units will forecast their next action with an icon above their map sprite, and in their status effect pane. Check 
the unit's ability with the `Preview Unit` button when highlighting them.

#### Independent Units

Some AI units are Independent. Independent units may attack or target other AI units in addition to player units. AI Units 
that are _not_ Independent will not target other AI units aggressively, even if other AI units are Independent.

# Configuration

Some settings can be set via the Configuration submenu on the Main Menu or the Pause Menu.

## Control Config

Set control mappings here. Mappings can be set for keyboard, P1 Gamepad and P2 Gamepad. Follow the instructions on screen. 
These settings will be saved from session to session, even after closing the game.

## Sound Config

Set the volume and mute/unmute game background music or sound effects.

# Netplay

## Hosting a Game

Select the Host Game option on the main menu and copy the IP address by selecting the option in the menu and sharing it 
with your opponent. Your IP will also be displayed on screen. Note that in order for your opponent to connect to you, you 
will need to port-forward your router so that they can access your computer over the Internet. This is not necessary if 
playing over a Local Area Network (LAN).

## Joining a Game

Select the Join Game option on the Main Menu. You can manually enter an IP address via the dial menu, or by selecting the 
Paste IP option if you have copied your opponent's IP address to your clipboard after they sent it to you.

Some minor IP address tips:

- If your opponent's IP address starts with `192.168`, they are sharing their _local_ IP address with you. You must be 
connected to the same router as your opponent in order for this IP address range to work. 
- `127.0.0.1`/`localhost`/`0.0.0.0` addresses will not work, since these addresses are only available within the same computer 
that is hosting the game. The IP address fetched on the Host Game screen should be sufficient.
- If you are using an appropriate IP address and you still cannot connect, ensure the game is allowed through both player's 
firewalls and that the Host has their router port-forwarded on the port specified in-game.
    - For more info on port-forwarding, visit https://portforward.com/
        - Every router has their own unique interface for configuring ports, so I cannot help you with this.

# Codex

The Codex is a helpful screen that details the baseline statistics and all of the available abilities for each class in the 
game. You can use this as a reference when selecting units during the draft phase, or to preview a unit's abilities while 
in-game. The codex can be access via certain context-sensitive uses of the `Preview Unit` button in-game, or via the 
Main Menu / Pause Menu under `Codex`.

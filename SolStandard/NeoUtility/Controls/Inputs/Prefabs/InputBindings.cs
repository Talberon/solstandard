using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.NeoUtility.Controls.Inputs.Gamepad;
using SolStandard.NeoUtility.Controls.Inputs.Keyboard;

namespace SolStandard.NeoUtility.Controls.Inputs.Prefabs
{
    public enum InputDevice
    {
        Keyboard1,
        Gamepad1,
        Gamepad2,
    }

    public class InputBindings
    {
        public GameControlParser KeyboardOneParser { get; set; }
        public GameControlParser GamepadOneParser { get; set; }
        public GameControlParser GamepadTwoParser { get; set; }

        public IEnumerable<GameControlParser> AllControllers { get; private set; }

        private readonly Dictionary<Team, GameControlParser> playerInputs;

        // ReSharper disable once NotNullMemberIsNotInitialized
        public InputBindings()
        {
            var keyboard1Save = GameDriver.FileIO.Load<KeyboardController>(
                ControlConfigContext.GetFileNameForDevice(InputDevice.Keyboard1));

            var gamepad1Save = GameDriver.FileIO.Load<GamepadController>(
                ControlConfigContext.GetFileNameForDevice(InputDevice.Gamepad1));
            var gamepad2Save = GameDriver.FileIO.Load<GamepadController>(
                ControlConfigContext.GetFileNameForDevice(InputDevice.Gamepad2));

            KeyboardOneParser = (keyboard1Save is object) ? new GameControlParser(keyboard1Save) : DefaultK1Parser();

            GamepadOneParser = (gamepad1Save is object)
                ? new GameControlParser(gamepad1Save)
                : DefaultGamepadParser(PlayerIndex.One);
            GamepadTwoParser = (gamepad2Save is object)
                ? new GameControlParser(gamepad2Save)
                : DefaultGamepadParser(PlayerIndex.Two);

            ResetControllerList();

            playerInputs = new Dictionary<Team, GameControlParser>();
        }

        public void ResetControllerList()
        {
            AllControllers = new List<GameControlParser>
            {
                KeyboardOneParser,
                GamepadOneParser,
                GamepadTwoParser,
            };
        }

        public ControlMapper GetControllerForTeam(Team player)
        {
            return playerInputs[player];
        }

        public Team? GetCharacterForController(GameControlParser controller)
        {
            foreach ((Team key, GameControlParser value) in playerInputs)
            {
                if (value == controller) return key;
            }

            return null;
        }

        public void UnmapControlsForController(GameControlParser controller)
        {
            var charactersToClear = new List<Team>();
            foreach ((Team key, GameControlParser value) in playerInputs)
            {
                if (value == controller) charactersToClear.Add(key);
            }

            foreach (Team character in charactersToClear)
            {
                playerInputs.Remove(character);
            }
        }

        public bool TeamHasControllerAssigned(Team player)
        {
            return playerInputs.ContainsKey(player);
        }

        public bool ControllerHasBeenAssigned(GameControlParser controller)
        {
            return playerInputs.ContainsValue(controller);
        }

        public void AssignControllerToTeam(Team player, GameControlParser parser)
        {
            playerInputs.Add(player, parser);
        }

        public void ClearAllControls()
        {
            playerInputs.Clear();
        }

        public void ResetControllerMapping(InputDevice inputDeviceType)
        {
            switch (inputDeviceType)
            {
                case InputDevice.Keyboard1:
                    KeyboardOneParser = DefaultK1Parser();
                    break;
                case InputDevice.Gamepad1:
                    GamepadOneParser = DefaultGamepadParser(PlayerIndex.One);
                    break;
                case InputDevice.Gamepad2:
                    GamepadTwoParser = DefaultGamepadParser(PlayerIndex.Two);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputDeviceType), inputDeviceType, null);
            }
        }

        public static GameControlParser GetDefaultMappingFor(InputDevice inputDeviceType)
        {
            return inputDeviceType switch
            {
                InputDevice.Keyboard1 => DefaultK1Parser(),
                InputDevice.Gamepad1 => DefaultGamepadParser(PlayerIndex.One),
                InputDevice.Gamepad2 => DefaultGamepadParser(PlayerIndex.Two),
                _ => throw new ArgumentOutOfRangeException(nameof(inputDeviceType), inputDeviceType, null)
            };
        }

        public ControlMapper GetControllerByDeviceType(InputDevice device)
        {
            return device switch
            {
                InputDevice.Keyboard1 => KeyboardOneParser,
                InputDevice.Gamepad1 => GamepadOneParser,
                InputDevice.Gamepad2 => GamepadTwoParser,
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };
        }

        private static GameControlParser DefaultK1Parser()
        {
            return new GameControlParser(new KeyboardController(KeyboardIndex.One));
        }

        private static GameControlParser DefaultGamepadParser(PlayerIndex playerIndex)
        {
            return new GameControlParser(new GamepadController(playerIndex));
        }

        public InputDevice GetDeviceForTeam(Team character)
        {
            return GetDeviceForController(GetControllerForTeam(character));
        }

        public static InputDevice GetDeviceForController(ControlMapper controlMapper)
        {
            if (controlMapper == GameDriver.InputBindings.KeyboardOneParser)
            {
                return InputDevice.Keyboard1;
            }

            if (controlMapper == GameDriver.InputBindings.GamepadOneParser)
            {
                return InputDevice.Gamepad1;
            }

            if (controlMapper == GameDriver.InputBindings.GamepadTwoParser)
            {
                return InputDevice.Gamepad2;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
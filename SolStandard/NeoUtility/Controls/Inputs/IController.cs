using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoUtility.Controls.Inputs.Prefabs;
using SolStandard.NeoUtility.Monogame.Interfaces;

namespace SolStandard.NeoUtility.Controls.Inputs
{
    public interface IController
    {
        AsepriteWrapper Icon { get; }
        SpriteAtlas IconForInput(Input input, Vector2 renderSize);

        InputDevice InputDevice { get; }
        ControlType ControlType { get; }
        GameControl GetInput(Input input);
        void RemapControl(Input inputToRemap, GameControl newInput);

        Dictionary<Input, GameControl> Inputs { get; }

        GameControl Confirm { get; }
        GameControl Cancel { get; }
        GameControl SubweaponLeft { get; }
        GameControl SubweaponTop { get; }

        GameControl MoveUp { get; }
        GameControl MoveDown { get; }
        GameControl MoveLeft { get; }
        GameControl MoveRight { get; }

        GameControl CameraUp { get; }
        GameControl CameraDown { get; }
        GameControl CameraLeft { get; }
        GameControl CameraRight { get; }

        GameControl Start { get; }
        GameControl Select { get; }

        GameControl TabLeft { get; }
        GameControl TabRight { get; }
        GameControl LeftTrigger { get; }
        GameControl RightTrigger { get; }
    }
}
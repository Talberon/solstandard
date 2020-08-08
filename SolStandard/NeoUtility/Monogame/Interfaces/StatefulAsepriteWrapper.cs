using System.Collections.Generic;
using MonoGame.Aseprite;
using SolStandard.Map;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoUtility.Monogame.Interfaces
{
    public class StatefulAsepriteWrapper : AsepriteWrapper
    {
        private enum States
        {
            Off,
            ToOn,
            On,
            ToOff
        }

        private static readonly Dictionary<States, string> StateStrings = new Dictionary<States, string>
        {
            {States.Off, "-off"},
            {States.ToOn, "-to-on"},
            {States.On, "-on"},
            {States.ToOff, "-to-off"},
        };

        private readonly bool hasTransitions;
        private readonly string animationBaseName;

        public StatefulAsepriteWrapper(AnimatedSprite sprite, string animationBaseName, bool enabledByDefault,
            bool hasTransitions) :
            base(sprite, Layer.TerrainDecoration)
        {
            this.hasTransitions = hasTransitions;
            this.animationBaseName = animationBaseName;

            string targetAnimation = enabledByDefault ? StateStrings[States.On] : StateStrings[States.Off];

            Sprite.Play($"{animationBaseName}{targetAnimation}");
        }

        public void TurnOn()
        {
            if (hasTransitions)
            {
                Sprite.PlayOnceThenPlayAnother(
                    $"{animationBaseName}{StateStrings[States.ToOn]}",
                    $"{animationBaseName}{StateStrings[States.On]}"
                );
            }
            else
            {
                Sprite.Play($"{animationBaseName}{StateStrings[States.On]}");
            }
        }

        public void TurnOff()
        {
            if (hasTransitions)
            {
                Sprite.PlayOnceThenPlayAnother(
                    $"{animationBaseName}{StateStrings[States.ToOff]}",
                    $"{animationBaseName}{StateStrings[States.Off]}"
                );
            }
            else
            {
                Sprite.Play($"{animationBaseName}{StateStrings[States.Off]}");
            }
        }

        public void TurnOnAndOff()
        {
            Sprite.PlayOnceThenPlayAnother(
                $"{animationBaseName}{StateStrings[States.On]}",
                $"{animationBaseName}{StateStrings[States.Off]}"
            );
        }
    }
}
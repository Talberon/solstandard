﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum SkillIcon
    {
        BasicAttack,
        Blink,
        DoubleTime,
        Draw,
        Inspire,
        Shove,
        Tackle,
        Wait,
        Harpoon,
        Ignite,
        Bulwark,
        Atrophy,
        Trap,
        Bloodthirst,
        Inferno,
        PoisonTip,
        Execute,
        Charge,
        Meditate,
        Uppercut,
        PressurePoint,
        AtkBuff,
        Challenge,
        Immobilize,
        PhaseStrike,
        Focus,
        Cleanse,
        Rage,
        Guillotine,
        Brace,
        Rampart,
        Intervention,
        Stun,
        Assassin,
        Kingslayer,
        Defender,
        TriggerHappy,
        TreasureHunter,
        Glutton,
        Summon,
        Wander
    }

    public static class SkillIconProvider
    {
        private static Dictionary<SkillIcon, ITexture2D> _skillIconDictionary;

        public static SpriteAtlas GetSkillIcon(SkillIcon icon, Vector2 iconSize)
        {
            return new SpriteAtlas(_skillIconDictionary[icon],
                new Vector2(_skillIconDictionary[icon].Width, _skillIconDictionary[icon].Height), iconSize);
        }

        public static void LoadSkillIcons(List<ITexture2D> skillIconTextures)
        {
            ITexture2D basicAttack = skillIconTextures.Find(texture => texture.Name.EndsWith("BasicAttack"));
            ITexture2D blink = skillIconTextures.Find(texture => texture.Name.EndsWith("Blink"));
            ITexture2D doubleTime = skillIconTextures.Find(texture => texture.Name.EndsWith("DoubleTime"));
            ITexture2D draw = skillIconTextures.Find(texture => texture.Name.EndsWith("Draw"));
            ITexture2D inspire = skillIconTextures.Find(texture => texture.Name.EndsWith("Inspire"));
            ITexture2D shove = skillIconTextures.Find(texture => texture.Name.EndsWith("Shove"));
            ITexture2D tackle = skillIconTextures.Find(texture => texture.Name.EndsWith("Tackle"));
            ITexture2D wait = skillIconTextures.Find(texture => texture.Name.EndsWith("Wait"));
            ITexture2D harpoon = skillIconTextures.Find(texture => texture.Name.EndsWith("Harpoon"));
            ITexture2D ignite = skillIconTextures.Find(texture => texture.Name.EndsWith("Ignite"));
            ITexture2D bulwark = skillIconTextures.Find(texture => texture.Name.EndsWith("Bulwark"));
            ITexture2D atrophy = skillIconTextures.Find(texture => texture.Name.EndsWith("Atrophy"));
            ITexture2D trap = skillIconTextures.Find(texture => texture.Name.EndsWith("Trap"));
            ITexture2D bloodthirst = skillIconTextures.Find(texture => texture.Name.EndsWith("Bloodthirst"));
            ITexture2D inferno = skillIconTextures.Find(texture => texture.Name.EndsWith("Inferno"));
            ITexture2D poisonTip = skillIconTextures.Find(texture => texture.Name.EndsWith("PoisonTip"));
            ITexture2D execute = skillIconTextures.Find(texture => texture.Name.EndsWith("Execute"));
            ITexture2D charge = skillIconTextures.Find(texture => texture.Name.EndsWith("Charge"));
            ITexture2D meditate = skillIconTextures.Find(texture => texture.Name.EndsWith("Meditate"));
            ITexture2D uppercut = skillIconTextures.Find(texture => texture.Name.EndsWith("Uppercut"));
            ITexture2D pressurePoint = skillIconTextures.Find(texture => texture.Name.EndsWith("PressurePoint"));
            ITexture2D atkBuff = skillIconTextures.Find(texture => texture.Name.EndsWith("AtkBuff"));
            ITexture2D challenge = skillIconTextures.Find(texture => texture.Name.EndsWith("Challenge"));
            ITexture2D freezeLance = skillIconTextures.Find(texture => texture.Name.EndsWith("FreezeLance"));
            ITexture2D phaseStrike = skillIconTextures.Find(texture => texture.Name.EndsWith("PhaseStrike"));
            ITexture2D focus = skillIconTextures.Find(texture => texture.Name.EndsWith("Focus"));
            ITexture2D cleanse = skillIconTextures.Find(texture => texture.Name.EndsWith("Cleanse"));
            ITexture2D rage = skillIconTextures.Find(texture => texture.Name.EndsWith("Rage"));
            ITexture2D guillotine = skillIconTextures.Find(texture => texture.Name.EndsWith("Guillotine"));
            ITexture2D brace = skillIconTextures.Find(texture => texture.Name.EndsWith("Brace"));
            ITexture2D rampart = skillIconTextures.Find(texture => texture.Name.EndsWith("Rampart"));
            ITexture2D intervention = skillIconTextures.Find(texture => texture.Name.EndsWith("Intervention"));
            ITexture2D stun = skillIconTextures.Find(texture => texture.Name.EndsWith("Stun"));
            ITexture2D assassin = skillIconTextures.Find(texture => texture.Name.EndsWith("Assassin"));
            ITexture2D kingslayer = skillIconTextures.Find(texture => texture.Name.EndsWith("Kingslayer"));
            ITexture2D defender = skillIconTextures.Find(texture => texture.Name.EndsWith("Defender"));
            ITexture2D triggerHappy = skillIconTextures.Find(texture => texture.Name.EndsWith("TriggerHappy"));
            ITexture2D treasureHunter = skillIconTextures.Find(texture => texture.Name.EndsWith("TreasureHunter"));
            ITexture2D glutton = skillIconTextures.Find(texture => texture.Name.EndsWith("Glutton"));
            ITexture2D summon = skillIconTextures.Find(texture => texture.Name.EndsWith("Summon"));
            ITexture2D wander = skillIconTextures.Find(texture => texture.Name.EndsWith("Wander"));

            _skillIconDictionary = new Dictionary<SkillIcon, ITexture2D>
            {
                {SkillIcon.BasicAttack, basicAttack},
                {SkillIcon.Blink, blink},
                {SkillIcon.DoubleTime, doubleTime},
                {SkillIcon.Draw, draw},
                {SkillIcon.Inspire, inspire},
                {SkillIcon.Shove, shove},
                {SkillIcon.Tackle, tackle},
                {SkillIcon.Wait, wait},
                {SkillIcon.Harpoon, harpoon},
                {SkillIcon.Ignite, ignite},
                {SkillIcon.Bulwark, bulwark},
                {SkillIcon.Atrophy, atrophy},
                {SkillIcon.Trap, trap},
                {SkillIcon.Bloodthirst, bloodthirst},
                {SkillIcon.Inferno, inferno},
                {SkillIcon.PoisonTip, poisonTip},
                {SkillIcon.Execute, execute},
                {SkillIcon.Charge, charge},
                {SkillIcon.Meditate, meditate},
                {SkillIcon.Uppercut, uppercut},
                {SkillIcon.PressurePoint, pressurePoint},
                {SkillIcon.AtkBuff, atkBuff},
                {SkillIcon.Challenge, challenge},
                {SkillIcon.Immobilize, freezeLance},
                {SkillIcon.PhaseStrike, phaseStrike},
                {SkillIcon.Focus, focus},
                {SkillIcon.Cleanse, cleanse},
                {SkillIcon.Rage, rage},
                {SkillIcon.Guillotine, guillotine},
                {SkillIcon.Brace, brace},
                {SkillIcon.Rampart, rampart},
                {SkillIcon.Intervention, intervention},
                {SkillIcon.Stun, stun},
                {SkillIcon.Assassin, assassin},
                {SkillIcon.Kingslayer, kingslayer},
                {SkillIcon.Defender, defender},
                {SkillIcon.TriggerHappy, triggerHappy},
                {SkillIcon.TreasureHunter, treasureHunter},
                {SkillIcon.Glutton, glutton},
                {SkillIcon.Summon, summon},
                                                          {SkillIcon.Wander, wander}
            };
        }
    }
}
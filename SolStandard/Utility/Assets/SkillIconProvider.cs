using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Assets
{
    public enum SkillIcon
    {
        //AI Routines
        Prey,
        Kingslayer,
        Defender,
        TriggerHappy,
        TreasureHunter,
        Glutton,
        Summon,
        Wander,

        //Unit Skills
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
        Assassinate,
        Charge,
        Meditate,
        Uppercut,
        Challenge,
        PhaseStrike,
        Focus,
        Cleanse,
        Rage,
        Guillotine,
        Brace,
        Rampart,
        Intervention,
        Stun,
        Recover,
        BetwixtPlate,
        FlowStrike,
        Frostbite,
        LeapStrike,
        Suplex,
        Terraform,
        Rescue,
        Jump,
        Venom,
        Grapple,
        Fortify,
        FadeStrike,
        Solo,
        Concerto,
        Tempest,
        Anthem,
        Freestyle,
        Gallop,
        Rob,
        Rend,
        PickLock,
        ThrowingKnife,
        StemTheTide,
        EnGarde,
        CorpsACorps,
        Fleche
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
            //AI Routines
            ITexture2D prey = skillIconTextures.Find(texture => texture.Name.EndsWith("Prey"));
            ITexture2D kingslayer = skillIconTextures.Find(texture => texture.Name.EndsWith("Kingslayer"));
            ITexture2D defender = skillIconTextures.Find(texture => texture.Name.EndsWith("Defender"));
            ITexture2D triggerHappy = skillIconTextures.Find(texture => texture.Name.EndsWith("TriggerHappy"));
            ITexture2D treasureHunter = skillIconTextures.Find(texture => texture.Name.EndsWith("TreasureHunter"));
            ITexture2D glutton = skillIconTextures.Find(texture => texture.Name.EndsWith("Glutton"));
            ITexture2D summon = skillIconTextures.Find(texture => texture.Name.EndsWith("Summon"));
            ITexture2D wander = skillIconTextures.Find(texture => texture.Name.EndsWith("Wander"));

            //Unit Skills
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
            ITexture2D assassinate = skillIconTextures.Find(texture => texture.Name.EndsWith("Assassinate"));
            ITexture2D charge = skillIconTextures.Find(texture => texture.Name.EndsWith("Charge"));
            ITexture2D meditate = skillIconTextures.Find(texture => texture.Name.EndsWith("Meditate"));
            ITexture2D uppercut = skillIconTextures.Find(texture => texture.Name.EndsWith("Uppercut"));
            ITexture2D challenge = skillIconTextures.Find(texture => texture.Name.EndsWith("Challenge"));
            ITexture2D phaseStrike = skillIconTextures.Find(texture => texture.Name.EndsWith("PhaseStrike"));
            ITexture2D focus = skillIconTextures.Find(texture => texture.Name.EndsWith("Focus"));
            ITexture2D cleanse = skillIconTextures.Find(texture => texture.Name.EndsWith("Cleanse"));
            ITexture2D rage = skillIconTextures.Find(texture => texture.Name.EndsWith("Rage"));
            ITexture2D guillotine = skillIconTextures.Find(texture => texture.Name.EndsWith("Guillotine"));
            ITexture2D brace = skillIconTextures.Find(texture => texture.Name.EndsWith("Brace"));
            ITexture2D rampart = skillIconTextures.Find(texture => texture.Name.EndsWith("Rampart"));
            ITexture2D intervention = skillIconTextures.Find(texture => texture.Name.EndsWith("Intervention"));
            ITexture2D stun = skillIconTextures.Find(texture => texture.Name.EndsWith("Stun"));
            ITexture2D recover = skillIconTextures.Find(texture => texture.Name.EndsWith("Recover"));
            ITexture2D betwixtPlate = skillIconTextures.Find(texture => texture.Name.EndsWith("BetwixtPlate"));
            ITexture2D flowStrike = skillIconTextures.Find(texture => texture.Name.EndsWith("FlowStrike"));
            ITexture2D frostbite = skillIconTextures.Find(texture => texture.Name.EndsWith("Frostbite"));
            ITexture2D leapStrike = skillIconTextures.Find(texture => texture.Name.EndsWith("LeapStrike"));
            ITexture2D suplex = skillIconTextures.Find(texture => texture.Name.EndsWith("Suplex"));
            ITexture2D venom = skillIconTextures.Find(texture => texture.Name.EndsWith("Venom"));
            ITexture2D terraform = skillIconTextures.Find(texture => texture.Name.EndsWith("Terraform"));
            ITexture2D grapple = skillIconTextures.Find(texture => texture.Name.EndsWith("Grapple"));
            ITexture2D fortify = skillIconTextures.Find(texture => texture.Name.EndsWith("Fortify"));
            ITexture2D rescue = skillIconTextures.Find(texture => texture.Name.EndsWith("Rescue"));
            ITexture2D jump = skillIconTextures.Find(texture => texture.Name.EndsWith("Jump"));
            ITexture2D fadeStrike = skillIconTextures.Find(texture => texture.Name.EndsWith("FadeStrike"));
            ITexture2D solo = skillIconTextures.Find(texture => texture.Name.EndsWith("Solo"));
            ITexture2D concerto = skillIconTextures.Find(texture => texture.Name.EndsWith("Concerto"));
            ITexture2D tempest = skillIconTextures.Find(texture => texture.Name.EndsWith("Tempest"));
            ITexture2D anthem = skillIconTextures.Find(texture => texture.Name.EndsWith("Anthem"));
            ITexture2D freestyle = skillIconTextures.Find(texture => texture.Name.EndsWith("Freestyle"));
            ITexture2D gallop = skillIconTextures.Find(texture => texture.Name.EndsWith("Horseshoe"));
            ITexture2D rob = skillIconTextures.Find(texture => texture.Name.EndsWith("Rob"));
            ITexture2D rend = skillIconTextures.Find(texture => texture.Name.EndsWith("Rend"));
            ITexture2D pickLock = skillIconTextures.Find(texture => texture.Name.EndsWith("PickLock"));
            ITexture2D throwingKnife = skillIconTextures.Find(texture => texture.Name.EndsWith("ThrowingKnife"));
            ITexture2D stemTheTide = skillIconTextures.Find(texture => texture.Name.EndsWith("StemTheTide"));
            ITexture2D enGarde = skillIconTextures.Find(texture => texture.Name.EndsWith("EnGarde"));
            ITexture2D corpsACorps = skillIconTextures.Find(texture => texture.Name.EndsWith("CorpsACorps"));
            ITexture2D fleche = skillIconTextures.Find(texture => texture.Name.EndsWith("Fleche"));

            _skillIconDictionary = new Dictionary<SkillIcon, ITexture2D>
            {
                //AI Routines
                {SkillIcon.Prey, prey},
                {SkillIcon.Kingslayer, kingslayer},
                {SkillIcon.Defender, defender},
                {SkillIcon.TriggerHappy, triggerHappy},
                {SkillIcon.TreasureHunter, treasureHunter},
                {SkillIcon.Glutton, glutton},
                {SkillIcon.Summon, summon},
                {SkillIcon.Wander, wander},

                //Unit Skills
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
                {SkillIcon.Assassinate, assassinate},
                {SkillIcon.Charge, charge},
                {SkillIcon.Meditate, meditate},
                {SkillIcon.Uppercut, uppercut},
                {SkillIcon.Challenge, challenge},
                {SkillIcon.PhaseStrike, phaseStrike},
                {SkillIcon.Focus, focus},
                {SkillIcon.Cleanse, cleanse},
                {SkillIcon.Rage, rage},
                {SkillIcon.Guillotine, guillotine},
                {SkillIcon.Brace, brace},
                {SkillIcon.Rampart, rampart},
                {SkillIcon.Intervention, intervention},
                {SkillIcon.Stun, stun},
                {SkillIcon.Recover, recover},
                {SkillIcon.BetwixtPlate, betwixtPlate},
                {SkillIcon.FlowStrike, flowStrike},
                {SkillIcon.Frostbite, frostbite},
                {SkillIcon.LeapStrike, leapStrike},
                {SkillIcon.Suplex, suplex},
                {SkillIcon.Venom, venom},
                {SkillIcon.Terraform, terraform},
                {SkillIcon.Grapple, grapple},
                {SkillIcon.Fortify, fortify},
                {SkillIcon.Rescue, rescue},
                {SkillIcon.Jump, jump},
                {SkillIcon.FadeStrike, fadeStrike},
                {SkillIcon.Concerto, concerto},
                {SkillIcon.Solo, solo},
                {SkillIcon.Tempest, tempest},
                {SkillIcon.Anthem, anthem},
                {SkillIcon.Freestyle, freestyle},
                {SkillIcon.Gallop, gallop},
                {SkillIcon.Rob, rob},
                {SkillIcon.Rend, rend},
                {SkillIcon.PickLock, pickLock},
                {SkillIcon.ThrowingKnife, throwingKnife},
                {SkillIcon.StemTheTide, stemTheTide},
                {SkillIcon.EnGarde, enGarde},
                {SkillIcon.CorpsACorps, corpsACorps},
                {SkillIcon.Fleche, fleche},
            };
        }
    }
}
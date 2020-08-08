using System.Collections.Generic;
using SolStandard.NeoUtility.Monogame.Interfaces;

// ReSharper disable NotNullMemberIsNotInitialized

namespace SolStandard.NeoUtility.Monogame.Assets
{
    public static class SoundBox
    {
        public static ISoundEffect Logo { get; private set; }
        
        public static ISoundEffect Death { get; private set; }
        public static ISoundEffect Damage { get; private set; }
        public static ISoundEffect Footstep { get; private set; }
        public static ISoundEffect DodgeRoll { get; private set; }
        public static ISoundEffect TapAttack { get; private set; }
        public static ISoundEffect JustCharged { get; private set; }
        public static ISoundEffect ChargedAttack { get; private set; }

        public static ISoundEffect Clash { get; private set; }
        public static ISoundEffect ParryStart { get; private set; }
        public static ISoundEffect ParryActivate { get; private set; }

        public static ISoundEffect MenuMove { get; private set; }
        public static ISoundEffect MenuMoveEdge { get; private set; }
        public static ISoundEffect MenuConfirm { get; private set; }
        public static ISoundEffect MenuCancel { get; private set; }
        public static ISoundEffect MenuError { get; private set; }
        public static ISoundEffect Pause { get; private set; }

        public static ISoundEffect Cancel { get; private set; }
        public static ISoundEffect HammerStart { get; private set; }
        public static ISoundEffect OpenDoor { get; private set; }
        public static ISoundEffect PickUp { get; private set; }
        public static ISoundEffect PowerUp { get; private set; }
        public static ISoundEffect Pushing { get; private set; }
        public static ISoundEffect ShieldReflect { get; private set; }
        public static ISoundEffect ShieldRaise { get; private set; }
        public static ISoundEffect TerrainDamage { get; private set; }
        public static ISoundEffect Throw { get; private set; }

        public static ISoundEffect WarningLowHealth { get; set; }
        public static ISoundEffect UnlockDoor { get; set; }
        public static ISoundEffect Slash { get; set; }
        public static ISoundEffect SlashSpinAttack { get; set; }
        public static ISoundEffect SlashFullyCharged { get; set; }
        public static ISoundEffect ShieldBlock { get; set; }
        public static ISoundEffect ScoreCounter { get; set; }
        public static ISoundEffect Portal { get; set; }
        public static ISoundEffect MoneyPickup { get; set; }
        public static ISoundEffect MenuCursorMove { get; set; }
        public static ISoundEffect MenuCursorMoveError { get; set; }
        public static ISoundEffect MenuConfirmSmall { get; set; }
        public static ISoundEffect MenuConfirmBig { get; set; }
        public static ISoundEffect GameStart { get; set; }
        public static ISoundEffect HeavyImpactBlunt { get; set; }
        public static ISoundEffect GridCursorMove1 { get; set; }
        public static ISoundEffect FallDownHole { get; set; }
        public static ISoundEffect Error { get; set; }
        public static ISoundEffect DropItem1 { get; set; }
        public static ISoundEffect DoorClose { get; set; }
        public static ISoundEffect DoorCloseTrap { get; set; }
        public static ISoundEffect DoorCloseMetal { get; set; }
        public static ISoundEffect Death2 { get; set; }
        public static ISoundEffect DashAttack { get; set; }
        public static ISoundEffect Countdown { get; set; }
        public static ISoundEffect CollectedMacguffin { get; set; }
        public static ISoundEffect Bump2 { get; set; }
        public static ISoundEffect Bump { get; set; }
        public static ISoundEffect BombExplode { get; set; }
        public static ISoundEffect Ready { get; set; }
        public static ISoundEffect MainMenuConfirm { get; set; }
        public static ISoundEffect SwitchFlip { get; set; }
        public static ISoundEffect PressurePlate { get; set; }
        public static ISoundEffect Respawn { get; set; }
        public static ISoundEffect RelicWin { get; set; }

        private static IEnumerable<ISoundEffect> SoundEffects => new List<ISoundEffect>
        {
            Logo,
            Death,
            Damage,
            Footstep,
            DodgeRoll,
            TapAttack,
            JustCharged,
            ChargedAttack,
            Clash,
            ParryStart,
            ParryActivate,
            MenuMove,
            MenuMoveEdge,
            MenuConfirm,
            MenuCancel,
            MenuError,
            Pause,
            Cancel,
            HammerStart,
            OpenDoor,
            PickUp,
            PowerUp,
            Pushing,
            ShieldReflect,
            ShieldRaise,
            TerrainDamage,
            Throw,
            WarningLowHealth,
            UnlockDoor,
            Slash,
            SlashSpinAttack,
            SlashFullyCharged,
            ShieldBlock,
            ScoreCounter,
            Portal,
            MoneyPickup,
            MenuCursorMove,
            MenuCursorMoveError,
            MenuConfirmSmall,
            MenuConfirmBig,
            GameStart,
            HeavyImpactBlunt,
            GridCursorMove1,
            FallDownHole,
            Error,
            DropItem1,
            DoorClose,
            DoorCloseTrap,
            DoorCloseMetal,
            Death2,
            DashAttack,
            Countdown,
            CollectedMacguffin,
            Bump2,
            Bump,
            BombExplode,
            Ready,
            MainMenuConfirm,
            SwitchFlip,
            PressurePlate,
            Respawn,
            RelicWin,
        };

        private const string SaveFileName = "sfxvolume";
        private static float _volume = 1;

        public static float Volume
        {
            get => _volume;
            private set
            {
                _volume = value;

                foreach (ISoundEffect sfx in SoundEffects)
                {
                    sfx.Volume = Volume;
                }

                SaveVolume();
            }
        }

        public static void ReduceVolume(float amount)
        {
            if (Volume - amount < 0) Volume = 0;
            else Volume -= amount;
        }

        public static void IncreaseVolume(float amount)
        {
            if (Volume + amount > 1) Volume = 1;
            else Volume += amount;
        }

        private static float LoadVolume()
        {
            const float defaultVolume = 1f;

            return GameDriver.FileIO.FileExists(SaveFileName)
                ? GameDriver.FileIO.Load<float>(SaveFileName)
                : defaultVolume;
        }

        private static void SaveVolume()
        {
            GameDriver.FileIO.Save(SaveFileName, Volume);
        }

        public static void LoadContent()
        {
            Logo = ContentLoader.LoadSFX("talberon-games-logo", 1f);
            
            Death = ContentLoader.LoadSFX("death", 1f, 0.1f);
            Damage = ContentLoader.LoadSFX("damage", 1f, 0.01f);
            Footstep = ContentLoader.LoadSFX("footstep", 0.2f, 0.01f);
            DodgeRoll = ContentLoader.LoadSFX("dodge-roll", 1f);
            TapAttack = ContentLoader.LoadSFX("slash", 1f, 0.01f);
            JustCharged = ContentLoader.LoadSFX("slash-fully-charged", 1f);
            ChargedAttack = ContentLoader.LoadSFX("slash-spin-attack", 1f);

            Clash = ContentLoader.LoadSFX("clash", 0.6f, 0.01f);
            ParryStart = ContentLoader.LoadSFX("parry-start", 1f);
            ParryActivate = ContentLoader.LoadSFX("parry-activate", 1f);

            MenuMove = ContentLoader.LoadSFX("menu-cursor-move", 1f, 0.1f);
            MenuMoveEdge = ContentLoader.LoadSFX("menu-cursor-move-error", 1f);
            MenuConfirm = ContentLoader.LoadSFX("menu-confirm", 1f);
            MenuCancel = ContentLoader.LoadSFX("menu-cancel", 1f);
            MenuError = ContentLoader.LoadSFX("cancel", 1f);
            Pause = ContentLoader.LoadSFX("pause", 1f);

            Cancel = ContentLoader.LoadSFX("cancel", 1f);
            HammerStart = ContentLoader.LoadSFX("hammer-start", 1f);
            OpenDoor = ContentLoader.LoadSFX("open-door", 1f);
            PickUp = ContentLoader.LoadSFX("pick-up", 1f);
            PowerUp = ContentLoader.LoadSFX("power-up", 1f);
            Pushing = ContentLoader.LoadSFX("pushing", 1f);
            ShieldReflect = ContentLoader.LoadSFX("shield-block", 1f);
            ShieldRaise = ContentLoader.LoadSFX("shield-raise", 1f);
            TerrainDamage = ContentLoader.LoadSFX("terrain-damage", 1f);
            Throw = ContentLoader.LoadSFX("throw", 1f);

            BombExplode = ContentLoader.LoadSFX("bomb-explode", 1f);
            Bump = ContentLoader.LoadSFX("bump", 1f, 0.1f);
            Bump2 = ContentLoader.LoadSFX("bump2", 1f, 0.1f);
            CollectedMacguffin = ContentLoader.LoadSFX("collected-macguffin", 1f);
            Countdown = ContentLoader.LoadSFX("countdown", 1f);
            DashAttack = ContentLoader.LoadSFX("dash-attack", 1f);
            Death2 = ContentLoader.LoadSFX("death2", 1f, 0.1f);
            DoorCloseMetal = ContentLoader.LoadSFX("door-close-metal", 1f, 0.1f);
            DoorCloseTrap = ContentLoader.LoadSFX("door-close-trap", 1f, 0.1f);
            DoorClose = ContentLoader.LoadSFX("door-close", 1f, 0.1f);
            DropItem1 = ContentLoader.LoadSFX("drop-item-1", 1f, 0.1f);
            Error = ContentLoader.LoadSFX("error", 0.6f);
            FallDownHole = ContentLoader.LoadSFX("fall-down-hole", 1f);
            GridCursorMove1 = ContentLoader.LoadSFX("grid-cursor-move-1", 1f, 0.1f);
            HeavyImpactBlunt = ContentLoader.LoadSFX("heavy-impact-blunt", 1f, 0.1f);
            MainMenuConfirm = ContentLoader.LoadSFX("main-menu-confirm", 1f);
            MenuConfirmBig = ContentLoader.LoadSFX("menu-confirm-big", 1f);
            MenuConfirmSmall = ContentLoader.LoadSFX("menu-confirm-small", 1f);
            MenuCursorMoveError = ContentLoader.LoadSFX("menu-cursor-move-error", 1f);
            MenuCursorMove = ContentLoader.LoadSFX("menu-cursor-move", 1f, 0.01f);
            MoneyPickup = ContentLoader.LoadSFX("money-pickup", 1f);
            Portal = ContentLoader.LoadSFX("portal", 1f);
            ScoreCounter = ContentLoader.LoadSFX("score-counter", 1f);
            ShieldBlock = ContentLoader.LoadSFX("shield-block", 1f);
            SlashFullyCharged = ContentLoader.LoadSFX("slash-fully-charged", 1f);
            SlashSpinAttack = ContentLoader.LoadSFX("slash-spin-attack", 1f);
            Slash = ContentLoader.LoadSFX("slash", 1f, 0.01f);
            UnlockDoor = ContentLoader.LoadSFX("unlock-door", 1f);
            WarningLowHealth = ContentLoader.LoadSFX("warning-low-health", 1f);
            GameStart = ContentLoader.LoadSFX("game-start", 1f);
            Ready = ContentLoader.LoadSFX("ready", 1f);
            SwitchFlip = ContentLoader.LoadSFX("switch-flip", 1f, 0.1f);
            PressurePlate = ContentLoader.LoadSFX("pressure-plate", 1f);
            Respawn = ContentLoader.LoadSFX("respawn", 1f, 0.1f);
            RelicWin = ContentLoader.LoadSFX("relic-win", 1f);

            Volume = LoadVolume();
        }
    }
}
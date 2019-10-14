using System;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ResetGameEvent : NetworkEvent
    {
        public override void Continue()
        {
            if (GameDriver.ConnectedAsClient || GameDriver.ConnectedAsServer)
            {
                GameDriver.ConnectionManager.CloseServer();
                GameDriver.ConnectionManager.DisconnectClient();
            }

            AssetManager.MenuConfirmSFX.Play();
            GameContext.Initialize(GameContext.MainMenuView, GameContext.NetworkMenuView);

            Complete = true;
        }
    }
}
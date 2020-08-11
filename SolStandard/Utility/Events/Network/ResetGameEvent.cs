using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.MainMenu;
using SolStandard.Containers.Components.Network;
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
            GlobalContext.Initialize(GlobalContext.MainMenuHUD, GlobalContext.NetworkHUD);
            
            Complete = true;
        }
    }
}
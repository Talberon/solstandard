using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Activation;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Bank : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; private set; }

        private int redMoney;
        private int blueMoney;

        public Bank(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool canMove,
            int[] interactRange, Dictionary<string, string> tiledProperties) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CanMove = canMove;
            InteractRange = interactRange;
            redMoney = 0;
            blueMoney = 0;
        }

        public void Deposit(GameUnit depositer, int goldToDeposit)
        {
            depositer.CurrentGold -= goldToDeposit;

            switch (depositer.Team)
            {
                case Team.Blue:
                    blueMoney += goldToDeposit;
                    break;
                case Team.Red:
                    redMoney += goldToDeposit;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("team", depositer.Team, null);
            }
        }

        public void Withdraw(GameUnit depositer, int goldToWithdraw)
        {
            depositer.CurrentGold += goldToWithdraw;

            switch (depositer.Team)
            {
                case Team.Blue:
                    blueMoney -= goldToWithdraw;
                    break;
                case Team.Red:
                    redMoney -= goldToWithdraw;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("team", depositer.Team, null);
            }
        }

        public List<UnitAction> TileActions()
        {
            //TODO Implement Deposit and Withdraw actions
            //TODO Apply bank total to team gold total in Taxes game mode
            return new List<UnitAction>();
        }
    }
}
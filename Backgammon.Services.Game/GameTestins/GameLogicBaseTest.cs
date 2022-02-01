using Backgammon.Services.Game.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameTestins
{
    [TestClass]
    public class GameLogicBaseTest
    {
        private GamePlay _game;
        [TestMethod]
        public void CheckAvalabiltyTest()
        {
            //Arrange
            _game = new GamePlay("f", "s", "g123", "fc", "sc") { CurrentPlayersTurn = "f",
                CurrentPlayes= new NumsToPlay(new TwoNums() { FirstCube=5, SecondCube =6 })             
            };
            PlayersMove move00 = new PlayersMove() { PlayersColor = Colors.Player1, CellNumber = 1, NumOfSteps = 6 };
            PlayersMove move01 = new PlayersMove() { PlayersColor = Colors.Player2, CellNumber = 2, NumOfSteps = 1 };
            PlayersMove move10 = new PlayersMove() { PlayersColor = Colors.Player2, CellNumber = 12, NumOfSteps = 6 };


            //Act
            int AssumedAnswerZero = _game.CheckAvailbilty(move00);
            _game.MoveTrueIfWins(move00, 0);//move the player for eat testing
            _game.CurrentPlayersTurn = "s";
            _game.CurrentPlayes = new NumsToPlay(new TwoNums() { FirstCube = 1, SecondCube = 6});
            int AssumedAswerZero1 = _game.CheckAvailbilty(move01);
            int AssumedAsnwerOne = _game.CheckAvailbilty(move10);

            //Assert
            Assert.AreEqual(1, AssumedAsnwerOne);
            Assert.AreEqual(0, AssumedAnswerZero);
            Assert.AreEqual(AssumedAswerZero1, 0);
        }
    }
}

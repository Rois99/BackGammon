using System;
using System.Collections.Generic;
using System.Linq;

namespace Backgammon.Services.Game.Domain.Models
{
    public class GamePlay
    {
        public string GameId { get; set; }
        public GameCell[] Cells { get; set; }
        public string FirstPlayerID { get; set; }
        public string FirstPlayerConnection { get; set; }
        public string SecondPlayerID { get; set; }
        public string SecondPlayerConnection { get; set; }
        public Dictionary<Colors, int> OutsidePlayers { get; set; } = new Dictionary<Colors, int>();

        //Current Status Propperties for Confirmation
        public string CurrentPlayersTurn { get; set; }

        //public NumbersToPlay { get; set; }
        public NumsToPlay CurrentPlayes { get; set; }

        public GamePlay(string firstId, string secondId, string gameId, string FirstConnection, string SecondConnection)
        {
            GameId = gameId;
            Cells = new GameCell[] {
                new GameCell() { NumOfPieces = 0, Color = Colors.Player1},
                new GameCell() { NumOfPieces = 2, Color = Colors.Player1},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 5, Color = Colors.Player2},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 3, Color = Colors.Player2},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 5, Color = Colors.Player1},
                new GameCell() { NumOfPieces = 5, Color = Colors.Player2},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 3, Color = Colors.Player1},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 5, Color = Colors.Player1},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 0, Color = Colors.NoColor},
                new GameCell() { NumOfPieces = 2, Color = Colors.Player2},
                new GameCell() { NumOfPieces = 0, Color = Colors.Player2},
            };
            FirstPlayerID = firstId;
            SecondPlayerID = secondId;
            OutsidePlayers.Add(Colors.Player1, 0);
            OutsidePlayers.Add(Colors.Player2, 0);
            FirstPlayerConnection = FirstConnection;
            SecondPlayerConnection = SecondConnection;
        }

        public void InitGamePlay(StartGameModel startGameModel)
        {
            CurrentPlayes = new NumsToPlay(startGameModel.PlayingCubes);//init the numbers of plays avalable

            if (startGameModel.WhosFirstCubes.FirstCube > startGameModel.WhosFirstCubes.SecondCube) //init the currnt players color
                CurrentPlayersTurn = FirstPlayerID;
            else
                CurrentPlayersTurn = SecondPlayerID;
        }

        //avalibilty Session
        public int CheckAvailbilty(PlayersMove playersMove)//-1 notAvalble 0 Avalble 1 AtePlayer 2 GotPlayerOut 3 Uncaged 4 UncagedAndAte
        {
            if (!CurrentPlayes.IsNumAvalble(playersMove.NumOfSteps))
                return -2;

            if (playersMove.PlayersColor == Colors.Player2)
            {
                playersMove.CellNumber = 25 - playersMove.CellNumber;
                playersMove.NumOfSteps = -playersMove.NumOfSteps;
            }


            int AbsoluteDestanationCell = playersMove.CellNumber + playersMove.NumOfSteps;
            if (IsEaten(playersMove.PlayersColor))
            {
                int Status = IsDestinationCellGoodForPlayer(AbsoluteDestanationCell, playersMove.PlayersColor);
                if (Status == -1)
                    return -1;
                else
                    return Status + 3;
            }
            if (AbsoluteDestanationCell < 25 && AbsoluteDestanationCell > 0)
            {
                if (Cells[AbsoluteDestanationCell].Color == Colors.NoColor || Cells[AbsoluteDestanationCell].Color == playersMove.PlayersColor)
                {
                    return 0;
                }
                else
                    if (Cells[AbsoluteDestanationCell].NumOfPieces == 1)//if eatsPlayer
                        return 1;
                    else //there more then two pieces of enemy
                        return -1;
            }
            else //if the player is trying toGetOutAPiece
            {
                if (CheckIfCanTakePieceOut(playersMove))
                    return 2;
                else
                    return -1;
            }
        }

        public int IsDestinationCellGoodForPlayer(int destinationCell, Colors playersColor)//
        {
            if (Cells[destinationCell].Color == playersColor || Cells[destinationCell].Color == Colors.NoColor)
                return 0;
            else
                if (Cells[destinationCell].NumOfPieces == 1)
                return 1;
            return -1;
        }

        public bool IsEaten(Colors playersColor)
        {
            if (playersColor == Colors.Player1)
                if (Cells[0].NumOfPieces > 0)
                    return true;
            if (playersColor == Colors.Player2)
                if (Cells[25].NumOfPieces > 0)
                    return true;
            return false;
        }

        private bool IsAbleToGetOutPlayers(Colors playersColor)
        {
            if (playersColor == Colors.Player1)
                for (int i = 1; i < 19; i++)
                {
                    if (Cells[i].NumOfPieces != 0)
                        if (Cells[i].Color == playersColor)
                        {
                            return false;
                        }
                }
            else
                for (int i = 7; i < 25; i++)
                {
                    if (Cells[i].NumOfPieces != 0)
                        if (Cells[i].Color == playersColor)
                        {
                            return false;
                        }
                }
            return true;
        }

        public bool CheckIfCanTakePieceOut(PlayersMove playersMove)//במידה ויכול להוציא שחקנים, בודק אם מבחינה מספרית זה אפשרי
        {
            if (!IsAbleToGetOutPlayers(playersMove.PlayersColor))
                return false;

            if (playersMove.CellNumber + playersMove.NumOfSteps == 25 || playersMove.CellNumber + playersMove.NumOfSteps == 0) //אם הוא הוציא את המספר המדויק
                return true;
            else //אם המספר גדול מהמספר המדוייק שנדרש כדי להוציא שחקן, בודק האם יש אופציה טובה יותר
            {
                if (playersMove.PlayersColor == Colors.Player1)
                    for (int i = 19; i < playersMove.CellNumber; i++)
                    {
                        if (Cells[i].NumOfPieces > 0 && Cells[i].Color == playersMove.PlayersColor)
                        {

                            //if (-1 < CheckAvailbilty(new PlayersMove { CellNumber = i,NumOfSteps = playersMove.NumOfSteps,PlayersColor = playersMove.PlayersColor}))
                            //{
                            return false;
                            //}
                        }
                    }
                else
                    for (int i = 6; i > playersMove.CellNumber; i--)
                    {
                        if (Cells[i].NumOfPieces > 0 && Cells[i].Color == playersMove.PlayersColor)
                        {
                            
                            //if (-1 < CheckAvailbilty(new PlayersMove { CellNumber = 25 - i, NumOfSteps = -playersMove.NumOfSteps, PlayersColor = playersMove.PlayersColor }))
                            //{
                            return false;
                            //}
                        }
                            
                    }
            }
            return true;// במקרה שזה השחקן הרחוק ביותר
        }

        //move session
        public bool MoveTrueIfWins(PlayersMove playersMove, int AvalibiltyCheck)//moves the player and Use the nums
        {
            //if (playersMove.PlayersColor == Colors.Player2)
            //{
            //    playersMove.NumOfSteps = -playersMove.NumOfSteps;
            //    playersMove.CellNumber = 25 - playersMove.CellNumber;
            //}

            int AbsolueDestination = playersMove.NumOfSteps + playersMove.CellNumber;
            CurrentPlayes.UseNum(Math.Abs(playersMove.NumOfSteps));
            if (AvalibiltyCheck == 3)
            {
                if (playersMove.PlayersColor == Colors.Player1)
                    Cells[0].NumOfPieces--;
                else
                    Cells[25].NumOfPieces--;
                Cells[AbsolueDestination].NumOfPieces++;
                Cells[AbsolueDestination].Color = playersMove.PlayersColor;
                return false;
            }
            else
                if (AvalibiltyCheck == 4)
            {
                if (playersMove.PlayersColor == Colors.Player1)
                    Cells[0].NumOfPieces--;
                else
                    Cells[25].NumOfPieces--;

                Cells[AbsolueDestination].Color = playersMove.PlayersColor;
                if (playersMove.PlayersColor == Colors.Player1)
                    Cells[25].NumOfPieces++;
                else
                    Cells[0].NumOfPieces++;
                return false;
            }

            //Removing player from cell
            Cells[playersMove.CellNumber].NumOfPieces--;
            if (Cells[playersMove.CellNumber].NumOfPieces == 0)
                Cells[playersMove.CellNumber].Color = Colors.NoColor;

            if (AvalibiltyCheck == 2)
            {
                OutsidePlayers[playersMove.PlayersColor]++;
                if (OutsidePlayers[playersMove.PlayersColor] > 14)
                    return true;
            }
            else
                if (AvalibiltyCheck == 1) //במידה ואכל שחקן מחליף את צבע היריב
            {
                Cells[AbsolueDestination].Color = playersMove.PlayersColor;
                if (playersMove.PlayersColor == Colors.Player1)
                {
                    Cells[25].NumOfPieces++;
                }
                else
                {
                    Cells[0].NumOfPieces++;
                }
            }
            else
                if (AvalibiltyCheck == 0)
            {
                Cells[AbsolueDestination].Color = playersMove.PlayersColor;
                Cells[AbsolueDestination].NumOfPieces++;
            }

            return false;
        }

        public string GetOthersPlayerConnection(string currentId) => currentId == FirstPlayerID ? SecondPlayerConnection : FirstPlayerConnection;

        public bool IsThereMoreMovements(Colors PlayersColor)//return false if there are no more possible moves
        {
            List<int> avalblePlays = CurrentPlayes.GetAvalableNumbers().ToList();
            // If eaten
            if (PlayersColor == Colors.Player1)
            {
                if (Cells[0].NumOfPieces > 0)
                {
                    foreach (var num in avalblePlays)
                    {
                        if (IsDestinationCellGoodForPlayer(num,PlayersColor) > -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else
            if (Cells[25].NumOfPieces > 0)
            {
                foreach (var num in avalblePlays)
                {
                    if (IsDestinationCellGoodForPlayer(25-num, PlayersColor) > -1)
                    {
                        return true;
                    }
                }
                return false;
            }

            // If not eaten
            foreach (var num in avalblePlays)
            {
                for (int i = 1; i < 25; i++)
                {
                    if (Cells[i].Color == PlayersColor && Cells[i].NumOfPieces > 0)//אם בתא הזה יש חייל של אותו שחקן תבדוק אם הוא יכול לזוז
                    {
                        if (PlayersColor == Colors.Player1)
                        {
                            if (CheckAvailbilty(new PlayersMove() { CellNumber = i, PlayersColor = PlayersColor, NumOfSteps = num }) > -1)
                            {
                                return true;
                            }
                        }else
                        {
                            if (CheckAvailbilty(new PlayersMove() { CellNumber =25-i, PlayersColor = PlayersColor, NumOfSteps = num }) > -1)
                            {
                                return true;
                            }
                        }
                    }
                        
                }
            }
            return false;
        }

        public void switchPlayersTurnAndRollCubes(TwoNums twoNums)//מקבל את תור השחקן הנוכחי ומחליף בחדש
        {
            if (CurrentPlayersTurn == FirstPlayerID)
                CurrentPlayersTurn = SecondPlayerID;
            else
                CurrentPlayersTurn = FirstPlayerID;
            CurrentPlayes = new NumsToPlay(twoNums);

        }

        public string GetOtherPlayersID(string CurrentPlayersID) => CurrentPlayersID == FirstPlayerID ? SecondPlayerID : FirstPlayerID;

    }
}

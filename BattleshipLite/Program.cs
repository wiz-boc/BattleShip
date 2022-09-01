using System;
using BattleshipLiteLibrary.Models;
using BattleshipLiteLibrary;

namespace BattleshipLite
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);
                RecordPlayerShot(activePlayer, opponent);
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue == true)
                {
                    //use tuple
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else {
                    winner = activePlayer;
                }

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to {winner.Username} for winning!");
            Console.WriteLine($"{ winner.Username} took {GameLogic.GetShotCount(winner)} shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex) {
                    isValidShot = false;
                    Console.WriteLine(ex.Message);
                }
                
                if (isValidShot == false) {
                    Console.WriteLine("Invalid Shot location. Please try again.");
                }
            } while (!isValidShot);

            //Determine shot results
            bool isAHit = GameLogic.IndentifyShotResult(opponent, row, column);
            //Record results
            GameLogic.MarkShotResult(activePlayer,row,column,isAHit);
            DisplayShotResults(row, column, isAHit);
        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($"{row}{column} is a Hit!");
            }
            else {
                Console.WriteLine($"{row}{column} is a Miss!");
            }
            Console.WriteLine();
            
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write($"{player.Username} Please enter your shot selection: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;
            foreach (var gridSpot in activePlayer.ShotGrid) {
                if (gridSpot.SpotLetter != currentRow) {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }
                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else {
                    Console.Write(" ?  ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage() {
            Console.WriteLine("Welcome to Battleship Lite");
            Console.WriteLine("Created by Wizz");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle) {
            PlayerInfoModel output = new PlayerInfoModel();
            Console.WriteLine($"Player information for {playerTitle}");
            //Ask the user for their name
            output.Username = AskForUsersName();
            //Load up the shot grid
            GameLogic.InitializeGrid(output);
            //Ask user for their 5 ship
            PlaceShips(output);
            //Clear
            Console.Clear();
            return output;
        }

        private static string AskForUsersName() {
            Console.Write("What is your name? ");
            string output = Console.ReadLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.WriteLine($"Where do you want to place ship number {model.ShipLocations.Count + 1}? : ");
                string location = Console.ReadLine();
                bool isValidLocation = false;
                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }

                if (!isValidLocation) {
                    Console.WriteLine("That was not a valid location. Please try again.");

                }
            } while (model.ShipLocations.Count < 5);
        }

    }
}

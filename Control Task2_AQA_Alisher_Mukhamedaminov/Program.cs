using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace TIC_TAC_TOE
{
    // Main program class
    class Program
    {
        // Array to represent the game board
        static char[] arr = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        // Variables to track the current player, chosen cell, and game status
        static int currentPlayer = 1;
        static int choice;
        static int flag = 0;
        // Player and bot names
        static string player1Name;
        static string player2Name;
        static string botName = "T-1000";

        // Dictionary to store player scores
        static Dictionary<string, int> playerScores = new Dictionary<string, int>();
        // List to store game history
        static List<string> gameHistory = new List<string>();

        // Path to the file with game data
        static string dataFilePath = "game_data.csv";

        // Main method of the program
        static void Main(string[] args)
        {
            // Load game data
            LoadGameData();

            // Main program loop
            do
            {
                // Reset game state
                currentPlayer = 1;
                flag = 0;
                char[] newValues = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                Array.Copy(newValues, arr, arr.Length);

                Console.WriteLine("Welcome to Tic-Tac-Toe!");
                Console.WriteLine("1. Play 1 Player (with bot)");
                Console.WriteLine("2. Play 2 Players");
                Console.WriteLine("3. View Rating");
                Console.WriteLine("4. View Game History");
                Console.WriteLine("5. View Game Data");
                Console.WriteLine("6. Quit");

                Console.Write("Enter your choice: ");
                string menuChoice = Console.ReadLine();

                // User's choice processing
                switch (menuChoice)
                {
                    case "1":
                        Console.Write("Enter your name: ");
                        player1Name = GetValidPlayerName();
                        PlayGameWithBot();
                        break;
                    case "2":
                        Console.Write("Enter the name of Player 1 (X): ");
                        player1Name = GetValidPlayerName();
                        Console.Write("Enter the name of Player 2 (O): ");
                        player2Name = GetValidPlayerName();
                        PlayTwoPlayers();
                        break;
                    case "3":
                        ViewRating();
                        break;
                    case "4":
                        ViewGameHistory();
                        break;
                    case "5":
                        ViewGameData();
                        break;
                    case "6":
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.Write("Do you want to go back to the main menu? (yes/no): ");
            } while (Console.ReadLine().ToLower() == "yes");
        }

        // Method for playing the game with a bot
        private static void PlayGameWithBot()
        {
            // Record the start time of the game
            DateTime gameStartTime = DateTime.Now;

            // Main game loop
            do
            {
                Console.Clear();
                Console.WriteLine($"Player: {player1Name} (X) and Bot: {botName} (O)\n");

                Console.WriteLine($"{(currentPlayer % 2 == 0 ? botName : player1Name)}'s turn");

                Console.WriteLine("\n");
                Board();

                // Bot's or player's move depending on the current player
                if (currentPlayer % 2 == 0)
                {
                    MakeBotMove();
                }
                else
                {
                    choice = GetValidMove();
                    MakeMove();
                }

                flag = CheckWin();
            } while (flag != 1 && flag != -1);

            Console.Clear();
            Board();
            currentPlayer--;

            // Record the end time of the game
            DateTime gameEndTime = DateTime.Now;

            // Display the game result
            if (flag == 1)
            {
                Console.WriteLine($"{(currentPlayer % 2 == 0 ? botName : player1Name)} has won!");
                UpdateRating(currentPlayer % 2 == 0 ? botName : player1Name);
                string gameResult = $"{player1Name} vs {botName}: {(currentPlayer % 2 == 0 ? botName : player1Name)} wins!";
                gameHistory.Add(gameResult);
                SaveGameData(player1Name, botName, gameStartTime, gameEndTime, gameResult);
            }
            else
            {
                Console.WriteLine("It's a draw!");
                string gameResult = $"{player1Name} vs {botName}: It's a draw!";
                gameHistory.Add(gameResult);
                SaveGameData(player1Name, botName, gameStartTime, gameEndTime, gameResult);
            }

            Console.ReadLine();
        }

        // Method for playing the game with two players
        private static void PlayTwoPlayers()
        {
            // Record the start time of the game
            DateTime gameStartTime = DateTime.Now;

            // Main game loop
            do
            {
                Console.Clear();
                Console.WriteLine($"Player 1: {player1Name} (X) and Player 2: {player2Name} (O)\n");

                Console.WriteLine($"{GetCurrentPlayerName()}'s turn");

                Console.WriteLine("\n");
                Board();
                choice = GetValidMove();
                MakeMove();

                flag = CheckWin();
            } while (flag != 1 && flag != -1);

            Console.Clear();
            Board();
            currentPlayer--;

            // Record the end time of the game
            DateTime gameEndTime = DateTime.Now;

            // Display the game result
            if (flag == 1)
            {
                Console.WriteLine($"{GetCurrentPlayerName()} has won!");
                UpdateRating(GetCurrentPlayerName());
                string gameResult = $"{player1Name} vs {player2Name}: {GetCurrentPlayerName()} wins!";
                gameHistory.Add(gameResult);
                SaveGameData(player1Name, player2Name, gameStartTime, gameEndTime, gameResult);
            }
            else
            {
                Console.WriteLine("It's a draw!");
                string gameResult = $"{player1Name} vs {player2Name}: It's a draw!";
                gameHistory.Add(gameResult);
                SaveGameData(player1Name, player2Name, gameStartTime, gameEndTime, gameResult);
            }

            Console.ReadLine();
        }

        // Method to get a valid player name
        private static string GetValidPlayerName()
        {
            string playerName;
            do
            {
                playerName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    Console.WriteLine("Please enter a valid name.");
                }
            } while (string.IsNullOrWhiteSpace(playerName));

            return playerName;
        }

        // Method to get a valid move from the player
        private static int GetValidMove()
        {
            int move;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out move) && move >= 1 && move <= 9 && arr[move] != 'X' && arr[move] != 'O')
                {
                    return move;
                }
                else
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            }
        }

        // Method to get the name of the current player
        private static string GetCurrentPlayerName()
        {
            return currentPlayer % 2 == 0 ? player2Name : player1Name;
        }

        // Method to view game data
        private static void ViewGameData()
        {
            if (File.Exists(dataFilePath))
            {
                Console.WriteLine("Game Data:");
                string[] lines = File.ReadAllLines(dataFilePath);
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine("No game data available.");
            }
        }

        // Method to save game data
        private static void SaveGameData(string player1, string player2, DateTime startTime, DateTime endTime, string result)
        {
            string data = $"{startTime},{endTime},{player1},{player2},{result}";
            File.AppendAllText(dataFilePath, data + Environment.NewLine);
        }

        // Method to load game data
        private static void LoadGameData()
        {
            if (File.Exists(dataFilePath))
            {
                string[] lines = File.ReadAllLines(dataFilePath);
                foreach (var line in lines)
                {
                    string[] data = line.Split(',');
                    if (data.Length == 5)
                    {
                        DateTime startTime = DateTime.Parse(data[0]);
                        DateTime endTime = DateTime.Parse(data[1]);
                        string player1 = data[2];
                        string player2 = data[3];
                        string result = data[4];
                        gameHistory.Add($"{startTime} - {endTime}: {player1} vs {player2} - {result}");
                    }
                }
            }
        }

        // Method to make a player's move
        private static void MakeMove()
        {
            if (arr[choice] != 'X' && arr[choice] != 'O')
            {
                arr[choice] = currentPlayer % 2 == 0 ? 'O' : 'X';
                currentPlayer++;
            }
            else
            {
                Console.WriteLine($"Sorry, the cell {choice} is already marked with {arr[choice]}");
                Console.WriteLine("\nPlease wait 2 seconds, the board is loading again.....");
                Thread.Sleep(2000);
            }
        }

        // Method to make the bot's move
        private static void MakeBotMove()
        {
            Random random = new Random();
            int botChoice;

            do
            {
                botChoice = random.Next(1, 10);
            } while (arr[botChoice] == 'X' || arr[botChoice] == 'O');

            arr[botChoice] = 'O';
            currentPlayer++;
        }

        // Method to view player ratings
        private static void ViewRating()
        {
            Console.WriteLine("Player Ratings:");
            foreach (var playerScore in playerScores)
            {
                Console.WriteLine($"{playerScore.Key}: {playerScore.Value} wins");
            }
        }

        // Method to view game history
        private static void ViewGameHistory()
        {
            Console.WriteLine("Game History:");
            foreach (var gameResult in gameHistory)
            {
                Console.WriteLine(gameResult);
            }
        }

        // Method to update player rating
        private static void UpdateRating(string playerName)
        {
            if (!playerScores.ContainsKey(playerName))
            {
                playerScores[playerName] = 1;
            }
            else
            {
                playerScores[playerName]++;
            }
        }

        // Method to check for a win
        private static int CheckWin()
        {
            for (int i = 0; i < 8; i++)
            {
                int line = -1;
                switch (i)
                {
                    case 0:
                        line = arr[1] + arr[2] + arr[3];
                        break;
                    case 1:
                        line = arr[4] + arr[5] + arr[6];
                        break;
                    case 2:
                        line = arr[7] + arr[8] + arr[9];
                        break;
                    case 3:
                        line = arr[1] + arr[4] + arr[7];
                        break;
                    case 4:
                        line = arr[2] + arr[5] + arr[8];
                        break;
                    case 5:
                        line = arr[3] + arr[6] + arr[9];
                        break;
                    case 6:
                        line = arr[1] + arr[5] + arr[9];
                        break;
                    case 7:
                        line = arr[3] + arr[5] + arr[7];
                        break;
                }

                if (line == 'X' * 3 || line == 'O' * 3)
                {
                    return 1;
                }
            }

            if (arr[1] != '1' && arr[2] != '2' && arr[3] != '3' && arr[4] != '4' && arr[5] != '5' && arr[6] != '6' && arr[7] != '7' && arr[8] != '8' && arr[9] != '9')
            {
                return -1;
            }

            return 0;
        }

        // Method to display the game board
        private static void Board()
        {
            Console.WriteLine("     |     |      ");
            Console.WriteLine($"  {arr[1]}  |  {arr[2]}  |  {arr[3]}");
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine($"  {arr[4]}  |  {arr[5]}  |  {arr[6]}");
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine($"  {arr[7]}  |  {arr[8]}  |  {arr[9]}");
            Console.WriteLine("     |     |      ");
        }
    }
}
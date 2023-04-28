using System;

namespace TicTacToeNeilLopes
{
    public class TicTacToeGame
    {
        private static int turnsTaken = 0;

        private static char[,] gameBoard =
        {
            {'7', '8', '9' },
            {'4', '5', '6' },
            {'1', '2', '3' }
        };

        class Player
        {
            public int Number { get; set; }
            public char Icon { get; set; }

            public Player(int number, char icon)
            {
                Number = number;
                Icon = icon;
            }
        }

        enum GameMode
        {
            PlayerVsPlayer = 1,
            PlayerVsAI = 2
        }

        public TicTacToeGame()
        {
            // Initialization
        }

        public void Start()
        {
            GameMode gameMode = (GameMode)GetGameMode();
            Player[] players = { new Player(1, 'X'), new Player(2, 'O') };
            int playerInput = 0;

            do
            {
                foreach (Player player in players)
                {
                    DisplayGameBoard(gameBoard);

                    bool isValidInput;
                    do
                    {
                        if (gameMode == GameMode.PlayerVsPlayer || (gameMode == GameMode.PlayerVsAI && player.Number == 1))
                        {
                            Console.Write("\nChoose your move Player {0}: ", player.Number);
                            string inputString = Console.ReadLine();

                            if (int.TryParse(inputString, out playerInput) && playerInput >= 1 && playerInput <= 9)
                            {
                                if (IsCellEmpty(playerInput))
                                {
                                    isValidInput = true;
                                }
                                else
                                {
                                    Console.WriteLine("\nCell is already occupied! Choose another cell!");
                                    isValidInput = false;
                                }
                            }
                            else
                            {
                                Console.WriteLine("\nIncorrect input! Please choose a number between 1 and 9!");
                                isValidInput = false;
                            }
                        }
                        else // A.I. turn
                        {
                            playerInput = FindBestMove();
                            if (IsCellEmpty(playerInput))
                            {
                                isValidInput = true;
                                Console.WriteLine("\nA.I. chooses move {0}.", playerInput);
                            }
                            else
                            {
                                isValidInput = false;
                            }
                        }

                    } while (!isValidInput);

                    SetCellValue(player.Icon, playerInput);
                    turnsTaken++;

                    if (IsWinner(player.Icon))
                    {
                        DisplayGameBoard(gameBoard);
                        if (player.Number == 2 && gameMode == GameMode.PlayerVsAI)
                        {
                            Console.WriteLine("\nThe A.I. has won!");
                        }
                        else
                        {
                            Console.WriteLine("\nPlayer {0} has won!", player.Number);
                        }
                        Console.WriteLine("Please press any key to reset game...");
                        Console.ReadKey();
                        ResetGameBoard();
                        break;
                    }
                    else if (turnsTaken == 9)
                    {
                        DisplayGameBoard(gameBoard);
                        Console.WriteLine("It's a draw. Try again!");
                        Console.WriteLine("Press any key to reset the game...");
                        Console.ReadKey();
                        ResetGameBoard();
                        break;
                    }
                }

            } while (turnsTaken < 9);
        }

        // Reset game board to initial state
        public static void ResetGameBoard()
        {
            char[,] initialBoard =
            {
            {'7', '8', '9' },
            {'4', '5', '6' },
            {'1', '2', '3' }
            };

            gameBoard = initialBoard;
            turnsTaken = 0;
        }

        // Display game board
        public static void DisplayGameBoard(char[,] board)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-----------------------------");
            Console.WriteLine("| -*-*-*- TicTacToe -*-*-*- |");
            Console.WriteLine("-----------------------------");
            Console.ResetColor();
            Console.WriteLine("|                           |");
            Console.WriteLine("|                           |");
            Console.WriteLine("|           |   |           |");
            Console.WriteLine("|         {0} | {1} | {2}         |", GetColoredSymbol(board[0, 0]), GetColoredSymbol(board[0, 1]), GetColoredSymbol(board[0, 2]));
            Console.WriteLine("|       ----|---|----       |");
            Console.WriteLine("|         {0} | {1} | {2}         |", GetColoredSymbol(board[1, 0]), GetColoredSymbol(board[1, 1]), GetColoredSymbol(board[1, 2]));
            Console.WriteLine("|       ----|---|----       |");
            Console.WriteLine("|         {0} | {1} | {2}         |", GetColoredSymbol(board[2, 0]), GetColoredSymbol(board[2, 1]), GetColoredSymbol(board[2, 2]));
            Console.WriteLine("|           |   |           |");
            Console.WriteLine("|                           |");
            Console.WriteLine("|                           |");
            Console.WriteLine("|---------------------------|");
        }

        // Check if a player has won
        public static bool IsWinner(char playerIcon)
        {
            // Check horizontal, vertical, and diagonal lines
            for (int i = 0; i < 3; i++)
            {
                if ((gameBoard[i, 0] == playerIcon && gameBoard[i, 1] == playerIcon && gameBoard[i, 2] == playerIcon) ||
                    (gameBoard[0, i] == playerIcon && gameBoard[1, i] == playerIcon && gameBoard[2, i] == playerIcon))
                {
                    return true;
                }
            }

            // Check diagonals separately
            if ((gameBoard[0, 0] == playerIcon && gameBoard[1, 1] == playerIcon && gameBoard[2, 2] == playerIcon) ||
                (gameBoard[0, 2] == playerIcon && gameBoard[1, 1] == playerIcon && gameBoard[2, 0] == playerIcon))
            {
                return true;
            }

            return false;
        }

        // Get colored symbol for display
        public static string GetColoredSymbol(char symbol)
        {
            string colorCode = "";
            switch (symbol)
            {
                case 'X':
                    colorCode = "\u001b[36m"; // set "X" color to dark cyan
                    break;
                case 'O':
                    colorCode = "\u001b[31m"; // set "O" color to red
                    break;
            }
            return colorCode + symbol + "\u001b[0m"; // reset color to default
        }

        // Set cell value on the game board
        public static void SetCellValue(char playerIcon, int playerInput)
        {
            int row = (playerInput - 1) / 3;
            int col = (playerInput - 1) % 3;

            gameBoard[2 - row, col] = playerIcon;
        }


        // Check if a cell is empty
        public static bool IsCellEmpty(int playerInput)
        {
            int row = (playerInput - 1) / 3;
            int col = (playerInput - 1) % 3;
            return gameBoard[2 - row, col] != 'X' && gameBoard[2 - row, col] != 'O';
        }

        // Get game mode from user input
        public static int GetGameMode()
        {
            int gameMode;
            do
            {
                Console.WriteLine("Please select the game mode:");
                Console.WriteLine("1. Player vs Player");
                Console.WriteLine("2. Player vs A.I.");
                if (int.TryParse(Console.ReadLine(), out gameMode) && (gameMode == 1 || gameMode == 2))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2.");
                }
            } while (true);

            return gameMode;
        }

        // Evaluate the game board for Minimax algorithm
        public static int EvaluateBoard()
        {
            int score = 0;

            if (IsWinner('X'))
            {
                score = -1;
            }
            else if (IsWinner('O'))
            {
                score = 1;
            }

            return score;
        }

        // AI Brain 

        public static int Minimax(char[,] board, int depth, bool isMaximizingPlayer, int alpha, int beta)
        {
            int boardValue = EvaluateBoard();

            if (boardValue == 1 || boardValue == -1 || turnsTaken == 9)
            {
                return boardValue;
            }

            if (isMaximizingPlayer)
            {
                int bestVal = -1000;

                for (int i = 1; i <= 9; i++)
                {
                    if (IsCellEmpty(i))
                    {
                        SetCellValue('O', i);
                        turnsTaken++;

                        int value = Minimax(board, depth + 1, !isMaximizingPlayer, alpha, beta);
                        bestVal = Math.Max(bestVal, value);
                        alpha = Math.Max(alpha, bestVal);

                        SetCellValue((char)(((i - 1) % 3) + '1' + ((i - 1) / 3) * 3), i);
                        turnsTaken--;

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return bestVal;
            }
            else
            {
                int bestVal = 1000;

                for (int i = 1; i <= 9; i++)
                {
                    if (IsCellEmpty(i))
                    {
                        SetCellValue('X', i);
                        turnsTaken++;

                        int value = Minimax(board, depth + 1, !isMaximizingPlayer, alpha, beta);
                        bestVal = Math.Min(bestVal, value);
                        beta = Math.Min(beta, bestVal);

                        SetCellValue((char)(((i - 1) % 3) + '1' + ((i - 1) / 3) * 3), i);
                        turnsTaken--;

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return bestVal;
            }
        }

        public static int FindBestMove()
        {
            int bestMove = -1;
            int bestValue = -1000;

            for (int i = 1; i <= 9; i++)
            {
                if (IsCellEmpty(i))
                {
                    SetCellValue('O', i);
                    turnsTaken++;

                    int moveValue = Minimax(gameBoard, 0, false, -1000, 1000);

                    SetCellValue((char)(((i - 1) % 3) + '1' + ((i - 1) / 3) * 3), i);
                    turnsTaken--;

                    if (moveValue > bestValue)
                    {
                        bestValue = moveValue;
                        bestMove = i;
                    }
                }
            }

            return bestMove;
        }


    }
}


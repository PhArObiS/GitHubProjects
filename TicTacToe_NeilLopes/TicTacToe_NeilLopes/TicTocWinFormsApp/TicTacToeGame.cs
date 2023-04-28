using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

internal class TicTacToeGame : Form
{
    // Static variable of turns taken
    static int turnsTaken = 0;

    // Constructor
    public TicTacToeGame()
    {
        InitializeComponent();
        using (GameModeForm gameModeForm = new GameModeForm())
        {
            gameModeForm.ShowDialog();
            gameMode = gameModeForm.SelectedGameMode;
        }
    }

    // Dark mode colors class
    public static class DarkModeColors
    {
        public static Color BackgroundColor { get; } = Color.FromArgb(43, 43, 43);
        public static Color ForegroundColor { get; } = Color.FromArgb(255, 255, 255);
        public static Color ButtonBackgroundColor { get; } = Color.FromArgb(64, 64, 64);
        public static Color ButtonForegroundColor { get; } = Color.FromArgb(255, 255, 255);
    }

    // OnLoad event handler
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set dark mode colors for UI 
        this.BackColor = DarkModeColors.BackgroundColor;
        this.ForeColor = DarkModeColors.ForegroundColor;

        foreach (Button button in this.Controls.OfType<Button>())
        {
            button.BackColor = DarkModeColors.ButtonBackgroundColor;
            button.ForeColor = DarkModeColors.ButtonForegroundColor;
        }

        gameStatusLabel.BackColor = DarkModeColors.BackgroundColor;
        gameStatusLabel.ForeColor = DarkModeColors.ForegroundColor;
    }

    
    private Button[,] buttons = new Button[3, 3];
    private Label gameStatusLabel;

    private void InitializeComponent()
    {
        // Set up form properties
        this.Text = "TicTacToe_NeilLopes";
        this.Size = new Size(300, 350);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create buttons for the Tic Tac Toe grid
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                buttons[i, j] = new Button
                {
                    Size = new Size(80, 80),
                    Location = new Point(10 + (i * 90), 10 + (j * 90))
                };
                buttons[i, j].Click += Button_Click;
                this.Controls.Add(buttons[i, j]);
            }
        }

        // Set up game status label
        gameStatusLabel = new Label
        {
            Size = new Size(250, 30),
            Location = new Point(20, 290),
            Text = "Player 1's turn"
        };
        this.Controls.Add(gameStatusLabel);

        // Set up reset button
        Button resetButton = new Button
        {
            Text = "Reset",
            Size = new Size(80, 30),
            Location = new Point(buttons[2, 2].Location.X + (buttons[2, 2].Size.Width / 2) - 40, this.ClientSize.Height - 40),
            Margin = new Padding(0, 10, 10, 0)
        };

        resetButton.Click += ResetButton_Click;
        this.Controls.Add(resetButton);
    }

    // Reset button click event handler
    private void ResetButton_Click(object sender, EventArgs e)
    {
        ResetGameBoard();

        foreach (Button button in buttons)
        {
            button.Enabled = true;
            button.Text = "";
        }

        gameStatusLabel.Text = "Player 1's turn";
        currentPlayer = 1;
    }

    private Player player1 = new Player(1, 'X');
    private Player player2 = new Player(2, 'O');
    private int currentPlayer = 1;
    private int gameMode = (int)GameMode.PlayerVsPlayer;

    private void Button_Click(object sender, EventArgs e)
    {
        Button clickedButton = sender as Button;
        // Cell already has a value
        if (clickedButton.Text != string.Empty) 
        {
            return;
        }

        int buttonNumber = GetButtonNumber(clickedButton) + 1;
        if (currentPlayer == 1)
        {
            clickedButton.ForeColor = Color.Cyan;
            clickedButton.Font = new Font(clickedButton.Font.FontFamily, 40);
            clickedButton.Text = player1.Icon.ToString();
            SetCellValue(player1.Icon, buttonNumber);
        }
        else
        {
            clickedButton.ForeColor = Color.Magenta;
            clickedButton.Font = new Font(clickedButton.Font.FontFamily, 40);
            clickedButton.Text = player2.Icon.ToString();
            SetCellValue(player2.Icon, buttonNumber);
        }


        if (IsWinner(currentPlayer == 1 ? player1.Icon : player2.Icon))
        {
            gameStatusLabel.Text = $"Player {currentPlayer} wins!";
            DisableButtons();
            return;
        }

        // Switch to the other player
        currentPlayer = 3 - currentPlayer; 
        gameStatusLabel.Text = $"Player {currentPlayer}'s turn";

        if (currentPlayer == 2 && gameMode == (int)GameMode.PlayerVsAI)
        {
            int bestMove = FindBestMove();
            int row = (bestMove - 1) / 3;
            int col = (bestMove - 1) % 3;
            Button aiButton = buttons[2 - row, col];
            aiButton.ForeColor = Color.Magenta;
            aiButton.Font = new Font(aiButton.Font.FontFamily, 40);
            aiButton.Text = player2.Icon.ToString();
            SetCellValue(player2.Icon, bestMove);

            if (IsWinner(player2.Icon))
            {
                gameStatusLabel.Text = $"Player {player2.Number} wins!";
                DisableButtons();
                return;
            }

            // Switch to the other player
            currentPlayer = 3 - currentPlayer; 
            gameStatusLabel.Text = $"Player {currentPlayer}'s turn";
        }
    }

    // returns the button number in the grid for a given clicked button
    private int GetButtonNumber(Button clickedButton)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (buttons[i, j] == clickedButton)
                {
                    return (2 - i) * 3 + j;
                }
            }
        }

        return -1;
    }

    // disables all the buttons in the grid
    private void DisableButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                buttons[i, j].Enabled = false;
            }
        }
    }



    // initial state of the game board
    static char[,] gameBoard =
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
        turnsTaken++;
    }

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

    public static string GetColoredSymbol(char symbol)
    {
        string colorCode = "";
        switch (symbol)
        {
            case 'X':
                // set "X" color to dark cyan
                colorCode = "\u001b[36m"; 
                break;
            case 'O':
                // set "O" color to red
                colorCode = "\u001b[31m"; 
                break;
        }
        // reset color
        return colorCode + symbol + "\u001b[0m"; 
    }


    public static void SetCellValue(char playerIcon, int playerInput)
    {
        int row = (playerInput - 1) / 3;
        int col = (playerInput - 1) % 3;

        gameBoard[2 - row, col] = playerIcon;
    }

    public static bool IsCellEmpty(int playerInput)
    {
        int row = (playerInput - 1) / 3;
        int col = (playerInput - 1) % 3;
        return gameBoard[2 - row, col] != 'X' && gameBoard[2 - row, col] != 'O';
    }

    public class GameModeForm : Form
    {
        public int SelectedGameMode { get; private set; }

        public GameModeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Select Game Mode";
            this.Size = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Button playerVsPlayerButton = new Button
            {
                Text = "Player vs Player",
                Location = new Point(60, 20),
                Size = new Size(180, 30)
            };
            playerVsPlayerButton.Click += (sender, e) => { SelectedGameMode = (int)GameMode.PlayerVsPlayer; this.Close(); };
            this.Controls.Add(playerVsPlayerButton);

            Button playerVsAIButton = new Button
            {
                Text = "Player vs AI",
                Location = new Point(60, 70),
                Size = new Size(180, 30)
            };
            playerVsAIButton.Click += (sender, e) => { SelectedGameMode = (int)GameMode.PlayerVsAI; this.Close(); };
            this.Controls.Add(playerVsAIButton);
        }
    }

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


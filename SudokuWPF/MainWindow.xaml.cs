using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SudokuWPF
{
    public partial class MainWindow : Window
    {
        SudokuEngine engine = new SudokuEngine(9);
        (int row, int col)? selected = null;
        string difficulty = "Café com Leite";
        bool won = false;

        DispatcherTimer timer = new DispatcherTimer();
        int seconds = 0;

        static Random rng = new Random();

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                seconds++;
                TimerLabel.Text = FormatTime(seconds);
            };
            StartNewGame();
        }

        // ════════════════════════════════════════
        // NOVO JOGO
        // ════════════════════════════════════════

        async void StartNewGame()
        {
            SetButtonsEnabled(false);
            MessageLabel.Text = "";

            int size, remove;
            switch (difficulty)
            {
                case "Normal":
                    size   = 16;
                    remove = rng.Next(100, 140);
                    LoadingLabel.Text = "⏳ Gerando grade 16x16...";
                    break;
                case "Deploy Sexta":
                    size   = 25;
                    remove = rng.Next(300, 380);
                    LoadingLabel.Text = "⏳ Gerando grade 25x25 (pode demorar)...";
                    break;
                default:
                    size   = 9;
                    remove = rng.Next(36, 46);
                    LoadingLabel.Text = "⏳ Gerando grade 9x9...";
                    break;
            }

            await Task.Run(() =>
            {
                engine = new SudokuEngine(size);
                engine.GeneratePuzzle(remove);
            });

            LoadingLabel.Text = "";
            selected  = null;
            won       = false;
            seconds   = 0;
            timer.Stop();
            timer.Start();

            DrawBoard();
            DrawNumpad();
            SetButtonsEnabled(true);
        }

        void SetButtonsEnabled(bool enabled)
        {
            BtnCafe.IsEnabled   = enabled;
            BtnNormal.IsEnabled = enabled;
            BtnDeploy.IsEnabled = enabled;
        }

        // ════════════════════════════════════════
        // DESENHAR TABULEIRO
        // ════════════════════════════════════════

        void DrawBoard()
        {
            SudokuGrid.Children.Clear();
            SudokuGrid.RowDefinitions.Clear();
            SudokuGrid.ColumnDefinitions.Clear();

            int size     = engine.Size;
            int cellSize = size == 9  ? 44 :
                           size == 16 ? 34 : 26;
            int fontSize = size == 9  ? 16 :
                           size == 16 ? 12 : 9;

            for (int i = 0; i < size; i++)
            {
                SudokuGrid.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(cellSize) });
                SudokuGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(cellSize) });
            }

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int row = r, col = c;
                    int val      = engine.Board[r, c];
                    bool isGiven = engine.IsGiven[r, c];
                    bool isError = val != 0 && !isGiven && !engine.IsCorrect(r, c);
                    bool isSel   = selected == (r, c);
                    bool isRelated = selected != null && !isSel && (
                        selected.Value.row == r ||
                        selected.Value.col == c ||
                        selected.Value.row / engine.BlockSize == r / engine.BlockSize &&
                        selected.Value.col / engine.BlockSize == c / engine.BlockSize);

                    var bg = isSel     ? "#E8A0A0"
                           : isRelated ? "#EDD8D8"
                                       : "#F5E6E6";

                    var border = new Border
                    {
                        Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(bg)),
                        BorderThickness = new Thickness(
                            c % engine.BlockSize == 0 ? 2 : 0.5,
                            r % engine.BlockSize == 0 ? 2 : 0.5,
                            c == size - 1             ? 2 : 0,
                            r == size - 1             ? 2 : 0),
                        BorderBrush = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#7A1A1A")),
                        Cursor = Cursors.Hand
                    };

                    if (val != 0)
                    {
                        border.Child = new TextBlock
                        {
                            Text = val.ToString(),
                            FontSize = fontSize,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment   = VerticalAlignment.Center,
                            Foreground = new SolidColorBrush(
                                isError ? Colors.Red :
                                isGiven ? Color.FromRgb(80, 30, 30) :
                                          Color.FromRgb(30, 30, 160))
                        };
                    }

                    border.MouseDown += (s, e) =>
                    {
                        selected = (row, col);
                        DrawBoard();
                    };

                    Grid.SetRow(border, r);
                    Grid.SetColumn(border, c);
                    SudokuGrid.Children.Add(border);
                }
            }
        }

        // ════════════════════════════════════════
        // DESENHAR NUMPAD
        // ════════════════════════════════════════

        void DrawNumpad()
        {
            NumPad.Children.Clear();

            for (int n = 1; n <= engine.Size; n++)
            {
                int num = n;
                var btn = new Button
                {
                    Content         = n.ToString(),
                    Width           = engine.Size == 9 ? 36 : 30,
                    Height          = engine.Size == 9 ? 36 : 30,
                    Margin          = new Thickness(2),
                    Background      = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#A03030")),
                    Foreground      = Brushes.White,
                    FontSize        = engine.Size == 9 ? 16 : 12,
                    FontWeight      = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Cursor          = Cursors.Hand
                };
                btn.Click += (s, e) => InputNumber(num);
                NumPad.Children.Add(btn);
            }

            var erase = new Button
            {
                Content         = "✕",
                Width           = engine.Size == 9 ? 36 : 30,
                Height          = engine.Size == 9 ? 36 : 30,
                Margin          = new Thickness(2),
                Background      = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#7A1A1A")),
                Foreground      = Brushes.White,
                FontSize        = 14,
                FontWeight      = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Cursor          = Cursors.Hand
            };
            erase.Click += (s, e) => InputNumber(0);
            NumPad.Children.Add(erase);
        }

        // ════════════════════════════════════════
        // INPUT
        // ════════════════════════════════════════

        void InputNumber(int num)
        {
            if (selected == null || won) return;
            var (r, c) = selected.Value;
            if (engine.IsGiven[r, c]) return;
            engine.SetValue(r, c, num);
            MessageLabel.Text = "";
            DrawBoard();
            CheckWin();
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (won) return;

            if (e.Key >= Key.D1 && e.Key <= Key.D9)
                InputNumber(e.Key - Key.D0);
            else if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9)
                InputNumber(e.Key - Key.NumPad0);
            else if (e.Key == Key.Delete || e.Key == Key.Back)
                InputNumber(0);

            if (selected == null) { selected = (0, 0); DrawBoard(); return; }
            var (r, c) = selected.Value;
            selected = e.Key switch
            {
                Key.Up    => (Math.Max(0, r - 1), c),
                Key.Down  => (Math.Min(engine.Size - 1, r + 1), c),
                Key.Left  => (r, Math.Max(0, c - 1)),
                Key.Right => (r, Math.Min(engine.Size - 1, c + 1)),
                _         => selected
            };
            DrawBoard();
        }

        // ════════════════════════════════════════
        // BOTÕES
        // ════════════════════════════════════════

        void BtnNovo_Click(object s, RoutedEventArgs e)    => StartNewGame();
        void BtnCafe_Click(object s, RoutedEventArgs e)    { difficulty = "Café com Leite"; StartNewGame(); }
        void BtnNormal_Click(object s, RoutedEventArgs e)  { difficulty = "Normal";         StartNewGame(); }
        void BtnDeploy_Click(object s, RoutedEventArgs e)  { difficulty = "Deploy Sexta";   StartNewGame(); }

        void BtnLimpar_Click(object s, RoutedEventArgs e)
        {
            engine.ClearBoard();
            selected = null;
            won = false;
            MessageLabel.Text = "";
            DrawBoard();
        }

        void BtnVerificar_Click(object s, RoutedEventArgs e)
        {
            for (int r = 0; r < engine.Size; r++)
                for (int c = 0; c < engine.Size; c++)
                    if (engine.Board[r, c] == 0)
                    {
                        MessageLabel.Foreground = Brushes.Yellow;
                        MessageLabel.Text = "⚠️ Ainda há células vazias!";
                        return;
                    }

            bool hasError = false;
            for (int r = 0; r < engine.Size; r++)
                for (int c = 0; c < engine.Size; c++)
                    if (!engine.IsCorrect(r, c)) hasError = true;

            MessageLabel.Foreground = hasError ? Brushes.Red : Brushes.LightGreen;
            MessageLabel.Text = hasError ? "❌ Há erros!" : "✅ Tudo certo!";
        }

        void BtnResolver_Click(object s, RoutedEventArgs e)
        {
            engine.Solve();
            won = true;
            timer.Stop();
            MessageLabel.Foreground = Brushes.LightGreen;
            MessageLabel.Text = "✅ Puzzle resolvido!";
            DrawBoard();
        }

        void CheckWin()
        {
            if (!engine.IsSolved()) return;
            won = true;
            timer.Stop();
            MessageLabel.Foreground = Brushes.Yellow;
            MessageLabel.Text = $"🎉 Parabéns! Resolvido em {FormatTime(seconds)}!";
        }

        string FormatTime(int s) => $"{s / 60:D2}:{s % 60:D2}";
    }
}
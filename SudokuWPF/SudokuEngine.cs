using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuWPF
{
    public class SudokuEngine
    {
        public int[,] Board    { get; private set; }
        public bool[,] IsGiven { get; private set; }
        private int[,] solution;

        public int Size      { get; private set; }
        public int BlockSize { get; private set; }

        private static Random rng = new Random();

        public SudokuEngine(int size = 9)
        {
            Size      = size;
            BlockSize = (int)Math.Sqrt(size);
            Board     = new int[size, size];
            IsGiven   = new bool[size, size];
            solution  = new int[size, size];
        }

        public void GeneratePuzzle(int cellsToRemove)
        {
            Board = new int[Size, Size];

            FillBoardByShift();

            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    solution[r, c] = Board[r, c];

            if (Size == 9)
                RemoveCellsUnique(cellsToRemove);
            else
                RemoveCellsFast(cellsToRemove);

            IsGiven = new bool[Size, Size];
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    IsGiven[r, c] = Board[r, c] != 0;
        }

        // ════════════════════════════════════════
        // PREENCHIMENTO POR ROTAÇÃO — O(n²) puro
        // ════════════════════════════════════════
        // Ideia: cada linha é um deslocamento da anterior
        // Linha 0: 1 2 3 4 5 6 7 8 9
        // Linha 1: 4 5 6 7 8 9 1 2 3  (desloca BlockSize)
        // Linha 2: 7 8 9 1 2 3 4 5 6  (desloca BlockSize)
        // Linha 3: 2 3 4 5 6 7 8 9 1  (desloca 1)
        // ...
        // Resultado é sempre válido matematicamente
        // Embaralhamos linhas/colunas dentro dos blocos para variar

        private void FillBoardByShift()
        {
            // Cria sequência base embaralhada
            int[] baseNums = ShuffledNumbers();

            for (int r = 0; r < Size; r++)
            {
                // Calcula o deslocamento para esta linha
                int blockRow = r / BlockSize;
                int rowInBlock = r % BlockSize;
                int shift = blockRow + rowInBlock * BlockSize;

                for (int c = 0; c < Size; c++)
                {
                    Board[r, c] = baseNums[(c + shift) % Size];
                }
            }

            // Embaralha linhas dentro de cada faixa de blocos
            ShuffleRowsWithinBands();

            // Embaralha colunas dentro de cada faixa de blocos
            ShuffleColsWithinBands();
        }

        // Embaralha linhas dentro de cada banda horizontal
        private void ShuffleRowsWithinBands()
        {
            for (int band = 0; band < BlockSize; band++)
            {
                int start = band * BlockSize;
                // Fisher-Yates nas linhas da banda
                for (int i = BlockSize - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    SwapRows(start + i, start + j);
                }
            }
        }

        // Embaralha colunas dentro de cada banda vertical
        private void ShuffleColsWithinBands()
        {
            for (int band = 0; band < BlockSize; band++)
            {
                int start = band * BlockSize;
                for (int i = BlockSize - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    SwapCols(start + i, start + j);
                }
            }
        }

        private void SwapRows(int r1, int r2)
        {
            for (int c = 0; c < Size; c++)
                (Board[r1, c], Board[r2, c]) = (Board[r2, c], Board[r1, c]);
        }

        private void SwapCols(int c1, int c2)
        {
            for (int r = 0; r < Size; r++)
                (Board[r, c1], Board[r, c2]) = (Board[r, c2], Board[r, c1]);
        }

        
        // REMOÇÃO DE CÉLULAS

        private void RemoveCellsUnique(int count)
        {
            var positions = new List<(int r, int c)>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    positions.Add((r, c));

            positions = positions.OrderBy(_ => rng.Next()).ToList();

            int removed = 0;
            foreach (var (r, c) in positions)
            {
                if (removed >= count) break;
                int backup = Board[r, c];
                Board[r, c] = 0;
                if (HasUniqueSolution()) removed++;
                else Board[r, c] = backup;
            }
        }

        private void RemoveCellsFast(int count)
        {
            var positions = new List<(int r, int c)>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    positions.Add((r, c));

            positions = positions.OrderBy(_ => rng.Next()).ToList();

            int removed = 0;
            foreach (var (r, c) in positions)
            {
                if (removed >= count) break;
                Board[r, c] = 0;
                removed++;
            }
        }

        
        // VALIDAÇÃO

        private int CountSolutions(int[,] grid, int limit = 2)
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                {
                    if (grid[r, c] != 0) continue;
                    int count = 0;
                    for (int num = 1; num <= Size; num++)
                    {
                        if (IsValidInGrid(grid, r, c, num))
                        {
                            grid[r, c] = num;
                            count += CountSolutions(grid, limit);
                            grid[r, c] = 0;
                            if (count >= limit) return count;
                        }
                    }
                    return count;
                }
            return 1;
        }

        private bool HasUniqueSolution()
        {
            int[,] copy = (int[,])Board.Clone();
            return CountSolutions(copy) == 1;
        }

        public bool IsValidMove(int row, int col, int num)
            => IsValidInGrid(Board, row, col, num);

        private bool IsValidInGrid(int[,] grid, int row, int col, int num)
        {
            for (int c = 0; c < Size; c++) if (grid[row, c] == num) return false;
            for (int r = 0; r < Size; r++) if (grid[r, col] == num) return false;

            int sr = (row / BlockSize) * BlockSize;
            int sc = (col / BlockSize) * BlockSize;
            for (int r = sr; r < sr + BlockSize; r++)
                for (int c = sc; c < sc + BlockSize; c++)
                    if (grid[r, c] == num) return false;

            return true;
        }

        public bool IsCorrect(int row, int col)
        {
            int val = Board[row, col];
            return val == 0 || val == solution[row, col];
        }

        public bool IsSolved()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (Board[r, c] != solution[r, c]) return false;
            return true;
        }

        public void Solve()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    Board[r, c] = solution[r, c];
        }

        public void SetValue(int row, int col, int value)
        {
            if (!IsGiven[row, col]) Board[row, col] = value;
        }

        public void ClearBoard()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (!IsGiven[r, c]) Board[r, c] = 0;
        }

        private int[] ShuffledNumbers()
        {
            int[] nums = Enumerable.Range(1, Size).ToArray();
            for (int i = Size - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (nums[i], nums[j]) = (nums[j], nums[i]);
            }
            return nums;
        }
    }
}
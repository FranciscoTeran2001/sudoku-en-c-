using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame
{
    internal class SudokuGenerator
    {
        
            private Random random;

            public SudokuGenerator()
            {
                random = new Random();
            }

            public int[,] GenerateSudoku(int difficulty)
            {
                int[,] sudoku = new int[9, 9];

                // Inicializa la matriz con ceros
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        sudoku[i, j] = 0;
                    }
                }

                // Fill in the diagonal sub-grids (3x3)
                FillDiagonalSubgrids(sudoku);

                // Fill the rest of the grid
                FillRemainingCells(sudoku);

                // Remove some numbers based on difficulty
                RemoveNumbersBasedOnDifficulty(sudoku, difficulty);

                return sudoku;
            }

            private void FillDiagonalSubgrids(int[,] sudoku)
            {
                for (int i = 0; i < 9; i += 3)
                {
                    FillSubgrid(sudoku, i, i);
                }
            }

            private void FillSubgrid(int[,] sudoku, int startRow, int startCol)
            {
                List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                ShuffleList(numbers);

                int index = 0;
                for (int i = startRow; i < startRow + 3; i++)
                {
                    for (int j = startCol; j < startCol + 3; j++)
                    {
                        sudoku[i, j] = numbers[index];
                        index++;
                    }
                }
            }

            private void FillRemainingCells(int[,] sudoku)
            {
                FillRemainingCellsRecursive(sudoku, 0, 3);
            }

            private bool FillRemainingCellsRecursive(int[,] sudoku, int row, int col)
            {
            if (row == 9)
            {
                return true; // All cells filled
            }

            if (sudoku[row, col] != 0)
            {
                int nextRow = row;
                int nextCol = col + 1;
                if (nextCol == 9)
                {
                    nextRow++;
                    nextCol = 0;
                }
                return FillRemainingCellsRecursive(sudoku, nextRow, nextCol);
            }

            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ShuffleList(numbers);

            foreach (int num in numbers)
            {
                if (IsValidPlacement(sudoku, row, col, num))
                {
                    sudoku[row, col] = num;

                    int nextRow = row;
                    int nextCol = col + 1;
                    if (nextCol == 9)
                    {
                        nextRow++;
                        nextCol = 0;
                    }

                    if (FillRemainingCellsRecursive(sudoku, nextRow, nextCol))
                    {
                        return true;
                    }

                    sudoku[row, col] = 0; // Backtrack
                }
            }

            return false;

        }

        private bool IsValidPlacement(int[,] sudoku, int row, int col, int num)
            {
                // Check row and column
                for (int i = 0; i < 9; i++)
                {
                    if (sudoku[row, i] == num || sudoku[i, col] == num)
                    {
                        return false;
                    }
                }

                // Check 3x3 subgrid
                int subgridStartRow = row - row % 3;
                int subgridStartCol = col - col % 3;
                for (int i = subgridStartRow; i < subgridStartRow + 3; i++)
                {
                    for (int j = subgridStartCol; j < subgridStartCol + 3; j++)
                    {
                        if (sudoku[i, j] == num)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            private void RemoveNumbersBasedOnDifficulty(int[,] sudoku, int difficulty)
            {
                int cellsToRemove = 0;
                switch (difficulty)
                {
                    case 0: cellsToRemove = 35; break; // Easy
                    case 1: cellsToRemove = 45; break; // Medium
                    case 2: cellsToRemove = 55; break; // Hard
                }

                for (int i = 0; i < cellsToRemove; i++)
                {
                    int row = random.Next(0, 9);
                    int col = random.Next(0, 9);
                    sudoku[row, col] = 0;
                }
            }

            private void ShuffleList<T>(List<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        
    }
}

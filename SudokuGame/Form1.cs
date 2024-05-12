using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuGame
{
    public partial class Form1 : Form
    {
        private SudokuGenerator sudokuGenerator;
        private int[,] currentSudoku;
        private int[,] initialSudoku;
        private int[,] userSudoku;
        private TextBox[,] textBoxGrid = new TextBox[9, 9];
        private TextBox selectedTextBox;
        public Form1()
        {
            InitializeComponent();
            sudokuGenerator = new SudokuGenerator();
            InitializeTextBoxGrid(); 
            InitializeDifficultyComboBox();
        }
        private void InitializeTextBoxGrid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    string textBoxName = $"textBox_{row}_{col}";
                    TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                    if (textBox != null)
                    {
                        textBoxGrid[row, col] = textBox; // Usar la variable miembro textBoxGrid
                        textBox.ReadOnly = true;
                        textBox.Click += TextBox_Click; // Adjuntar el controlador de eventos
                    }
                    else
                    {
                        // Manejar el caso en que no se encuentra el TextBox con el nombre esperado
                        // Esto puede ser útil para depuración o para manejar errores en tiempo de ejecución
                    }
                }
            }
        }
        private void InitializeDifficultyComboBox()
        {
            difficultyComboBox.Items.Add("Easy");
            difficultyComboBox.Items.Add("Medium");
            difficultyComboBox.Items.Add("Hard");
            difficultyComboBox.SelectedIndex = 0; // Set default selection
        }
        private void SetCellColors()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (currentSudoku[row, col] != 0)
                    {
                        textBoxGrid[row, col].BackColor = SystemColors.Window; // Cambiar color de fondo para celdas bloqueadas
                    }
                    else
                    {
                        textBoxGrid[row, col].BackColor = Color.LightGray; // Cambiar color de fondo para celdas vacías
                    }
                }
            }
        }
        
private void GenerateSudoku()
        {
            int difficulty = difficultyComboBox.SelectedIndex; // 0: Easy, 1: Medium, 2: Hard

            currentSudoku = sudokuGenerator.GenerateSudoku(difficulty);

            // Clonar currentSudoku para la interacción del usuario
            userSudoku = (int[,])currentSudoku.Clone();

            // Clonar currentSudoku para el Sudoku inicial
            initialSudoku = (int[,])currentSudoku.Clone();

            SetCellColors(); // Cambiar los colores de fondo de las celdas bloqueadas y vacías
            UpdateTextBoxGrid(); // Actualizar los TextBox después de generar el Sudoku
        }
        



        

        private void UpdateTextBoxGrid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (textBoxGrid[row, col] != null && currentSudoku != null && initialSudoku != null)
                    {
                        textBoxGrid[row, col].Text = currentSudoku[row, col] == 0 ? "" : currentSudoku[row, col].ToString();
                        textBoxGrid[row, col].ReadOnly = initialSudoku[row, col] != 0;

                        if (initialSudoku[row, col] != 0)
                        {
                            textBoxGrid[row, col].Click -= TextBox_Click; // Desactivar el evento Click
                        }
                        else
                        {
                            textBoxGrid[row, col].Click += TextBox_Click; // Activar el evento Click

                            textBoxGrid[row, col].BackColor = Color.LightGray; // Cambiar color de fondo para celdas no bloqueadas
                        }
                    }
                }
            }
        }


        private void numberButton_Click(object sender, EventArgs e)
        {
            Button numberButton = sender as Button;
            if (selectedTextBox != null && numberButton != null)
            {
                selectedTextBox.Text = numberButton.Text;
            }
        }

        private void ClearSudokuGrid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    currentSudoku[row, col] = 0; // Establecer el valor en 0 para borrarlo
                    userSudoku[row, col] = 0; // También borra la entrada del usuario
                    textBoxGrid[row, col].Text = ""; // Borrar el texto en el TextBox
                    textBoxGrid[row, col].ReadOnly = false; // Permitir la edición nuevamente
                }
            }
        }
        private void deleteButton_Click(object sender, EventArgs e)
        {
            ClearTextBoxBackgroundColors();
            ClearSudokuGrid();
        
        }
        private bool IsProgressCorrect(TextBox[,] textBoxGrid)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    string value = textBoxGrid[row, col].Text;

                    if (!string.IsNullOrEmpty(value))
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (i != col && textBoxGrid[row, i].Text == value)
                            {
                                return false; // Número repetido en la misma fila
                            }
                            if (i != row && textBoxGrid[i, col].Text == value)
                            {
                                return false; // Número repetido en la misma columna
                            }
                        }

                        int startRow = row - row % 3;
                        int startCol = col - col % 3;
                        for (int i = startRow; i < startRow + 3; i++)
                        {
                            for (int j = startCol; j < startCol + 3; j++)
                            {
                                if ((i != row || j != col) && textBoxGrid[i, j].Text == value)
                                {
                                    return false; // Número repetido en el mismo subgrid
                                }
                            }
                        }
                    }
                }
            }

            return true; // No se encontraron repeticiones
        }
        private void CheckProgressAndHighlightErrors()
        {
            bool progressCorrect = IsProgressCorrect(textBoxGrid);

            if (progressCorrect)
            {
                MessageBox.Show("¡Vas por buen camino!", "Progreso Correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Hay números repetidos en filas, columnas o subgrids. Revisa tu solución.", "Progreso Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (!IsTextBoxValueCorrect(row, col))
                        {
                            textBoxGrid[row, col].BackColor = Color.Red; // Resaltar el TextBox incorrecto en rojo
                        }
                    }
                }
            }
        }

        private bool IsTextBoxValueCorrect(int row, int col)
        {
            string value = textBoxGrid[row, col].Text;

            if (string.IsNullOrEmpty(value))
            {
                return true; // Celda vacía, no hay error
            }

            // Verificar fila y columna
            for (int i = 0; i < 9; i++)
            {
                if (i != col && textBoxGrid[row, i].Text == value)
                {
                    return false; // Número repetido en la misma fila
                }
                if (i != row && textBoxGrid[i, col].Text == value)
                {
                    return false; // Número repetido en la misma columna
                }
            }

            // Verificar subgrid 3x3
            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if ((i != row || j != col) && textBoxGrid[i, j].Text == value)
                    {
                        return false; // Número repetido en el mismo subgrid
                    }
                }
            }

            return true; // No se encontraron repeticiones
        }
        private void ClearTextBoxBackgroundColors()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (!textBoxGrid[row, col].ReadOnly)
                    {
                        textBoxGrid[row, col].BackColor = SystemColors.Window; // Restaurar el color de fondo predeterminado
                    }

                    if (!string.IsNullOrEmpty(textBoxGrid[row, col].Text) && textBoxGrid[row, col].BackColor == Color.Red)
                    {
                        textBoxGrid[row, col].BackColor = SystemColors.Window; // Borrar el color rojo de las celdas incorrectas
                    }
                }
            }

        }

        private void checkButton_Click(object sender, EventArgs e)
        {
            ClearTextBoxBackgroundColors();
            SetCellColors();
            CheckProgressAndHighlightErrors();
        }

      
        private bool SolveSudoku(int[,] sudoku)
        {
            int row, col;

            if (!FindEmptyCell(sudoku, out row, out col))
            {
                // No quedan celdas vacías, el Sudoku está resuelto
                return true;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(sudoku, row, col, num))
                {
                    sudoku[row, col] = num;

                    if (SolveSudoku(sudoku))
                    {
                        return true;
                    }

                    sudoku[row, col] = 0; // Backtrack
                }
            }

            return false;
        }

        private bool FindEmptyCell(int[,] sudoku, out int row, out int col)
        {
            for (row = 0; row < 9; row++)
            {
                for (col = 0; col < 9; col++)
                {
                    if (sudoku[row, col] == 0)
                    {
                        return true;
                    }
                }
            }

            row = -1; // Establecer valores por defecto en caso de no encontrar celda vacía
            col = -1;
            return false;
        }

        private bool IsSafe(int[,] sudoku, int row, int col, int num)
        {
            // Verificar fila y columna
            for (int i = 0; i < 9; i++)
            {
                if (sudoku[row, i] == num || sudoku[i, col] == num)
                {
                    return false;
                }
            }

            // Verificar subgrid 3x3
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

        private void resetButton_Click(object sender, EventArgs e)
        {
            int[,] sudokuToSolve = new int[9, 9]; // Aquí debes proporcionar la matriz del Sudoku que deseas resolver

            // Copiar el contenido de los TextBox a la matriz sudokuToSolve
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (int.TryParse(textBoxGrid[row, col].Text, out int value))
                    {
                        sudokuToSolve[row, col] = value;
                    }
                    else
                    {
                        sudokuToSolve[row, col] = 0;
                    }
                }
            }

            if (SolveSudoku(sudokuToSolve))
            {
                // Si se resolvió correctamente, actualiza los TextBox con la solución
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        textBoxGrid[row, col].Text = sudokuToSolve[row, col].ToString();
                    }
                }

                MessageBox.Show("¡Sudoku resuelto!", "Sudoku Resuelto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo resolver el Sudoku.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Exit the application
        }



        private void TextBox_Click(object sender, EventArgs e)
        {
            selectedTextBox = sender as TextBox;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            GenerateSudoku();
        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox67_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

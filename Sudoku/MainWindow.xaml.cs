using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[,] sudoku = new string[9, 9];
        public string[,] history = new string[9, 9];
        public string[,] first = new string[9, 9];
        public bool isImport = false;
        public int x = 0;
        public int y = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitSudoku();
            InitSudokuButton();
            InitInputButton();

            Status.Text = "准备就绪";
        }

        private void InitInputButton()
        {
            for (int i = 0; i < InputPanel.Children.Count; i++)
            {
                if (InputPanel.Children[i] is Button b)
                {
                    b.Click += new RoutedEventHandler(OnInputButtonClick);
                }
            }
        }

        private void OnInputButtonClick(object sender, RoutedEventArgs e)
        {
            if (x != 0 && y != 0)
            {
                Array.Copy(sudoku, history, sudoku.Length);
                Button b = (Button)sender;
                string[] name = b.Name.Split('_');
                sudoku[x - 1, y - 1] = name[1];
                UpdateTable();

                if (IsFull())
                {
                    if (IsCorrect())
                    {
                        Solve.Content = "重置数独";
                        Status.Text = "Congratulations!";
                        MessageBox.Show("你完成了一个数独", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Solve.Content = "求解与验证";
                        Status.Text = "数独解得不对哦";
                    }
                }
            }
        }

        private void InitSudokuButton()
        {
            for (int i = 0; i < SudokuPanel.Children.Count; i++)
            {
                if (SudokuPanel.Children[i] is Grid g)
                {
                    for (int j = 0; j < g.Children.Count; j++)
                    {
                        if (g.Children[j] is Button b)
                        {
                            b.Click += new RoutedEventHandler(OnSudokuButtonClick);
                        }
                    }
                }
            }
        }

        private void OnSudokuButtonClick(object sender, RoutedEventArgs e)
        {
            RemoveAllSudokuButtonStyle();
            Button b = (Button)sender;
            b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBEE6FD"));
            b.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF424242"));

            string[] name = b.Name.Split('_');
            x = Int32.Parse(name[1]);
            y = Int32.Parse(name[2]);
            Status.Text = $"已选中 [{x},{y}]";
        }

        private void RemoveAllSudokuButtonStyle()
        {
            for (int i = 0; i < SudokuPanel.Children.Count; i++)
            {
                if (SudokuPanel.Children[i] is Grid g)
                {
                    for (int j = 0; j < g.Children.Count; j++)
                    {
                        if (g.Children[j] is Button b)
                        {
                            b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                            b.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0E0E0"));
                        }
                    }
                }
            }
        }

        private void InitSudoku()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudoku[i, j] = " ";
                    history[i, j] = " ";
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog import = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "数独初盘文本文件（*.csv）|*.csv"
            };


            if (import.ShowDialog().GetValueOrDefault())
            {
                Status.Text = "已读入文件";
                string data = File.ReadAllText(import.FileName);
                string[] table = data.Split(new char[] { ',', '\n' });

                int count = 0;
                Array.Copy(sudoku, history, sudoku.Length);

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {

                        sudoku[i, j] = table[count];
                        count++;
                    }
                }

                UpdateTable();
                Solve.Content = "求解与验证";
                Array.Copy(sudoku, first, sudoku.Length);
                isImport = true;
            }
        }

        private string[] GetRandomArray()
        {
            Random r = new Random();
            string[] arr = new string[9] { " ", " ", " ", " ", " ", " ", " ", " ", " ", };
            int count = 1;
            while (count <= 9)
            {
                int num = r.Next(0, 9);
                if (arr[num] == " ")
                {
                    arr[num] = count.ToString();
                    count++;
                }

            }
            return arr;
        }

        private void UpdateTable()
        {
            for (int i = 0; i < this.SudokuPanel.Children.Count; i++)
            {
                if (SudokuPanel.Children[i] is Grid)
                {
                    Grid g = (Grid)this.SudokuPanel.Children[i];
                    for (int j = 0; j < g.Children.Count; j++)
                    {
                        if (g.Children[j] is Button b)
                        {
                            int x = Int32.Parse(b.Name.Split("_")[1]) - 1;
                            int y = Int32.Parse(b.Name.Split("_")[2]) - 1;
                            b.Content = sudoku[x, y].Trim();
                        }
                    }
                }
            }
        }

        private string[] GetRow(int x)
        {
            string[] row = new string[9];
            for (int i = 0; i < 9; i++)
                row[i] = (sudoku[x, i]);

            return row;
        }

        private string[] GetColumn(int y)
        {
            string[] column = new string[9];
            for (int i = 0; i < 9; i++)
                column[i] = (sudoku[i, y]);

            return column;
        }

        private string[] GetBox(int x, int y)
        {
            int dx = (int)(x / 3);
            int dy = (int)(y / 3);

            string[] arr = new string[9];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    arr[i * 3 + j] = (sudoku[i + dx * 3, j + dy * 3]);

            }

            return arr;
        }

        private static int[] GetPossibleNumber(string[] row, string[] col, string[] box)
        {
            int[] arr = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < 9; i++)
            {
                if (row[i].Trim() != "")
                    arr[Int32.Parse(row[i]) - 1]++;

                if (col[i].Trim() != "")
                    arr[Int32.Parse(col[i]) - 1]++;

                if (box[i].Trim() != "")
                    arr[Int32.Parse(box[i]) - 1]++;
            }

            ArrayList res = new ArrayList();
            for (int i = 0; i < 9; i++)
            {
                if (arr[i] == 0)
                    res.Add(i + 1);
            }

            return (int[])res.ToArray(typeof(int));
        }

        private void SolveSudoku()
        {
            Find(0, 0);

            void Find(int x, int y)
            {
                for (int i = x; i < 9; i++)
                {
                    int z = i == x ? y : 0;
                    for (int j = z; j < 9; j++)
                    {
                        if (sudoku[i, j].Trim() == "")
                        {
                            string[] row = GetRow(i);
                            string[] col = GetColumn(j);
                            string[] box = GetBox(i, j);
                            int[] result = GetPossibleNumber(row, col, box);

                            switch (result.Length)
                            {
                                case 0:
                                    {
                                        return;
                                    }
                                case 1:
                                    {
                                        sudoku[i, j] = result[0].ToString();
                                        Find(i, j);


                                        if (sudoku[8, 8].Trim() == "")
                                            sudoku[i, j] = " ";
                                        else
                                            return;

                                        return;
                                    }
                                default:
                                    {
                                        for (int k = 0; k < result.Length; k++)
                                        {
                                            sudoku[i, j] = result[k].ToString();
                                            Find(i, j);


                                            if (sudoku[8, 8].Trim() == "")
                                                sudoku[i, j] = " ";
                                            else
                                                return;

                                        }
                                        return;
                                    }
                            }
                        }
                    }
                }
                return;
            }

        }

        private bool IsFull()
        {
            int count = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudoku[i, j].Trim() == "")
                        count++;
                }
            }

            if (count == 0)
                return true;
            return false;
        }

        private bool IsCorrect()
        {
            for (int i = 0; i < 9; i++)
            {
                HashSet<int> test = new HashSet<int>();
                string[] row = GetRow(i);
                for (int j = 0; j < 9; j++)
                {
                    if (row[j].Trim() != "")
                    {
                        test.Add(Int32.Parse(row[j]));
                    }
                }
                if (test.Count != 9)
                {
                    return false;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                HashSet<int> test = new HashSet<int>();
                string[] col = GetColumn(i);
                for (int j = 0; j < 9; j++)
                {
                    if (col[j].Trim() != "")
                    {
                        test.Add(Int32.Parse(col[j]));
                    }
                }
                if (test.Count != 9)
                {
                    return false;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    HashSet<int> test = new HashSet<int>();
                    string[] box = GetBox(i, j);
                    for (int k = 0; k < 9; k++)
                    {
                        if (box[k].Trim() != "")
                        {
                            test.Add(Int32.Parse(box[k]));
                        }
                    }
                    if (test.Count != 9)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsReasonable()
        {
            for (int i = 0; i < 9; i++)
            {
                HashSet<int> test = new HashSet<int>();
                string[] row = GetRow(i);
                int count = 0;
                for (int j = 0; j < 9; j++)
                {
                    if (row[j].Trim() != "")
                    {
                        test.Add(Int32.Parse(row[j]));
                        count++;
                    }
                }
                if (test.Count != count)
                {
                    return false;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                HashSet<int> test = new HashSet<int>();
                string[] col = GetColumn(i);
                int count = 0;
                for (int j = 0; j < 9; j++)
                {
                    if (col[j].Trim() != "")
                    {
                        test.Add(Int32.Parse(col[j]));
                        count++;
                    }
                }
                if (test.Count != count)
                {
                    return false;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    HashSet<int> test = new HashSet<int>();
                    string[] box = GetBox(i, j);
                    int count = 0;
                    for (int k = 0; k < 9; k++)
                    {
                        if (box[k].Trim() != "")
                        {
                            test.Add(Int32.Parse(box[k]));
                            count++;
                        }
                    }
                    if (test.Count != count)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            Status.Text = "正在生成";
            Array.Copy(sudoku, history, sudoku.Length);

            const int blank = 40;

            do
            {
                InitSudoku();

                for (int i = 0; i < 3; i++)
                {
                    string[] temp = GetRandomArray();
                    int p = 0;
                    for (int j = 0 + (i * 3); j < 3 + (i * 3); j++)
                    {
                        for (int k = 0 + (i * 3); k < 3 + (i * 3); k++)
                        {
                            sudoku[j, k] = temp[p];
                            p++;
                        }
                    }
                }

                SolveSudoku();

            } while (sudoku[8, 5] == " ");

            DigHole(blank);
            UpdateTable();
            isImport = false;
            Status.Text = "已生成数独";
            Solve.Content = "求解与验证";
        }

        private void DigHole(int blank)
        {
            int num = blank;
            do
            {
                Random rx = new Random();
                Random ry = new Random();
                int x = rx.Next(0, 9);
                int y = ry.Next(0, 9);
                if (sudoku[x, y] != " ")
                {
                    string[,] copy = new string[9, 9];
                    Array.Copy(sudoku, copy, sudoku.Length);
                    sudoku[x, y] = " ";
                    SolveSudoku();
                    if (IsFull())
                    {
                        copy[x, y] = " ";
                        num--;
                    }
                    Array.Copy(copy, sudoku, sudoku.Length);
                }
            } while (num > 0);
        }

        private string GetText()
        {
            string res = "";
            for (int i = 0; i < 9; i++)
            {
                string row = "";
                for (int j = 0; j < 9; j++)
                    if (j == 0)
                        row = sudoku[i, j];
                    else
                        row = row + "," + sudoku[i, j];


                if (i == 0)
                    res += row;
                else
                    res = res + "\n" + row;
            }

            return res;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string res = GetText();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, res);
                Status.Text = "已保存文件";
            }
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            if (!IsReasonable())
            {
                MessageBox.Show("这个数独可能不合理，因为某一行、列或者九宫格中存在重复的数字。", "Tips", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (!IsFull())
            {
                Array.Copy(sudoku, history, sudoku.Length);

                if (isImport)
                {
                    Array.Copy(first, sudoku, sudoku.Length);
                }

                SolveSudoku();

                if (!IsCorrect())
                {
                    Status.Text = "求解失败";
                    MessageBox.Show("这个数独可能无解", "求解结果", MessageBoxButton.OK, MessageBoxImage.Error);
                    Array.Copy(history, sudoku, sudoku.Length);
                    UpdateTable();
                }
                else
                {
                    UpdateTable();
                    Status.Text = "已尝试求解";
                    Solve.Content = "重置数独";
                }
            }
            else if (IsCorrect())
            {
                InitSudoku();
                UpdateTable();
                isImport = false;
                Status.Text = "准备就绪";
                Solve.Content = "求解与验证";
            }
        }

        private void Revoke_Click(object sender, RoutedEventArgs e)
        {
            Array.Copy(history, sudoku, sudoku.Length);
            UpdateTable();
            Status.Text = "已撤销操作";
            if (!IsFull())
            {
                Solve.Content = "求解与验证";
            }
            else
            {
                Solve.Content = "重置数独";
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (x != 0 && y != 0 && sudoku[x - 1, y - 1].Trim() != "")
            {
                Array.Copy(sudoku, history, sudoku.Length);
                string num = sudoku[x - 1, y - 1];
                sudoku[x - 1, y - 1] = "";
                UpdateTable();
                Status.Text = $"已删除 [{x - 1},{y - 1}]={num}";
            }
        }
    }
}

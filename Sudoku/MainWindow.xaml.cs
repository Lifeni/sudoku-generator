using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.Permissions;
using System.Text;
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

        public MainWindow()
        {
            InitializeComponent();
            InitSudoku();

            Status.Text = "准备就绪";
        }

        private void InitSudoku()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudoku[i, j] = " ";
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
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {

                        sudoku[i, j] = table[count];
                        count++;
                    }
                }

                UpdateTable();
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
                            b.Content = sudoku[x, y];
                        }
                    }
                }
            }
        }

        private void SolveSudoku()
        {
            Find(0, 0);

            string[] GetRow(int x)
            {
                string[] row = new string[9];
                for (int i = 0; i < 9; i++)
                    row[i] = (sudoku[x, i]);

                return row;
            }

            string[] GetColumn(int y)
            {
                string[] column = new string[9];
                for (int i = 0; i < 9; i++)
                    column[i] = (sudoku[i, y]);

                return column;
            }

            string[] GetBox(int x, int y)
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

            int[] GetPossibleNumber(string[] row, string[] col, string[] box)
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
                            //MessageBox.Show(i + " " + j + " " + String.Join(',', result));

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

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            InitSudoku();
            Status.Text = "正在生成";

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

                GetText();

            } while (sudoku[8, 5] == " ");

            UpdateTable();
            Status.Text = "已生成数独";
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

            //MessageBox.Show(res);
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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Plan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static int agr_num = 5;     //число агрегатов
        public static int C_length = 40;   //длина вектора C (число параметров вектора X)
        public static int week_num = 8;    //число дней календарного плана
        public static int delta;           //начальная величина максимального колебания
        public static int Exitflag;        //начальная величина максимального колебания
        public static int[,] aij;          //матрица затрат
        public static int[,] startij;      //матрица раннего/позднего старта (2х5)

        int i = 1;//счетчик агрегатов                 
        public static int[,] A =new int[week_num + 1, C_length + 1];       //матрица А
        public static int[,] Aeq = new int[agr_num + 2, C_length + 1];     //матрица Аeq (+доп. строка р/п начал)
        public static int[,] Result = new int[agr_num + 20, C_length + 1]; //матрица чтения текстовых файлов
        public static int[,] Xij = new int[week_num + 1, C_length + 1];    //матрица оптимального старта Xij
        public static int[,] aij_test = new int[6, 4];        //матрица условий задачи
        public static int[,] startij_test = new int[6, 3];    //матрица условий задачи
        public static int[] b = new int[week_num + 1];  //вектор b правых частей неравенств
        public static int[] beq = new int[agr_num + 2]; //вектор beq правых частей равенств
        public static int[] C = new int[C_length + 1];  //вектор С
        public static int[] s = new int[C_length + 1];  //строка р/п начал матрицы Aeq

        // конструктор строки матрицы неравенств A
        public int Сhoice_a_part(int n, int k, int nn, int kk)
        {
            if (k == n-kk || k == n-kk + week_num || k == n-kk + 2 * week_num || k == n-kk + 3 * week_num || k == n-kk + 4 * week_num && i <= agr_num)
            { i++; return aij[i - 1, 1 + nn]; }
            else { return A[n, k]; }
        }
        // конструктор строки вектора коэффициентов С
        public void Vector_С()
        {
            int k = 1;
            for(int i = 1; i <= agr_num; i++)
            {
                for(int j = 1; j <= 6; j++)
                {
                    C[k]= aij[i, 1] + aij[i, 2]+ aij[i, 3];//Xi1-Xi6
                    k++;
                }
                C[k] = aij[i, 1] + aij[i, 2];//Xi7
                k++;
                C[k] = aij[i, 1];//Xi8
                k++;
            }
        }
               
        // конструктор  матрицы неравенств A
        public void Matrix_A()
        {        
            // n-номер неравенства

            i = 1;//счетчик агрегатов
            for (int n = 1; n <= week_num; n++)
            {
                if (n == 1)
                {
                    int kk = 0; int nn = 0;
                    i = 1;//счетчик агрегатов
                    for (int k = 1; k <= C_length; k++) { A[n, k] = Сhoice_a_part(n, k, nn, kk); }
                }
                else
                if (n == 2)
                {
                    int kk = 0; int nn = 0;
                    i = 1;//счетчик агрегатов
                    for (int k = 1; k <= C_length; k++) { A[n, k] = Сhoice_a_part(n, k, nn, kk); }

                    kk = 1; nn = 1;
                    i = 1;//счетчик агрегатов
                    for (int k = 1; k <= C_length; k++) { A[n, k] = Сhoice_a_part(n, k, nn, kk); }
                }
                else
                if (n > 2)
                {
                    int kk = 0; int nn = 0;
                    i = 1;//счетчик агрегатов
                    for (int k = 1; k <= C_length; k++) { A[n, k] = Сhoice_a_part(n, k, nn, kk); }

                    kk = 1; nn = 1;
                    i = 1;//счетчик агрегатов
                    for (int k = 1; k <= C_length; k++) { A[n, k] = Сhoice_a_part(n, k, nn, kk); }

                    kk = 2; nn = 2;
                    i = 1;//счетчик агрегатов
                    for (int k = 1; k <= C_length; k++) { A[n, k] = Сhoice_a_part(n, k, nn, kk); }
                }
            }
        }
        // конструктор вектора b
        public void Vector_b()
        {
            for (int n = 1; n <= week_num; n++)
            {
                b[n] = delta;
            }
        }
        // конструктор вектора beq
        public void Vector_beq()
        {
            for (int n = 1; n <= agr_num+1; n++)
            {
                if (n<=agr_num) beq[n] = 1;
            }
        }

        // конструктор строки условий р/п начал матрицы равенств Aeq 
        public void A_s()
        {
            int k = 1;

            for (int i = 1; i <= agr_num; i++) { 
            for (int n = 1; n <= week_num; n++)
            {
                    if (n >= startij[i, 1] && n <= startij[i, 2]) { s[k] = 0; k++; } else { s[k] = 1; k++; };
            }
                                               }
        }
        // конструктор Матрицы Aeq
        public void Matrix_Aeq()
        {           
            int j = 1;
            for (int i = 1; i <= agr_num; i++)
            {
                for (int k = 1; k <= C_length; k++)
                {
                    if (k > (j - 1) * week_num && k <= j * week_num) { Aeq[i, k] = 1; };                   
                }
            j++;
            }

        }

        // вывод в текстовый файл матрицы А

        public void Write_A()
        {
            FileStream fStream = new FileStream("A.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fStream, System.Text.Encoding.Default);

            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            for (int wn = 1; wn <= week_num; wn++)
            {
                for (int k = 1; k <= C_length; k++)
                {
                    sw.Write(A[wn, k].ToString("G", ci) + " ");
                }
                sw.Write("\r\n");
            }
            sw.Close();
        }

        public void Write_aij()
        {
            FileStream fStream = new FileStream("aij.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fStream, System.Text.Encoding.Default);

            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            for (int wn = 0; wn < 6; wn++)
            {
                for (int k = 0; k < 4; k++)
                {
                    sw.Write(aij[wn, k].ToString("G", ci) + " ");
                }
                sw.Write("\r\n");
            }
            sw.Close();
        }

        public void Write_Aeq()
        {
            FileStream fStream = new FileStream("Aeq.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fStream, System.Text.Encoding.Default);

            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            for (int wn = 1; wn <= agr_num; wn++)
            {
                for (int k = 1; k <= C_length; k++)
                {
                    sw.Write(Aeq[wn, k].ToString("G", ci) + " ");
                }
                if (wn < agr_num) sw.Write("\r\n"); // не выводим пустую последнюю строку
            }
            sw.Close();
        }

        public void Write_b()
        {
            FileStream fStream = new FileStream("b.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fStream, System.Text.Encoding.Default);

            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            for (int wn = 1; wn <= week_num; wn++)
            {               
                    sw.Write(b[wn].ToString("G", ci) + "\r\n");                            
            }
            sw.Close();
        }

        public void Write_beq()
        {
            FileStream fStream = new FileStream("beq.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fStream, System.Text.Encoding.Default);

            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            for (int wn = 1; wn <= agr_num+1; wn++)
            {
                sw.Write(beq[wn].ToString("G", ci) + "\r\n");
            }
            sw.Close();
        }

        public void Write_C()
        {
            FileStream fStream = new FileStream("C.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fStream, System.Text.Encoding.Default);
            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            
            for (int wn = 1; wn <= C_length; wn++)
            {
                sw.Write(C[wn].ToString("G", ci) + "\r\n");
            }
            sw.Close();
        }

        public void Write_add_line()
        {           
            TextWriter tw = new StreamWriter("Aeq.txt", true);
            CultureInfo ci = new CultureInfo("en-us");  //для вывода десятичного разделителя в числе в виде точки (матлаб не понимает запятую)
            tw.Write("\r\n");
            for (int wn = 1; wn <= C_length; wn++)
            {
                tw.Write(s[wn].ToString("G", ci) + " ");
            }
            tw.Close();
        }       

        public void Read(string file_name)
        {
            // считываем данные из текстовых файлов

            String input = "";
            CultureInfo ci = new CultureInfo("en-us"); // десятичный разделитель - точка          
           
            input = File.ReadAllText(file_name);
            
            int i = 0, j = 0;           
            foreach (var row in input.Split('\n'))
            {
                j = 0;
                foreach (var col in row.Trim().Split(' '))
                {
                    try // предохранитель от пустой строки в считываемом файле
                    { Result[i, j] = int.Parse(col.Trim(), ci); }
                    catch { Result[i, j] = 0; }
                    j++;
                }
                i++;
            }
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private bool Is_aij_equal()  //сравниваем массивы
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (aij_test[i, j] != aij[i, j])
                    {
                        return true;                       
                    }
                }
            }
            return false;
        }

       

        private bool Is_startij_equal()  //сравниваем массивы
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (startij_test[i, j] != startij[i, j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public void button1_Click(object sender, EventArgs e)
        {
            //массив расходов
            aij = new int[,]{ { 0, 0, 0, 0 },
                           { 0, Int32.Parse(n11.Text), Int32.Parse(n12.Text), Int32.Parse(n13.Text) },
                           { 0, Int32.Parse(n21.Text), Int32.Parse(n22.Text), Int32.Parse(n23.Text) },
                           { 0, Int32.Parse(n31.Text), Int32.Parse(n33.Text), Int32.Parse(n33.Text) },
                           { 0, Int32.Parse(n41.Text), Int32.Parse(n42.Text), Int32.Parse(n43.Text) },
                           { 0, Int32.Parse(n51.Text), Int32.Parse(n52.Text), Int32.Parse(n53.Text) },
                         };
            //матрица раннего/позднего старта (2х5)
            startij = new int[,]{ { 0, 0, 0},
                           { 0, Int32.Parse(start11.Text), Int32.Parse(start12.Text)},
                           { 0, Int32.Parse(start21.Text), Int32.Parse(start22.Text)},
                           { 0, Int32.Parse(start31.Text), Int32.Parse(start33.Text)},
                           { 0, Int32.Parse(start41.Text), Int32.Parse(start42.Text)},
                           { 0, Int32.Parse(start51.Text), Int32.Parse(start52.Text)},
                         };

            Matrix_A();
            Matrix_Aeq();
            Vector_b();
            Vector_beq();
            Vector_С();
            A_s();          
            Write_A();
            Write_Aeq();
            Write_add_line();
            //Write_aij();

            Write_beq();
            Write_C();
            Exitflag = -2;

            delta = aij.Cast<int>().Max(); //возвращает макс. элемент массива затрат
            for (int ii=1; ii<= week_num; ii++)//формируем вектор b
            {
                b[ii] = delta;
            }
            Write_b();

            Read("aij_test.txt");//считываем исходные данные задачи            
            for (int wn = 0; wn < 6; wn++)
            {
                for (int k = 0; k < 4; k++)
                {
                    aij_test[wn, k] = Result[wn, k];
                }
            }

            Read("startij_test.txt");
            for (int wn = 0; wn < 6; wn++)
            {
                for (int k = 0; k < 3; k++)
                {
                    startij_test[wn, k] = Result[wn, k];
                }
            }


            if (Is_aij_equal() || Is_startij_equal()) //если массивы равны не запускаем модуль
            {
                var process = System.Diagnostics.Process.Start("LineProg.exe");
                process.WaitForExit();//wait until Matlab program will close

                Read("b_result.txt");
                delta = Result[0, 0];

                Read("X.txt");
                listBox1.DataSource = File.ReadAllLines("X.txt");
            }
            else
            {
                Read("b_optim.txt");
                delta = Result[0, 0];

                Read("Xoptim.txt");
                listBox1.DataSource = File.ReadAllLines("Xoptim.txt");

            }

            for (int wn = 1; wn <= agr_num; wn++)
                {
                    for (int k = 1; k <= C_length; k++)
                    {
                        Xij[wn, k] = Result[wn - 1, k - 1];
                    }
                }

                int[] ni = new int[week_num + 1];  //вектор недельных затрат
                int[] ni_delta = new int[week_num - 1];  //вектор разницы недельных затрат


                // затраты первой недели
                for (int wn = 1; wn <= agr_num; wn++)
                {
                    ni[1] = ni[1] + aij[wn, 1] * Xij[wn, 1];
                }

                // затраты второй недели
                for (int wn = 1; wn <= agr_num; wn++)
                {
                    ni[2] = ni[2] + aij[wn, 1] * Xij[wn, 2] + aij[wn, 2] * Xij[wn, 1];
                }

                // затраты остальных недель

                for (int w = 3; w <= week_num; w++)
                {
                    for (int wn = 1; wn <= agr_num; wn++)
                    {
                        ni[w] = ni[w] + aij[wn, 1] * Xij[wn, w] + aij[wn, 2] * Xij[wn, w - 1] + aij[wn, 3] * Xij[wn, w - 2];
                    }
                }

                // колебания межнедельных затрат
                for (int w = 1; w < week_num; w++)
                {
                    ni_delta[w - 1] = Math.Abs(ni[w + 1] - ni[w]);
                }

                // построение гистограммы межнедельных колебаний:

                chart1.Series.Clear();
                if (chart1.Series.IsUniqueName("колеб.")) { chart1.Series.Add("колеб."); }
                chart1.Series["колеб."].ChartType = SeriesChartType.Column;
                chart1.Series["колеб."].Color = Color.FromArgb(60, 179, 113);
                gis1.Text = "недели";
                gis2.Text = "колебания";
                int t = 1;
                foreach (var ii in ni_delta)
                {
                    this.chart1.Series["колеб."].Points.AddXY(t, ii);
                    t++;
                }

                // сумма недельных колебаний за весь период
                int sum_delta = 0;
                foreach (var ii in ni_delta)
                {
                    sum_delta = sum_delta + ii;
                }

                sum.Text = sum_delta.ToString();

                ni[0] = 0; //лишний элемент при вычислении Max()
                int max_w = ni.Cast<int>().Max(); //возвращает макс. элемент массива затрат
                ni[0] = 1000; //лишний элемент при вычислении Min()
                int min_w = ni.Cast<int>().Min(); //возвращает мин. элемент массива затрат

                max_week_delta.Text = (max_w - min_w).ToString(); //значение максимального межнедельного колебания затрат            
                max_week.Text = delta.ToString();
                
        }
    }
}

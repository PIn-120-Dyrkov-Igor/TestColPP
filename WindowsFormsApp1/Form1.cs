using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "for i := 10 to 100 do y := i + x ;";
        }

        bool exeption = false;//Указатель ошибки в разбираемом коде

        List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();//Пары символов

        string[] terminals = { "var", "int", "boolean", "begin", "end", "for", "to", "do" };//Таблица служебных слов
        string[] separators = { ";", ",", "=", ":", ":=", "+", "*" };//Таблица разделителей

        Dictionary<string, string> terminalsD = new Dictionary<string, string>();//Служебные слова
        Dictionary<string, string> separatorsD = new Dictionary<string, string>();//Разделители
        Dictionary<string, string> variablesD = new Dictionary<string, string>();//Переменные
        Dictionary<string, string> literalsD = new Dictionary<string, string>();//Литералы
        Dictionary<string, string> tableD = new Dictionary<string, string>();//Стандартные символы

        Dictionary<string, string> newadsf = new Dictionary<string, string>();//Стандартные символы

        private void button1_Click(object sender, EventArgs e)
        {
            string text = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"D:\3 курс\Курсовая ТАиФЯ";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = "";
                    var filePath = openFileDialog.FileName;
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        while (true)
                        {
                            // Читаем строку из файла во временную переменную.
                            string temp = reader.ReadLine();

                            // Если достигнут конец файла, прерываем считывание.
                            if (temp == null) break;

                            // Пишем считанную строку в итоговую переменную.
                            text += temp;

                            // Выводим строку в форму
                            textBox1.Text += temp.ToString()/* + "\r\n"*/;
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text.ToString() + " ";
            textBox2.Text = "";
            pairs.Clear();
            string buffer = "";
            string type = "";
            string ind = "идентификатор";
            string raz = "разделитель";
            string lit = "литерал";

            string divide = ":=+;";

            for (int i = 0; i <= input.Length - 1;)
            {
                if (Char.IsLetter(input[i]))
                {
                    type = ind;
                    while (Char.IsLetter(input[i]))
                    {
                        buffer += input[i];
                        i++;
                    }
                    while (Char.IsDigit(input[i]))
                    {
                        buffer += input[i];
                        i++;
                    }
                    pairs.Add(new KeyValuePair<string, string>(buffer, type));
                    buffer = ""; type = "";
                }
                else if (Char.IsDigit(input[i]))
                {
                    type = lit;
                    while (Char.IsDigit(input[i]))
                    {
                        buffer += input[i];
                        i++;
                    }
                    pairs.Add(new KeyValuePair<string, string>(buffer, type));
                    buffer = ""; type = "";
                }
                else if (divide.Contains(input[i]))
                {
                    type = raz;
                    while (divide.Contains(input[i]))
                    {
                        buffer += input[i];
                        i++;
                    }

                    //Добавить!!!
                    //Проверка самого разделителя на принадлежность к ним
                    if (buffer.Length == 2)//При длине разделителя больше двух
                    {

                        if(!separators.Contains(buffer))
                        {            
                            exeption = true;
                            MessageBox.Show($"Проверка в сепараторе", "Ошибка");
                            break;
                        }
                        
                    }

                    //Проверка

                    if (buffer.Length > 2)//При длине разделителя больше двух
                    {
                        textBox2.Text = $"Синтаксическая ошибка кода\r\nЛексема '{buffer}' имеет недопустимую запись!\r\n";
                        MessageBox.Show($"Символ '{buffer + input[i]}' имеет недопустимое значение, использовано {buffer}", "Ошибка");
                        break;
                    }
                    pairs.Add(new KeyValuePair<string, string>(buffer, type));
                    buffer = ""; type = "";

                }
                else if (!divide.Contains(input[i]) && !Char.IsLetterOrDigit(input[i]) && !Char.IsWhiteSpace(input[i]))//Символы не существующие в нашем языке
                {
                    MessageBox.Show($"Символ '{input[i]}' имеет недопустимое значение", "Ошибка");
                    i++;
                }
                else if (Char.IsWhiteSpace(input[i]) || Char.IsSeparator(input[i]))
                {
                    i++;
                }
            }

            if(exeption == false)
            {
                foreach (var p in pairs)
                {
                    textBox2.Text += $"{p.Key} \t- {p.Value}\r\n";
                }
                MessageBox.Show($"Операция выполнена", "Уведомление");
            }
            else
            {
                MessageBox.Show($"Ошибка при выполнении кода", "Уведомление");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "if true\r\n\tthen a = 1;\r\nend; ";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "if true then\r\n\ta = 1;\r\nelse\r\n\ta = 2;\r\nend; ";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "if true then\r\n\ta = 1;\r\nelse if true then\r\n\ta = 2;\r\nelse\r\n\ta = 3;\r\nend; ";
        }
    }
}

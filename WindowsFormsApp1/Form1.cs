using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //textBox1.Text = "for i := 10 to 100 do y := i + x ;";
            textBox1.Text = "if (a==b) && (b!=c) then \r\n\tc=a; \r\nelse \r\n\tc=a//b; \r\nend;";
            textBox3.Text = "Исходный код: " + textBox1.Text;
            button6.Enabled = false;
        }

        bool exeption = false;//Указатель ошибки в разбираемом коде

        List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();//Пары символов

        //string[] terminals = { "var", "int", "boolean", "begin", "end", "for", "to", "do" };//Таблица служебных слов
        string[] terminals = { "if", "then", "end", "else", "elsif" };//Таблица служебных слов
        //string[] separators = { ";", ",", "=", ":", ":=", "+", "*" };//Таблица разделителей
        string[] separators = { ";", "+", "-", "//", "*", "=", ">", "<", "==", ">=", "<=", "!=", "&&", "||", "(", ")" };//Таблица разделителей

        public static Dictionary<string, string> terminalsD = new Dictionary<string, string>();//Служебные слова
                      Dictionary<string, string> separatorsD = new Dictionary<string, string>();//Разделители
        public static Dictionary<string, string> variablesD = new Dictionary<string, string>();//Переменные
        public static Dictionary<string, string> literalsD = new Dictionary<string, string>();//Литералы
                      Dictionary<string, string> tableD = new Dictionary<string, string>();//Стандартные символы

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

            string divide = ":=+;()/*<>&!";

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
                textBox10.Text = "";
                foreach (var p in pairs)
                {
                    textBox2.Text += $"{p.Key} \t- {p.Value}\r\n";
                    textBox10.Text += $"{p.Key} \t- {p.Value}\r\n";//Вывод в 3 вкладку
                }
                textBox9.Text = textBox1.Text;
                MessageBox.Show($"Операция выполнена", "Уведомление");
                textBox3.Text = "Исходный код: " + textBox1.Text;
                button6.Enabled = true;
            }
            else
            {
                MessageBox.Show($"Ошибка при выполнении кода", "Уведомление");
                button6.Enabled = false;
            }      
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "if (a==b)\r\n\tthen a = 1;\r\nend; ";
            textBox3.Text = "Исходный код: " + textBox1.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "if (a==b) then\r\n\ta = 1;\r\nelse\r\n\ta = 2;\r\nend; ";
            textBox3.Text = "Исходный код: " + textBox1.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "if (a==b) then\r\n\ta = 1;\r\nelse if (a==b) then\r\n\ta = 2;\r\nelse\r\n\ta = 3;\r\nend; ";
            textBox3.Text = "Исходный код: " + textBox1.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";

            terminalsD.Clear();
            textBox4.Text = "Служебные слова:\r\n";
            for(int i = 0; i< terminals.Length; i++)
            {
                terminalsD.Add(terminals[i], (i + 1).ToString());
            }

            foreach(var t in terminalsD)
            {
                textBox4.Text += $"{t.Value} \t {t.Key}\r\n";
            }

            separatorsD.Clear();
            textBox5.Text = "Разделители:\r\n";
            for(int i = 0; i < separators.Length; i++)
            {
                separatorsD.Add(separators[i], (i + 1).ToString());
            }

            foreach(var s in separatorsD)
            {
                textBox5.Text += $"{s.Value} \t {s.Key}\r\n";
            }

            variablesD.Clear();
            textBox6.Text = "Переменные:\r\n";
            foreach(var p in pairs)
            {
                if (!variablesD.ContainsKey(p.Key) && !terminalsD.ContainsKey(p.Key) && !separatorsD.ContainsKey(p.Key) && Char.IsLetter(p.Key[0]))
                {
                    variablesD.Add(p.Key, (variablesD.Count + 1).ToString());
                }
            }

            foreach(var v in variablesD)
            {
                textBox6.Text += $"{v.Value} \t {v.Key}\r\n";
            }
            //Проверка новой ветви

            literalsD.Clear();
            textBox7.Text += $"Литералы:\r\n";
            foreach (var p in pairs)
            {
                if (!literalsD.ContainsKey(p.Key) && !terminalsD.ContainsKey(p.Key) && !separatorsD.ContainsKey(p.Key) && Char.IsDigit(p.Key[0]))
                {
                    literalsD.Add(p.Key, (literalsD.Count + 1).ToString());
                }
            }

            foreach (var l in literalsD)
            {
                textBox7.Text += $"{l.Value} \t- {l.Key}\r\n";
            }

            tableD.Clear();
            textBox8.Text += $"Стандартные символы:\r\n";
            foreach (var p in pairs)
            {
                if (terminalsD.ContainsKey(p.Key) && !tableD.ContainsKey(p.Key))
                {
                    tableD.Add(p.Key, ($"1,{terminalsD[p.Key]}").ToString());
                }
                else if (separatorsD.ContainsKey(p.Key) && !tableD.ContainsKey(p.Key))
                {
                    tableD.Add(p.Key, ($"2,{separatorsD[p.Key]}").ToString());
                }
                else if (variablesD.ContainsKey(p.Key) && !tableD.ContainsKey(p.Key))
                {
                    tableD.Add(p.Key, ($"3,{variablesD[p.Key]}").ToString());
                }
                else if (literalsD.ContainsKey(p.Key) && !tableD.ContainsKey(p.Key))
                {
                    tableD.Add(p.Key, ($"4,{literalsD[p.Key]}").ToString());
                }
            }

            foreach (var t in tableD)
            {
                textBox8.Text += $"{t.Key} \t-{t.Value}\r\n";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Обнуляем выходное поле
            textBox11.Text = "";

            //Заносим список всех элементов в класс и выводим сообщение
            MessageBox.Show($"{CheckLexem.addElements(pairs)}");            
        }
    }
}

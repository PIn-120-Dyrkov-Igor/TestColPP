using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class CheckLexem
    {
        private static int thisElement = 0;
        private static int countElements;
        //private bool correctly = true;
        private static string exeptionValue;
        private static List<KeyValuePair<string, int>> allElements = new List<KeyValuePair<string, int>>();
        private static string[] sign = { "+", "-", "//", "*" };
        private static List<KeyValuePair<string, int>> inputOPN = new List<KeyValuePair<string, int>>();
        private static List<KeyValuePair<string, int>> priority = new List<KeyValuePair<string, int>>();
        //Заносим список всех элементов в класс
        public static string addElements(List<KeyValuePair<string, string>> into)
        {
            thisElement = 0;                                //Обнуляем текущий элемент при каждом запуске
            exeptionValue = "Не удалось завершить разбор";  //Стандартная запись при начале разбора
            allElements.Clear();                            //Очищяем лист элементов

            //Переносим входной лист в лист класса
            foreach (var i in into)                          
                allElements.Add(new KeyValuePair<string, int>(i.Key, allElements.Count()));

            //Добавляем симвоол для проверки конца разбора                                                
            allElements.Add(new KeyValuePair<string, int>("$", allElements.Count()));

            countElements = allElements.Count();            //Записываем количество элементов списка в переменную
            theCode();                                      //Вызываем функцию начала разбора
            return returnEx();                              //Возвращаем сообщение о выполненном разборе
        }

        public static void theCode()//Код
        {
            if (isTrue(thisElement))
            {
                exeptionValue = $"Разбор закончен без ошибок+0";/* +
                    $"\r\n(По количеству элементов)";*/
            }
            else if (allElements[thisElement].Key == "$")
            {
                exeptionValue = $"Разбор закончен без ошибок$" +
                    $"\r\n(По последнему элементов)";
            }
            else
            {
                statementBlock();
            }
        }

        //Определение первого вхогдного элемента
        public static void statementBlock()//Блок операторов
        {
            if (Form1.variablesD.ContainsKey(allElements[thisElement].Key))
            {
                firstVariable();//Вызываем если первый входящий элемент - переменная
            }
            else if (Form1.terminalsD.ContainsKey(allElements[thisElement].Key))
            {
                firstTerminal();//Вызываем если первый входящий элемент - терминал
            }
            else if (!Form1.variablesD.ContainsKey(allElements[thisElement].Key) && !Form1.terminalsD.ContainsKey(allElements[thisElement].Key))
            {
                exeptionValue = $"Ожидалась переменная 'id,lit' или начало цикла 'if'\r\nПолучено '{allElements[thisElement-1].Key}>{allElements[thisElement].Key}'";
            }
            else
            {
                exeptionValue = $"Неизвестная ошибка\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        //Вызываем если первый входящий элемент - переменная
        public static void firstVariable()//Переменная(тело переменной id)
        {
            if (Form1.variablesD.ContainsKey(allElements[thisElement].Key))
            {
                thisElement++;
                separatorVariable();
            }
            else
            {
                exeptionValue = $"Ожидалась переменная\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void separatorVariable()//Тело переменной =
        {
            if(allElements[thisElement].Key == "=")
            {
                thisElement++;
                secondVariable();
            }
            else
            {
                exeptionValue = $"Ожидалось '='\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void secondVariable()//Тело переменной - операнд
        {
            if (Form1.variablesD.ContainsKey(allElements[thisElement].Key) || Form1.literalsD.ContainsKey(allElements[thisElement].Key)) 
            {
                thisElement++;
                endVariable();
            }
            else
            {
                exeptionValue = $"Ожидалась переменная\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void endVariable()//Конец переменной ';'
        {
            if (allElements[thisElement].Key == ";")
            {
                thisElement++;
                addVariable();
            }
            else if (sign.Contains(allElements[thisElement].Key))
            {
                thisElement++;
                thirdVariable();
            }
            else
            {
                exeptionValue = $"Ожидалсь ';'\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void addVariable()//Добавление переменной
        {
            if (Form1.variablesD.ContainsKey(allElements[thisElement].Key))
            {
                firstVariable();
            }
            else
            {
                statementBlockAdd();
            }
        }

        public static void thirdVariable()//Конец переменной (операнд id,lit)
        {
            if (Form1.variablesD.ContainsKey(allElements[thisElement].Key) || Form1.literalsD.ContainsKey(allElements[thisElement].Key))
            {
                thisElement++;
                endFirstVariable();
            }
            else
            {
                exeptionValue = $"Ожидалась переменная\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void endFirstVariable()//Конец переменной ';'
        {
            if (allElements[thisElement].Key == ";")
            {
                thisElement++;
                addVariable();
            }
            else
            {
                exeptionValue = $"Ожидалсь ';'\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void statementBlockAdd()//Добавление оператора
        {
            if (allElements[thisElement].Key == "$")
            {
                theCode();
            }
            else if (allElements[thisElement].Key == "end")
            {
                thisElement++;
                endFirstTerminal();
            }
            else if (allElements[thisElement].Key == "else")
            {
                thisElement++;
                firstTerminalElse();
            }
            else if (allElements[thisElement].Key == "elsif")
            {
                thisElement++;
                firstTerminalElsif();
            }
            else if(Form1.variablesD.ContainsKey(allElements[thisElement].Key) || Form1.terminalsD.ContainsKey(allElements[thisElement].Key))/////////////////////////////////////////////
            {
                statementBlock();
            }
            else
            {
                exeptionValue = $"Ожидался конец строки или начало нового цикла\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        //Вызываем если первый входящий элемент - терминал
        public static void firstTerminal()//Условный оператор
        {
            if (allElements[thisElement].Key == "if")
            {
                thisElement++;
                expr();
            }
            else
            {
                exeptionValue = $"Ожидалось 'if'\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void expr()
        {
            if(allElements[thisElement].Key == "(")
            {
                inputOPN.Add(new KeyValuePair<string, int>(allElements[thisElement].Key, allElements.Count()));
                int rowValue = 1;
                for (;rowValue == 1; )
                {
                    for(; allElements[thisElement].Key != ")"; )
                    {
                        thisElement++;
                        inputOPN.Add(new KeyValuePair<string, int>(allElements[thisElement].Key, allElements.Count()));
                    }
                    if(allElements[thisElement].Key == ")")
                    {
                        rowValue = 0;
                        thisElement++;
                        inputOPN.Add(new KeyValuePair<string, int>(allElements[thisElement].Key, allElements.Count()));
                    }
                    if(allElements[thisElement].Key == "&&" || allElements[thisElement].Key == "||")
                    {
                        rowValue = 1;
                        thisElement++;
                        inputOPN.Add(new KeyValuePair<string, int>(allElements[thisElement].Key, allElements.Count()));
                        continue;
                    }
                }
                then();//Переход к следующему элементу
                //exprOPN();//Метод Дейкстры//////////////////////////////////////////////////
            }
            else
            {
                exeptionValue = $"Ожидалось '('\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void then()//Условный оператор then
        {
            if (allElements[thisElement].Key == "then")
            {
                thisElement++;
                statementBlock();
            }
            else
            {
                exeptionValue = $"Ожидалось 'then'\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void endFirstTerminal()
        {
            if (allElements[thisElement].Key == ";")
            {
                thisElement++;
                statementBlockAdd();
            }
            else
            {
                exeptionValue = $"Ожидалось ';'\r\nПолучено '{allElements[thisElement].Key}'";
            }
        }

        public static void firstTerminalElse()
        {
            statementBlock();
        }

        public static void firstTerminalElsif()
        {
            expr();
        }

        //Вывод результата работы анализатора
        public static string returnEx()
        {
            return exeptionValue;
        }

        //Проверка текушего элемента и общего количества элементов
        public static bool isTrue (int thisIndexElement)
        {
            if(thisIndexElement + 1 == allElements.Count())
                return true;
            else
                return false;          
        }

        public static void exprOPN()
        {
            priorityAdd();//Запись таблицы приоритетов
            int input = 0;
            int currentPriority = 0;
            Stack<string> stack  = new Stack<string>();
            List <string> output = new List <string>();

            foreach(var i in inputOPN)
            {
                if(Form1.variablesD.ContainsKey(i.Key) || Form1.literalsD.ContainsKey(i.Key))//Операнды
                {   //Запись операнда в выходную строку
                    output.Add(i.Key);
                }
                else if (priority.Contains(i) && priority[0].Key == i.Key)//На входе "("
                {   //Запись открывающейся скобки
                    stack.Push(i.Key);
                    currentPriority = 0;
                }
                else if(priority.Contains(i) && priority[1].Key == i.Key)//На входе ")"
                {              
                    for(string s= stack.Pop(); s != "(";)
                    {   //Закрывающая скобка выталкивает все знаки до ближайшей открывающейся скобки
                        output.Add(s);
                    }
                }
                else if (priority.Contains(i) && priority[2].Key == i.Key)//На входе "||"
                {
                    if(priority[2].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 2;
                    }
                    else if(priority[2].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[3].Key == i.Key)//На входе "&&"
                {
                    if (priority[3].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 3;
                    }
                    else if (priority[3].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[4].Key == i.Key)//На входе "<"
                {
                    if (priority[4].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 4;
                    }
                    else if (priority[4].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[5].Key == i.Key)//На входе "<="
                {
                    if (priority[5].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 4;
                    }
                    else if (priority[5].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[6].Key == i.Key)//На входе ">"
                {
                    if (priority[6].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 4;
                    }
                    else if (priority[6].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[7].Key == i.Key)//На входе ">="
                {
                    if (priority[7].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 4;
                    }
                    else if (priority[7].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[8].Key == i.Key)//На входе "=="
                {
                    if (priority[8].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 4;
                    }
                    else if (priority[8].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
                else if (priority.Contains(i) && priority[9].Key == i.Key)//На входе "!="
                {
                    if (priority[9].Value > currentPriority)
                    {
                        stack.Push(i.Key);
                        currentPriority = 4;
                    }
                    else if (priority[9].Value < currentPriority)
                    {   //Достаем файлы >= priority[2].Value
                        output.Add(stack.Pop());
                        output.Add(i.Key);
                    }
                }
            }
            

            foreach(var i in output)
            {
                exeptionValue += $"\r\n{i}";
            }
            foreach(var p in stack)
            {
                exeptionValue += $"\r\n{p}";
            }
            //exeptionValue += $"\r\n{}";
        }

        public static void priorityAdd()
        {
            priority.Add(new KeyValuePair<string, int>("(" , 0));
            priority.Add(new KeyValuePair<string, int>(")" , 1));
            priority.Add(new KeyValuePair<string, int>("||", 2));
            priority.Add(new KeyValuePair<string, int>("&&", 3));
            priority.Add(new KeyValuePair<string, int>("<" , 4));
            priority.Add(new KeyValuePair<string, int>("<=", 4));
            priority.Add(new KeyValuePair<string, int>(">" , 4));
            priority.Add(new KeyValuePair<string, int>(">=", 4));
            priority.Add(new KeyValuePair<string, int>("==", 4));
            priority.Add(new KeyValuePair<string, int>("!=", 4));
        }
    }
}

using System.Text;
using System.Text.RegularExpressions;

class CommentFinder
{
    //Дефолтные директории в проекте
    private string _fileDirInput = "input";
    private string _fileDirOutput = "output";

    private string _inputFindText = "text"; // Текст для поиска

    private int _parametrWork = 0; //Параметр для меню
    private int _encodingState = 1; //Параметр по умолчанию меню кодировки
    private int _fileScan = 0; //счетчик открытых файлов для сканирования

    static Regex regex = new Regex(@"^*\/\/.*?$|\/\*.*?\*\/"); // регулярка для Поиска одно/много - строчных комментриев
    private string FileDirInput
    {
        get { return _fileDirInput; }
        set { _fileDirInput = value; }
    }
    private string FileDirOutput
    {
        get { return _fileDirOutput; }
        set { _fileDirOutput = value; }
    }
    private int ParametrWork
    {
        get { return _parametrWork; }
        set { _parametrWork = value; }
    }
    private int EncodingState
    {
        get { return _encodingState; }
        set { _encodingState = value; }
    }
    private int FileScan
    {
        get { return _fileScan; }
        set { _fileScan = value; }
    }
    private string InputFindText
    {
        get { return _inputFindText; }
        set { _inputFindText = value; }
    }
    class ExtentionStats
    {
        public int Amount { get; set; }
        public string Extention { get; set; }

    }

    static Dictionary<string, string> extAsm = new Dictionary<string, string>
    {
        ["*.c"] = "C",
        ["*.h"] = "C"
    };
    //Словарь расширений
    static Dictionary<string, string> extLang = new Dictionary<string, string>()
    {
        ["*.kt"] = "Kotlin",
        ["*.kts"] = "Kotlin",
        ["*.java"] = "Java",
        ["*.class"] = "Java",
        ["*.jar"] = "Java",
        ["*.jmod"] = "Java",
        ["*.js"] = "JavaScript",
        ["*.cjs"] = "JavaScript",
        ["*.mjs"] = "JavaScript",
        ["*.es"] = "ECMAScript",
        ["*.css"] = "CSS",
        ["*.h"] = "C",
        ["*.c"] = "C",
        ["*.cs"] = "C#",
        ["*.csx"] = "C#",
        //["*.C"] = "C++",
        ["*.cc"] = "C++",
        ["*.cpp"] = "C++",
        ["*.cxx"] = "C++",
        ["*.c++"] = "C++",
        // ["*.H"] = "C++",
        ["*.hh"] = "C++",
        ["*.hpp"] = "C++",
        ["*.hxx"] = "C++",
        ["*.h++"] = "C++",
        ["*.php"] = "PHP",
        ["*.phar"] = "PHP",
        ["*.phtml"] = "PHP",
        ["*.pht"] = "PHP",
        ["*.phpt"] = "PHP",
        ["*.php3"] = "PHP",
        ["*.php4"] = "PHP",
        ["*.php5"] = "PHP",
        ["*.phps"] = "PHP",
        ["*.go"] = "Go",
        ["*.rs"] = "Rust",
        ["*.rlib"] = "Rust",
        ["*.py"] = "Python",
        ["*.pyi"] = "Python",
        ["*.pyc"] = "Python",
        ["*.pyd"] = "Python",
        ["*.pyw"] = "Python",
        ["*.pyz"] = "Python",
        ["*.pyo"] = "Python"
    };
    private static void Main()
    {
        CommentFinder CommentFinder = new CommentFinder();

        CreateFolderWork(CommentFinder.FileDirInput);
        CreateFolderWork(CommentFinder.FileDirOutput);
        ShowParametrWork();
        CheckForNumber();

        switch (CommentFinder.ParametrWork)
        {
            case 0:
                HelpInfo();
                RepeatMenu();
                break;
            case 1:
                SearchTextDirectory(extAsm, " asm"); break;
            case 2:
                SearchTextDirectory(extLang, "Комментариев"); break;
            case 3:
                Console.Write("Задайте текст для поиска:");
                CommentFinder.InputFindText = Console.ReadLine()!;
                SearchTextDirectory(extLang, CommentFinder.InputFindText);
                RepeatMenu();
                break;
            case 4:
                ClearDirOutput(CommentFinder.FileDirOutput);
                RepeatMenu();
                break;
            case 5:
                EncodingInputMenu();
                Main();
                break;
            case 6:
                Console.Write("Введите путь к исходникам: ");
                CommentFinder.FileDirInput = Console.ReadLine()!;
                Main();
                break;
            default:
                Console.WriteLine("Error: Выберите значение из меню");
                RepeatMenu();
                break;
        }

        void ShowParametrWork()  // Оснвоное меню
        {
            Console.WriteLine("Задайте Параметр для работы от 0 до 6: ");
            Console.WriteLine();
            Console.WriteLine(new string('!', 84));
            Console.WriteLine("!! 0. Справка по работе программы                                                 !!");
            Console.WriteLine("!! 1. Провести поиск асемблерных вставок в файлах с расширением [.c , .h]         !!");
            Console.WriteLine("!! 2. Провести поиск комментов. В исходниках С/C++/C#/java/JavaScript/PHP/Go/Rust !!");
            Console.WriteLine("!! 3. Поиск заданного текста. В исходниках С/C++/C#/java/JavaScript/PHP/Go/Rust   !!");
            Console.WriteLine("!! 4. Очистить папку output                                                       !!");
            Console.WriteLine("!! 5. Задать кодировку для файла                                                  !!");
            Console.WriteLine("!! 6. Задать путь к исходникам                                                    !!");
            Console.WriteLine(new string('!', 84));
            Console.WriteLine();
        }

        void HelpInfo() //меню помощи
        {
            Console.WriteLine(new string('!', 114));
            Console.Write("!! 1. Исходники перенести в папку проекта input или задать новый путь в меню." + new string(' ', 35) + "!!\n");
            Console.Write("!! 2. Выбрать один из режимов работы, отчет будет сгенерирован в папке output." + new string(' ', 34) + "!!\n");
            Console.Write("!! 3. Для работы требуется версия  NET Framework не ниже 6.0." + new string(' ', 51) + "!!\n");
            Console.Write("!!" + new string(' ', 87) + "AsmFinder Версия 2.1.0 !!\n");
            Console.WriteLine(new string('!', 114));
            Console.WriteLine();
        }

        void EncodingInputMenu() // меню выбора кодировки
        {
            Console.WriteLine(new string("1. UTF8 || по умолчанию стоит"));
            Console.WriteLine(new string("2. win1251"));
            CheckForNumber();
        }

        void CreateFolderWork(string path) //Создание директории
        {
            Directory.CreateDirectory(path);
        }

        void ClearDirOutput(string FolderPath) //очистка папки output
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(FolderPath);

                if (directory.GetFileSystemInfos().Length != 0)
                {
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in directory.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    Console.WriteLine("Info: Папка output Очищена ");
                    Main();
                }
                else { Console.WriteLine("Info:Папка Пуста"); }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine("Directory not found: " + dirEx.Message);
                Console.WriteLine("Будет создана папки output");
                Directory.CreateDirectory(CommentFinder.FileDirOutput);
                RepeatMenu();
            }
        }

        Encoding EncodeDefault()
        {
            Encoding encodeDefault = Encoding.UTF8;

            switch (CommentFinder.EncodingState)
            {
                case 1:
                    encodeDefault = Encoding.UTF8;
                    break;
                case 2:
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    encodeDefault = Encoding.GetEncoding(1251);
                    break;
            }

            return encodeDefault;
        }

        int RepeatMenu() // повтор работы
        {
            Console.WriteLine("\nДля выхода в Главное Меню =>  1, для Закрытия Программы => 0");
            CheckForNumber();

            if (CommentFinder.ParametrWork == 1)
                Main();
            if (CommentFinder.ParametrWork == 0) return 0;
            if (CommentFinder.ParametrWork >= 2)
            {
                Console.WriteLine("Вы ввели значение больше 1, будет выполнен возврат в меню");
                Console.WriteLine();
                Main();
            }
            return 0;
        }

        void CheckForNumber() //Проверка введенного значения на int для меню
        {
            int number;

            Console.Write("Жду ввода команды ==> ");
            bool result = int.TryParse(Console.ReadLine(), out number);
            if (result == true)
                CommentFinder.ParametrWork = number;
            else
            {
                Console.WriteLine("Вы ввели не число");
                RepeatMenu();
            }
        }

        int SearchTextDirectory(Dictionary<string, string> extentionLang, string findText)
        {
            ExtentionStats ex = new ExtentionStats();

            bool findResult = false;
            int countFind = 0; // счетчик найденых файлов
            var resultFindFiles = new List<string>(); // Список для записи всех файлов в котрых найден текст                    
            var findExtentionDict = new Dictionary<int, ExtentionStats[]>(); //Словарь для всех найденных раширений исходников
            var resultScanTry = new List<string>(); // Список для записи найденных комментов/текста
            int dictionaryKey = 0; // key для словаря

            DirectoryInfo directory = new DirectoryInfo(CommentFinder.FileDirInput);

            try
            {
                if (Directory.EnumerateFiles(CommentFinder.FileDirInput, "*.*", SearchOption.AllDirectories).Any())
                {
                    foreach (KeyValuePair<string, string> e in extentionLang)
                    {
                        FileInfo[] fi = directory.GetFiles(e.Key, SearchOption.AllDirectories);

                        if (fi.Length != 0)
                        {

                            findExtentionDict.Add(dictionaryKey++, new ExtentionStats[] {
                                new ExtentionStats
                                {
                                Amount = fi.Length,
                                Extention  = e.Key
                                }});

                            Console.WriteLine(new string('=', 114));
                            Console.WriteLine($"Найден(о) {fi.Length} Файл(ов) c расширением {e.Key}");
                            Console.WriteLine(new string('=', 114));
                        }
                        CommentFinder.FileScan += fi.Length;
                        for (int i = 0; i < fi.Length; ++i)
                        {
                            string[] readText = File.ReadAllLines(fi[i].ToString(), EncodeDefault());

                            string openLine = "";

                            if (CommentFinder.FileDirInput == "input") //если стандартная дириктория
                            {
                                openLine = $"Открыт файл {fi[i].FullName.Remove(0, fi[i].FullName.IndexOf("input"))}  для анализа => "; //обрезаем путь к файлу до папки проекта input
                            }
                            else { openLine = $"Открыт файл {fi[i].FullName}  для анализа => "; } // выводим полностью путь до файла

                            Console.WriteLine(openLine);
                            resultScanTry.Add(openLine);

                            for (int k = 0; k < readText.Count(); k++)
                            {
                                string saveLine = $"В строке {k} [ {readText[k]} ]";

                                switch (CommentFinder.ParametrWork)
                                {
                                    case 1:
                                        if (readText[k].Contains(findText))
                                        {
                                            findResult = true;
                                            Console.WriteLine(saveLine);
                                            resultScanTry.Add(saveLine);
                                        }
                                        break;
                                    case 2:
                                        if (regex.IsMatch(readText[k]))
                                        {
                                            findResult = true;
                                            Console.WriteLine(saveLine);
                                            resultScanTry.Add(saveLine);
                                        }
                                        break;
                                    case 3:
                                        if (readText[k].Contains(findText))
                                        {
                                            findResult = true;
                                            Console.WriteLine(saveLine);
                                            resultScanTry.Add(saveLine);
                                        }
                                        break;
                                }
                            }
                            if (findResult == true)
                            {
                                findResult = false;
                                countFind++;
                            }
                            else
                            {
                                string notResultLine = $"В файле {fi[i].Name} не обнаружено {findText}";
                                Console.WriteLine(notResultLine);
                                resultScanTry.Add(notResultLine);
                            }
                        }
                    }

                    Console.WriteLine();
                    string resultLine = $"Из {CommentFinder.FileScan} Проверенн(ых) файл(ов) найден(о) {countFind} файл(ов) с содержанием  [{findText.ToUpper().Trim()}]";
                    Console.WriteLine(resultLine);
                    Console.WriteLine();

                    foreach (KeyValuePair<int, ExtentionStats[]> e in findExtentionDict)
                    {
                        for (int i = 0; i < e.Value.Length; i++)
                            Console.WriteLine($"Найден(о) {e.Value[0].Amount} Файл(ов) c расширением {e.Value[i].Extention}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Статистика по языкам:");
             
                    WritingToFile(findExtentionDict, resultScanTry, resultLine);
                    MatchCalc(findExtentionDict);
                    CommentFinder.FileScan = 0;
                    RepeatMenu();
                }
                else
                {
                    Console.WriteLine("Error: Папка input пуста, для работы нужно перекинуть исходники в папку");
                    RepeatMenu();
                }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine("Directory not found: " + dirEx.Message);
                Console.WriteLine("Будет создана папки input/output, перезапустите");
                Directory.CreateDirectory(CommentFinder.FileDirInput);
                Directory.CreateDirectory(CommentFinder.FileDirOutput);
                RepeatMenu();
            }
            return 0;
        }

        void WritingToFile(Dictionary<int, ExtentionStats[]> findExtentionDict, List<string> resultScanTry, string resultLine)
        {
            string dataStart = DateTime.Now.ToString("yy.MM.dd HH.mm.ss"); //задаем дату для имени отчета
            string filename = $"output\\output_{dataStart}.txt"; // имя отчета

            try
            {
                StreamWriter f = new StreamWriter(filename);

                f.WriteLine(resultLine);
                f.WriteLine();

                foreach (KeyValuePair<int, ExtentionStats[]> e in findExtentionDict)
                {
                    for (int i = 0; i < e.Value.Length; i++)
                        f.WriteLine($"Найден(о) {e.Value[0].Amount} Файл(ов) c расширением {e.Value[i].Extention}");
                }

                f.WriteLine();
                f.WriteLine("Статистика по языкам:");

                foreach (KeyValuePair<string, double> item in MatchCalc(findExtentionDict))
                {
                    Console.WriteLine(item.Key + " : " + item.Value + "%");
                    f.WriteLine(item.Key + " : " + item.Value + "%");
                }
                f.WriteLine();
                for (int r = 0; r < resultScanTry.Count; r++)
                    f.WriteLine(resultScanTry[r]);

                Console.WriteLine();
                f.WriteLine();
                Console.WriteLine("GenerateTxt: Отчет создан успешно. Находится в папке проекта output");
                f.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        Dictionary<string, double> MatchCalc(Dictionary<int, ExtentionStats[]> findExtentionDict)
        {
            var grouped = new Dictionary<string, double>();

            foreach (KeyValuePair<int, ExtentionStats[]> ext in findExtentionDict) //Соотносим расширение с языком и добавляем в словарь
            {
                for (int i = 0; i < ext.Value.Length; i++)
                {
                    if (extLang.ContainsKey(ext.Value[i].Extention))
                    {
                        findExtentionDict[ext.Key] = 
                            new ExtentionStats[] {
                                new ExtentionStats
                                {
                                Amount = ext.Value[0].Amount,
                                Extention  =  extLang[ext.Value[i].Extention]
                                }};
                    }
                }
            }

            foreach (KeyValuePair<int, ExtentionStats[]> e in findExtentionDict) // группируем по ЯП [Extention] и суммируем значение [Amount]
            {
                for (int i = 0; i < e.Value.Length; i++)
                {
                    grouped = findExtentionDict.AsEnumerable()
                        .GroupBy(pair => pair.Value[i].Extention)
                        .ToDictionary(group => group.Key, group => group.Select(group => Convert.ToDouble(group.Value[i].Amount)).Sum());

                }
            }

            foreach (KeyValuePair<string, double> item in grouped)
            {
                double calc = (Convert.ToDouble(item.Value) / Convert.ToDouble(CommentFinder.FileScan)) * 100;
                calc = Math.Round(calc, 2, MidpointRounding.AwayFromZero);

                grouped[item.Key] = calc;
            }

            return grouped;
        }
    }
}





using System.Text;
using System.Text.RegularExpressions;

class CommentFinder
{
    //Дефолтные директории в проекте
    private static string _fileDirInput = "input";
    private static string _fileDirOutput = "output";

    private string _inputFindText = "text"; // Текст для поиска

    private int _parametrWork = 0; //Параметр для меню
    private static int _encodingState = 1; //Параметр по умолчанию для меню кодировки
    private static Encoding _encodeDefault = Encoding.UTF8;
    private int _fileScan = 0; //счетчик открытых файлов для сканирования
    private int _matchFound = 0; //счетчик найденных совпадений

    static string asm = @"asm(.)*[{|\(][\s\S]*?[}|\)]"; // Поиск асемблерных вставок

    static string lineComment = @"\/\/.*?$"; // one comment
    static string lineCommentSQL = @"--.*?$"; // one comment SQL

    static string multiComment = @"\/\*[\s\S]*?\*\/"; // multilne comments
    static string DelphiMultiComment = @"{[\s\S]*?}"; // multilne comments Delphi

    static string HTMLComments = @"<!--.*|.*-->";  // HTML comments

    static string link = @"(https?:\/\/)?([\w-]{1,32}\.[\w-]{1,32})[^\s@]*$";

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
    private Encoding EncodeDefault
    {
       get { return _encodeDefault; }
       set { _encodeDefault = value; }
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
    private int СountComment
    {
        get { return _matchFound; }
        set { _matchFound = value; }
    }

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
        ["*.es"] = "JavaScript",
        ["*.ts"] = "TypeScript",
        ["*.tsx"] = "TypeScript",
        ["*.mts"] = "TypeScript",
        ["*.cts"] = "TypeScript",
        ["*.html"] = "HTML",
        ["*.css"] = "CSS",
        ["*.scss"] = "CSS",       
        ["*.h"] = "C",
        ["*.c"] = "C",
        ["*.d"] = "D",
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
        ["*.pyo"] = "Python",
        ["*.sql"] = "SQL",
        ["*.p"] = "Delphi/Object Pascal",
        ["*.pp"] = "Delphi/Object Pascal",
        ["*.pas"] = "Delphi/Object Pascal"
    };
    private static void Main()
    {
        CommentFinder CommentFinder = new CommentFinder();
        ShowParametrWork();
        CheckForNumber();

        switch (CommentFinder.ParametrWork)
        {
            case 0:
                HelpInfo();
                RepeatMenu();
                break;
            case 1:
                SearchTextDirectory(extLang); break;
            case 2:
                SearchTextDirectory(extLang); break;
            case 3:
                Console.Write("Set search text:");
                CommentFinder.InputFindText = Console.ReadLine()!;
                SearchTextDirectory(extLang);
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
                Console.Write("Enter source path: ");
                CommentFinder.FileDirInput = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(CommentFinder.FileDirInput) == true)
                {
                    Console.WriteLine("This path is empty, set default directory");
                    CommentFinder.FileDirInput = "input";
                    Main();
                }
                else
                {
                    Console.WriteLine($"Info: Source path set: {CommentFinder.FileDirInput}");
                    Main();
                    break;
                }
                break;

            default:
                Console.WriteLine("Error: Select a value from the menu");
                RepeatMenu();
                break;
        }

        void ShowParametrWork()  // Оснвоное меню
        {
            Console.WriteLine("Set Parameter to work for 0 tо 6: ");
            Console.WriteLine();
            Console.WriteLine(new string('!', 47));
            Console.WriteLine("!! 0. Help for the program                   !!");
            Console.WriteLine("!! 1. Search for assembler inserts [.c , .h] !!");
            Console.WriteLine("!! 2. Search for comments in the source      !!");
            Console.WriteLine("!! 3. Search for text in sources             !!");
            Console.WriteLine("!! 4. Clear output folder                    !!");
            Console.WriteLine("!! 5. Set encoding                           !!");
            Console.WriteLine("!! 6. Set source path                        !!");
            Console.WriteLine(new string('!', 47));
            Console.WriteLine(new string(' ', 27) + CommentFinder.EncodeDefault.EncodingName);
            Console.WriteLine();
            Console.WriteLine("Source path: " + CommentFinder.FileDirInput);
            Console.WriteLine();

            //Проверка существуют ли дефолтные директории проекта
            if (!Directory.Exists("input"))
                CreateFolderWork("input");
            if (!Directory.Exists("output"))
                CreateFolderWork("output");
        }

        void HelpInfo() //меню помощи
        {
            Console.WriteLine(new string('!', 82));
            Console.Write("!! 1. Исходники перенести в папку проекта input или задать новый путь в меню." + new string(' ', 3) + "!!\n");
            Console.Write("!! 2. Выбрать один из режимов работы, отчет будет сгенерирован в папке output." + new string(' ', 2) + "!!\n");
            Console.Write("!! 3. Для работы требуется версия  NET Framework не ниже 6.0." + new string(' ', 19) + "!!\n");
            Console.WriteLine("!! 4. Работает с исходниками ЯП:" + new string(' ', 48) + "!!");
            Console.WriteLine("!! C/C++/C#/Java" + new string(' ', 64) + "!!");
            Console.WriteLine("!! TypeScript/JavaScript" + new string(' ', 56) + "!!");
            Console.WriteLine("!! CSS/HTML/Sass" + new string(' ', 64) + "!!");
            Console.WriteLine("!! Go/Kotlin/PHP/Python/Rust" + new string(' ', 52) + "!!");
            Console.WriteLine("!! PL/SQL" + new string(' ', 71) + "!!");
            Console.WriteLine("!! Delphi/Object Pascal" + new string(' ', 57) + "!!");
            Console.Write("!!" + new string(' ', 51) + "CommentFinder Version 2.8.5!!\n");
            Console.WriteLine(new string('!', 82));
            Console.WriteLine();
        }

        void EncodingInputMenu() // меню выбора кодировки
        {
            Console.WriteLine(new string("1. Info: UTF8 - default"));
            Console.WriteLine(new string("2. Win1251"));

            if (CheckForNumber() > 2)
            {
                Console.WriteLine("Info: Set value from menu");
                CheckForNumber();
            }
            else
            {
                CommentFinder.EncodingState = CommentFinder.ParametrWork;
                EncodeDefault();
            }

            switch (CommentFinder.EncodingState)
            {
                case 1:
                    Console.WriteLine("Encoding set: " + CommentFinder.EncodeDefault.EncodingName);
                    Console.WriteLine(); break;
                case 2:
                    Console.WriteLine("Encoding set: " + CommentFinder.EncodeDefault.EncodingName);
                    Console.WriteLine(); break;
            }
        }

        void CreateFolderWork(string path) //Создание директории
        {
            try { Directory.CreateDirectory(path); }
            catch (Exception e) { Console.WriteLine(e.Message); }
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
                    Console.WriteLine("Info: output folder Cleaned up");
                    Console.WriteLine();
                    Main();
                }
                else { Console.WriteLine("Info: Folder is empty"); }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine("Directory not found: " + dirEx.Message);
                Console.WriteLine("output folder will be created");
                Directory.CreateDirectory(CommentFinder.FileDirOutput);
                RepeatMenu();
            }
        }

        Encoding EncodeDefault()
        {
            switch (CommentFinder.EncodingState)
            {
                case 1:
                    CommentFinder.EncodeDefault = Encoding.UTF8;
                    break;
                case 2:
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    CommentFinder.EncodeDefault = Encoding.GetEncoding(1251);
                    break;
            }

            return CommentFinder.EncodeDefault;
        }

        int RepeatMenu() // повтор работы
        {
            Console.WriteLine("\nTo exit to the Main Menu =>  1, to close the program => 0");
            CheckForNumber();

            if (CommentFinder.ParametrWork == 1)
                Main();
            if (CommentFinder.ParametrWork == 0) return 0;
            if (CommentFinder.ParametrWork >= 2)
            {
                Console.WriteLine("Info: You entered a value < 1, return to the menu");
                Console.WriteLine();
                Main();
            }
            return 0;
        }

        int CheckForNumber() //Проверка введенного значения на int для меню
        {
            int number;

            Console.Write("Waiting for a command ==> ");
            bool result = int.TryParse(Console.ReadLine(), out number);
            if (result == true)
                CommentFinder.ParametrWork = number;
            else
            {
                Console.WriteLine("You didn't enter a number");
                RepeatMenu();
            }

            return number;
        }

        int SearchTextDirectory(Dictionary<string, string> extentionLang)
        {
            ExtentionStats ex = new ExtentionStats();

            var resultFindFiles = new List<string>(); // Список для записи всех файлов в которых найден текст
            string openFile = "";
            var FindALLFindExtDir = new List<string>();   // Список расширений в директории
            var findExtentionDict = new Dictionary<int, ExtentionStats[]>(); //Словарь для всех найденных раширений исходников
            var resultScanTry = new List<string>(); // Список для записи найденных комментов/текста
            int dictionaryKey = 0; // key для словаря
            CommentFinder.СountComment = 0;
            string findText = "";

            DirectoryInfo directory = new DirectoryInfo(CommentFinder.FileDirInput);

            try
            {
                if (Directory.EnumerateFiles(CommentFinder.FileDirInput, "*.*", SearchOption.AllDirectories).Any())
                {
                    var extensionCounts = directory.EnumerateFiles("*.*", SearchOption.AllDirectories)
                              .GroupBy(x => x.Extension)
                              .Select(g => new { Extension = g.Key, Count = g.Count() })
                              .ToList();
                    FindALLFindExtDir.Add("Total File  found in directory: " + directory.GetFiles("*.*", SearchOption.AllDirectories).Length);
                    FindALLFindExtDir.Add("Total Extention All found in directory: " + extensionCounts.Count);

                    foreach (var group in extensionCounts)
                    {
                        FindALLFindExtDir.Add($"Search {group.Count} extension file {group.Extension}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Info: Searching for text in sources ==>");
                    Console.WriteLine();

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
                            Console.WriteLine($"Search {fi.Length} file with extension {e.Key}");
                            Console.WriteLine(new string('=', 114));
                            resultScanTry.Add(new string('=', 114));
                            resultScanTry.Add($"Search {fi.Length} file with extension {e.Key}");
                            resultScanTry.Add(new string('=', 114));
                        }

                        CommentFinder.FileScan += fi.Length;

                        for (int i = 0; i < fi.Length; ++i)
                        {
                            string[] AllLines = File.ReadAllLines(fi[i].ToString(), EncodeDefault());  //открываем файл и считываем его по строке
                             
                            if (CommentFinder.FileDirInput == "input") //если стандартная дириктория
                            {
                                openFile = $"Open check file ==> {fi[i].FullName.Remove(0, fi[i].FullName.IndexOf("input"))}"; //обрезаем путь к файлу до папки проекта input
                            }
                            else { openFile = $"Open check file ==> {fi[i].FullName}"; } // выводим полностью путь до файла

                            Console.WriteLine(openFile);
                            resultScanTry.Add(openFile);

                            switch (CommentFinder.ParametrWork)
                            {                     
                                case 1:
                                    switch (e.Key)
                                    {
                                        case "*.c":
                                        case "*.h":
                                    findText = "asm";
                                    resultScanTry.AddRange(CommnentList(AllLines, asm, "NotPattern"));
                                    break;
                                    }
                                   break;   

                                    case 2:
                                    {
                                        findText = "comment";
                                        switch (e.Key)
                                        {
                                            case "*.sql":
                                                resultScanTry.AddRange(CommnentList(AllLines, lineCommentSQL, multiComment));
                                                break;
                                            case "*.p":
                                            case "*.pp":
                                            case "*.pas":
                                                resultScanTry.AddRange(CommnentList(AllLines, lineComment, DelphiMultiComment));
                                                break;
                                            case "*.html":
                                                resultScanTry.AddRange(CommnentList(AllLines, "NotPattern", HTMLComments));
                                                break;
                                            default:
                                                resultScanTry.AddRange(CommnentList(AllLines, lineComment, multiComment));
                                                break;
                                        }
                                    }
                                        break;

                                    case 3:
                                    findText = CommentFinder.InputFindText;
                                    resultScanTry.AddRange(CommnentList(AllLines,  findText, "NotPattern"));
                                    // FindText(AllLines, findText);
                                    break;                               
                            }
                        }
                    }

                    Console.WriteLine();
                    for (int f = 0; f < FindALLFindExtDir.Count; f++)
                    {
                        Console.WriteLine(FindALLFindExtDir[f]);
                    }

                    Console.WriteLine();
                    string resultLine = $"In {CommentFinder.FileScan} Checked file found {CommentFinder.СountComment} with content [{findText}]";
                    Console.WriteLine(resultLine);
                    Console.WriteLine();


                    foreach (KeyValuePair<int, ExtentionStats[]> e in findExtentionDict)
                    {
                        for (int i = 0; i < e.Value.Length; i++)
                            Console.WriteLine($"Search {e.Value[0].Amount} File with extension {e.Value[i].Extention}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Statistics by language:");
             
                    
                    WritingToFile(findExtentionDict, resultScanTry, resultLine);
                    MatchCalc(findExtentionDict);
                    CommentFinder.FileScan = 0;
                    RepeatMenu();
                }
                else
                {
                    Console.WriteLine("ErrorInfo: The input folder is empty, to work you need to transfer the sources to the folder");
                    RepeatMenu();
                }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine("Directory not found: " + dirEx.Message);
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

                f.WriteLine("Search extension:");

                foreach (KeyValuePair<int, ExtentionStats[]> e in findExtentionDict)
                {
                    for (int i = 0; i < e.Value.Length; i++)
                        f.WriteLine($"Search {e.Value[0].Amount} File with extension {e.Value[i].Extention}");
                }

                f.WriteLine();
                f.WriteLine("Statistics by language:");

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
                Console.WriteLine("GenerateTxt: The report was created successfully. Located in the project's output folder");
                f.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ErrorInfo: { ex.Message}");
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

            foreach (KeyValuePair<string, double> item in grouped) // считаем статистику в %
            {
                double calc = (Convert.ToDouble(item.Value) / Convert.ToDouble(CommentFinder.FileScan)) * 100;
                calc = Math.Round(calc, 2, MidpointRounding.AwayFromZero);

                grouped[item.Key] = calc;
            }

            return grouped;
        }

        List<string> CommnentList (string[] AllLines, string oneLine, string multiLine)
        {
            var resultScanTry = new List<string>();
            bool Result = false;
            string CompareText = "";
            Regex LinkRegex = new Regex(link); 

            for (int i = 0; i < AllLines.Length; i++) // Поиск одинарного комментария
                {
                if (oneLine != "NotPattern")
                {
                    Regex lineComment = new Regex(oneLine); // one comment

                    if (lineComment.IsMatch(AllLines[i]))
                    {
                        if (!LinkRegex.IsMatch(AllLines[i]))
                        { 
                            Result = true;
                            CommentFinder.СountComment++;
                            Console.WriteLine(AllLines[i]);
                            resultScanTry.Add(AllLines[i]);
                        }
                    }
                }
                CompareText += AllLines[i] + "\n"; // соединяем строки в сплошной текст
             }
            if (multiLine != "NotPattern")
            {
                Regex multiComment = new Regex(multiLine); //  multiComment
                Match match = multiComment.Match(CompareText); // Поиск по регулярному выражения совпадений

                while (match.Success)
                {
                    Result = true;
                    CommentFinder.СountComment++;
                    Console.WriteLine(match.Value);
                    resultScanTry.Add(match.Value);
                    // Переходим к следующему совпадению
                    match = match.NextMatch();
                }
            }

            if (Result == false)
            {
                resultScanTry.Add("No matches found in the file");
                Console.WriteLine("No matches found in the file");
            }
            return resultScanTry;
        }
    }      
}
class ExtentionStats
{
    public int Amount { get; set; }
    public string Extention { get; set; }

}

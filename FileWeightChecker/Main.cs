using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;

public class FileLength
{
    public static void DirectoryLengthCheck(DirectoryInfo CurrentDirectory, DirectoryInfo StartingDirectory, string OutputFileName)
    {
        
        DirectoryInfo[] DirectoryArray = CurrentDirectory.GetDirectories();
        foreach(DirectoryInfo directory in DirectoryArray)
        {
            DirectoryLengthCheck(directory, StartingDirectory, OutputFileName);
        }
        FileLengthCheck(CurrentDirectory, StartingDirectory, OutputFileName);
    }
    public static void FileLengthCheck(DirectoryInfo Directory, DirectoryInfo StartingDirectory, string OutputFileName)
    {
        FileInfo[] FileArray = Directory.GetFiles();
        foreach(FileInfo file in FileArray)
        {
            using (StreamWriter sw = new StreamWriter(OutputFileName, true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"{Directory.Name}, {file.Name}, {file.Length}"); 
                sw.Close(); 
            }
        }
    }
    public static void AverageAndMaxCheck(string InputFileName, string OutputFileName)
    {
        int MaxMass = 0;
        int MassMaxIndex = 0;
        int counter = 0;
        double Average = 0.0;

        string[] AllLines = File.ReadAllLines (InputFileName);

        /*
        //пример нахождения максимального значения с помощью LINQ
        
        //What are these question marks doing? They should be some check for null filled lines here but what is their exact purpose?
        var maxLen = AllLines?.Select(l => l //операция преобразования над каждым элементом, а именно:
                             ?.Split(',') // сплит по запятым (у тебя был по пробелам, а в именах файлов могут быть пробелы, внимательнее!)
                                          // Last "word" in line is always after a space(can be seen in the output), so there is no reason to split by commas. 
                             ?.LastOrDefault() // взять взять последний элемент или null
                             ? //если получился null то значение всего выражения = null
                             .Trim(new char[] {' '})) //отбросить пробелы
                             ?.Select( str => !string.IsNullOrEmpty(str) && int.TryParse(str, out var lengthValue) //This line checks last "word" we got and converts it to int.
                             ? lengthValue : 0) //Checks for "word" length.
                             ?.Where( len => len != 0) //If "word" length is not 0 - get this "word".
                             .Max(); //Getting maximal of these "words".        
        */

        //TODO: Improve that LINQ query(or add a new one) to also check length size and to output line with maximal length(not only maximal length).

        foreach (string line in AllLines)
        {
            //пробелы в именах файлов/папок?
            // It does not matter because there is always space after comma. And the last space is after the last comma.
            if(MaxMass<int.Parse(line.Split(' ').Last()))
            {
                MaxMass = int.Parse(line.Split(' ').Last());
                MassMaxIndex = counter;
            }
            Average += int.Parse(line.Split(' ').Last());
            counter++;
        }

        Average = Average / counter;
        using (StreamWriter sw = new StreamWriter(OutputFileName, true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"File with maximal length: {AllLines[MassMaxIndex]}");
                sw.WriteLine($"Maximal length equals {MaxMass} bytes");
                sw.WriteLine($"Avegage length equals {Average} bytes");
                sw.Close(); 
            }

    }

    public static void Main()
    {
        //обработку ошибок бы хотелось
        //Done. At least places where i know they may happen.
        string AverageAndMaxOutputFileDefaultName = @"AverageAndMax.txt";
        string AllLengthsFileDefaultName = @"AllLengths.txt";
        string OutputFileDirectory;
        DirectoryInfo StartingDirectory;

        //Creating output directory.
        TryAgainOutputDirectory:
        Console.WriteLine("Enter directory to save output files(if it doesn't exist - the new one will be created)");
        try
        {
            OutputFileDirectory = Console.ReadLine();
            Directory.CreateDirectory(OutputFileDirectory);
        }
        catch
        {
            //If trying again is not the best option use this:
            //return;
            Console.WriteLine("Incorrect directory name. Try again.");
            goto TryAgainOutputDirectory;        
        }
        

        // а зачем? файл и так создается же. закомментил.
        // To replace files that were created earlier. If you need one output directory for several tests.
        
        //Creating output files.
        FileStream AllLengthsOutputFile = new FileStream(Path.Combine(OutputFileDirectory, AllLengthsFileDefaultName), FileMode.Create, FileAccess.ReadWrite);
        AllLengthsOutputFile.Close();
        FileStream AverageAndMaxMassOutputFile = new FileStream(Path.Combine(OutputFileDirectory, AverageAndMaxOutputFileDefaultName), FileMode.Create, FileAccess.ReadWrite);
        AverageAndMaxMassOutputFile.Close();

        TryAgainStrartingDirectory:
        Console.WriteLine("Enter directory to check");
        try
        {
            StartingDirectory = new DirectoryInfo(Console.ReadLine());
            
        }
        catch
        {
            //If trying again is not the best option use this:
            //return;
            Console.WriteLine("Incorrect directory name. Try again.");
            goto TryAgainStrartingDirectory;        
        }   
        
        DirectoryLengthCheck(StartingDirectory, StartingDirectory, AllLengthsOutputFile.Name);
        AverageAndMaxCheck(AllLengthsOutputFile.Name, AverageAndMaxMassOutputFile.Name);
    }
}
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;

public class FileLength
{
    public static void DirectoryLengthCheck (DirectoryInfo CurrentDirectory, DirectoryInfo StartingDirectory, string OutputFileName)
    {
        DirectoryInfo[] DirectoryArray = CurrentDirectory.GetDirectories();
        foreach(DirectoryInfo directory in DirectoryArray)
        {
            DirectoryLengthCheck (directory, StartingDirectory, OutputFileName);
        }
        FileLengthCheck (CurrentDirectory, StartingDirectory, OutputFileName);
    }
    public static void FileLengthCheck (DirectoryInfo CurrentDirectory, DirectoryInfo StartingDirectory, string OutputFileName)
    {
        FileInfo[] FileArray = CurrentDirectory.GetFiles();
        foreach(FileInfo file in FileArray)
        {   
            using (StreamWriter sw = new StreamWriter(OutputFileName, true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"{CurrentDirectory.Name}, {file.Name}, {file.Length}"); 
                    sw.Close(); 
            }
        }
    }
    public static void AverageAndMaxCheck (string InputFileName, string OutputFileName)
    {
        int MassLength = 0;
        int MaxLengthIndex = 0;
        int counter = 0;
        double Average = 0.0;

        string[] AllLines = File.ReadAllLines(InputFileName);

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
            // It does not matter because there is always space after a comma. And the last space is after the last comma.
            if(MassLength<int.Parse(line.Split(' ').Last()))
            {
                MassLength = int.Parse(line.Split(' ').Last());
                MaxLengthIndex = counter;
            }
            Average += int.Parse(line.Split(' ').Last());
            counter++;
        }

        Average = Average / counter;
        using (StreamWriter sw = new StreamWriter(OutputFileName, true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"File with maximal length: {AllLines[MaxLengthIndex]}");
                sw.WriteLine($"Maximal length equals {MassLength} bytes");
                sw.WriteLine($"Avegage length equals {Average} bytes");
                sw.Close(); 
            }

    }

    public static void Main()
    {
        int DirectoryInputTryingCounter = 0;

        while(DirectoryInputTryingCounter<3)
        {
            try
            {
                string AverageAndMaxOutputFileDefaultName = @"AverageAndMax.txt";
                string AllLengthsFileDefaultName = @"AllLengths.txt";
                string OutputFileDirectory;
                DirectoryInfo StartingDirectory;

                //Creating output directory.
                Console.WriteLine("Enter directory to save output files(if it doesn't exist - the new one will be created)");
                OutputFileDirectory = Console.ReadLine();
                Directory.CreateDirectory(OutputFileDirectory); 

                // а зачем? файл и так создается же. закомментил.
                // To replace files that were created earlier. If you need one output directory for several tests.
                
                //Creating output files.
                FileStream AllLengthsOutputFile = new FileStream(Path.Combine(OutputFileDirectory, AllLengthsFileDefaultName), FileMode.Create, FileAccess.ReadWrite);
                AllLengthsOutputFile.Close();
                FileStream AverageAndMaxMassOutputFile = new FileStream(Path.Combine(OutputFileDirectory, AverageAndMaxOutputFileDefaultName), FileMode.Create, FileAccess.ReadWrite);
                AverageAndMaxMassOutputFile.Close();

                
                
                Console.WriteLine("Enter directory to check");
                StartingDirectory = new DirectoryInfo(Console.ReadLine());
                
                try
                {
                    DirectoryLengthCheck (StartingDirectory, StartingDirectory, AllLengthsOutputFile.Name);
                    AverageAndMaxCheck (AllLengthsOutputFile.Name, AverageAndMaxMassOutputFile.Name);
                }
                catch(IndexOutOfRangeException)
                { 
                    using (StreamWriter sw = new StreamWriter(AllLengthsOutputFile.Name, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine("There is no files in directory specified for checking."); 
                        sw.Close(); 
                    }
                }
                
                // If there was no exceptions we exit the cycle.
                // Maybe it would be better not to use break, but something else.
                break;
            }
            catch(SystemException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Press any key to try again.");
                Console.ReadKey();
            }
            DirectoryInputTryingCounter++;
            if(DirectoryInputTryingCounter>=3)
            {
                // I don't really know what is correct way to tell user to restart.
               Console.WriteLine("You have entered incorrect directory name too many times. Restart the program if you want to proceed."); 
            }
        }
    }
}
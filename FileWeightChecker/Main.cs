using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;

public class FileLength
{
    public static void DirectoryMassCheck(DirectoryInfo CurrentDirectory, DirectoryInfo StartingDirectory, string OutputFileName)
    {
        
        DirectoryInfo[] DirectoryArray = CurrentDirectory.GetDirectories();
        foreach(DirectoryInfo directory in DirectoryArray)
        {
            DirectoryMassCheck(directory, StartingDirectory, OutputFileName);
        }
        FileMassCheck(CurrentDirectory, StartingDirectory, OutputFileName);
    }
    public static void FileMassCheck(DirectoryInfo Directory, DirectoryInfo StartingDirectory, string OutputFileName)
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
        foreach(string line in AllLines)
        {
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
                sw.WriteLine($"File with maximal mass: {AllLines[MassMaxIndex]}"); 
                sw.WriteLine($"Maximal mass equals {MaxMass} bytes");
                sw.WriteLine("Avegage mass equals {0:#.####} bytes", Average);
                sw.Close(); 
            }

    }

    public static void Main()
    {
        string AverageAndMaxOutputFileDefaultName = @"AverageAndMax.txt";
        string CheckingOutputFileDefaultName = @"CheckingOutput.txt";
        //Creating output directory.
        Console.WriteLine("Enter directory to save output files(if it doesn't exist - the new one will be created)");
        string OutputFileDirectory = new string(Console.ReadLine());
        Directory.CreateDirectory(OutputFileDirectory);

        //Creating output files.
        FileStream AllMassesOutputFile = new FileStream(Path.Combine(OutputFileDirectory, CheckingOutputFileDefaultName), FileMode.Create, FileAccess.ReadWrite);
        AllMassesOutputFile.Close();
        FileStream AverageAndMaxMassOutputFile = new FileStream(Path.Combine(OutputFileDirectory, AverageAndMaxOutputFileDefaultName), FileMode.Create, FileAccess.ReadWrite);
        AverageAndMaxMassOutputFile.Close();

        Console.WriteLine("Enter directory to check");
        DirectoryInfo StartingDirectory = new DirectoryInfo(Console.ReadLine());
        
        
        DirectoryMassCheck(StartingDirectory, StartingDirectory, AllMassesOutputFile.Name);
        AverageAndMaxCheck(AllMassesOutputFile.Name, AverageAndMaxMassOutputFile.Name);

    }
}
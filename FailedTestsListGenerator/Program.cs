// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");
//Console.WriteLine("Enter the path to the .txt file to analyze:");
string? logFilePath = @"C:\\REPOS\\MC_8_DEV\\Build\\outputVS\\Debug\\batchPYFailedTestsLOGS.txt"; Console.ReadLine();

// Remove leading/trailing quotes and whitespace
logFilePath = logFilePath?.Trim().Trim('"', '\'');

Console.WriteLine($"You entered: '{logFilePath}'"); // Debug print

if (string.IsNullOrWhiteSpace(logFilePath))
{
    Console.WriteLine("No file path provided. Exiting.");
    return;
}

if (!File.Exists(logFilePath))
{
    Console.WriteLine($"File not found: {logFilePath}");
    return;
}

var lines = File.ReadAllLines(logFilePath);
var pyFailedRegex = new Regex(@"([^\s]+\.py)\s+Failed", RegexOptions.IgnoreCase);

int filesEdited = 0; // Counter for edited files

foreach (var line in lines)
{
    var match = pyFailedRegex.Match(line);
    if (match.Success)
    {
        Console.WriteLine();
        Console.WriteLine($"############################");
        Console.WriteLine($"Found line with '.py Failed': {line}");

        string pyFilePath = match.Groups[1].Value.Trim();
        Console.WriteLine($"Extracted .py file path: {pyFilePath}");

        if (File.Exists(pyFilePath))
        {
            var pyFileLines = File.ReadAllLines(pyFilePath);
            bool changed = false;
            for (int i = 0; i < pyFileLines.Length; i++)
            {
                string originalLine = pyFileLines[i];
                pyFileLines[i] = pyFileLines[i]
                    .Replace("setStatus(Status.PendingAcceptance)", "setStatus(Status.Injured)")
                    .Replace("setStatus(Status.CriticalAccepted)", "setStatus(Status.Injured)");
                if (pyFileLines[i] != originalLine)
                {
                    changed = true;
                }
            }
            if (changed)
            {
                File.WriteAllLines(pyFilePath, pyFileLines);
                Console.WriteLine($"Replaced all 'setStatus(Status.PendingAcceptance)' and 'setStatus(Status.CriticalAccepted)' with 'setStatus(Status.Injured)' and file saved: {pyFilePath}");
                filesEdited++; // Increment counter
            }
        }


    }
}

Console.WriteLine($"Total .py files edited: {filesEdited}");
Console.WriteLine($"Press any key to end program..");
Console.ReadLine();
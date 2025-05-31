using LLTable.Objects;
using Newtonsoft.Json;

namespace LLTable.FileWorkers;

public class Lexer
{
    public readonly List<Token> Tokens;

    public Lexer(string input)
    {
        var args = $"{input} tokens.json";

        using var pProcess = new System.Diagnostics.Process();
        pProcess.StartInfo.FileName = @"lexer.exe"; //exe path
        pProcess.StartInfo.Arguments = args; //argument
        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = false; //able to read console output
        pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        pProcess.StartInfo.CreateNoWindow = false; //not display a windows
        pProcess.Start();
        pProcess.WaitForExit();

        using var r = new StreamReader("tokens.json");
        var json = r.ReadToEnd();
        Tokens = JsonConvert.DeserializeObject<List<Token>>(json) ?? [];
    }
}
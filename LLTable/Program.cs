using LLTable.FileWorkers;

namespace LLTable;

static class Program
{
    static void Main()
    {
        // var lexer = new Lexer("input.txt");

        var rawRules = FileWorker.ReadFileToArray("rules1.txt");
        FileWorker.ValidateRules(rawRules);
        var dict = FileWorker.ConvertToRulesDict(rawRules);
        FileWorker.ValidateProductivity(dict);
        FileWorker.WriteToConsole(dict);
        
        var table = new Objects.LLTable( dict );
        
        Console.WriteLine( table.ToConsoleTable() );
        //
        //
        // var walker = new TableWalker( table, lexer );
        // walker.Run();
        //
        // using StreamWriter sw = new StreamWriter( "test.csv" );
        // sw.WriteLine(table.ToTable());
    }
}

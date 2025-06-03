using LLTable.FileWorkers;

namespace LLTable;

static class Program
{
    static void Main()
    {
        // var lexer = new Lexer("input.txt");
        // var tokens = lexer.Tokens;

        var rawRules = FileWorker.ReadFileToArray("rules.txt");
        FileWorker.ValidateRules(rawRules);
        var dict = FileWorker.ConvertToRulesDict(rawRules);
        FileWorker.ValidateProductivity(dict);
        FileWorker.WriteToConsole(dict);
        
        // var table = new Objects.LLTable( dict );
        //
        // Console.WriteLine( table.ToConsoleTable() );


        // var input = FileWorker.ReadFileToString( "input.txt" );
        // var walker = new TableWalker( table );
        // walker.Run( input );
        //
        // using StreamWriter sw = new StreamWriter( "test.csv" );
        // sw.WriteLine(table.ToTable());
    }
}

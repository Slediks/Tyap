using LLTable.FileWorkers;

namespace LLTable;

static class Program
{
    static void Main()
    {
        var rawRules = FileWorker.ReadFileToArray("rules.txt");
        var filteredRules = FileWorker.FilterRules(rawRules);
        var dict = FileWorker.ConvertToRulesDict(filteredRules);
        FileWorker.WriteToConsole(dict);
        
        var table = new Objects.LLTable( dict );

        Console.WriteLine( table.ToConsoleTable() );

        
        var input = FileWorker.ReadFileToString( "input.txt" );
        var walker = new TableWalker( table );
        walker.Run( input );

        // using StreamWriter sw = new StreamWriter( "test.csv" );
        // sw.WriteLine(table.ToTable());
    }
}

//Todo Удаление ошибочных вершин.
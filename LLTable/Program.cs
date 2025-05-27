using LLTable.FileWorkers;

namespace LLTable;

static class Program
{
    static void Main()
    {
        var dict = FileWorker.ConvertToRulesDict( FileWorker.ReadFileToArray( "rules.txt" ) );
        FileWorker.WriteToConsole( dict );
        FileWorker.CleanUnusedKeys(ref dict);
        FileWorker.WriteToConsole( dict );
        
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
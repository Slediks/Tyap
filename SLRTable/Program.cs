using SLRTable.FileWorkers;
using SLRTable.Objects.Rules;
using SLRTable.Objects.Table;

namespace SLRTable;

static class Program
{
    static void Main( string[] args )
    {
        var lexer = new Lexer("input.txt");
        
        var rawRules = FileWorker.ReadFileToArray("rules.txt");
        var filteredRules = FileWorker.FilterRules(rawRules);
        var dict = FileWorker.ConvertToRulesDict(filteredRules);
        var rules = new RulesList( dict );
        
        Console.WriteLine( rules.ToString() );
        
        var table = new Table( rules.GetDistItems(), rules );
        
        
        var walker = new TableWalker( table, lexer );
        walker.Run();

        using StreamWriter sw = new StreamWriter( "test.csv" );
        sw.WriteLine( table.ToTable() );
    }
}
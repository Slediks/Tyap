using SLRTable.FileWorkers;
using SLRTable.Objects.Rules;
using SLRTable.Objects.Table;

namespace SLRTable;

static class Program
{
    static void Main( string[] args )
    {
        var rules = new RulesList( FileWorker.ConvertToRulesDict( FileWorker.ReadFileToArray( "test.txt" ) ) );
        
        var table = new Table( rules.GetDistItems(), rules );

        using StreamWriter sw = new StreamWriter( "test.csv" );
        
        sw.WriteLine( table.ToTable() );
    }
}
namespace SLRTable;

public class TableWalker( Objects.Table.Table table )
{
    public void Run( string input )
    {
        input += " #";
        Console.WriteLine( $"Входная строка: {input}" );

        var words = input.Split( ' ' ).ToList();
        
    }
}
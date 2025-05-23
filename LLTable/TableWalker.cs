namespace LLTable;

public class TableWalker( Objects.LLTable table )
{
    public void Run(string input)
    {
        input += " #";
        Console.WriteLine( $"Входная строка: {input}" );
        
        var words = input.Split( ' ' ).ToList();
        List<int> path = [];
        List<int> stack = [];
        int ruleIndex = 1;
        bool isEnd = false;

        while ( words.Any() )
        {
            CheckRule(ruleIndex, words.First(), out var nextIndex, out var stackIndex, out var isShift, out isEnd, ref path);

            if ( isShift )
            {
                words.RemoveAt( 0 );
            }

            if ( isEnd )
            {
                if ( stack.Any() )
                {
                    Console.WriteLine("Программа завершила свое выполнение, но слова не закончились");
                    Console.WriteLine( $"Оставшиеся слова: {String.Join( " ", words )}" );
                    isEnd = false;
                }
                
                break;
            }

            if ( stackIndex != null )
            {
                stack.Add( (int)stackIndex );
            }

            if ( nextIndex != null )
            {
                ruleIndex = nextIndex.Value;
            }
            else
            {
                ruleIndex = stack.Last();
                stack.RemoveAt( stack.Count - 1 );
            }
        }

        Console.WriteLine(isEnd ? "Тест пройден успешно" : "Тест не пройден((");
        Console.WriteLine( $"Пройденный путь: {String.Join( ", ", path )}" );
    }

    private void CheckRule( int ruleIndex, string word, out int? nextIndex, out int? stack, out bool isShift, out bool isEnd, ref List<int> path )
    {
        var currentRule = table.Table.First(row => row.Id == ruleIndex);
        if ( !currentRule.DirectSet.Contains( word ) )
        {
            if ( currentRule.IsError )
            {
                Console.WriteLine($"Слово '{word}' не подходит правилу {ruleIndex}({ String.Join( ", ", currentRule.DirectSet ) })");
                nextIndex = null;
                stack = null;
                isShift = false;
                isEnd = true;
                return;
            }
            CheckRule( ruleIndex + 1, word, out nextIndex, out stack, out isShift, out isEnd, ref path );
            return;
        }
        
        nextIndex = currentRule.Transition;
        stack = currentRule.Stack;
        isShift = currentRule.IsShift;
        isEnd = currentRule.IsEnd;
        path.Add( ruleIndex );
    }
}
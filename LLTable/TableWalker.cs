using LLTable.FileWorkers;
using LLTable.Objects;

namespace LLTable;

public class TableWalker( Objects.LLTable table, Lexer lexer )
{
    private Token? _currentToken;
    private int _nextTokenIndex;

    public void Run()
    {
        NextToken();
        List<int> path = [];
        List<int> stack = [];
        int ruleIndex = 1;
        bool isEnd = false;

        while ( _currentToken != null )
        {
            CheckRule( ruleIndex, _currentToken.Value.TokenName, out var nextIndex, out var stackIndex, out var isShift,
                out isEnd, ref path );

            if ( isShift )
            {
                NextToken();
            }

            if ( isEnd )
            {
                if ( _currentToken != null )
                {
                    if ( isShift )
                    {
                        Console.WriteLine( "Программа завершила свое выполнение, но слова не закончились" );
                        Console.WriteLine(
                            $"Следующее слово: {_currentToken.Value.TokenValue} ({_currentToken.Value.Line}:{_currentToken.Value.StartPos})" );
                    }

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

        Console.WriteLine( isEnd ? "Тест пройден успешно" : "Тест не пройден((" );
        Console.WriteLine();
        Console.WriteLine( $"Пройденный путь: {String.Join( ", ", path )}" );
    }

    private bool HasNextToken() => _nextTokenIndex < lexer.Tokens.Count;

    private Token? GetNextToken() => HasNextToken() ? lexer.Tokens[_nextTokenIndex] : null;

    private void NextToken()
    {
        if ( _currentToken == null && !HasNextToken() ) return;
        do
        {
            _currentToken = GetNextToken();
            _nextTokenIndex++;
        } while ( _currentToken is { Type: "comment" } );
    }

    private void CheckRule( int ruleIndex, string word, out int? nextIndex, out int? stack, out bool isShift,
        out bool isEnd, ref List<int> path )
    {
        var currentRule = table.Table.First( row => row.Id == ruleIndex );
        if ( !currentRule.DirectSet.Contains( word ) )
        {
            if ( currentRule.IsError )
            {
                Console.WriteLine(
                    $"Слово {_currentToken.Value.TokenName}({_currentToken.Value.TokenValue}) ({_currentToken.Value.Line}:{_currentToken.Value.StartPos}) не подходит правилу {ruleIndex}({String.Join( ", ", currentRule.DirectSet )})" );
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
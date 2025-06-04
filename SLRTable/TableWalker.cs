using SLRTable.FileWorkers;
using SLRTable.Objects;
using SLRTable.Objects.Table;

namespace SLRTable;

public class TableWalker( Table table, Lexer lexer )
{
    private Token? _currentToken;
    private int _nextTokenIndex;
    
    public void Run()
    {
        NextToken();
        var nextToken = _currentToken;
        var tokensStack = new Stack<Token>();
        var rulesStack = new Stack<RowItem>( [table.GetStartKeyItem] );

        while ( nextToken != null && _currentToken != null )
        {
            var nextRuleItem = table.GetRowByKey( rulesStack.Peek() ).GetItemByKey( nextToken.Value.TokenName );
            if ( nextRuleItem == null )
            {
                if ( tokensStack.Count == 0 && rulesStack.Count == 1 && _currentToken.Value.TokenName == "end" &&
                     rulesStack.Peek().Name == nextToken.Value.TokenName )
                {
                    break;
                }

                throw new Exception(
                    $"Встречено неожиданное слово: {nextToken.Value.TokenValue}({nextToken.Value.TokenName}) ({nextToken.Value.Line}:{nextToken.Value.StartPos})\n" +
                    $"Последнее правило: {rulesStack.Peek()}\n");
            }

            if ( !nextRuleItem.IsEnd )
            {
                rulesStack.Push( nextRuleItem );
                tokensStack.Push( nextToken.Value);
                if (nextToken == _currentToken)
                {
                    NextToken();
                }
                nextToken = _currentToken;
                continue;
            }
            

            nextToken = Collapse( nextRuleItem, ref tokensStack, ref rulesStack );
        }

        if ( nextToken == null || HasNextToken() )
        {
            Console.WriteLine("Входные данные не соответствуют правилам");
            return;
        }
        
        Console.WriteLine("Входные данные соответствуют правилам");
    }
    
    private bool HasNextToken() => _nextTokenIndex < lexer.Tokens.Count;

    private Token? GetNextToken() => HasNextToken() ? lexer.Tokens[_nextTokenIndex] : null;

    private void NextToken()
    {
        if (_currentToken == null && !HasNextToken()) return;
        do
        {
            _currentToken = GetNextToken();
            _nextTokenIndex++;
        } while (_currentToken is { Type: "comment" }); 
    }

    private Token Collapse( RowItem ruleItem, ref Stack<Token> wordsStack, ref Stack<RowItem> rulesStack )
    {
        var ruleIndex = (int)ruleItem.Items.First().ParentIndex!;
        var rule = table.GetRuleByIndex( ruleIndex );
        var ruleList = rule.GetItemsList();
        ruleList.Reverse();
        if ( rule.Name == table.GetStartKeyItem.Name )
        {
            ruleList.RemoveAt( 0 );
        }

        var tokenValue = "";
        var line = 0;
        var startPos = 0;
        var endPos = wordsStack.TryPeek( out var endToken ) ? endToken.EndPos : 0;

        foreach ( var item in ruleList )
        {
            wordsStack.TryPop( out var wordStack );
            rulesStack.TryPop( out var ruleStack );
            if ( wordStack.TokenName != item || ruleStack!.Name != item )
            {
                throw new Exception($"Ошибка при свертке: {rule}\n" +
                                    $"Стэк слов: {wordStack.TokenName}\n" +
                                    $"Стэк правил: {ruleStack}");
            }

            tokenValue += wordStack.TokenValue + " ";
            line = wordStack.Line;
            startPos = wordStack.StartPos;
        }

        return new Token
        {
            Type = "collapsed_token",
            TokenName = rule.Name,
            TokenValue = tokenValue.Trim(),
            Line = line,
            StartPos = startPos,
            EndPos = endPos
        };
    }
}
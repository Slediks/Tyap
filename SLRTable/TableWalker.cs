using SLRTable.Objects.Table;

namespace SLRTable;

public class TableWalker( Table table )
{
    public void Run( string input )
    {
        input += " #";
        input = input.Trim();
        Console.WriteLine( $"Входная строка: {input}" );

        var words = new Stack<string>( input.Split( ' ' ).Reverse() );
        var wordsStack = new Stack<string>();
        var rulesStack = new Stack<RowItem>( [table.GetStartKeyItem] );

        while ( words.Count != 0 )
        {
            var nextRuleItem = table.GetRowByKey( rulesStack.Peek() ).GetItemByKey( words.Peek() );
            if ( nextRuleItem == null )
            {
                if ( wordsStack.Count == 0 && rulesStack.Count == 1 && words.Count == 2 &&
                     rulesStack.Peek().Name == words.Peek() )
                {
                    break;
                }

                throw new Exception($"Встречено неожиданное слово: {words.Peek()}\n" +
                                    $"Последнее правило: {rulesStack.Peek()}\n" +
                                    $"Оставшаяся строка: {String.Join( " ", words )}");
            }

            if ( !nextRuleItem.IsEnd )
            {
                rulesStack.Push( nextRuleItem );
                wordsStack.Push( words.Pop() );
                continue;
            }

            words.Push( Collapse( nextRuleItem, ref wordsStack, ref rulesStack ) );
        }

        if ( words.Count == 0 )
        {
            Console.WriteLine("Входные данные не соответсвуют правилам");
            return;
        }
        
        Console.WriteLine("Входные данные соответствуют правилам");
    }

    private string Collapse( RowItem ruleItem, ref Stack<string> wordsStack, ref Stack<RowItem> rulesStack )
    {
        var ruleIndex = (int)ruleItem.Items.First().ParentIndex!;
        var rule = table.GetRuleByIndex( ruleIndex );
        var ruleList = rule.GetItemsList();
        ruleList.Reverse();
        if ( rule.Name == table.GetStartKeyItem.Name )
        {
            ruleList.RemoveAt( 0 );
        }

        foreach ( var item in ruleList )
        {
            wordsStack.TryPop( out var wordStack );
            rulesStack.TryPop( out var ruleStack );
            if ( wordStack != item || ruleStack!.Name != item )
            {
                throw new Exception($"Ошибка при свертке: {rule}\n" +
                                    $"Стэк слов: {wordStack}\n" +
                                    $"Стэк правил: {ruleStack}");
            }
        }

        return rule.Name;
    }
}
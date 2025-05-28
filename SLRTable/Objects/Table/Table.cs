using SLRTable.Objects.Rules;

namespace SLRTable.Objects.Table;

public class Table
{
    public List<string> Columns;
    public List<Row> Rows = [];

    private readonly RulesList _rules;

    public RowItem GetStartKeyItem => Rows.First().Key;
    public Rule GetRuleByIndex( int index ) => _rules.Rules.First( rule => rule.Index == index );

    public Table( List<string> columns, RulesList rules )
    {
        Columns = columns;
        _rules = rules;

        GenerateRows();
    }

    public Row GetRowByKey( RowItem key )
    {
        return Rows.First( r => r.Key == key );
    } 

    public string ToTable()
    {
        var header = ';' + String.Join( ';', Columns  );
        return header + '\n' + String.Join( '\n', Rows.Select( r => r.ToTable( Columns ) ) );
    }

    private void GenerateRows()
    {
        List<RowItem> checkedKeys = [];
        List<RowItem> keysToCheck = [];

        var firstRow = GenerateFirstRow( Columns.First() );

        Rows.Add( new Row( new RowItem( Columns.First(), [new RuleItem( null, null, Columns.First(), true )] ),
            firstRow ) );

        keysToCheck.AddRange( firstRow );
        keysToCheck.RemoveAll( item => item.IsEnd );

        while ( keysToCheck.Count != 0 )
        {
            var keyToCheck = keysToCheck.First();
            keysToCheck.Remove( keyToCheck );
            checkedKeys.Add( keyToCheck );

            var newRow = GenerateRow( keyToCheck );
            Rows.Add( new Row( keyToCheck, newRow ) );

            keysToCheck.AddRange( newRow.Where( ri =>
                !( keysToCheck.Any( r => r.Equals( ri ) ) || checkedKeys.Any( r => r.Equals( ri ) ) || ri.IsEnd ) ) );
        }
    }

    private List<RowItem> GenerateFirstRow( string key )
    {
        List<RuleItem> result = [];
        List<RuleItem> itemsToCheck = _rules.GetItemsByKey( key );

        while ( itemsToCheck.Count != 0 )
        {
            var item = itemsToCheck.First();
            itemsToCheck.Remove( item );
            result.Add( item );

            if ( item.IsKey )
            {
                itemsToCheck.AddRange( _rules.GetItemsByKey( item.Name ) );
                itemsToCheck.RemoveAll( ri => result.Contains( ri ) );
            }
        }

        return ConvertRuleItemsToRowItems( result.Distinct().ToList() );
    }

    private List<RowItem> GenerateRow( RowItem keyRowItem )
    {
        List<RuleItem> result = [];
        List<RuleItem> itemsToCheck = [];
        itemsToCheck.AddRange( keyRowItem.Items.SelectMany( ri => _rules.GetNextItems( ri ) ) );

        while ( itemsToCheck.Count != 0 )
        {
            var item = itemsToCheck.First();
            itemsToCheck.Remove( item );
            result.Add( item );

            if ( item.IsKey )
            {
                itemsToCheck.AddRange( _rules.GetItemsByKey( item.Name ) );
                itemsToCheck.RemoveAll( ri => result.Contains( ri ) );
            }
        }

        return ConvertRuleItemsToRowItems( result.Distinct().ToList() );
    }

    private static List<RowItem> ConvertRuleItemsToRowItems( List<RuleItem> items )
    {
        List<RowItem> result = [];

        foreach ( var groupedItems in items.GroupBy( ri => ri.Name ) )
        {
            result.Add( new RowItem( groupedItems.Key, groupedItems.OrderBy( r => r.Index ).ToList() ) );
        }

        return result;
    }
}
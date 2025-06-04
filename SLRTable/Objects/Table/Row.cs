using SLRTable.Objects.Rules;

namespace SLRTable.Objects.Table;

public class Row(RowItem key, List<RowItem> items)
{
    public RowItem Key = key;
    public List<RowItem> Items = items;

    public RowItem? GetItemByKey( string key )
    {
        return Items.FirstOrDefault( item => item.Name == key );
    }

    public string ToTable( List<string> columns )
    {
        
        var strings = columns.Select( str =>
        {
            var ruleItems = Items.FirstOrDefault( i => i.Name == str )?.Items;
            return ruleItems != null ? String.Join( ' ', ruleItems ) : " ";
        } ).ToList();
        
        return Key + ";" + String.Join( ';', strings );
    }

    public override string ToString()
    {
        return $"{Key} -> {String.Join( " | ", Items )}";
    }
}
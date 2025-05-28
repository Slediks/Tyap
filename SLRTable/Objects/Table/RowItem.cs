using SLRTable.Objects.Rules;

namespace SLRTable.Objects.Table;

public class RowItem
{
    public readonly string Name;
    public readonly List<RuleItem> Items;
    public readonly bool IsEnd;

    public RowItem(string name, List<RuleItem> items)
    {
        Name = name;
        Items = items;
        IsEnd = Items.Any( ri => ri.IsEnd );
    }

    public override string ToString()
    {
        return String.Join( ' ', Items );
    }

    public static bool operator ==( RowItem? a, RowItem? b )
    {
        if ( ReferenceEquals( a, null ) && ReferenceEquals( b, null ) )
        {
            return true;
        }

        if ( ReferenceEquals( a, null ) || ReferenceEquals( b, null ) )
        {
            return false;
        }
        
        return a.Equals( b );
    }

    public static bool operator !=( RowItem? a, RowItem? b )
    {
        return !( a == b );
    }

    public override bool Equals( object? o ) => Equals( (o as RowItem)! );

    private bool Equals( RowItem other )
    {
        return Name == other.Name && Items.All( i => other.Items.Contains( i ) ) && IsEnd == other.IsEnd;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( Name, Items, IsEnd );
    }
}
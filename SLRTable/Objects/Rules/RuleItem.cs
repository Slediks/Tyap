namespace SLRTable.Objects.Rules;

public class RuleItem
{
    public readonly int? ParentIndex;
    public readonly int? Index;
    public readonly string Name;
    public readonly bool IsKey;
    public readonly bool IsEnd;

    public RuleItem( int? parentIndex, int? index, string name, bool isKey, bool isEnd = false )
    {
        if ( parentIndex != null )
        {
            ParentIndex = parentIndex;
        }

        if ( index != null )
        {
            Index = index;
        }
        
        Name = name;
        IsKey = isKey;
        IsEnd = isEnd;
    }
    
    public override bool Equals( object? obj ) => Equals( (obj as RuleItem)! );

    private bool Equals( RuleItem other )
    {
        return ParentIndex == other.ParentIndex && Index == other.Index && Name == other.Name && IsKey == other.IsKey && IsEnd == other.IsEnd;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( ParentIndex, Index, Name, IsKey, IsEnd );
    }

    public override string ToString()
    {
        return IsEnd ? "R" + ParentIndex : Name + ParentIndex + Index;
    }
}
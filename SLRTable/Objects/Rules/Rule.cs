namespace SLRTable.Objects.Rules;

public class Rule
{
    public int Index;
    public string Name;
    public RuleItem RItem;
    public readonly List<RuleItem> RuleItems = [];

    public Rule( int index, string name, List<string> ruleItems, List<string> keys )
    {
        Index = index;
        Name = name;
        RItem = new RuleItem( Index, null, "R", false );
        
        ruleItems.ForEach( ri =>
        {
            RuleItems.Add( new RuleItem( Index, RuleItems.Count + 1, ri, keys.Contains( ri ) ) );
        } );
    }

    public List<string> GetRuleNotKeyItems()
    {
        return RuleItems.Where( r => !r.IsKey ).Select( r => r.Name ).ToList();
    }

    public RuleItem GetFirst()
    {
        if ( RuleItems.Count == 0 )
        {
            throw new Exception( $"{Index} {Name} no items" );
        }

        return RuleItems.First();
    }

    public RuleItem? GetNext( RuleItem item )
    {
        return RuleItems.FirstOrDefault( ri => ri.Index > item.Index );
    }

    public bool Contains( string key )
    {
        return RuleItems.Any( ri => ri.Name == key );
    }

    public List<RuleItem> GetItemsByKey( string key )
    {
        return RuleItems.Where( r => r.Name == key ).ToList();
    }

    public override string ToString()
    {
        return $"{Index} {Name} -> {String.Join(' ', RuleItems)}";
    }
}
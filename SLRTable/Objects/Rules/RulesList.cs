﻿namespace SLRTable.Objects.Rules;

public class RulesList
{
    public List<Rule> Rules = [];
    public List<string> Keys;
    private const string End = "end";

    public RulesList( Dictionary<string, List<List<string>>> dict )
    {
        Keys = dict.Keys.ToList();

        foreach ( var keyValuePair in dict )
        {
            foreach ( var rule in keyValuePair.Value )
            {
                Rules.Add( new Rule( Rules.Count + 1, keyValuePair.Key, rule, Keys ) );
            }
        }
    }

    public List<string> GetDistItems()
    {
        var notKeyItems = Rules.SelectMany( r => r.GetRuleNotKeyItems() ).Distinct().ToList();
        if ( notKeyItems.Remove( End ) )
        {
            notKeyItems.Add( End );
        }

        List<string> result = [];
        result.AddRange( Keys );
        result.AddRange( notKeyItems );
        return result;
    }

    public List<RuleItem> GetItemsByKey( string key )
    {
        var rules = Rules.Where( r => r.Name == key ).Select( r => r.GetFirst() ).ToList();
        if ( rules.All( item => item.Name != End ) ) return rules;
        
        var newRules = rules.ToList();
        newRules.RemoveAll( item => item.Name == End );

        newRules.AddRange( from rule in rules.Where( item => item.Name == End )
            select new RuleItem( rule.ParentIndex, rule.Index, rule.Name, rule.IsKey, true ) );
        return newRules;
    }

    public List<RuleItem> GetNextItems( RuleItem item )
    {
        var rule = Rules.First( r => r.RuleItems.Contains( item ) );
        var nextItem = rule.GetNext( item );

        if ( nextItem == null ) return GetEndItems( rule );

        if ( nextItem.Name == End )
        {
            return [new RuleItem( rule.Index, null, nextItem.Name, false, true )];
        }

        return [nextItem];
    }

    private List<RuleItem> GetEndItems( Rule rule )
    {
        List<RuleItem> result = [];
        List<RuleItem> checkedItems = [];
        List<RuleItem> itemsToCheck = Rules.Where( r => r.Contains( rule.Name ) )
            .SelectMany( r => r.GetItemsByKey( rule.Name ) ).ToList();
        while ( itemsToCheck.Count != 0 )
        {
            var item = itemsToCheck.First();
            itemsToCheck.Remove( item );
            checkedItems.Add( item );

            var localRule = Rules.First( r => r.RuleItems.Contains( item ) );
            var nextItem = localRule.GetNext( item );

            if ( nextItem != null )
            {
                result.Add( new RuleItem( rule.Index, null, nextItem.Name, nextItem.IsKey, true ) );
                continue;
            }

            itemsToCheck.AddRange( Rules.Where( r => r.Contains( localRule.Name ) )
                .SelectMany( r => r.GetItemsByKey( localRule.Name ) )
                .Where( r => !checkedItems.Contains( r ) && !itemsToCheck.Contains( r ) ) );
        }

        return result;
    }

    public override string ToString()
    {
        return String.Join( "\n", Rules.Select( r => r.ToString() ) );
    }
}
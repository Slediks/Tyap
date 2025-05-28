namespace SLRTable.FileWorkers;

public static class FileWorker
{
    public static string[] ReadFileToArray( string fileName )
    {
        var lines = ReadAllLines( fileName );
        ValidateFileNotEmpty( lines, fileName );

        return lines.ToArray();
    }
    
    public static string ReadFileToString( string fileName )
    {
        var stringArray = ReadFileToArray( fileName );
        return NormalizeWhitespace(stringArray);
    }

    public static string[] FilterRules( string[] rawRules )
    {
        var usedKeys = new HashSet<string>();
        var rules = ParseRawRules( rawRules );

        usedKeys.Add( rules[0][0] );

        bool updated;
        do
        {
            updated = false;
            foreach ( var rule in rules )
            {
                var key = rule[0];
                var values = rule[1].Split( " | " ).SelectMany( v => v.Split( ' ' ) ).ToList();

                if ( usedKeys.Contains( key ) &&
                     values.Any( v => !usedKeys.Contains( v ) && rules.Any( r => r[0] == v ) ) )
                {
                    foreach ( var value in values )
                    {
                        if ( !usedKeys.Contains( value ) && rules.Any( r => r[0] == value ) )
                        {
                            usedKeys.Add( value );
                            updated = true;
                        }
                    }
                }
            }
        } while ( updated );

        return rawRules.Where( rule => usedKeys.Contains( rule.Split( " -> " )[0] ) ).ToArray();
    }

    public static Dictionary<string, List<List<string>>> ConvertToRulesDict( string[] array )
    {
        var rawRules = ParseRawRules( array );
        ValidateUniqueKeys( rawRules, array.Length );
        Dictionary<string, List<List<string>>> result = new();
        AddStartRule( result, rawRules[0][0] );

        foreach ( var line in rawRules )
        {
            ProcessRule( line, ref result );
        }

        while ( HasEpsilon( result ) )
        {
            HandleRemoveEpsilon( GetFirstEpsilon( result ), ref result );
        }

        return result;
    }

    public static void WriteToConsole( Dictionary<string, List<List<string>>> dict )
    {
        foreach ( var keyValuePair in dict )
        {
            foreach ( var val in keyValuePair.Value )
            {
                Console.WriteLine( $"{keyValuePair.Key}: {String.Join( ", ", val )}" );
            }
        }
    }

    private static List<string> ReadAllLines( string fileName )
    {
        List<string> result = new();
        using var sr = new StreamReader( fileName );
        while ( !sr.EndOfStream )
        {
            var line = sr.ReadLine();
            if ( line != null ) result.Add( line );
        }

        return result;
    }

    private static void ValidateFileNotEmpty( List<string> lines, string fileName )
    {
        if ( lines.Count == 0 )
        {
            throw new Exception( $"Файл {fileName} пуст" );
        }
    }
    
    private static string NormalizeWhitespace(string[] lines)
    {
        return string.Join(" ", lines.Select(s =>
        {
            var newString = s.Replace('\t', ' ');
            while (newString.Contains("  "))
            {
                newString = newString.Replace("  ", " ");
            }
            return newString;
        }));
    }

    private static List<string[]> ParseRawRules( string[] array )
    {
        return array.Select( str => str.Split( " -> " ) ).ToList();
    }

    private static void ValidateUniqueKeys( List<string[]> rawRules, int ruleCount )
    {
        if ( rawRules.Select( sArr => sArr[0] ).Distinct().Count() < ruleCount )
        {
            throw new Exception( "Правила не должны содержать несколько одинаковых ключей" );
        }
    }

    private static void AddStartRule( Dictionary<string, List<List<string>>> result, string startKey )
    {
        result.Add( "Z", [new List<string> { startKey, "#" }] );
    }

    private static void ProcessRule( string[] line, ref Dictionary<string, List<List<string>>> result )
    {
        result.Add( line[0], new List<List<string>>() );
        var variants = ParseVariants( line[1] );

        result[line[0]].AddRange( variants );
    }

    private static List<List<string>> ParseVariants( string rule )
    {
        return rule.Split( " | " ).Select( str => str.Split( ' ' ).ToList() ).ToList();
    }

    private static bool HasEpsilon( Dictionary<string, List<List<string>>> rules )
    {
        return rules.Any( pair => pair.Value.Any( list => list.Contains( "eps" ) ) );
    }

    private static string GetFirstEpsilon( Dictionary<string, List<List<string>>> rules )
    {
        return rules.First( pair => pair.Value.Any( list => list.Contains( "eps" ) ) ).Key;
    }

    private static void HandleRemoveEpsilon( string epsilonKey, ref Dictionary<string, List<List<string>>> result )
    {
        foreach ( var rule in result.Where( pair => pair.Value.Any( list => list.Contains( epsilonKey ) ) ) )
        {
            List<List<string>> uncheckedValues = rule.Value.Where( list => list.Contains( epsilonKey ) ).ToList();
            List<List<string>> checkedValues = rule.Value.ToList();
            while ( uncheckedValues.Count != 0)
            {
                var value = uncheckedValues.First();
                uncheckedValues.RemoveAt(0);
                
                for ( int i = 0; i < value.Count; i++ )
                {
                    if ( value[i] == epsilonKey )
                    {
                        var newValue = value.ToList();
                        newValue.RemoveAt( i );
                        if ( newValue.Count == 0 )
                        {
                            newValue.Add( "eps" );
                        }

                        if ( !checkedValues.Contains( newValue ) )
                        {
                            uncheckedValues.Add( newValue );
                            checkedValues.Add( newValue );
                            rule.Value.Add( newValue );
                        }
                    }
                }
            }
        }

        var epsilonRule = result.First( pair => pair.Key == epsilonKey ).Value;
        epsilonRule.Remove( epsilonRule.First( list => list.Contains( "eps" ) ) );
    }
}
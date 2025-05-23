namespace LLTable.FileWorkers;

public static class FileWorker
{
    public static string[] ReadFileToArray( string fileName )
    {
        List<string> result = [];
        using var sr = new StreamReader( fileName );
        while ( !sr.EndOfStream )
        {
            var line = sr.ReadLine();
            if ( line != null ) result.Add( line );
        }

        if ( result.Count == 0 )
        {
            throw new Exception( $"Файл {fileName} пуст" );
        }

        return result.ToArray();
    }

    public static string ReadFileToString( string fileName )
    {
        var stringArray = ReadFileToArray( fileName );

        return String.Join( " ", stringArray.Select( s =>
        {
            var newString = s.Replace( '\t', ' ' );

            while ( newString.Contains( "  " ) )
            {
                newString = newString.Replace( "  ", " " );
            }

            return newString;
        } ) );
    }

    public static Dictionary<string, List<List<string>>> ConvertToRulesDict( string[] array )
    {
        Dictionary<string, List<List<string>>> result = new();
        var rawRules = array.Select( str => str.Split( " -> " ) ).ToArray();

        if ( rawRules.Select( sArr => sArr[0] ).Distinct().Count() < array.Length )
        {
            throw new Exception( "Правила не должны содержать несколько одинаковых ключей" );
        }

        result.Add( "Z", [[rawRules[0][0], "#"]] );

        foreach ( var line in rawRules )
        {
            result.Add( line[0], [] );

            var ruleCount = 0;
            var variants = line[1].Split( " | " ).Select( str => str.Split( ' ' ).ToList() ).ToList();

            RemoveSameStart( line[0], ref variants, ref ruleCount, ref result );

            if ( variants.Any( variant => variant.First() == line[0] ) ) // Левая рекурсия
            {
                var newName = line[0] + ruleCount;
                var recursiveVariants = variants.Where( variant => variant.First() == line[0] )
                    .Select( variant => variant.Skip( 1 ).Append( newName ).ToList() )
                    .Append( ["eps"] )
                    .ToList();
                variants = variants.Where( variant => variant.First() != line[0] )
                    .Select( variant => variant.Append( newName ).ToList() )
                    .ToList();


                result.Add( newName, recursiveVariants );
            }


            result[line[0]].AddRange( variants );
        }

        return result;
    }

    private static void RemoveSameStart( string name, ref List<List<string>> variants, ref int ruleCount,
        ref Dictionary<string, List<List<string>>> result )
    {
        var sameVariants = variants.Select( variant => variant.ToList() ).GroupBy( variant => variant.First() )
            .ToList();
        if ( !sameVariants.Any( group => group.Count() > 1 ) ) return;
        List<List<string>> newVariants = [];

        foreach ( var variant in sameVariants )
        {
            if ( variant.Count() == 1 )
            {
                newVariants.Add( variant.SelectMany( list => list ).ToList() );
                continue;
            }

            List<string> sameStart = [];
            var newGroup = variant;
            var isEps = false;

            do
            {
                sameStart.Add( newGroup.Key );

                if ( newGroup.Any( group => group.Count == sameStart.Count ) )
                {
                    isEps = true;
                    break;
                }

                newGroup = newGroup.Select( list => list ).GroupBy( list => list.Skip( sameStart.Count ).First() )
                    .First();
            } while ( newGroup.Count() == variant.Count() );

            var newName = name + ruleCount++;
            newVariants.Add( [..sameStart, newName] );

            var resultVariants = variant.Select( list => list.Skip( sameStart.Count ).ToList() )
                .Where( list => list.Count > 0 ).ToList();
            if ( isEps )
            {
                resultVariants.Add( ["eps"] );
            }

            result.Add( newName, resultVariants );

            RemoveSameStart( name, ref resultVariants, ref ruleCount, ref result );
        }

        variants = newVariants;
    }

    public static void WriteToConsole( Dictionary<string, List<List<string>>> dict )
    {
        foreach ( var keyValuePair in dict )
        {
            foreach ( var val in keyValuePair.Value )
            {
                Console.WriteLine( $"{keyValuePair.Key}: {String.Join( " ", val )}" );
            }
        }
    }
}
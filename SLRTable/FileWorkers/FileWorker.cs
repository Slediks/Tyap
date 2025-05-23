namespace SLRTable.FileWorkers;

public static class FileWorker
{
    public static string[] ReadFileToArray( string fileName )
    {
        List<string> result = new();
        using var sr = new StreamReader( fileName );
        while ( !sr.EndOfStream )
        {
            var line = sr.ReadLine();
            if (line != null) result.Add( line );
        }

        return result.ToArray();
    }
    
    public static Dictionary<string, List<List<string>>> ConvertToRulesDict( string[] array )
    {
        Dictionary<string, List<List<string>>> result = new();

        foreach ( var s in array )
        {
            var line = s.Split( " -> " );

            result.Add( line[0], new List<List<string>>() );
            foreach ( var variant in line[1].Split( " | " ) )
            {
                result[line[0]].Add( new List<string>( variant.Split( ' ' ) ) );
            }
        }

        return result;
    }

    public static void WriteToConsole( Dictionary<string, List<List<string>>> dict )
    {
        foreach ( var keyValuePair in dict )
        {
            foreach ( var val in keyValuePair.Value )
            {
                Console.WriteLine($"{keyValuePair.Key}: {String.Join( ", ", val )}");
            }
        }
    }
}
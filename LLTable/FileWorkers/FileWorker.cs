namespace LLTable.FileWorkers;

public static class FileWorker
{
    public static string[] ReadFileToArray( string fileName )
    {
        var lines = ReadAllLines(fileName);
        ValidateFileNotEmpty(lines, fileName);

        return lines.ToArray();
    }
    
    private static List<string> ReadAllLines(string fileName)
    {
        List<string> result = new();
        using var sr = new StreamReader(fileName);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (line != null) result.Add(line);
        }
        return result;
    }

    private static void ValidateFileNotEmpty(List<string> lines, string fileName)
    {
        if (lines.Count == 0)
        {
            throw new Exception($"Файл {fileName} пуст");
        }
    }

    public static string ReadFileToString( string fileName )
    {
        var stringArray = ReadFileToArray( fileName );
        return NormalizeWhitespace(stringArray);
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

    public static Dictionary<string, List<List<string>>> ConvertToRulesDict( string[] array )
    {   
        var rawRules = ParseRawRules(array);
        ValidateUniqueKeys(rawRules, array.Length);
        Dictionary<string, List<List<string>>> result = new();
        AddStartRule(result, rawRules[0][0]);

        foreach ( var line in rawRules )
        {
            ProcessRule(line, ref result);
        }

        return result;
    }
    
    private static List<string[]> ParseRawRules(string[] array)
    {
        return array.Select(str => str.Split(" -> ")).ToList();
    }
    
    private static void ValidateUniqueKeys(List<string[]> rawRules, int ruleCount)
    {
        if (rawRules.Select(sArr => sArr[0]).Distinct().Count() < ruleCount)
        {
            throw new Exception("Правила не должны содержать несколько одинаковых ключей");
        }
    }
    
    private static void AddStartRule(Dictionary<string, List<List<string>>> result, string startKey)
    {
        result.Add("Z", new List<List<string>> { new List<string> { startKey, "#" } });
    }
    
    private static void ProcessRule(string[] line, ref Dictionary<string, List<List<string>>> result)
    {   
        result.Add(line[0], new List<List<string>>());
        var ruleCount = 0;
        var variants = ParseVariants(line[1]);

        RemoveSameStart(line[0], ref variants, ref ruleCount, ref result);

        if (HasLeftRecursion(line[0], variants))
        {
            HandleLeftRecursion(line[0], ref variants, ref ruleCount, ref result);
        }

        result[line[0]].AddRange(variants);
    }
    
    private static List<List<string>> ParseVariants(string rule)
    {
        return rule.Split(" | ").Select(str => str.Split(' ').ToList()).ToList();
    }
    
    private static bool HasLeftRecursion(string key, List<List<string>> variants)
    {
        return variants.Any(variant => variant.First() == key);
    }
    
    private static void HandleLeftRecursion(string key, ref List<List<string>> variants, ref int ruleCount,
        ref Dictionary<string, List<List<string>>> result)
    {
        var newName = key + ruleCount;
        var recursiveVariants = ExtractRecursiveVariants(key, variants, newName);
        variants = RemoveRecursiveVariants(key, variants, newName);

        result.Add(newName, recursiveVariants);
    }
    
    private static List<List<string>> ExtractRecursiveVariants(string key, List<List<string>> variants, string newName)
    {
        return variants.Where(variant => variant.First() == key)
            .Select(variant => variant.Skip(1).Append(newName).ToList())
            .Append(new List<string> { "eps" })
            .ToList();
    }

    private static List<List<string>> RemoveRecursiveVariants(string key, List<List<string>> variants, string newName)
    {
        return variants.Where(variant => variant.First() != key)
            .Select(variant => variant.Append(newName).ToList())
            .ToList();
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
    
    public static string[] FilterRules(string[] rawRules)
    {
        var usedKeys = new HashSet<string>();
        var rules = rawRules.Select(str => str.Split(" -> ")).ToList();
        
        usedKeys.Add(rules[0][0]);

        bool updated;
        do
        {
            updated = false;
            foreach (var rule in rules)
            {
                var key = rule[0];
                var values = rule[1].Split(" | ").SelectMany(v => v.Split(' ')).ToList();

                if (usedKeys.Contains(key) && values.Any(v => !usedKeys.Contains(v) && rules.Any(r => r[0] == v)))
                {
                    foreach (var value in values)
                    {
                        if (!usedKeys.Contains(value) && rules.Any(r => r[0] == value))
                        {
                            usedKeys.Add(value);
                            updated = true;
                        }
                    }
                }
            }
        } while (updated);
        
        return rawRules.Where(rule => usedKeys.Contains(rule.Split(" -> ")[0])).ToArray();
    }
}
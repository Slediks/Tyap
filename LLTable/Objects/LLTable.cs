﻿namespace LLTable.Objects;

// ReSharper disable once InconsistentNaming
public class LLTable
{
    public readonly List<LLRow> Table = [];
    private const string End = "end";
    private const string Eps = "eps";

    private static readonly Dictionary<string, string> Names = new()
    {
        { "Id", "Номер" },
        { "Name", "Символ" },
        { "DirectSet", "Напр. Множ." },
        { "Transition", "Переход" },
        { "Error", "Ошибка" },
        { "Shift", "Сдвиг" },
        { "Stack", "Стек" },
        { "End", "Конец" }
    };

    public LLTable( Dictionary<string, List<List<string>>> dict )
    {
        GenerateTable( dict );
    }

    private void GenerateTable( Dictionary<string, List<List<string>>> dict )
    {
        var epsCount = 0;

        foreach ( var dictPair in dict )
        {
            foreach ( var val in dictPair.Value )
            {
                Table.Add( new LLRow(
                    Table.Count + 1,
                    dictPair.Key,
                    [],
                    null,
                    val == dictPair.Value.Last(),
                    false,
                    null,
                    false,
                    isKey: true ) ); // Без directSet, transition
            }

            foreach ( var val in dictPair.Value )
            {
                Table.First( row => row.Name == dictPair.Key && row.Transition == null ).Transition =
                    Table.Count + 1; // Заполняем transition предыдущему шагу

                val.ForEach( str =>
                    {
                        var isKey = dict.ContainsKey( str );

                        Table.Add( new LLRow(
                            Table.Count + 1,
                            str,
                            [],
                            isKey ? 0 : Table.Count + 2,
                            true,
                            !isKey && str != Eps,
                            isKey ? Table.Count + 2 : null,
                            str == End,
                            isWord: !isKey,
                            epsNumber: str == Eps ? epsCount : null ) );

                        if ( str == Eps )
                        {
                            epsCount++;
                        }
                    }
                );

                if ( dict.ContainsKey( Table.Last().Name ) )
                {
                    Table.Last().Stack = null;
                }
                else
                {
                    Table.Last().Transition = null;
                }
            }
        }

        FillTransitions();
        GenerateDirectSet();
    }

    private void FillTransitions()
    {
        foreach ( var row in Table.Where( row => row.Transition == 0 ) )
        {
            row.Transition = Table.First( r => r.Name == row.Name && r.IsKey ).Id;
        }
    }

    private void GenerateDirectSet()
    {
        InitializeDirectSets();
        ProcessEpsilonRows();
        DeleteDouble();
    }

    private void InitializeDirectSets()
    {
        foreach ( var row in Table.Where( row => row.DirectSet.Count == 0 ) )
        {
            if ( row.DirectSet.Count > 0 ) continue;

            List<string> rowDirSet = [];
            if ( row.IsKey )
            {
                List<int> checkedRows = [];
                rowDirSet = GetDirectionSet( row.Transition, ref checkedRows );
            }
            else
            {
                var keyRows = Table.Where( r => r.IsKey && r.Name == row.Name ).ToList();
                foreach ( var keyRow in keyRows )
                {
                    List<int> checkedRows = [];
                    rowDirSet.AddRange( GetDirectionSet( keyRow.Transition, ref checkedRows ) );
                }
            }

            row.DirectSet.AddRange( rowDirSet.Distinct() );
        }
    }

    private void ProcessEpsilonRows()
    {
        var epsCounter = 0;

        while ( Table.Any( row => row.DirectSet.Any( str => str.StartsWith( Eps + "_" ) ) ) )
        {
            var epsRows = Table
                .Where( row => row.EpsNumber != null && row.DirectSet.Any( str => str.StartsWith( Eps + "_" )) )
                .ToList();

            if ( epsCounter == epsRows.Count && epsCounter == 1 ) throw new Exception( "Loop in one epsilon? How?" );
            epsCounter = epsRows.Count;

            foreach ( var row in epsRows )
            {
                HandleEpsilonRow( row );
            }
        }
    }

    private void HandleEpsilonRow( LLRow row )
    {
        var key = Table.First( r => r.Transition == row.Id ).Name;

        if ( !CreateEps( key, row ) ) return;
        ReplaceEps( row );
    }

    private void DeleteDouble()
    {
        Table.ForEach( row => row.DirectSet = row.DirectSet.Distinct().ToList() );
    }

    private List<string> GetDirectionSet( int? id, ref List<int> checkedRows )
    {
        if ( id != null && !checkedRows.Contains( id.Value ) ) checkedRows.Add( id.Value );
        var thatRow = Table.First( row => row.Id == id );

        if ( !thatRow.IsKey )
            return thatRow.DirectSet.Count > 0
                ? thatRow.DirectSet
                : GetDirectionSet( thatRow.Transition, ref checkedRows );

        List<string> dirSet = [];
        var keyRows = Table.Where( row => row.IsKey && row.Name == thatRow.Name ).ToList();
        foreach ( var row in keyRows )
        {
            List<string> rowDirSet = [];
            if ( row.DirectSet.Count > 0 )
            {
                rowDirSet = row.DirectSet;
            }
            else if ( !checkedRows.Contains( row.Transition!.Value ) )
            {
                rowDirSet = GetDirectionSet( row.Transition, ref checkedRows );
            }

            dirSet.AddRange( rowDirSet );
        }

        return dirSet.Distinct().ToList();
    }

    private bool CreateEps( string key, LLRow epsRow )
    {
        if ( epsRow.DirectSet.Contains( epsRow.Name ) )
        {
            var newSet = GetEpsSet( key );
            if ( newSet.Contains( epsRow.Name ) )
            {
                newSet.Remove( epsRow.Name );
            }

            epsRow.DirectSet = newSet;
            return !newSet.Any( str => str.StartsWith( Eps + "_" ) );
        }

        // Все eps правила, которые встречаются в текущем eps правиле
        var epsRowsSet = epsRow.DirectSet.Where( str => str.StartsWith( Eps + "_" ) ).Select( str => Table.First( row => row.EpsNumber != null && row.Name == str ) ).ToList();

        // Убираем из списка те правила, которые еще не были предварительно созданны выше
        foreach ( var row in epsRowsSet.ToList().Where( row => row.DirectSet.Contains( row.Name ) ) )
        {
            epsRowsSet.Remove( row );
        }
            
        // Добавляем в текущее eps правило dirSet'ы
        epsRowsSet.ForEach( row =>
        {
            epsRow.DirectSet.AddRange( row.DirectSet );
        } );

        // Убираем повторы и уже добавленные правила
        epsRow.DirectSet = epsRow.DirectSet.Distinct().ToList();
        epsRow.DirectSet.Remove( epsRow.Name );
        epsRowsSet.ForEach( row => epsRow.DirectSet.Remove( row.Name ) );
            
        return !epsRow.DirectSet.Any( str => str.StartsWith( Eps + "_" ) );
    }

    private List<string> GetEpsSet( string key, string? baseKey = null )
    {
        var newSet = new List<string>();
        // Все записи, каоторые могут быть преобразованны в eps
        var rows = Table.Where( row => row.Name == key && !row.IsKey ).ToList();

        // Не полседние в правиле
        newSet.AddRange( rows.Where( row => row.Stack != null )
            .SelectMany( row => Table.First( r => r.Id == row.Stack ).DirectSet ).Distinct() );

        // Последние в правиле
        newSet.AddRange( rows
            .Where( row => row.Stack == null && row.Name != GetParentKey( row.Id ) && row.Name != baseKey )
            .SelectMany( row => GetEpsSet( GetParentKey( row.Id ), baseKey ?? key ) ) );

        return newSet.Distinct().ToList();
    }

    private string GetParentKey( int id )
    {
        return Table.Last( row => row.IsKey && row.Id < id ).Name;
    }

    private void ReplaceEps( LLRow epsRow )
    {
        foreach ( var row in Table.Where( row => row.DirectSet.Contains( epsRow.Name ) ) )
        {
            row.DirectSet.Remove( epsRow.Name );
            List<int> checkedRows = [];
            row.DirectSet.AddRange( row.IsKey || row.Stack == null
                ? epsRow.DirectSet
                : GetDirectionSet( row.Stack, ref checkedRows ) );
        }
    }

    public override string ToString()
    {
        var header = String.Join( ' ', Names.Values );
        return header + '\n' + String.Join( '\n', Table );
    }

    public string ToConsoleTable()
    {
        var maxId = Math.Max( Names["Id"].Length, Table.Select( row => row.Id.ToString().Length ).Max() );
        var maxName = Math.Max( Names["Name"].Length, Table.Select( row => row.Name.Length ).Max() );
        var maxSet = Math.Max( Names["DirectSet"].Length,
            Table.Select( row =>
                    $"[{String.Join( ", ", row.DirectSet.GetRange( 0, Math.Min( row.DirectSet.Count, 5 ) ) )}]".Length )
                .Max() );
        var maxTrans = Names["Transition"].Length;
        var maxError = Names["Error"].Length;
        var maxShift = Names["Shift"].Length;
        var maxStack = Names["Stack"].Length;
        var maxEnd = Names["End"].Length;

        var format = "{0, " + maxId +
                     "} {1, " + maxName +
                     "} {2, " + maxSet +
                     "} {3, " + maxTrans +
                     "} {4, " + maxError +
                     "} {5, " + maxShift +
                     "} {6, " + maxStack +
                     "} {7, " + maxEnd + "}";

        var header = String.Format( format, Names["Id"], Names["Name"], Names["DirectSet"], Names["Transition"],
            Names["Error"], Names["Shift"], Names["Stack"], Names["End"] );

        return header + '\n' + String.Join( '\n', Table.Select( row => row.ToConsoleRow( format ) ) );
    }

    public string ToTable()
    {
        var header = String.Join( ";", Names.Values ) + ";";
        return header + '\n' + String.Join( '\n', Table.Select( row => row.ToTableRow() ) );
    }
}
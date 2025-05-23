namespace LLTable.Objects;

// ReSharper disable once InconsistentNaming
public class LLRow
{
    public int Id;

    public string Name => EpsNumber == null ? _name : _name + "_" + EpsNumber;

    public List<string> DirectSet;
    public int? Transition;
    public readonly bool IsError;
    public readonly bool IsShift;
    public int? Stack;
    public readonly bool IsEnd;
    public readonly bool IsKey;
    private readonly bool _isWord;
    public int? EpsNumber;
    private readonly string _name;

    public LLRow( int id, string name, List<string> directSet, int? transition, bool isError, bool isShift, int? stack,
        bool isEnd, bool isKey = false, bool isWord = false, int? epsNumber = null )
    {
        if ( isKey && isWord ) throw new Exception( "Row can't be key and word at same time." );

        Id = id;
        _name = name;
        DirectSet = directSet;
        Transition = transition;
        IsError = isError;
        IsShift = isShift;
        Stack = stack;
        IsEnd = isEnd;
        IsKey = isKey;
        _isWord = isWord;
        EpsNumber = epsNumber;

        if ( !_isWord ) return;

        DirectSet.Add( Name );
    }

    private string BoolToString( bool state ) => state ? "+" : "-";

    private string IntToString( int? state ) => state == null ? "-" : state.ToString()!;

    public override string ToString()
    {
        return
            $"{Id} {_name} [{String.Join( ", ", DirectSet )}] {IntToString( Transition )} {BoolToString( IsError )} {BoolToString( IsShift )} {IntToString( Stack )} {BoolToString( IsEnd )}";
    }

    public string ToConsoleRow( string format )
    {
        return String.Format( format,
            Id,
            _name,
            $"[{String.Join( ", ", DirectSet.GetRange( 0, Math.Min( DirectSet.Count , 10 ) ) )}]",
            IntToString( Transition ),
            BoolToString( IsError ),
            BoolToString( IsShift ),
            IntToString( Stack ),
            BoolToString( IsEnd )
        );
    }

    public string ToTableRow()
    {
        return
            $"{Id};{_name};{String.Join( ", ", DirectSet )};{IntToString( Transition )};{BoolToString( IsError )};{BoolToString( IsShift )};{IntToString( Stack )};{BoolToString( IsEnd )};";
        ;
    }
}
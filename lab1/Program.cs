namespace lab1;

static class Program
{
    public static void Main( string[] args )
    {
        StreamReader sr = new( args[0] );
        string[] words = sr.ReadLine()!.Trim().Replace( "  ", " " ).Split( ' ' );
        if ( new Soc1( words ).Start() )
        {
            Console.WriteLine( "Soc1" );
        }
        else if ( new Soc2( words ).Start() )
        {
            Console.WriteLine( "Soc2" );
        }
        else
        {
            Console.WriteLine( "Not Soc" );
        }
    }
}

class Soc1
{
    private readonly string[] _words;
    private int _index;

    public Soc1( string[] words )
    {
        _words = words;
    }

    private string Word()
    {
        return _index < _words.Length ? _words[_index] : "";
    }

    public bool Start()
    {
        var ans = A();
        if (!ans) Console.WriteLine(_index);
        return ans;
    }

    private bool A()
    {
        return B() && A1();
    }

    private bool A1()
    {
        if ( Word() != "ау" ) return true;
        _index++;
        return B() && A1();
    }

    private bool B()
    {
        return C() && B1();
    }

    private bool B1()
    {
        if ( Word() != "ку" ) return true;
        _index++;
        return C() && B1();
    }

    private bool C()
    {
        if ( Word() == "ух-ты" )
        {
            _index++;
            return true;
        }

        if ( Word() == "хо" )
        {
            _index++;
            return C();
        }

        if ( Word() != "ну" ) return false;
        _index++;
        if ( !A() ) return false;
        if ( Word() != "и_ну" ) return false;
        _index++;
        return true;
    }
}

class Soc2
{
    private readonly string[] _words;
    private int _index;

    public Soc2( string[] words )
    {
        _words = words;
    }

    private string Word()
    {
        return _index < _words.Length ? _words[_index] : "";
    }

    public bool Start()
    {
        var ans = A();
        if (!ans) Console.WriteLine(_index);
        return ans;
    }

    private bool A()
    {
        if ( Word() != "ой" ) return false;
        _index++;
        if ( !B() ) return false;
        if ( Word() != "ай" ) return false;
        _index++;
        return C();
    }

    private bool B()
    {
        if ( Word() != "ну" ) return false;
        _index++;
        return B1();
    }

    private bool B1()
    {
        if ( Word() != "ну" ) return true;
        _index++;
        return B1();
    }

    private bool C()
    {
        if ( Word() == "ух-ты" )
        {
            _index++;
            return true;
        }

        if ( Word() != "хо" ) return false;
        _index++;
        if ( !C() ) return false;
        return Word() == "хо";
    }
}
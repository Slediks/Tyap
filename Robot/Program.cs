namespace Robot;

static class Program
{
    public static void Main( string[] args )
    {
        StreamReader sr = new( args[0] );
        new Robot( sr.ReadLine()!.Trim().Replace( "  ", " " ).Split( " " ) );
    }
}

class Robot
{
    private readonly string[] _words;
    private int _index;

    private string Word()
    {
        return _index < _words.Length ? _words[_index] : "";
    }

    public Robot( string[] words )
    {
        _words = words;
        if ( Z() )
        {
            Console.WriteLine( "OK" );
            Console.WriteLine( _words.Length );
        }
        else
        {
            Console.WriteLine( "Сосал?" );
            Console.WriteLine( _index + 1 );
        }
    }

    private bool Z()
    {
        if ( Word() != "start" ) return false;
        _index++;
        if ( !A() ) return false;
        return Word() == "stop";
    }

    private bool A()
    {
        return B() && A1();
    }

    private bool A1()
    {
        if ( !D() ) return true;
        return B() && A1();
    }

    private bool B()
    {
        return C() && B1();
    }

    private bool B1()
    {
        if ( !E() ) return true;
        return C() && B1();
    }

    private bool C()
    {
        if ( Word() is "left" or "right" )
        {
            _index++;
            return true;
        }

        if ( Word() == "on45" )
        {
            _index++;
            return C();
        }

        if ( Word() != "hands_up" ) return false;
        _index++;
        if ( !A() ) return false;

        if ( Word() != "hands_down" ) return false;
        _index++;
        return true;
    }

    private bool D()
    {
        if ( Word() != "step_(" ) return false;
        _index++;
        if ( !F() ) return false;
        if ( Word() != ")" ) return false;
        _index++;
        return true;
    }

    private bool E()
    {
        if ( Word() != "turn_head" ) return false;
        _index++;
        return true;
    }

    private bool F()
    {
        if ( Word()[0] is < '0' or > '9' ) return false;
        if ( Word()[0] == '0' && Word().Length > 1 ) return false;
        if ( !F1( 1 ) ) return false;
        _index++;
        return true;
    }

    private bool F1( int index )
    {
        if ( Word().Length == index ) return true;
        return Word()[index] is >= '0' and <= '9' && F1( index + 1 );
    }
}
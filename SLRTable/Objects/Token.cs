using Newtonsoft.Json;

namespace SLRTable.Objects;

public struct Token
{
    [JsonProperty("type")]
    public string Type;
    [JsonProperty("token_name")]
    public string TokenName;
    [JsonProperty("item")]
    public string TokenValue;
    [JsonProperty("line")]
    public int Line;
    [JsonProperty("start_pos")]
    public int StartPos;
    [JsonProperty("end_pos")]
    public int EndPos;

    public override string ToString()
    {
        return TokenName;
    }

    public static bool operator ==(Token? token1, Token? token2)
    {
        if (!token1.HasValue && !token2.HasValue)
        {
            return true;
        }

        if (!token1.HasValue || !token2.HasValue)
        {
            return false;
        }
        
        return token1.Equals(token2);
    }

    public static bool operator !=(Token? token1, Token? token2)
    {
        return !(token1 == token2);
    }
    
    public override bool Equals(object? obj) => Equals(obj as Token? ?? default);

    private bool Equals(Token other)
    {
        return Type == other.Type && TokenName == other.TokenName && TokenValue == other.TokenValue && Line == other.Line && StartPos == other.StartPos && EndPos == other.EndPos;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, TokenName, TokenValue, Line, StartPos, EndPos);
    }
}
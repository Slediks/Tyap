using Newtonsoft.Json;

namespace LLTable.Objects;

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
}
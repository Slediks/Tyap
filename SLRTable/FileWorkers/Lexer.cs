using Newtonsoft.Json;
using SLRTable.Objects;

namespace SLRTable.FileWorkers;

public class Lexer
{
    public readonly List<Token> Tokens;

    public Lexer(string input)
    {
        var args = $"{input} tokens.json";

        using var pProcess = new System.Diagnostics.Process();
        pProcess.StartInfo.FileName = "lexer.exe"; //exe path
        pProcess.StartInfo.Arguments = args; //argument
        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = false; //able to read console output
        pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        pProcess.StartInfo.CreateNoWindow = false; //not display a windows
        pProcess.Start();
        pProcess.WaitForExit();

        using var r = new StreamReader("tokens.json");
        var json = r.ReadToEnd();
        Tokens = JsonConvert.DeserializeObject<List<Token>>(json) ?? [];
        ValidateErrors();
    }

    private void ValidateErrors()
    {
        var errors = Tokens.Where(token => token.Type == "error").ToList();
        if (errors.Count == 0) return;

        var errorMsg = "Синтаксические ошибки:\n" +
                       String.Join("\n",
                           errors.Select(error =>
                           {    
                               var errorDescription = error.TokenName switch
                               {
                                   "unterminated_comment" => "Незавершённый многострочный комментарий",
                                   "unterminated_double_quote" => "Незакрытые двойные кавычки",
                                   "unterminated_single_quote" => "Незакрытые одинарные кавычки",
                                   "unterminated_bracket_open" => "Незакрытая круглая скобка",
                                   "unterminated_brace_open" => "Незакрытая фигурная скобка",
                                   "unterminated_bracket_sq_open" => "Незакрытая квадратная скобка",
                                   "overflow_id" => "Слишком длинный идентификатор",
                                   "invalid_binary" => "Некорректное двоичное число",
                                   "invalid_octal" => "Некорректное восьмеричное число",
                                   "invalid_hex" => "Некорректное шестнадцатеричное число",
                                   "unsupported_number_system" => "Неподдерживаемая система счисления",
                                   "invalid_float" => "Некорректное число с плавающей точкой",
                                   "invalid_scientific" => "Некорректная научная нотация",
                                   "invalid_leading_dot" => "Число начинается с точки без целой части",
                                   "unterminated_binary" => "Незавершённый префикс двоичного числа",
                                   "unterminated_octal" => "Незавершённый префикс восьмеричного числа",
                                   "unterminated_hex" => "Незавершённый префикс шестнадцатеричного числа",
                                   _ => "Неизвестная ошибка"
                               };
                               
                               return $"{error.TokenName} ({errorDescription}): {error.TokenValue} ({error.Line}:{error.StartPos})";
                           }));
        throw new Exception(errorMsg);
    }
}
using System;
using System.Text.RegularExpressions;

namespace MyEcho
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;
            var text = args[0];
            if (text == "?" || text == "/?" || text == "h" || text == "/h")
            {
                Console.WriteLine("# MyEcho by Mustafa Yücel\n");
                Console.WriteLine(@"# Usage : myecho ""{0,15} Menu {DarkBlue,Red} Title""");
                
                Console.WriteLine(@"- Argument: ""{BackColor,ForeColor}text...{BackColor,ForeColor}text...""" + "\n");
                Console.WriteLine(@"- Pass argument within """"(double quotes)." + "\n");
                Console.WriteLine(@"- Color values could be name of color or number." + "\n");
                Console.WriteLine("- Colors: 0=Black,    1=DarkBlue,     2=DarkGreen,    3=DarkCyan,\n" +
                                  "          4=DarkRed,  5=DarkMagenta,  6=DarkYellow,   7=Gray,\n" +
                                  "          8=DarkGray, 9=Blue,        10=Green,       11=Cyan,\n" +
                                  "          12=Red,    13=Magenta,     14=Yellow,      15=White\n");
                return;
            }

            // - Color 07 will set it to the default scheme that cmd.exe uses.

            // - Pass argument within ""(quotes) so it will be treated as a whole, otherwise spaces will separate the text into arguments.
            //string text = "{111,2}bbb {Black, Blue}asdfas {White, Blue} xxx{111,2}bbb {0,441}ccc";
            
            string colorPattern = @"(?:{\s*(?:(?<backColorText>[a-z]+)|(?<backColorNum>\d+))\s*,\s*(?:(?<foreColorText>[a-z]+)|(?<foreColorNum>\d+))\s*})+";
            var matches = Regex.Matches(text, colorPattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                ConsoleColor? GetColor(string colorType)
                {
                    ConsoleColor? consoleColor = null;
                    var colorText = match.Groups[colorType + "Text"];
                    if (colorText != Match.Empty)
                    {
                        if (Enum.TryParse(colorText.Value, true, out ConsoleColor _consoleColor)) consoleColor = _consoleColor;
                    }

                    if (consoleColor == null)
                    {
                        var colorNum = match.Groups[colorType + "Num"];
                        if (colorNum != Match.Empty)
                        {
                            if (int.TryParse(colorNum.Value, out int _consoleColor) && Enum.IsDefined(typeof(ConsoleColor), _consoleColor)) consoleColor = (ConsoleColor)_consoleColor;
                        }
                    }

                    return consoleColor;
                }

                var backColor = GetColor("backColor");
                var foreColor = GetColor("foreColor");

                if (backColor != null && foreColor != null)
                {
                    Console.BackgroundColor = backColor.Value;
                    Console.ForegroundColor = foreColor.Value;
                }
                else
                    Console.ResetColor();

                var textStart = match.Index + match.Length;
                int textEnd;
                var nextMatch = match.NextMatch();
                if (nextMatch != Match.Empty)
                    textEnd = nextMatch.Index - 1;
                else
                    textEnd = text.Length - 1;
                Console.Write(text.Substring(textStart, textEnd - textStart + 1));
            }

            if (matches.Count == 0)
                Console.Write(text);

            Console.ResetColor();
        }
    }
}

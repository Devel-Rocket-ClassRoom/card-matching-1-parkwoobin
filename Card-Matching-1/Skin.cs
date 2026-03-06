using System;


public interface ISkin
{
    void DisplaySkin();
    string GetCardValue(int number);
    ConsoleColor GetColor(int cardValue);
}

class AlphabetSkin : ISkin
{
    public void DisplaySkin()
    {
        Console.WriteLine("알파벳 카드 스킨이 적용되었습니다.");
    }

    public string GetCardValue(int number)
    {
        // 1→A, 2→B, 3→C, ...
        return ((char)('A' + number - 1)).ToString();
    }
    public ConsoleColor GetColor(int cardValue)
    {
        ConsoleColor[] colors = {
            ConsoleColor.Yellow,      // A
            ConsoleColor.Blue,    // B
            ConsoleColor.Red,         // C
            ConsoleColor.Green,        // D
            ConsoleColor.Cyan,       // E
            ConsoleColor.Magenta,     // F
            ConsoleColor.White,   // G
            ConsoleColor.DarkCyan,    // H
            ConsoleColor.DarkBlue,        // I
            ConsoleColor.DarkRed,     // J
            ConsoleColor.DarkMagenta, // K
            ConsoleColor.Gray   // L
        };
        if (cardValue >= 1 && cardValue <= colors.Length)
            return colors[cardValue - 1];
        return ConsoleColor.White;
    }
}

class SymbolSkin : ISkin
{
    public void DisplaySkin()
    {
        Console.WriteLine("기호 카드 스킨이 적용되었습니다.");
    }

    public string GetCardValue(int number)
    {
        string[] symbols = { "★", "♠", "♥", "◆", "♣", "●", "■", "▲", "○", "▼", "☎", "※" };
        return symbols[(number - 1) % symbols.Length];
    }

    public ConsoleColor GetColor(int cardValue)
    {
        ConsoleColor[] colors = {
            ConsoleColor.Yellow,      // ★
            ConsoleColor.Blue,    // ♠
            ConsoleColor.Red,         // ♥
            ConsoleColor.Green,        // ◆
            ConsoleColor.Cyan,       // ♣
            ConsoleColor.Magenta,     // ●
            ConsoleColor.White,   // ■
            ConsoleColor.DarkYellow,    // ▲
            ConsoleColor.DarkBlue,        // ○
            ConsoleColor.DarkRed,     // ▼
            ConsoleColor.DarkMagenta, // ☎
            ConsoleColor.Gray   // ※
        };
        if (cardValue >= 1 && cardValue <= colors.Length)
            return colors[cardValue - 1];
        return ConsoleColor.White;
    }
}
class NumberSkin : ISkin
{
    public void DisplaySkin()
    {
        Console.WriteLine("숫자 카드 스킨이 적용되었습니다.");
    }

    public string GetCardValue(int number)
    {
        return number.ToString();
    }
    public ConsoleColor GetColor(int cardValue)
    {
        return ConsoleColor.White;
    }
}

using System;


public class CardDisplay
{
    int i = 0;  // 게임 시작 시 한 번만 초기 메시지를 출력하기 위한 변수
    public void Display(Card card)  // 카드 상태를 콘솔에 출력하는 메서드
    {
        if (i == 0)
        {
            Console.Clear();
            Console.WriteLine("=== 카드 짝 맞추기 게임 ===\n");
            Console.WriteLine("카드 섞는 중\n");
            i++;
        }
        // 열 번호 헤더 출력
        Console.Write("     ");
        for (int c = 1; c <= card.Cols; c++)
            Console.Write($"{c}열  ");
        Console.WriteLine();

        for (int i = 0; i < card.Rows; i++)
        {
            Console.Write($"{i + 1}행  ");  // 카드 좌표 표시
            for (int j = 0; j < card.Cols; j++)
            {

                if (card.cardState[i, j] == 0)
                {
                    Console.Write($"{"**",-5}");  // 뒤집힌 카드
                }
                else if (card.cardState[i, j] == 1)    // 1면 드러난 것
                {
                    string val = card.Skin != null ? card.Skin.GetCardValue(card.card[i, j]) : card.card[i, j].ToString();
                    if (card.Skin != null)
                        Console.ForegroundColor = card.Skin.GetColor(card.card[i, j]);
                    Console.Write($"[{val,1}]  ");  // 드러난 카드
                    Console.ResetColor();
                }
                else if (card.cardState[i, j] == 2)    // 2면 맞춘 것
                {
                    string val = card.Skin != null ? card.Skin.GetCardValue(card.card[i, j]) : card.card[i, j].ToString();
                    if (card.Skin != null)
                        Console.ForegroundColor = card.Skin.GetColor(card.card[i, j]);
                    Console.Write($"{val,2}   ");  // 드러난 카드
                    Console.ResetColor();
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    public void CountTry(int count, int matchCount, Card card, Difficulty difficulty, GameBase gameBase = null)
    {
        if (gameBase != null)
        {
            int statusY = Console.CursorTop;
            Console.Write(gameBase.GetStatus());
            // 타임어택 모드면 매초 상태 줄 리프레시
            if (gameBase is TimeAttackMode ta)
                ta.StartRefresh(statusY);
        }
        else
        {
            Console.WriteLine($"총 시도 횟수: {count}/{difficulty.GetDifficulty()} | 찾은 쌍: {matchCount}/{card.card.Length / 2}\n");
        }
    }
}

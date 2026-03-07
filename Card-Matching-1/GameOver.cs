using System;


public class Over
{
    public void GameOver(ref int clearFlag, int count, int matchCount, Card card, Difficulty difficulty, CardDisplay cardDisplay, ref int GameFlag, GameBase gameBase = null)
    {
        if (gameBase != null)
        {
            gameBase.Count = count;
            gameBase.MatchCount = matchCount;
            gameBase.CheckGameOver(ref clearFlag, card, cardDisplay, ref GameFlag);
        }
        else
        {
            // 폴백: 기본 클래식 로직
            if (matchCount == card.card.Length / 2)
            {
                Console.Clear();
                cardDisplay.Display(card);
                clearFlag = 1;
                GameFlag = 1;
                return;
            }
            if (count == difficulty.GetDifficulty())
            {
                Console.WriteLine("=== 게임 오버! ===");
                Console.WriteLine($"총 시도 횟수 모두 사용했습니다.");
                Console.WriteLine($"찾은 쌍: {matchCount}/{card.card.Length / 2}\n");
                GameFlag = 1;
            }
        }
    }
}

public class Difficulty
{
    public int Input { get; private set; }  // 난이도 입력 값 저장

    public void SetDifficulty()
    {
        Console.WriteLine("난이도를 선택하세요: ");
        Console.WriteLine("1. 쉬움 (2x4)");
        Console.WriteLine("2. 보통 (4x4)");
        Console.WriteLine("3. 어려움 (4x6)");
        Console.Write("선택: ");
        Input = int.Parse(Console.ReadLine());
    }
    public int GetDifficulty()
    {
        if (Input == 1)
        {
            return 10;
        }
        else if (Input == 2)
        {
            return 20;
        }
        else if (Input == 3)
        {
            return 30;
        }
        else
        {
            Console.WriteLine("잘못된 입력입니다. 기본 난이도(보통)로 설정됩니다.");
            return 20;
        }
    }
}

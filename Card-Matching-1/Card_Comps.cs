using System;

public class Card  // 카드 구성 클래스
{
    public int diffLevel = 2;  // 현재 난이도 (1: 쉬움, 2: 보통, 3: 어려움)

    // 쉬움 난이도 기준 카드 배열 (2x4)
    public int[,] card1 = {     // 카드의 숫자 배열
        { 1, 2, 3, 4 },
        { 1, 2, 3, 4 },
        };

    // 카드 상태 배열 (0: 뒤집힘, 1: 앞면)
    public int[,] cardState1 = new int[2, 4];

    // 보통 난이도 기준 카드 배열 (4x4)
    public int[,] card2 = {     // 카드의 숫자 배열
        { 1, 2, 3, 4 },
        { 5, 6, 7, 8 },
        { 1, 2, 3, 4 },
        { 5, 6, 7, 8 }
        };

    // 카드 상태 배열 (0: 뒤집힘, 1: 앞면)
    public int[,] cardState2 = new int[4, 4];

    // 어려움 난이도 기준 카드 배열 (4x6)
    public int[,] card3 = {     // 카드의 숫자 배열
        { 1, 2, 3, 4, 5, 6 },
        { 7, 8, 9, 10, 11, 12 },
        { 1, 2, 3, 4, 5, 6 },
        { 7, 8, 9, 10, 11, 12 }
        };

    // 카드 상태 배열 (0: 뒤집힘, 1: 앞면)
    public int[,] cardState3 = new int[4, 6];



    // 난이도에 따라 현재 사용할 카드 배열 반환
    public int[,] card => diffLevel == 1 ? card1 : diffLevel == 3 ? card3 : card2;

    // 난이도에 따라 현재 사용할 카드 상태 배열 반환
    public int[,] cardState => diffLevel == 1 ? cardState1 : diffLevel == 3 ? cardState3 : cardState2;

    // 현재 카드 배열의 행 수
    public int Rows => card.GetLength(0);

    // 현재 카드 배열의 열 수
    public int Cols => card.GetLength(1);
}



public class CardRandom  // 카드 섞기 클래스
{
    Random random = new Random();

    public void Shuffle(Card card)  // 카드 섞기 메서드
    {
        int rows = card.Rows;
        int cols = card.Cols;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int x = random.Next(0, rows);
                int y = random.Next(0, cols);

                // 카드 위치 교환
                int temp = card.card[i, j];
                card.card[i, j] = card.card[x, y];
                card.card[x, y] = temp;
            }
        }
    }
}

public class CardStatus  // 카드 상태 클래스
{
    // 각 카드마다 0으로 초기화하여 모두 뒤집힘 상태로 설정
    public void Hide(Card card)  // 카드 숨기기 메서드
    {
        for (int i = 0; i < card.Rows; i++)
        {
            for (int j = 0; j < card.Cols; j++)
            {
                card.cardState[i, j] = 0;  // 모든 카드를 뒤집힘 상태로 설정
            }
        }
    }
    public void Reveal(Card card, int x, int y)  // 카드 드러내기 메서드
    {
        card.cardState[x, y] = 1;  // 해당 카드를 앞면 상태로 설정
    }
}


public class CardCheck  // 카드 일치 검사 클래스
{
    public bool CheckMatch(Card card, int x1, int y1, int x2, int y2)  // 카드 일치 검사 메서드
    {
        return card.card[x1, y1] == card.card[x2, y2];  // 두 카드의 숫자가 같은지 비교
    }
    public void match(Card card, int x1, int y1, int x2, int y2, ref int matchCount)
    {
        bool isMatch = CheckMatch(card, x1, y1, x2, y2);  // 카드 일치 여부 확인
        if (isMatch)
        {
            Console.WriteLine("짝을 찾았습니다!");
            card.cardState[x1, y1] = 2;  // 일치한 카드는 상태를 2로 설정
            card.cardState[x2, y2] = 2;
            matchCount++;  // 일치한 카드 쌍 수 증가
        }
        else
        {
            Console.WriteLine("짝이 맞지 않습니다!");
            card.cardState[x1, y1] = 0;  // 일치하지 않은 카드는 다시 뒤집힘 상태로 설정
            card.cardState[x2, y2] = 0;
        }
    }

}

public class CardChoice // 카드 선택 클래스
{
    public void ChooseCard1(Card card, CardStatus cardStatus, CardDisplay cardDisplay, out int x1, out int y1)  // 첫 번째 카드 선택 메서드
    {
        Console.Write("첫 번째 카드를 선택하세요 (행 열): ");
        string input1 = Console.ReadLine();
        string[] parts1 = input1.Split(new char[] { ' ', ',' });
        if (parts1.Length < 2 || !int.TryParse(parts1[0], out x1) || !int.TryParse(parts1[1], out y1))
        {
            Console.WriteLine("숫자만 입력해주세요.");
            ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1);
            return;
        }
        x1 -= 1;
        y1 -= 1;
        if (x1 < 0 || x1 >= card.Rows || y1 < 0 || y1 >= card.Cols)
        {
            Console.WriteLine($"범위를 벗어났습니다. (1~{card.Rows}행, 1~{card.Cols}열) 다시 선택해주세요.");
            ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1);
            return;
        }
        if (card.cardState[x1, y1] == 2)
        {
            Console.WriteLine("이미 선택된 카드입니다. 다시 선택해주세요.");
            ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1);   // 카드 선택 및 드러내기
            return;
        }
        else
        {
            card.cardState[x1, y1] = 1; // 일단 1로 설정해서 카드 드러내기
        }
        cardStatus.Reveal(card, x1, y1);
    }
    public void ChooseCard2(Card card, CardStatus cardStatus, CardDisplay cardDisplay, out int x2, out int y2)  // 카드 선택 메서드
    {

        Console.Write("두 번째 카드를 선택하세요 (행 열): ");
        string input2 = Console.ReadLine();
        string[] parts2 = input2.Split(new char[] { ' ', ',' });
        if (parts2.Length < 2 || !int.TryParse(parts2[0], out x2) || !int.TryParse(parts2[1], out y2))
        {
            Console.WriteLine("숫자만 입력해주세요.");
            ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2);
            return;
        }
        x2 -= 1;
        y2 -= 1;
        if (x2 < 0 || x2 >= card.Rows || y2 < 0 || y2 >= card.Cols)
        {
            Console.WriteLine($"범위를 벗어났습니다. (1~{card.Rows}행, 1~{card.Cols}열) 다시 선택해주세요.");
            ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2);
            return;
        }
        if (card.cardState[x2, y2] == 2)
        {
            Console.WriteLine("이미 선택된 카드입니다. 다시 선택해주세요.");
            ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2);   // 카드 선택 및 드러내기
            return;
        }
        else
        {
            card.cardState[x2, y2] = 1; // 일단 1로 설정해서 카드 드러내기
        }
        cardStatus.Reveal(card, x2, y2);
    }
}

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
                    Console.Write($"[{card.card[i, j],1}]  ");  // 드러난 카드
                }
                else if (card.cardState[i, j] == 2)    // 2면 맞춘 것
                {
                    Console.Write($"{card.card[i, j],2}   ");  // 드러난 카드
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    public void CountTry(int count, int matchCount, Card card, Difficulty difficulty)
    {
        Console.WriteLine($"총 시도 횟수: {count}/{difficulty.GetDifficulty()} | 찾은 쌍: {matchCount}/{card.card.Length / 2}\n");
    }
}
public class Over
{
    public void GameOver(ref int clearFlag, int count, int matchCount, Card card, Difficulty difficulty, CardDisplay cardDisplay, ref int GameFlag)
    {
        if (count == difficulty.GetDifficulty())
        {
            Console.WriteLine("=== 게임 오버! ===");
            Console.WriteLine($"총 시도 횟수 모두 사용했습니다.");
            Console.WriteLine($"찾은 쌍: {matchCount}/{card.card.Length / 2}\n");
            GameFlag = 1;  // 게임 종료
        }
        if (matchCount == card.card.Length / 2)  // 모든 카드 쌍을 찾았는지 확인
        {
            Console.Clear();
            cardDisplay.Display(card);  // 카드 상태 출력
            clearFlag = 1;  // 게임 클리어
            GameFlag = 1;  // 게임 종료
        }
    }
}

public class Difficulty
{
    public int input;  // 난이도 입력 값 저장

    public void SetDifficulty()
    {
        Console.WriteLine("난이도를 선택하세요: ");
        Console.WriteLine("1. 쉬움 (2x4)");
        Console.WriteLine("2. 보통 (4x4)");
        Console.WriteLine("3. 어려움 (4x6)");
        Console.Write("선택: ");
        input = int.Parse(Console.ReadLine());
    }
    public int GetDifficulty()
    {
        if (input == 1)
        {
            return 10;
        }
        else if (input == 2)
        {
            return 20;
        }
        else if (input == 3)
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
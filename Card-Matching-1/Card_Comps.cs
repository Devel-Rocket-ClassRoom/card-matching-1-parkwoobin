using System;


public class Card  // 카드 구성 클래스
{
    public ISkin Skin { get; set; }
    public int DiffLevel { get; set; } = 2;  // 현재 난이도 (1: 쉬움, 2: 보통, 3: 어려움)

    // 쉬움 난이도 기준 카드 배열 (2x4)
    private int[,] card1 = {     // 카드의 숫자 배열
        { 1, 2, 3, 4 },
        { 1, 2, 3, 4 },
        };

    // 카드 상태 배열 (0: 뒤집힘, 1: 앞면)
    private int[,] cardState1 = new int[2, 4];

    // 보통 난이도 기준 카드 배열 (4x4)
    private int[,] card2 = {     // 카드의 숫자 배열
        { 1, 2, 3, 4 },
        { 5, 6, 7, 8 },
        { 1, 2, 3, 4 },
        { 5, 6, 7, 8 }
        };

    // 카드 상태 배열 (0: 뒤집힘, 1: 앞면)
    private int[,] cardState2 = new int[4, 4];

    // 어려움 난이도 기준 카드 배열 (4x6)
    private int[,] card3 = {     // 카드의 숫자 배열
        { 1, 2, 3, 4, 5, 6 },
        { 7, 8, 9, 10, 11, 12 },
        { 1, 2, 3, 4, 5, 6 },
        { 7, 8, 9, 10, 11, 12 }
        };

    // 카드 상태 배열 (0: 뒤집힘, 1: 앞면)
    private int[,] cardState3 = new int[4, 6];



    // 난이도에 따라 현재 사용할 카드 배열 반환
    public int[,] card => DiffLevel == 1 ? card1 : DiffLevel == 3 ? card3 : card2;

    // 난이도에 따라 현재 사용할 카드 상태 배열 반환
    public int[,] cardState => DiffLevel == 1 ? cardState1 : DiffLevel == 3 ? cardState3 : cardState2;

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
            Console.WriteLine("\n짝을 찾았습니다!");
            card.cardState[x1, y1] = 2;  // 일치한 카드는 상태를 2로 설정
            card.cardState[x2, y2] = 2;
            matchCount++;  // 일치한 카드 쌍 수 증가
        }
        else
        {
            Console.WriteLine("\n짝이 맞지 않습니다!");
            card.cardState[x1, y1] = 0;  // 일치하지 않은 카드는 다시 뒤집힘 상태로 설정
            card.cardState[x2, y2] = 0;
        }
    }

}
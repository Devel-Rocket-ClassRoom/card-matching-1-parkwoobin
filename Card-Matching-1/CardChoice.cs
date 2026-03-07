using System;


public class CardChoice // 카드 선택 클래스
{
    public void ChooseCard1(Card card, CardStatus cardStatus, CardDisplay cardDisplay, out int x1, out int y1, GameBase gameBase = null)  // 첫 번째 카드 선택 메서드
    {
        Console.Write("\n첫 번째 카드를 선택하세요 (행 열): ");
        string input1 = Console.ReadLine();
        // 타임어택 시간 초과 시 즉시 탈출
        if (gameBase is TimeAttackMode ta && (ta.TimeUpFlag || ta.IsTimeUp()))
        {
            x1 = -1; y1 = -1;
            return;
        }
        string[] parts1 = input1.Split(new char[] { ' ', ',' });
        if (parts1.Length < 2 || !int.TryParse(parts1[0], out x1) || !int.TryParse(parts1[1], out y1))
        {
            Console.WriteLine("숫자만 입력해주세요.");
            ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1, gameBase);
            return;
        }
        x1 -= 1;
        y1 -= 1;
        if (x1 < 0 || x1 >= card.Rows || y1 < 0 || y1 >= card.Cols)
        {
            Console.WriteLine($"범위를 벗어났습니다. (1~{card.Rows}행, 1~{card.Cols}열) 다시 선택해주세요.");
            ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1, gameBase);
            return;
        }
        if (card.cardState[x1, y1] == 2)
        {
            Console.WriteLine("이미 선택된 카드입니다. 다시 선택해주세요.");
            ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1, gameBase);
            return;
        }
        else
        {
            card.cardState[x1, y1] = 1; // 일단 1로 설정해서 카드 드러내기
        }
        cardStatus.Reveal(card, x1, y1);
    }
    public void ChooseCard2(Card card, CardStatus cardStatus, CardDisplay cardDisplay, out int x2, out int y2, GameBase gameBase = null)  // 카드 선택 메서드
    {

        Console.Write("\n두 번째 카드를 선택하세요 (행 열): ");
        string input2 = Console.ReadLine();
        // 타임어택 시간 초과 시 즉시 탈출
        if (gameBase is TimeAttackMode ta && (ta.TimeUpFlag || ta.IsTimeUp()))
        {
            x2 = -1; y2 = -1;
            return;
        }
        string[] parts2 = input2.Split(new char[] { ' ', ',' });
        if (parts2.Length < 2 || !int.TryParse(parts2[0], out x2) || !int.TryParse(parts2[1], out y2))
        {
            Console.WriteLine("숫자만 입력해주세요.");
            ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2, gameBase);
            return;
        }
        x2 -= 1;
        y2 -= 1;
        if (x2 < 0 || x2 >= card.Rows || y2 < 0 || y2 >= card.Cols)
        {
            Console.WriteLine($"범위를 벗어났습니다. (1~{card.Rows}행, 1~{card.Cols}열) 다시 선택해주세요.");
            ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2, gameBase);
            return;
        }
        if (card.cardState[x2, y2] == 2)
        {
            Console.WriteLine("이미 선택된 카드입니다. 다시 선택해주세요.");
            ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2, gameBase);
            return;
        }
        else
        {
            card.cardState[x2, y2] = 1; // 일단 1로 설정해서 카드 드러내기
        }
        cardStatus.Reveal(card, x2, y2);
    }
}

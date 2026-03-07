using System;
using System.Threading;


class Sleep
{
    public int previewTime; // 미리보기 시간 (밀리초 단위)

    // 난이도에 따른 기본 미리보기 시간 설정
    public void SetPreviewTime(Card card)
    {
        if (card.diffLevel == 1)
            previewTime = 5000; // 쉬움: 5초
        else if (card.diffLevel == 2)
            previewTime = 3000; // 보통: 3초
        else
            previewTime = 2000; // 어려움: 2초
    }

    // 게임 시작 시 모든 카드를 미리보기로 보여준 뒤 뒤집기
    public void PreviewCards(Card card, CardDisplay cardDisplay, CardStatus cardStatus)
    {
        // 모든 카드를 앞면으로 표시
        for (int i = 0; i < card.Rows; i++)
            for (int j = 0; j < card.Cols; j++)
                card.cardState[i, j] = 1;

        Console.Clear();
        cardDisplay.Display(card);
        Console.WriteLine($"잘 기억하세요! ({previewTime / 1000}초 후 뒤집힙니다)");
        Thread.Sleep(previewTime);

        // 모든 카드를 다시 뒤집기
        cardStatus.Hide(card);
    }
}
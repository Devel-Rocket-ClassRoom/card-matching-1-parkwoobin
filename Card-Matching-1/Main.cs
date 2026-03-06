using System;
using System.Threading;

int GameFlag = 0;   // 게임 종료 플래그 (0: 진행 중, 1: 종료)
int count = 0;  // 시도 횟수 카운트 변수
int matchCount = 0;  // 일치한 카드 쌍의 수를 세는 변수
int ClearFlag = 0;  // 게임 클리어 플래그 (0: 클리어 안됨, 1: 클리어)


CardChoice cardChoice = new CardChoice();
CardStatus cardStatus = new CardStatus();

CardCheck cardCheck = new CardCheck();
CardRandom cardRandom = new CardRandom();

CardDisplay cardDisplay = new CardDisplay();
Card card = new Card();
Difficulty difficulty = new Difficulty();
Over over = new Over();
Sleep sleep = new Sleep();




while (true)
{
    Console.Clear();
    difficulty.SetDifficulty();
    card.diffLevel = difficulty.input;  // 선택한 난이도를 카드에 설정


    Console.Clear();
    cardRandom.Shuffle(card);
    // 카드 숨기기

    cardStatus.Hide(card);
    // 숨긴상태 **로 출력
    while (GameFlag == 0)
    {
        int x1, y1;  // 예시로 첫 번째 카드 선택 좌표
        int x2, y2;  // 예시로 두 번째 카드 선택 좌표
        Console.Clear();
        cardDisplay.Display(card);  // 카드 상태 출력
        cardDisplay.CountTry(count, matchCount, card, difficulty);


        cardChoice.ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1);   // 카드 선택 및 드러내기
        Console.Clear();
        cardDisplay.Display(card);  // 카드 상태 출력
        cardDisplay.CountTry(count, matchCount, card, difficulty);

        cardChoice.ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2);   // 카드 선택 및 드러내기
        if (x1 == x2 && y1 == y2)
        {
            Console.WriteLine("잘못된 선택입니다. 다시 선택해주세요.");
            cardChoice.ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2);   // 카드 선택 및 드러내기
        }
        count++;  // 시도 횟수 증가
        Console.Clear();
        cardDisplay.Display(card);  // 카드 상태 출력
        cardDisplay.CountTry(count, matchCount, card, difficulty);

        cardCheck.match(card, x1, y1, x2, y2, ref matchCount);  // 카드 일치 검사 및 결과 처리
        sleep.SleepTime(card);  // 난이도에 따라 카드 확인 시간 조절      

        over.GameOver(ref ClearFlag, count, matchCount, card, difficulty, cardDisplay, ref GameFlag);  // 게임 오버 검사 및 처리

    }
    while (GameFlag == 1)
    {
        if (ClearFlag == 1)
        {
            Console.WriteLine("=== 게임 클리어! ===");
            Console.WriteLine($"총 시도 횟수: {count} | 찾은 쌍: {matchCount}/{card.card.Length / 2}\n");

            Console.Write("새 게임을 하시겠습니까? (Y/N): ");
            string input = Console.ReadLine().ToLower();
            if (input.Equals("y"))
            {
                // 게임 초기화
                matchCount = 0;
                ClearFlag = 0;
                count = 0;
                GameFlag = 0;   // 게임 진행 상태로 초기화
                cardRandom.Shuffle(card);
                cardStatus.Hide(card);
                continue;  // while 조건 재검사 → GameFlag == 0이므로 루프 탈출
            }
            else if (input.Equals("n"))
            {
                Console.WriteLine("게임을 종료합니다.");
                return;  // 프로그램 종료
            }
            else
            {
                Console.WriteLine("다시 입력해주세요. (Y/N) ");
                continue;  // 잘못된 입력 시 다시 입력 받기
            }
        }
        return;  // 게임 종료
    }
}

using System;
using System.Threading;
Console.InputEncoding = System.Text.Encoding.UTF8;

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
GameBase currentMode = null;


while (true)
{
    Console.Clear();

    Console.WriteLine("카드 스킨을 선택하세요: ");
    Console.WriteLine("1. 알파벳 스킨");
    Console.WriteLine("2. 기호 스킨");
    Console.WriteLine("3. 숫자 스킨");
    Console.Write("선택: ");

    int skinChoice = int.Parse(Console.ReadLine());
    ISkin selectedSkin;
    switch (skinChoice)
    {
        case 1:
            selectedSkin = new AlphabetSkin();
            break;
        case 2:
            selectedSkin = new SymbolSkin();
            break;
        case 3:
            selectedSkin = new NumberSkin();
            break;
        default:
            Console.WriteLine("잘못된 선택입니다. 기본 스킨을 사용합니다.");
            selectedSkin = new NumberSkin();
            break;
    }
    Console.Clear();
    selectedSkin.DisplaySkin(); // 선택한 스킨 미리보기
    card.Skin = selectedSkin;

    Console.Clear();
    difficulty.SetDifficulty(); // 난이도 설정
    card.DiffLevel = difficulty.Input;  // 선택한 난이도를 카드에 설정

    Console.Clear();
    int modeChoice = GameModeSelector.SelectMode(); // 게임 모드 선택
    int totalPairs = card.card.Length / 2;
    currentMode = GameModeSelector.CreateMode(modeChoice, card.DiffLevel, totalPairs, difficulty);

    Console.Clear();
    cardRandom.Shuffle(card);

    // 미리보기 시간 설정
    sleep.SetPreviewTime(card);

    // 미리보기: 모든 카드를 보여준 뒤 뒤집기
    sleep.PreviewCards(card, cardDisplay, cardStatus);

    // 타임어택 모드: 미리보기 끝나고 타이머 시작
    currentMode.Start();

    while (GameFlag == 0)
    {
        int x1, y1;  // 예시로 첫 번째 카드 선택 좌표
        int x2, y2;  // 예시로 두 번째 카드 선택 좌표

        // 타임어택: 시간 초과 체크
        if (currentMode is TimeAttackMode ta1 && ta1.IsTimeUp())
        {
            ta1.StopRefresh();
            over.GameOver(ref ClearFlag, count, matchCount, card, difficulty, cardDisplay, ref GameFlag, currentMode);
            break;
        }

        if (currentMode is TimeAttackMode taStop1) taStop1.StopRefresh();
        Console.Clear();
        cardDisplay.Display(card);  // 카드 상태 출력
        currentMode.Count = count;
        currentMode.MatchCount = matchCount;
        cardDisplay.CountTry(count, matchCount, card, difficulty, currentMode);


        cardChoice.ChooseCard1(card, cardStatus, cardDisplay, out x1, out y1, currentMode);   // 카드 선택 및 드러내기

        // 타임어택: 시간 초과 체크 (ReadLine 복귀 후)
        if (currentMode is TimeAttackMode ta2 && (ta2.IsTimeUp() || ta2.TimeUpFlag))
        {
            ta2.StopRefresh();
            over.GameOver(ref ClearFlag, count, matchCount, card, difficulty, cardDisplay, ref GameFlag, currentMode);
            break;
        }

        if (currentMode is TimeAttackMode taStop2) taStop2.StopRefresh();
        Console.Clear();
        cardDisplay.Display(card);  // 카드 상태 출력
        currentMode.Count = count;
        currentMode.MatchCount = matchCount;
        cardDisplay.CountTry(count, matchCount, card, difficulty, currentMode);

        cardChoice.ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2, currentMode);   // 카드 선택 및 드러내기

        // 타임어택: 시간 초과 체크 (ReadLine 복귀 후)
        if (currentMode is TimeAttackMode ta3 && (ta3.IsTimeUp() || ta3.TimeUpFlag))
        {
            ta3.StopRefresh();
            over.GameOver(ref ClearFlag, count, matchCount, card, difficulty, cardDisplay, ref GameFlag, currentMode);
            break;
        }

        if (x1 == x2 && y1 == y2)
        {
            Console.WriteLine("잘못된 선택입니다. 다시 선택해주세요.");
            cardChoice.ChooseCard2(card, cardStatus, cardDisplay, out x2, out y2, currentMode);   // 카드 선택 및 드러내기
        }
        count++;  // 시도 횟수 증가
        int prevMatchCount = matchCount;  // 매치 전 카운트 저장
        if (currentMode is TimeAttackMode taStop3) taStop3.StopRefresh();
        Console.Clear();
        cardDisplay.Display(card);  // 카드 상태 출력
        currentMode.Count = count;
        cardDisplay.CountTry(count, matchCount, card, difficulty, currentMode);

        cardCheck.match(card, x1, y1, x2, y2, ref matchCount);  // 카드 일치 검사 및 결과 처리

        // 모드별 매치 결과 처리 (서바이벌: 연속 실패 추적)
        currentMode.OnMatchResult(matchCount > prevMatchCount);

        Thread.Sleep(2000);  // 카드 확인 후 잠시 대기

        over.GameOver(ref ClearFlag, count, matchCount, card, difficulty, cardDisplay, ref GameFlag, currentMode);  // 게임 오버 검사 및 처리

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
                currentMode.Reset();
                GameFlag = 0;   // 게임 진행 상태로 초기화
                cardRandom.Shuffle(card);
                sleep.PreviewCards(card, cardDisplay, cardStatus);  // 미리보기 후 카드 뒤집기
                currentMode.Start();
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

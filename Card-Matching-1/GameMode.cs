using System;
using System.Diagnostics;
using System.Threading;


// 게임 모드 추상 클래스 — 모든 게임 모드(클래식, 타임어택, 서바이벌)의 공통 기반
public abstract class GameBase
{
    public int Count { get; set; }        // 시도 횟수
    public int MatchCount { get; set; }   // 찾은 쌍
    public int TotalPairs { get; protected set; }   // 전체 쌍 수


    protected abstract bool IsGameOver();   // 게임 오버 조건 판별

    protected abstract string GetStatusText();  // 현재 상태 텍스트 반환


    public string GetStatus() => GetStatusText();   // 외부에서 상태 텍스트를 가져오는 공개 메서드


    public bool CheckGameOver(ref int clearFlag, Card card, CardDisplay cardDisplay, ref int GameFlag)
    {
        // 타임어택 모드일 경우 타이머 리프레시 정지
        if (this is TimeAttackMode ta)
            ta.StopRefresh();

        // 클리어 체크 (모든 모드 공통) — 모든 쌍을 찾았으면 클리어
        if (MatchCount == TotalPairs)
        {
            Console.Clear();
            cardDisplay.Display(card);
            clearFlag = 1;
            GameFlag = 1;
            return true;
        }

        // 각 모드별 게임 오버 조건 확인
        if (IsGameOver())
        {
            GameFlag = 1;
            return true;
        }
        return false;
    }

    // 게임 상태 초기화 (시도 횟수, 매칭 수 리셋)
    public virtual void Reset()
    {
        Count = 0;
        MatchCount = 0;
    }

    // 매칭 결과 처리 (서바이벌 등에서 오버라이드)
    public virtual void OnMatchResult(bool matched) { }
    // 게임 시작 시 호출 (타임어택에서 타이머 시작 등)
    public virtual void Start() { }
}

// 1. 클래식 모드: 제한된 시도 횟수 내에 모든 쌍을 찾아야 하는 모드
public class ClassicMode : GameBase
{
    private int maxTries; // 최대 시도 횟수 (난이도에 따라 결정)

    public ClassicMode(int maxTries, int totalPairs)
    {
        this.maxTries = maxTries;
        this.TotalPairs = totalPairs;
    }

    // 시도 횟수를 모두 소진하면 게임 오버
    protected override bool IsGameOver()
    {
        if (Count >= maxTries)
        {
            Console.WriteLine("=== 게임 오버! ===");
            Console.WriteLine($"총 시도 횟수 모두 사용했습니다.");
            Console.WriteLine($"찾은 쌍: {MatchCount}/{TotalPairs}\n");
            return true;
        }
        return false;
    }

    // 현재 시도 횟수 / 최대 횟수 및 찾은 쌍 수 표시
    protected override string GetStatusText()
    {
        return $"총 시도 횟수: {Count}/{maxTries} | 찾은 쌍: {MatchCount}/{TotalPairs}\n";
    }
}

// 2. 타임어택 모드: 제한 시간 내에 모든 쌍을 찾아야 하는 모드
public class TimeAttackMode : GameBase
{
    private Stopwatch timer;   // 경과 시간 측정용 스톱워치
    private int timeLimit;     // 제한 시간 (초)

    public TimeAttackMode(int timeLimit, int totalPairs)
    {
        this.timeLimit = timeLimit;
        this.TotalPairs = totalPairs;
        timer = new Stopwatch();
    }

    private Timer refreshTimer;                      // 상태 줄 자동 갱신용 타이머
    private int statusLineY;                         // 콘솔에서 상태 줄이 표시되는 Y 좌표
    private readonly object consoleLock = new object(); // 콘솔 출력 동기화용 락

    // 게임 시작 시 스톱워치 가동
    public override void Start()
    {
        timer = Stopwatch.StartNew();
    }

    // 남은 시간(초)을 반환 (0 이하면 0)
    public int GetRemainingSeconds()
    {
        return Math.Max(0, timeLimit - (int)timer.Elapsed.TotalSeconds);
    }

    // 제한 시간 초과 여부 확인
    public bool IsTimeUp()
    {
        return timer.Elapsed.TotalSeconds >= timeLimit;
    }

    // 상태 줄 위치를 기록하고 매초 리프레시 시작
    public void StartRefresh(int lineY)
    {
        statusLineY = lineY;
        StopRefresh();
        refreshTimer = new Timer(_ => RefreshStatus(), null, 1000, 1000);
    }

    // 리프레시 정지
    public void StopRefresh()
    {
        if (refreshTimer != null)
        {
            refreshTimer.Dispose();
            refreshTimer = null;
        }
    }

    private volatile bool _timeUpFlag; // 시간 초과 플래그 (스레드 안전)
    public bool TimeUpFlag { get => _timeUpFlag; private set => _timeUpFlag = value; }

    // 매초 호출되어 콘솔 상태 줄을 갱신하는 콜백
    private void RefreshStatus()    // 타이머 나오는 줄만 리프레시 하도록 만듬
    {
        lock (consoleLock) // 콘솔 동시 접근 방지
        {
            try
            {
                // 시간 초과 시 플래그 설정 후 리프레시 중단
                if (IsTimeUp())
                {
                    TimeUpFlag = true;
                    StopRefresh();
                    // 커서를 입력 줄 아래로 이동해서 시간 초과 메시지 출력
                    Console.WriteLine();
                    Console.WriteLine("\n시간 초과! Enter를 눌러주세요.");
                    return;
                }
                // 현재 커서 위치를 저장한 뒤 상태 줄 위치로 이동하여 갱신
                int savedLeft = Console.CursorLeft;
                int savedTop = Console.CursorTop;
                Console.SetCursorPosition(0, statusLineY);
                string status = GetStatusText().TrimEnd('\n');
                // 이전 텍스트 잔여물 제거를 위해 공백으로 패딩
                Console.Write(status + new string(' ', Math.Max(0, Console.WindowWidth - status.Length - 1)));
                // 커서를 원래 위치로 복원
                Console.SetCursorPosition(savedLeft, savedTop);
            }
            catch { }
        }
    }

    // 시간 초과 시 게임 오버 처리
    protected override bool IsGameOver()
    {
        if (IsTimeUp())
        {
            StopRefresh();
            Console.WriteLine("=== 게임 오버! ===");
            Console.WriteLine($"제한 시간 {timeLimit}초가 초과되었습니다!");
            Console.WriteLine($"찾은 쌍: {MatchCount}/{TotalPairs}\n");
            return true;
        }
        return false;
    }

    // 경과 시간 및 찾은 쌍 수 표시
    protected override string GetStatusText()
    {
        int remaining = GetRemainingSeconds();
        return $"경과 시간: {(int)timer.Elapsed.TotalSeconds}초 / {timeLimit}초 | 찾은 쌍: {MatchCount}/{TotalPairs}\n";
    }

    // 타이머, 플래그 등 모든 상태 초기화
    public override void Reset()
    {
        base.Reset();
        StopRefresh();
        TimeUpFlag = false;
        timer.Reset();
    }
}

// 3. 서바이벌 모드: 연속으로 3번 틀리면 즉시 게임 오버되는 모드
public class SurvivalMode : GameBase
{
    private int consecutiveFails; // 현재 연속 실패 횟수

    public SurvivalMode(int totalPairs)
    {
        this.TotalPairs = totalPairs;
    }

    // 매칭 결과에 따라 연속 실패 카운트 갱신
    public override void OnMatchResult(bool matched)
    {
        if (matched)
            consecutiveFails = 0;  // 성공 시 연속 실패 초기화
        else
            consecutiveFails++;    // 실패 시 연속 실패 누적
    }

    // 연속 3회 실패 시 게임 오버
    protected override bool IsGameOver()
    {
        if (consecutiveFails >= 3)
        {
            Console.WriteLine("=== 게임 오버! ===");
            Console.WriteLine($"연속 3번 틀려서 게임 오버!");
            Console.WriteLine($"찾은 쌍: {MatchCount}/{TotalPairs}\n");
            return true;
        }
        return false;
    }

    // 연속 실패 횟수 및 찾은 쌍 수 표시
    protected override string GetStatusText()
    {
        return $"연속 실패: {consecutiveFails}/3 | 찾은 쌍: {MatchCount}/{TotalPairs}\n";
    }

    // 연속 실패 카운트 포함 전체 상태 초기화
    public override void Reset()
    {
        base.Reset();
        consecutiveFails = 0;
    }
}

// 게임 모드 선택 및 생성을 담당하는 헬퍼 클래스
public class GameModeSelector
{
    // 사용자에게 모드를 선택받아 모드 번호(1~3)를 반환
    public static int SelectMode()
    {
        Console.WriteLine("게임 모드를 선택하세요:");
        Console.WriteLine("1. 클래식");
        Console.WriteLine("2. 타임어택");
        Console.WriteLine("3. 서바이벌");
        Console.Write("선택: ");
        if (!int.TryParse(Console.ReadLine(), out int mode) || mode < 1 || mode > 3)
        {
            Console.WriteLine("잘못된 입력입니다. 클래식 모드로 설정됩니다.");
            mode = 1;
        }
        return mode;
    }

    // 선택된 모드 번호에 따라 해당 게임 모드 인스턴스를 생성하여 반환
    public static GameBase CreateMode(int mode, int diffLevel, int totalPairs, Difficulty difficulty)
    {
        switch (mode)
        {
            case 2: // 타임어택 모드: 난이도별 제한 시간 설정 (쉬움 60초, 보통 90초, 어려움 120초)
                int timeLimit = diffLevel == 1 ? 60 : diffLevel == 2 ? 90 : 120;
                return new TimeAttackMode(timeLimit, totalPairs);
            case 3: // 서바이벌 모드: 연속 3회 실패 시 게임 오버
                return new SurvivalMode(totalPairs);
            default: // 클래식 모드: 난이도에 따른 최대 시도 횟수 제한
                return new ClassicMode(difficulty.GetDifficulty(), totalPairs);
        }
    }
}

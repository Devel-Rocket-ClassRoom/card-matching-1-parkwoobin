using System;
using System.Threading;


class Sleep
{
    public void SleepTime(Card card)
    {
        if (card.diffLevel == 1)
        {
            Console.WriteLine("잘 기억하세요! (5초 후 뒤집힙니다)");
            Thread.Sleep(5000); // 쉬움: 5초 대기
        }
        else if (card.diffLevel == 2)
        {
            Console.WriteLine("잘 기억하세요! (3초 후 뒤집힙니다)");
            Thread.Sleep(3000); // 보통: 3초 대기
        }
        else
        {
            Console.WriteLine("잘 기억하세요! (2초 후 뒤집힙니다)");
            Thread.Sleep(2000); // 어려움: 2초 대기
        }
    }
}
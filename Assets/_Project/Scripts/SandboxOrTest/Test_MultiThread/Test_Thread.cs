using UnityEngine;
using System.Threading;

public class Test_Thread : MonoBehaviour
{
    private Thread myThread;
    private bool isRunning = false;

    private int threadResult = 0;
    private bool isDataReady = false;

    void Start()
    {
        Debug.Log("메인 쓰레드: Start() 시작");
        this.isRunning = true;

        ThreadStart myDelegate = new ThreadStart(RunHeavyCalculation);

        myThread = new Thread(myDelegate);

        myThread.Start();
    }

    private void Update()
    {
        if (isDataReady)
        {
            isDataReady = false;
            Debug.Log($"[메인 스레드가 받음!] 계산 결과: {threadResult}");
        }
    }

    void RunHeavyCalculation()
    {
        Debug.Log("서브 스레드: 계산 시작...");
        int sum = 0;
        for (int i=1;i<=100;i++)
        {
            if (!isRunning) break;
            sum += i;
            Thread.Sleep(200);
        }

        threadResult = sum;
        isDataReady = true;
        Debug.Log("서브 스레드: 계산 완료!");
    }

    private void OnDestroy()
    {
        isRunning = false;
        if (myThread != null && myThread.IsAlive)
        {
            myThread.Join();
            Debug.Log("서브 스레드: 안전하게 종료 완료");
        }
    }
}

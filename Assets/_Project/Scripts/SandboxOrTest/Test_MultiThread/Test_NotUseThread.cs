using UnityEngine;
using System.Threading;

public class Test_NotUseThread : MonoBehaviour
{
    private int count = 0;

    void Update()
    {
        count++;

        Thread.Sleep(200);

        Debug.Log($"[렉 유발] 메인 스레드 혼자 처리 중: {count}");
    }
}
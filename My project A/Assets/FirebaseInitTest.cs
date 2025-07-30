using Firebase;
using UnityEngine;

public class FirebaseInitTest : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("파이어베이스 정상 초기화됨");
            }
            else
            {
                Debug.LogError("파이어베이스 초기화 실패: " + task.Result);
            }
        });
    }
}
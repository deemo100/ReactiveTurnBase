using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public void OnRetryClicked()
    {
        // 현재 씬 이름을 직접 입력
        SceneManager.LoadScene("1-1");
    }

    public void OnExitClicked()
    {
        SceneManager.LoadScene("mainmenu");
    }
}
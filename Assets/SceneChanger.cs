using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 1. 버튼용 (Title에서 씀)
    public void StartGame()
    {
        SceneManager.LoadScene("Room01");
    }

    // 2. 트리거용 (방 이동할 때 씀)
    // 플레이어(XR Origin)가 문에 닿으면 실행됩니다.
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 물체가 플레이어인지 확인 (Tag가 Player여야 함)
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            // 현재 씬 번호보다 1 큰 씬으로 이동
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
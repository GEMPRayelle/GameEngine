using UnityEngine;

// 점수와 게임 오버 여부, 게임 UI를 관리하는 게임 매니저
public class GameManager : MonoBehaviour
{
    private static GameManager instance; // 싱글톤이 할당될 static 변수

    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    public bool isGameover { get; private set; } // 게임 오버 상태

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면 자신을 파괴
        if (Instance != this) Destroy(gameObject);
    }

    public void EndGame()
    {
        isGameover = true;
        UIManager.Instance.SetActiveGameoverUI(true);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void StartGame() {
        Debug.Log("Start Game");
        SceneManager.LoadScene(1);
    }

    public void ExitGame() {
        Debug.Log("Exit Game");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour {
    public static readonly int width = 10;
    public static readonly int height = 30;
    public static double speed = 0.8;
    public static double score = 0;
    public static Transform[,,] board = new Transform[width, height, width];    // board[x, y, z]
    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void ExitGame() {
        SceneManager.LoadScene(2);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void StartGame() {
        Debug.Log("Start Game");
    }

    public void ExitGame() {
        Debug.Log("Exit Game");
    }
}

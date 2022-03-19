using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Controller: MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(this);
    }
}

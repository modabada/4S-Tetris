using UnityEngine;

public class BG_Controller: MonoBehaviour {
    private void Awake() {
        transform.localScale = new Vector2((float) 0.06 * Screen.width, (float) 0.06 * Screen.height);
        // 0.04 * 카메라사이즈 / 10
    }
}

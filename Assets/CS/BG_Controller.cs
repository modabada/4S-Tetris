using UnityEngine;

public class BG_Controller: MonoBehaviour {
    private void Awake() {
        transform.localScale = new Vector2((float) 0.053 * Screen.width, (float) 0.053 * Screen.height);
    }
}

using UnityEngine;

public class BGI_Controller: MonoBehaviour {
    private void Awake() {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}

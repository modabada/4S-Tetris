using UnityEngine;

public class BGI_Controller: MonoBehaviour {
    private void Start() {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}

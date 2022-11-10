using UnityEngine;

public class BGI_Controller : MonoBehaviour
{
    private void FixedUpdate()
    {
        float aspectedWidth = Screen.height * 9 / 16;
        if (Screen.width >= aspectedWidth)
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(aspectedWidth, Screen.height);
        }
        else
        {
            float aspectedHeight = Screen.width * 16 / 9;
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, aspectedHeight);
        }
    }
}

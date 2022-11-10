using UnityEngine;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour
{
    public void Awake()
    {
        GetComponent<Text>().text += GameManager.score.ToString("N0");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Controller : MonoBehaviour
{
    private void Update() {
        //transform.Rotate(transform.position, 0.5f);
        transform.Rotate(0, 0.1f, 0);
    }
}

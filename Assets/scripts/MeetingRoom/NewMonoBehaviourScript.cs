using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            return;
        }

        // 카메라 정면을 향하게 회전
        transform.LookAt(transform.position + mainCam.transform.forward);
    }
}
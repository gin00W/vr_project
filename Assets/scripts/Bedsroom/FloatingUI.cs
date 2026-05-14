using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 10f;   // 위아래 움직이는 폭
    public float floatSpeed = 2f;        // 움직이는 속도

    [Header("Optional Rotation")]
    public bool useRotate = false;
    public float rotateSpeed = 20f;

    private Vector3 startLocalPosition;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = startLocalPosition + new Vector3(0f, yOffset, 0f);

        if (useRotate)
        {
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }
}
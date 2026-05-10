using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    [Header("--- 문 ---")]
    public Transform doorPivot;
    public float openAngle = -90f;
    public float openDuration = 1.2f;

    [Header("--- 사운드 ---")]
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip lockedClip;

    private bool isLocked = true;
    private bool isOpening = false;

    void Start()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        interactable.selectEntered.AddListener(OnDoorInteract);
    }

    // MeetingRoomManager에서 클리어 시 호출
    public void Unlock()
    {
        isLocked = false;
        // 문 빛나는 효과 (있으면)
        StartCoroutine(UnlockEffect());
    }

    IEnumerator UnlockEffect()
    {
        // 문 살짝 흔들어서 "열려있어요" 표시
        if (doorPivot == null) yield break;

        for (int i = 0; i < 2; i++)
        {
            doorPivot.localRotation = Quaternion.Euler(0, -5f, 0);
            yield return new WaitForSeconds(0.1f);
            doorPivot.localRotation = Quaternion.identity;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnDoorInteract(SelectEnterEventArgs args)
    {
        if (isLocked)
        {
            audioSource?.PlayOneShot(lockedClip);
            MeetingRoomManager.Instance?.uiManager
                .ShowToast("🔒 먼저 미션을 완료하세요!");
            return;
        }

        if (!isOpening)
            StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        audioSource?.PlayOneShot(openClip);

        Quaternion from = doorPivot.localRotation;
        Quaternion to = Quaternion.Euler(0f, openAngle, 0f);
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            // ease out 부드럽게
            float t = 1f - Mathf.Pow(1f - elapsed / openDuration, 3f);
            doorPivot.localRotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }

        doorPivot.localRotation = to;

        // 1.5초 후 다음 씬
        yield return new WaitForSeconds(1.5f);
        MeetingRoomManager.Instance?.GoNextScene();
    }
}
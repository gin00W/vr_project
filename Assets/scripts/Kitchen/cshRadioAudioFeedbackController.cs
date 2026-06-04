using System.Collections;
using TMPro;
using UnityEngine;

public sealed class RadioAudioFeedbackController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource radioAudioSource;
    [SerializeField] private AudioClip defaultClip;

    [Header("Effect")]
    [SerializeField] private GameObject soundEffectRoot;
    [SerializeField] private ParticleSystem soundParticle;

    [Header("Subtitle")]
    [SerializeField] private GameObject subtitlePanelRoot;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField][TextArea] private string defaultSubtitle;
    [SerializeField] private bool clearSubtitleWhenFinished = true;

    private Coroutine playRoutine;

    private void Reset()
    {
        radioAudioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (radioAudioSource == null)
        {
            radioAudioSource = GetComponent<AudioSource>();
        }

        SetEffectActive(false);
        SetSubtitleVisible(false);

        if (subtitleText != null)
        {
            subtitleText.text = string.Empty;
        }
    }

    public void PlayRadio()
    {
        PlayRadio(defaultClip, defaultSubtitle);
    }

    public void PlayRadio(AudioClip clip, string subtitle)
    {
        if (radioAudioSource == null || clip == null)
        {
            Debug.LogWarning("RadioAudioFeedbackController: AudioSource or AudioClip is missing.");
            return;
        }

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
        }

        playRoutine = StartCoroutine(PlayRoutine(clip, subtitle));
    }

    private IEnumerator PlayRoutine(AudioClip clip, string subtitle)
    {
        string resolvedSubtitle = string.IsNullOrWhiteSpace(subtitle)
            ? defaultSubtitle
            : subtitle;

        radioAudioSource.Stop();
        radioAudioSource.clip = clip;
        radioAudioSource.Play();

        SetEffectActive(true);
        SetSubtitleVisible(true);

        if (subtitleText != null)
        {
            subtitleText.text = resolvedSubtitle;
            subtitleText.enabled = true;
        }

        yield return new WaitWhile(() => radioAudioSource != null && radioAudioSource.isPlaying);

        SetEffectActive(false);

        if (subtitleText != null && clearSubtitleWhenFinished)
        {
            subtitleText.text = string.Empty;
        }

        SetSubtitleVisible(false);
        playRoutine = null;
    }

    private void SetEffectActive(bool active)
    {
        if (soundEffectRoot != null)
        {
            soundEffectRoot.SetActive(active);
        }

        if (soundParticle != null)
        {
            if (active)
            {
                if (!soundParticle.isPlaying)
                {
                    soundParticle.Play();
                }
            }
            else
            {
                soundParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private void SetSubtitleVisible(bool visible)
    {
        if (subtitlePanelRoot != null)
        {
            subtitlePanelRoot.SetActive(visible);
        }
    }
}

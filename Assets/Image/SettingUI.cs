using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider masterVolume;        // 0..1
    public GameObject panel;

    [Header("AudioMixer")]
    public AudioMixer mixer;           // ����� �� �������
    public string exposedParam = "MasterVolume"; // ��� ���������� �������

    const string VOL_KEY = "vol";

    void Awake()
    {
        if (masterVolume) masterVolume.onValueChanged.AddListener(OnVolumeChanged);
    }

    void Start()
    {
        float vol = PlayerPrefs.GetFloat(VOL_KEY, 1f);

        if (masterVolume) masterVolume.value = vol;

        OnVolumeChanged(vol);
    }

    void OnDestroy()
    {
        if (masterVolume) masterVolume.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float v)
    {
        // ���� 0..1 ��� dB (�� -80dB ��� 0dB)
        float dB = (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
        if (mixer) mixer.SetFloat(exposedParam, dB);
        PlayerPrefs.SetFloat(VOL_KEY, v);
        PlayerPrefs.Save();
    }



    public void Close() { if (panel) panel.SetActive(false); }
}

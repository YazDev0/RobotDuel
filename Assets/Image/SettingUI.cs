using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public Slider masterVolume;        // 0..1
    public Slider mouseSensitivity;
    public GameObject panel;

    [Header("AudioMixer")]
    public AudioMixer mixer;           // √”Õ»Â „‰ «·„‘—Ê⁄
    public string exposedParam = "MasterVolume"; // «”„ «·»«—«„Ì — «·„ﬂ‘Ê›

    const string VOL_KEY = "vol";
    const string SENS_KEY = "sens";

    void Awake()
    {
        if (masterVolume) masterVolume.onValueChanged.AddListener(OnVolumeChanged);
        if (mouseSensitivity) mouseSensitivity.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void Start()
    {
        float vol = PlayerPrefs.GetFloat(VOL_KEY, 1f);
        float sens = PlayerPrefs.GetFloat(SENS_KEY, 1f);

        if (masterVolume) masterVolume.value = vol;
        if (mouseSensitivity) mouseSensitivity.value = sens;

        OnVolumeChanged(vol);
        OnSensitivityChanged(sens);
    }

    void OnDestroy()
    {
        if (masterVolume) masterVolume.onValueChanged.RemoveListener(OnVolumeChanged);
        if (mouseSensitivity) mouseSensitivity.onValueChanged.RemoveListener(OnSensitivityChanged);
    }

    void OnVolumeChanged(float v)
    {
        // ÕÊ¯· 0..1 ≈·Ï dB („‰ -80dB ≈·Ï 0dB)
        float dB = (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
        if (mixer) mixer.SetFloat(exposedParam, dB);
        PlayerPrefs.SetFloat(VOL_KEY, v);
        PlayerPrefs.Save();
    }

    void OnSensitivityChanged(float v)
    {
        PlayerPrefs.SetFloat(SENS_KEY, v);
        PlayerPrefs.Save();
    }

    public void Close() { if (panel) panel.SetActive(false); }
}

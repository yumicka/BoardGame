using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;
    private float initialVolume;


    private const string VolumeKey = "volume";
    private const float DefaultVolume = 1f;

    private void Start()
    {
        float saved = PlayerPrefs.GetFloat(VolumeKey, DefaultVolume);

        initialVolume = saved;
        slider.value = saved;
        SetVolume(saved);

        slider.onValueChanged.AddListener(v =>
        {
            SetVolume(v);
            PlayerPrefs.SetFloat(VolumeKey, v);
        });
    }


    public void SetVolume(float value)
    {
        if (value <= 0.0001f)
        {
            mixer.SetFloat("Volume", -80f);
        }
        else
        {
            mixer.SetFloat("Volume", Mathf.Log10(value) * 20);
        }
    }

    public void ResetSettings()
    {
        slider.value = initialVolume;
        SetVolume(initialVolume);
        PlayerPrefs.SetFloat(VolumeKey, initialVolume);
    }

}

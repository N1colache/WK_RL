using UnityEngine;
using UnityEngine.Audio;    


public class SettingsMenu : MonoBehaviour
{

    public AudioMixer mixer;
    
    public void SetVolume(float volume)
    {
        mixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Set Graphic level to index " + qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        Debug.Log("Set Full Screen");
    }
    
}

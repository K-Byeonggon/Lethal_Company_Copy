using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class VolumeController : MonoBehaviour
{
    [SerializeField]
    private VolumeProfile volumeProfile;

    [SerializeField]
    public Volume volume;

    private void Start()
    {
        ActiveAnalogGlitch();
        ActiveDigitalGlitch();
        Invoke("DeactiveAnalogGlitch", 1f);
        Invoke("DeactiveDigitalGlitch", 1f);
    }

    public void ActiveAnalogGlitch()
    {
        AnalogGlitchVolume glitch;
        if(volumeProfile.TryGet<AnalogGlitchVolume>(out glitch))
        {
            Debug.Log("있음");
            glitch.scanLineJitter.value = 0.414f;
            glitch.verticalJump.value = 0.044f;
            glitch.horizontalShake.value = 0.283f;
            glitch.colorDrift.value = 0.673f;

            glitch.active = true;
        }
        else
        {
            Debug.Log("없음");
        }
    }
    public void ActiveDigitalGlitch()
    {
        DigitalGlitchVolume glitch;
        if (volumeProfile.TryGet<DigitalGlitchVolume>(out glitch))
        {
            Debug.Log("있음");
            glitch.intensity.value = 0.474f;

            glitch.active = true;
        }
        else
        {
            Debug.Log("없음");
        }
    }

    public void DeactiveAnalogGlitch()
    {
        AnalogGlitchVolume glitch;
        if (volumeProfile.TryGet<AnalogGlitchVolume>(out glitch))
            glitch.active = false;
    }
    public void DeactiveDigitalGlitch()
    {
        DigitalGlitchVolume glitch;
        if (volumeProfile.TryGet<DigitalGlitchVolume>(out glitch))
            glitch.active = false;
    }
}

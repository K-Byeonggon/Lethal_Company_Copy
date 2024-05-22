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

    public static VolumeController Instance;

    [SerializeField]
    public Volume volume;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ActiveAnalogGlitch();
        ActiveDigitalGlitch();
        Invoke("DeactiveAnalogGlitch", 0.5f);
        Invoke("DeactiveDigitalGlitch", 0.5f);
    }

    public void StartWarpGlitch()
    {
        StartCoroutine(WarpGlitch());
    }

    IEnumerator WarpGlitch()
    {
        AnalogGlitchVolume glitch;
        volumeProfile.TryGet<AnalogGlitchVolume>(out glitch);
        float ratio = 1;
        while (ratio > 0)
        {
            glitch.scanLineJitter.value = 0f;
            glitch.verticalJump.value = 0f;
            glitch.horizontalShake.value = 0.283f * ratio;
            glitch.colorDrift.value = 0.673f * ratio;

            glitch.active = true;
            yield return YieldInstructionCache.WaitForSeconds(0.02f);
            ratio -= 0.01f;
        }
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

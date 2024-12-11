using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectManager : MonoBehaviour
{
    private static EffectManager m_Instance;
    public static EffectManager Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = FindObjectOfType<EffectManager>();
            return m_Instance;
        }
    }

    public enum EffectType
    {
        Common,
        Flesh
    }

    public ParticleSystem commonHitEffect;
    public ParticleSystem fleshHitEffect;

    private Volume volume;
    private Vignette vignette;
    bool vignetteCorutineEnable;

    private void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null, EffectType effectType = EffectType.Common)
    {
        var targetPrefab = commonHitEffect;

        if (effectType == EffectType.Flesh)
        {
            targetPrefab = fleshHitEffect;
        }

        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if (parent != null) effect.transform.SetParent(parent);

        effect.Play();
    }


    public IEnumerator PlayHiteVignetteEffect()
    {
        vignetteCorutineEnable = true;
        vignette.active = true;
        vignette.intensity.value = 1.0f;
        for (float i = 0; vignette.intensity.value >= 0.01f;)
        {
            vignette.intensity.value -= 0.01f;
            yield return new WaitForSeconds(0.008f);
        }
        vignette.active = false;
        vignette.intensity.value = 0;
        vignetteCorutineEnable = false;
        yield return null;
    }
}
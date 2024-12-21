using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>();

            return instance;
        }
    }

    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private Crosshair crosshair;

    [SerializeField] private Text healthText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text timeText;

    // 데미지 이펙트용
    [SerializeField] GameObject hitEffect;
    Image hitEffectImage;
    private int healthValTmp;

    private void Start()
    {
        hitEffectImage = hitEffect.GetComponent<Image>();
        healthValTmp = 100;    
    }

    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    public void UpdateCrossHairPosition(Vector3 worldPosition)
    {
        crosshair.UpdatePosition(worldPosition);
    }
    public void UpdateTimeText(int min, int sec)
    {
        timeText.text = min + ":" + sec;
    }

    public void UpdateHealthText(float health)
    {
        healthText.text = Mathf.Floor(health).ToString();

        // 체력 내려가면 이펙트 코루틴 실행
        if (healthValTmp > Convert.ToInt32(healthText.text))
        {
            StartCoroutine(PlayHitEffect());
            healthValTmp = Convert.ToInt32(healthText.text);
        }
        // 체력회복시 임시체력값도 올림
        if (healthValTmp < Convert.ToInt32(healthText.text))
        {
            healthValTmp = Convert.ToInt32(healthText.text);
        }
    }

    public void SetActiveCrosshair(bool active)
    {
        crosshair.SetActiveCrosshair(active);
    }

    public void SetActiveGameoverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator PlayHitEffect()
    {
        hitEffect.SetActive(true);
        hitEffectImage.color = new Color(1, 1, 1, 0.7f);
        for (float i = 0.7f; hitEffectImage.color.a >= 0.01f; i-= 0.02f)
        {
            hitEffectImage.color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(0.001f);
        }
        hitEffect.SetActive(false);
        hitEffectImage.color = new Color(1, 1, 1, 0);
        yield return null;
    }
}
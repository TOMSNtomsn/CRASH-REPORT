using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject player;

    [SerializeField] Image[] lifeImages;
    [SerializeField] Sprite lifeOn;
    [SerializeField] Sprite lifeOff;

    [SerializeField] GameObject laserGaugeParent;
    [SerializeField] Image gauge;
    [SerializeField] Image laserIcon;
    [SerializeField] Sprite[] laserIconSprites;
    [SerializeField] Color[] gaugeColors;

    RectTransform gaugeRect;

    void Awake()
    {
        player.GetComponent<PlayerLifeController>().AddOnLifeChanged(OnLifeChanged);

        PlayerLaserController laserController = player.GetComponent<PlayerLaserController>();
        laserController.AddOnLaserModeChanged(OnLaserModeChanged);
        laserController.AddOnTimerChanged(OnTimerChanged);
    }

    void OnLifeChanged(int life)
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].sprite = i < life ? lifeOn : lifeOff;
        }
    }

    void OnLaserModeChanged(LaserMode mode)
    {
        if (mode == LaserMode.Default)
        {
            laserGaugeParent.SetActive(false);
        }
        else
        {
            if (gaugeRect == null) gaugeRect = gauge.GetComponent<RectTransform>();

            laserGaugeParent.SetActive(true);
            laserIcon.sprite = laserIconSprites[(int)mode - 1];
            gauge.color = gaugeColors[(int)mode - 1];
        }
    }

    void OnTimerChanged(float progress)
    {
        var size = gaugeRect.sizeDelta;
        size.x = 560 * (1 - progress);
        gaugeRect.sizeDelta = size;

        gaugeRect.localPosition = new((size.x - 560) / 2, 0, 0);
    }
}

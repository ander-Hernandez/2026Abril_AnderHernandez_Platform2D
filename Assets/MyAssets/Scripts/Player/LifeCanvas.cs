using System;
using UnityEngine;
using UnityEngine.UI;

public class LifeCanvas : MonoBehaviour
{
    [SerializeField] Image mask;
    [SerializeField] LifeManager lifeManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        lifeManager.OnLifeChanged.AddListener(OnLifeChanged);
    }
    private void OnDisable()
    {
        lifeManager.OnLifeChanged.RemoveListener(OnLifeChanged);
    }

    private void OnLifeChanged(float currentLife, float startLife)
    {
        mask.fillAmount = Math.Clamp(currentLife / startLife, 0f, 1f);
    }

    
}

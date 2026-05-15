using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();
    [SerializeField] private Image LifebarImage;
    private void Update()
    {
        if (GameManager.LocalHealth == null)
            return;
        LifebarImage.fillAmount = (float)GameManager.LocalHealth.CurrentHealth / (float)GameManager.LocalHealth.MaxHealth;
    }

}
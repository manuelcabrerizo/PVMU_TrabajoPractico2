using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
    
    [SerializeField] private Image LifebarImage;

    private void Start()
    {
        EventBus.Subscribe<OnHealthChangeEvent>(OnHealthChange);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnHealthChangeEvent>(OnHealthChange);

    }

    private void OnHealthChange(in OnHealthChangeEvent onHealthChangeEvent)
    {
        LifebarImage.fillAmount = (float)onHealthChangeEvent.CurrentHealth / (float)onHealthChangeEvent.MaxHealth;
    }
}
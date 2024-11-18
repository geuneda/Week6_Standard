using UnityEngine;

public class PickupHeal : PickupItem
{
    [SerializeField] private int healValue = 10;

    protected override void OnPickedUp(GameObject gameObject)
    {
        HealthSystem healthSystem = gameObject.GetComponent<HealthSystem>();
        healthSystem.ChangeHealth(healValue);
    }
}
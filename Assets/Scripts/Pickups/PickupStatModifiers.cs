using System.Collections.Generic;
using UnityEngine;

public class PickupStatModifiers : PickupItem
{
    [SerializeField]List<CharacterStat> statsModifier = new List<CharacterStat>();
    protected override void OnPickedUp(GameObject gameObject)
    {
        CharacterStatHandler statHandler = gameObject.GetComponent<CharacterStatHandler>();

        foreach (CharacterStat modifier in statsModifier)
        {
            statHandler.AddStatModifier(modifier);
        }
        
        HealthSystem healthSystem = gameObject.GetComponent<HealthSystem>();
        healthSystem.ChangeHealth(0);
    }
}
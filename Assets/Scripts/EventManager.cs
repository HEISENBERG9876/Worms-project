using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event System.Action<GameObject> WormDiedEvent;
    public static event System.Action WeaponUsedEvent;
    public static event System.Action WeaponLandedEvent;
    public static event System.Action TurnStartedEvent;
    public static event System.Action TurnEndedEvent;
    public static event System.Action<GameObject> WormTookDamageEvent;

    public static void TriggerWormDied(GameObject worm)
    {
        WormDiedEvent?.Invoke(worm);
    }

    public static void TriggerWeaponUsed()
    {
        WeaponUsedEvent?.Invoke();
    }

    public static void TriggerWeaponLanded()
    {
        WeaponLandedEvent?.Invoke();
    }

    public static void TriggerTurnEnded()
    {
        TurnEndedEvent?.Invoke();
    }

    public static void TriggerTurnStarted()
    {
        TurnStartedEvent?.Invoke();
    }

    public static void TriggerWormTookDamage(GameObject worm)
    {
        WormTookDamageEvent?.Invoke(worm);
    }
}
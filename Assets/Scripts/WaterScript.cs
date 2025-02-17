using UnityEngine;

public class WaterScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) //przeniesc do WormScript
    {
        if (other.CompareTag("Worm"))
        {
            WormScript wormScript = other.gameObject.GetComponent<WormScript>();
            if (wormScript != null)
            {
                wormScript.Drown();
            }
        }
    }
}

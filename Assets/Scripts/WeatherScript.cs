using UnityEngine;

public class WeatherScript : MonoBehaviour
{
    public static WeatherScript Instance;
    private static float windForce = 3.0f;

    private void Start()
    {
        EventManager.TurnStartedEvent += RandomizeWindForce;
        RandomizeWindForce();
        Instance = this;
    }

    private void RandomizeWindForce()
    {
        windForce = Random.Range(-3.0f, 3.0f);
    }

    public static void ApplyWindForce(Rigidbody rb)
    {
            Vector3 windVector = new Vector3(windForce, 0f, 0f);
            rb.AddForce(windVector, ForceMode.Force);
    }
}
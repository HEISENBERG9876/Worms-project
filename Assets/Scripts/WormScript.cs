using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WormScript : MonoBehaviour
{
    public Rigidbody wormRb;
    public GameObject wormIdleModel;
    public GameObject wormMovingModel;
    public GameObject wormJumpingModel;
    public GameObject graveStonePrefab;
    public GameObject wormModelContainer;
    public GameObject wormPivot;
    public string wormName;

    public int collisionCounter = 0;
    public int health = 100;
    private bool alive = true;

    public enum WormType //przyda sie, zeby liczyc, czy ktorys z graczy nie wygral
    {
        WormGreen,
        WormRed
    }

    [SerializeField] WormType wormType;

    private List<string> wormNames = new List<string>
    {
        "Slippy", "Squiggly", "Wiggly", "Slimy", "Gooey",
        "Crawly", "Slinky", "Squeezy", "Jiggly", "Slinky",
        "Zigzag", "Dizzy", "Twisty", "Flutter", "Spotty",
        "Giggly", "Jumpy", "Noodle", "Squirmy", "Bumpy",
        "Sloppy", "Whirly", "Snappy", "Snazzy", "Bristle",
        "Jelly", "Sneezy", "Zippy", "Fizz", "Frothy",
        "Wobbly", "Droopy", "Swooshy", "Zany", "Wavy",
        "Bouncy", "Scooter", "Whizzy", "Spunky", "Snuggly",
        "Glider", "Glimmer", "Sparky", "Blitz", "Zoomer",
        "Twinkle", "Flash", "Glowy", "Ripple", "Bubbly",
        "Sonic", "Zest", "Dazzle", "Fizzle", "Sizzle",
        "Pulse", "Spectral", "Radiant", "Echo", "Lively",
        "Vibrant", "Funky", "Quasar", "Stellar", "Neon",
        "Nova", "Orbit", "Galaxy", "Celestial", "Pulsar",
        "Quark", "Astro", "Nebula", "Supernova", "Comet",
        "Meteor", "Asteroid", "Cosmic", "Infinity", "Eclipse",
        "Paradox", "Spiral", "Infinity", "Abyss", "Plasma",
        "Solstice", "Equinox", "Aurora", "Spectrum", "Ephemeral",
        "Charm", "Enigma", "Mystery", "Cipher", "Whisper"
    };

    private void Start()
    {
        wormName = GetRandomWormName();
    }

    private string GetRandomWormName()
    {
        int randomIndex = Random.Range(0, wormNames.Count);
        return wormNames[randomIndex];
    }

    void OnCollisionEnter(Collision collision)
    {
            collisionCounter++;
    }

    void OnCollisionExit(Collision collision)
    {
        collisionCounter--;
    }

    public void TakeDamage(int damage, GameObject source)
    {
        EventManager.TriggerWormTookDamage(this.gameObject);
        health -= damage;

        health = Mathf.Clamp(health, 0, 100);

        if (health == 0)
        {
            DieFromDamage();
        }
    }

    public void DieFromDamage()
    {
        if (alive)
        {
            EventManager.TriggerWormDied(this.gameObject);
            alive = false;
        }
        StartCoroutine(WaitForZeroVelocityAndDie());
    }

    private IEnumerator WaitForZeroVelocityAndDie()
    {
        while (!wormRb.IsSleeping())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);

        Instantiate(graveStonePrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void Drown()
    {
        if (alive)
        {
            EventManager.TriggerWormDied(this.gameObject);
            alive = false;
        }
        StartCoroutine(StartDrowningAndDie());
    }

    private IEnumerator StartDrowningAndDie() // w sumie to niezbyt potrzebne
    {
        float drowningSpeed = 5.0f;
        float totalTime = 0.5f;
        float timePassed = 0f;


        while (timePassed < totalTime)
        {
            wormRb.velocity = Vector3.zero;
            wormRb.position += (Vector3.down * drowningSpeed * Time.deltaTime);
            timePassed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void SetWormType(WormType wormType)
    {
        this.wormType = wormType;
    }

    private IEnumerator WinAnimation()
    {
        while (true)
        {
            wormRb.AddForce(Vector3.up * 2);
            yield return new WaitForSeconds(1.0f);
        }

    }

    public void StartWinAnimation()
    {
        StartCoroutine(WinAnimation());
    }

    public WormType GetWormType()
    {
        return wormType;
    }

}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using WormType = WormScript.WormType;
public class GameManager : MonoBehaviour
{

    public GameObject WormGreenPrefab;
    public GameObject WormRedPrefab;
    public GameObject minePrefab;

    [Header("Map parameters")]
    [SerializeField] int mapVoxelWidth = 200;
    [SerializeField] int mapVoxelHeight = 60;
    [SerializeField] float voxelSideLength = 0.5f;
    [SerializeField] int numberOfMines = 10;
    [SerializeField] GenerateMapScript mapScript;
    [SerializeField] UIScript uiScript;
    [SerializeField] WeatherScript weatherScript;
    [SerializeField] AudioSource ambientMusicSource;
    [SerializeField] AudioClip ambientMusicClip;


    [Header("Game parameters")]
    private int numOfWormsEachTeam = 5;
    private int turnDuration = 30;
    private int overtimeDuration = 5;

    private int greenWormsRemaining;
    private int redWormsRemaining;
    private GameObject[] wormsGreen;
    private GameObject[] wormsRed;
    private int greenTeamPlayerIndex = 0;
    private int redTeamPlayerIndex = 0;
    private GameObject currentActiveWorm;
    private WormType currentPlayerType;

    private bool turnEnded = false;
    private bool weaponUsed = false;
    private bool weaponLanded = false;

    GameObject[] mines;

    private Coroutine turnTimerCoroutine;
    void Start()
    {
        StartGame();
    }
    void StartGame()
    {
        EventManager.WormDiedEvent += HandleWormDied;
        EventManager.WeaponUsedEvent += HandleWeaponUsed;
        EventManager.WeaponLandedEvent += HandleWeaponLanded;
        EventManager.TurnEndedEvent += SetTurnToEnded;
        EventManager.WormTookDamageEvent += HandleWormTookDamage;

        wormsGreen = new GameObject[numOfWormsEachTeam];
        wormsRed = new GameObject[numOfWormsEachTeam];
        greenWormsRemaining = numOfWormsEachTeam;
        redWormsRemaining = numOfWormsEachTeam;

        mapScript.GenerateMap(mapVoxelWidth, mapVoxelHeight, voxelSideLength);
        SpawnWorms(numOfWormsEachTeam);
        SpawnMines();
        mines = GameObject.FindGameObjectsWithTag("Mine");

        currentActiveWorm = wormsGreen[0];
        currentActiveWorm.GetComponent<WormControlScript>().enabled = true;
        greenTeamPlayerIndex = 0;
        redTeamPlayerIndex = 0;

        turnEnded = false;
        weaponUsed = false;
        weaponLanded = false;

        turnTimerCoroutine = StartCoroutine(uiScript.StartTimer(30));
        StartCoroutine(StartWormTurn());

        uiScript.HideGameMenu();
    }

    private void SetTurnToEnded()
    {
        turnEnded = true;
    }

    public IEnumerator StartWormTurn()
    {
        EventManager.TriggerTurnStarted();

        currentPlayerType = currentActiveWorm.GetComponent<WormScript>().GetWormType();
        WormUIScript wormUIScript = currentActiveWorm.GetComponent<WormUIScript>();
        wormUIScript.HideWormInfo();

        if (turnTimerCoroutine != null) {
            SetTurnTimer(turnDuration);
        }

        while (!turnEnded) //tura konczy sie, gdy minal timer. TODO ma sie konczyc, gdy robak otrzyma obrazenia
        {
            yield return null;
        }

        yield return EndTurn();

    }

    public void SetTurnTimer(int time)
    {
        if (turnTimerCoroutine != null) 
        {
            StopCoroutine(turnTimerCoroutine);
        }
        turnTimerCoroutine = StartCoroutine(uiScript.StartTimer(time));
    }

    public IEnumerator EndTurn() //gdy czas w timer sie wykonczy ALBO aktualny robak otrzyma obrazenia
    {
        DisableCurrentPlayer();

        yield return WaitForWeaponToLand();
        yield return WaitForMinesToExplode();
        yield return WaitForWormsToStopMoving();
        yield return new WaitForSeconds(2.0f);

        if (CheckForWin())
        {
            EndGame();
            yield return null;
        }
        else
        {
            SetNewPlayer();
            turnEnded = false;
            yield return StartWormTurn();
        }
    }

    private void DisableCurrentPlayer()
    {
        WormControlScript controlScript = currentActiveWorm.GetComponent<WormControlScript>();
        controlScript.StopAllCoroutines();
        controlScript.SetModelToIdle();
        controlScript.enabled = false;

        WormUIScript wormUIScript = currentActiveWorm.GetComponent<WormUIScript>();
        wormUIScript.ShowWormInfo();


    }

    private void SetNewPlayer()
    {
        currentActiveWorm = chooseNextActiveWorm();
        if (currentActiveWorm != null)
        {
            WormControlScript controlScript = currentActiveWorm.GetComponent<WormControlScript>();
            controlScript.enabled = true;
        }
        else
        {
            EventManager.TriggerTurnEnded();
        }
    }

    private IEnumerator WaitForWeaponToLand()
    {
        while(weaponUsed && !weaponLanded)
        {
            yield return null;
        }
        weaponUsed = false;
        weaponLanded = false;
    }

    private IEnumerator WaitForMinesToExplode()
    {
        while (IsMineActive())
        {
            yield return null;
        }
    }

    private bool IsMineActive()
    {
        foreach(GameObject mine in mines)
        {
            if(mine != null) {
                MineScript mineScript = mine.GetComponent<MineScript>();
                if (mineScript != null && mine.GetComponent<MineScript>().CheckIfActive())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator WaitForWormsToStopMoving()
    {
        while (!CheckIfWormsNotMoving())
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool CheckIfWormsNotMoving()
    {
        foreach (GameObject worm in wormsGreen)
        {
            if (!worm.GetComponent<Rigidbody>().IsSleeping())
            {
                return false;
            }
        }
        foreach (GameObject worm in wormsGreen)
        {
            if (!worm.GetComponent<Rigidbody>().IsSleeping())
            {
                return false;
            }
        }
        return true;
    }

    private GameObject chooseNextActiveWorm()
    {
        WormType currentPlayer = currentActiveWorm.GetComponent<WormScript>().GetWormType();

        if (currentPlayer == WormType.WormRed)
        {
            for (int i = 0; i < numOfWormsEachTeam; i++)
            {
                greenTeamPlayerIndex = (greenTeamPlayerIndex + 1) % numOfWormsEachTeam;
                GameObject worm = wormsGreen[greenTeamPlayerIndex];
                
                if (worm.activeSelf)
                {
                    return worm;
                }
            }
        }
        else
        {
            for (int i = 0; i < numOfWormsEachTeam; i++)
            {
                redTeamPlayerIndex = (redTeamPlayerIndex + 1) % numOfWormsEachTeam;
                GameObject worm = wormsRed[redTeamPlayerIndex];

                if (worm.activeSelf)
                {
                    return worm;
                }
            }
        }

        return null;
    }

    //TODO przeniesienie do odpowiednich skryptow + refaktoryzacja
    private void SpawnWorms(int number)
    {
        for(int i = 0; i < number; i++)
        {
            SpawnWorm(WormGreenPrefab);
            SpawnWorm(WormRedPrefab);
        }
    }

    private void SpawnWorm(GameObject wormPrefab)
    {
        int maxSpawnX = (int)(mapVoxelWidth * voxelSideLength);
        int maxSpawnY = (int)(mapVoxelHeight * voxelSideLength);

        Vector3 spawnPosition;

        do
        {
            int randomX = Random.Range(0, maxSpawnX);
            int randomY = Random.Range(0, maxSpawnY);
            spawnPosition = new Vector3(randomX, randomY, 0);

        } while (!IsValidWormSpawnPosition(spawnPosition));

        GameObject newWorm = Instantiate(wormPrefab, spawnPosition, Quaternion.identity);

        AddWormToArray(newWorm);
    }

    private void SpawnMines()
    {
        for (int i = 0; i < numberOfMines; i++)
        {
            SpawnMine();
        }
    }

    private void SpawnMine()
    {
        int maxSpawnX = (int)(mapVoxelWidth * voxelSideLength);
        int maxSpawnY = (int)(mapVoxelHeight * voxelSideLength + 5.0f);

        Vector3 spawnPosition;

        do
        {
            int randomX = Random.Range(0, maxSpawnX);
            int randomY = Random.Range(0, maxSpawnY);
            spawnPosition = new Vector3(randomX, randomY, 0);

        } while (!IsValidMineSpawnPosition(spawnPosition));

        Instantiate(minePrefab, spawnPosition, Quaternion.identity);

    }

    private void AddWormToArray(GameObject worm)
    {
        WormType wormType = worm.GetComponent<WormScript>().GetWormType();
        if (wormType == WormType.WormGreen)
        {
            for (int i = 0; i < wormsGreen.Length; i++)
            {
                if (wormsGreen[i] == null)
                {
                    wormsGreen[i] = worm;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < wormsGreen.Length; i++)
            {
                if (wormsRed[i] == null)
                {
                    wormsRed[i] = worm;
                    break;
                }
            }
        }
    }

    private bool IsValidWormSpawnPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, voxelSideLength);
        if(colliders.Length != 0)
        {
            return false;
        }

        RaycastHit hit;
        if(Physics.Raycast(position, Vector3.down, out hit, 1000.0f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsValidMineSpawnPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, voxelSideLength);
        if (colliders.Length != 0)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 1000.0f))
        {
            Collider[] colliders2 = Physics.OverlapSphere(hit.collider.gameObject.transform.position, voxelSideLength);

            bool noWorms = true;
            foreach (Collider collider in colliders2)
            {
                if (collider.CompareTag("Worm"))
                {
                    noWorms = false;
                    break;
                }
            }

            if (!noWorms)
            {
                return false;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return false;
            }
        }

        return true;
    }

    private void HandleWormDied(GameObject worm)
    {
        if(worm == currentActiveWorm)
        {
            SetTurnToEnded();
        }

        WormType wormType = worm.GetComponent<WormScript>().GetWormType();
        if (wormType == WormType.WormGreen)
        {
            greenWormsRemaining--;
        }
        else if (wormType == WormType.WormRed)
        {
            redWormsRemaining--;
        }
    }

    private void HandleWormTookDamage(GameObject worm)
    {
        if(worm == currentActiveWorm)
        {
            uiScript.HideTimer();
            SetTurnToEnded();
        }
    }

    private void HandleWeaponUsed()
    {
        weaponUsed = true;
        SetTurnTimer(overtimeDuration);
    }

    private void HandleWeaponLanded()
    {
        weaponLanded = true;
    }

    private bool CheckForWin()
    {
        return greenWormsRemaining == 0 || redWormsRemaining == 0;
    }

    private void EndGame()
    {
        uiScript.HideTimer();
        uiScript.ShowGameMenu();
    }

    private void StartWinningAnimationForAll()
    {
        //todo
    }

    public void RestartGame()
    {
        StopAllCoroutines();

        EventManager.WormDiedEvent -= HandleWormDied;
        EventManager.WeaponUsedEvent -= HandleWeaponUsed;
        EventManager.WeaponLandedEvent -= HandleWeaponLanded;
        EventManager.TurnEndedEvent -= SetTurnToEnded;
        EventManager.WormTookDamageEvent -= HandleWormTookDamage;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetMapWidth()
    {
        return voxelSideLength * mapVoxelWidth;
    }

    public float GetMapHeight()
    {
        return voxelSideLength * mapVoxelHeight;
    }
}

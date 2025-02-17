using UnityEngine;
using System.Collections;

public class MineScript : MonoBehaviour
{
    [SerializeField] GameObject mineNotActive;
    [SerializeField] GameObject mineActive;
    [SerializeField] ExplosionScript explosion;
    private bool isMineActive = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Worm"))
        {
            StartCoroutine(MineExplodeCoroutine());
        }
    }

    private IEnumerator MineExplodeCoroutine() //TODO zeby wiele min nie wybuchalo rownoczesnie
    {
        isMineActive = true;
        for(int i = 0; i < 3; i++)
        {
            mineNotActive.SetActive(false);
            mineActive.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            mineNotActive.SetActive(true);
            mineActive.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        explosion.Explode();
    }

    public void MineExplode()
    {
        StartCoroutine(MineExplodeCoroutine());
    }

    public bool CheckIfActive()
    {
        return isMineActive;
    }
}

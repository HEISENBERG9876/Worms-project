using UnityEngine;
using TMPro;

public class WormUIScript : MonoBehaviour
{
    public TextMeshProUGUI wormHealthText;
    public TextMeshProUGUI wormNameText;
    public GameObject wormInfoCanvas;
    public WormScript wormScript;
    void Start()
    {
        SetWormHealthText(wormScript.health);
        SetWormNameText(wormScript.wormName);
    }

    private void FixedUpdate()
    {
        SetWormHealthText(wormScript.health);
        SetWormNameText(wormScript.wormName);
    }

    public void SetWormHealthText(int health)
    {
        wormHealthText.text = health.ToString();
    }

    public void SetWormNameText(string name)
    {
        wormNameText.text = name;
    }

    public void HideWormInfo()
    {
        wormInfoCanvas.SetActive(false);
    }

    public void ShowWormInfo()
    {
        wormInfoCanvas.SetActive(true);
    }
}

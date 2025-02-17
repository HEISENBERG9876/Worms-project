using UnityEngine;

public abstract class UsableItemScript : MonoBehaviour
{
    [SerializeField] string itemName;

    public abstract void Use();

    public abstract void Rotate();

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

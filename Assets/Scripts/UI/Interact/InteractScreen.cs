using UnityEngine;

public class InteractScreen : MonoBehaviour
{
    bool isActive = false;

    private void Start()
    {
        gameObject.SetActive(isActive);
    }

    public void PopUp()
    {
        isActive = true;
        gameObject.SetActive(isActive);
    }

    public void ClosePopUp()
    {
        isActive = false;
        gameObject.SetActive(isActive);
    }
}

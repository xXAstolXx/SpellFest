using UnityEngine;

public class PopUpMenu : MonoBehaviour
{
    private bool isOpen = false;

    private void Start()
    {
        gameObject.SetActive(isOpen);
    }

    public void PopUp()
    {
        isOpen = true;
        gameObject.SetActive(isOpen);
    }
}

using Interface;
using UnityEngine;

public class ButtonInteracteble : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactUI;

    public void Interact()
    {
        Debug.Log("Button pressed");
        interactUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.SetActive(false);
        }
    }
}
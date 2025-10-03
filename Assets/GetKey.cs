using UnityEngine;

public class GetKey : MonoBehaviour
{
    public GameObject getKeybtn, Key;
    public DoorLocked doorlocked;
    public bool isDoorLocked;

    void Start()
    {
        getKeybtn.SetActive(false);
        isDoorLocked = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            getKeybtn.SetActive(true);
        }
    }



    public void OnGetKeyButtonClick()
    {
        getKeybtn.SetActive(false);
        isDoorLocked = false;
        // Add logic to give the player a key or perform any other action
        Debug.Log("Key obtained!");
        // Optionally, you can disable the button after clicking
        gameObject.SetActive(false);
        Key.SetActive(false);
    }
}

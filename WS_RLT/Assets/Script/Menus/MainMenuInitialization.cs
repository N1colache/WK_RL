using UnityEngine;

public class MainMenuInitialization : MonoBehaviour
{
    
    
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    

    void Start()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
}

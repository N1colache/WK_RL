using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraObject;

    void Start()
    {
        // Désactive la caméra au lancement
        cameraObject.SetActive(false);

        // Exemple : la réactiver après 2 secondes
        Invoke("ActivateCamera", 2f);
    }

    void ActivateCamera()
    {
        cameraObject.SetActive(true);
    }
}

using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform target;

    void LateUpdate()
    {
        if (target == null) return;

        // On copie simplement la position du joueur
        transform.position = new Vector3(
            target.position.x,
            transform.position.y,   // on garde la hauteur fixe
            target.position.z
        );
    }
}
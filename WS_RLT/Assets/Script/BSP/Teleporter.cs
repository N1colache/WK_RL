using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    private Vector3 _destination;

    [SerializeField] private float teleportDelay = 0.5f;

    private bool isTeleporting = false;

    public void SetDestination(Vector3 dest)
    {
        _destination = dest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportWithDelay(other.transform));
        }
    }

    IEnumerator TeleportWithDelay(Transform player)
    {
        isTeleporting = true;

        // Optionnel : tu pourras ajouter ici un effet visuel / son
        yield return new WaitForSeconds(teleportDelay);

        player.position = _destination;

        // Petit cooldown pour éviter les boucles instantanées
        yield return new WaitForSeconds(0.2f);

        isTeleporting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (_destination != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _destination);
        }
    }
}
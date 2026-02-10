using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private Vector3 destination;
    [SerializeField] private float delayBeforeTeleport = 0.5f;

    private float timer = 0f;

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        Debug.Log(gameObject.name + " -> destination définie vers : " + dest);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " : OnTriggerEnter avec " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player détecté dans " + gameObject.name);
            timer = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        timer += Time.deltaTime;

        Debug.Log(gameObject.name + " timer = " + timer);

        if (timer >= delayBeforeTeleport)
        {
            Debug.Log("TELEPORT DU JOUEUR vers " + destination);

            other.transform.position = destination;

            timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player sorti de " + gameObject.name);
            timer = 0f;
        }
    }
}
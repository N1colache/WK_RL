using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private float cameraDistance = 10f;

    private Transform player;

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        Vector3 newPos = new Vector3(
            player.position.x,
            player.position.y + yOffset,
            -cameraDistance
        );

        transform.position = Vector3.Slerp(
            transform.position,
            newPos,
            followSpeed * Time.deltaTime
        );
    }

    void FindPlayer()
    {
        GameObject found = GameObject.FindGameObjectWithTag("Player");

        if (found != null)
        {
            player = found.transform;
        }
    }
}
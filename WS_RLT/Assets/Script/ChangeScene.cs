using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Scene à charger")]
    [SerializeField] private string sceneName;

    [Header("Options")]
    [SerializeField] private float delay = 0.5f;

    private bool isChangingScene = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isChangingScene)
        {
            isChangingScene = true;
            Invoke(nameof(LoadScene), delay);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.7f);
    }
}
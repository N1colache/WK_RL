using UnityEngine;
using UnityEngine.UI;

public class PlayerDashUI : MonoBehaviour
{
    private Slider _slider;
    private Controller _playerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_slider == null)
            return;

        if (_playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                _playerController = player.GetComponent<Controller>();
            }
            else
            {
                return;
            }
        }

        if (_playerController != null)
        {
            _slider.maxValue = 10;
            _slider.value = 10 - (_playerController.cooldownTimer * 10);
        }
    }
}

using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PocheDeSang : MonoBehaviour
{

    private Collider _collider;
    private GameObject _player;
    private Health _healthScript;

    [SerializeField] private int healAmmount = 20;

    public void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _healthScript = _player.GetComponent<Health>();
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _healthScript.Heal(healAmmount);
        }
    }
}

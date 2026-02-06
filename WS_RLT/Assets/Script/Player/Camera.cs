using System;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float yOffset = 1f;
    [SerializeField] private Transform _target;

    public void Update()
    {
        Vector3 newPos = new Vector3(_target.position.x, _target.position.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
}
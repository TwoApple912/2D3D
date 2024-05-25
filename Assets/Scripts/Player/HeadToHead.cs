using UnityEngine;

public class HeadToHead : MonoBehaviour
{
    [SerializeField] private Transform head;

    private bool skip = true; // Use for OnEnabled to prevent being called the first time during scene load

    private void Awake()
    {
        if (GetComponentInParent<PlayerMovement2D>()) head = FindObjectOfType<PlayerMovement3D>().transform.Find("Model/Head");
        else head = FindObjectOfType<PlayerMovement2D>().transform.Find("Model/Head");
    }

    private void LateUpdate()
    {
        head.position = transform.position;
    }
}
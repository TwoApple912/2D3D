using System;
using UnityEngine;

public class LockedByLock : MonoBehaviour
{
    [SerializeField] private bool unlocked = false;
    [Space]
    [SerializeField] private MovingPlatform movingPlatform;

    private void Awake()
    {
        movingPlatform = GetComponent<MovingPlatform>();
    }

    private void Start()
    {
        if (movingPlatform) movingPlatform.enabled = false;
    }

    private void Update()
    {
        UnlockLock();
    }

    void UnlockLock()
    {
        if (LockedStateCheck())
        {
            if (movingPlatform) movingPlatform.enabled = true;
        }
    }

    bool LockedStateCheck()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Lock")
            {
                unlocked = false;
                return false;
            }
        }

        unlocked = true;
        return true;
    }
}
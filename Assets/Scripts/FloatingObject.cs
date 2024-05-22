using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;
    [Space]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float axisChangeInterval = 5f;
    [Space]
    [SerializeField] private bool fasterWhenNearPlayer;
    [SerializeField] private float playerCheckDistance = 6f;
    [SerializeField] private float speedMultiplier = 2f;

    private Vector3 startPos;
    private Vector3 currentAxis;
    private Vector3 targetAxis;
    private float axisChangeTimer;
    private float currentSpeedMultiplier;

    void Start()
    {
        startPos = transform.position;
        
        currentAxis = Random.onUnitSphere;
        targetAxis = Random.onUnitSphere;
        axisChangeTimer = axisChangeInterval;
    }

    void Update()
    {
        Float();
        RotateAroundRotatingAxes();

        if (fasterWhenNearPlayer) IncreaseSpeedWhenNearPlayer();
    }

    void Float()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void RotateAroundRotatingAxes()
    {
        transform.Rotate(currentAxis, rotationSpeed * currentSpeedMultiplier * Time.deltaTime, Space.World);
        
        axisChangeTimer -= Time.deltaTime;
        if (axisChangeTimer <= 0f)
        {
            axisChangeTimer = axisChangeInterval;
            targetAxis = Random.onUnitSphere;
        }
        currentAxis = Vector3.Slerp(currentAxis, targetAxis, Time.deltaTime / axisChangeInterval);
    }

    void IncreaseSpeedWhenNearPlayer()
    {
        if (Vector3.Distance(transform.position, GetActivePlayer().position) < playerCheckDistance)
        {
            currentSpeedMultiplier = speedMultiplier;
        }
        else currentSpeedMultiplier = 1;
    }
    
    Transform GetActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            if (player.activeInHierarchy)
            {
                return player.transform;
            }
        }
        return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInWhenNearPlayer : MonoBehaviour
{
    public float interactionRange = 15f; // Maximum distance at which the alpha starts changing
    public float minAlpha = 0f; // Minimum alpha value when the player is at the maximum interaction range or beyond
    private SpriteRenderer spriteRenderer; // Sprite renderer attached to the object
    private List<Transform> players = new List<Transform>(); // List to store the transforms of all players

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Find all GameObjects tagged as Player and add their Transform components to the list
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in playerObjects)
        {
            players.Add(player.transform);
        }
    }

    void Update()
    {
        float minDistance = float.MaxValue; // Start with a very large distance

        // Iterate over each player and find the one that is the closest
        foreach (Transform player in players)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        // Normalize the minimum distance found based on the interaction range
        float normDistance = Mathf.Clamp01(minDistance / interactionRange);
        // Calculate the new alpha value using linear interpolation
        float alpha = Mathf.Lerp(1.0f, minAlpha, normDistance);
        // Set the alpha value of the sprite renderer
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }
}

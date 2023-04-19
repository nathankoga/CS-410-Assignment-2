using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoylePlayerObserver : MonoBehaviour
{
    /*
     * Observes the player's position, modulating the model's light albedo to reflect safety.
     */

    public GameObject collisionObj;
    public Material lightCone;

    public Color startColor = Color.green;
    public Color endColor = Color.red;

    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;

    void Update()
    {
        /*
         * Each frame, update the light cone's color depending on player distance.
         */
        // Calculate theta.
        float theta = Mathf.InverseLerp(maxDistance, minDistance, CalculateDistance());
        
        // Set color.
        lightCone.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, theta));
    }

    GameObject GetPlayer()
    {
        /*
         * Gets the player game object.
         */
        return GameObject.FindGameObjectsWithTag("Player")[0];
    }

    GameObject[] GetGargoyles()
    {
        /*
         * Gets all gargoyles.
         */
        return GameObject.FindGameObjectsWithTag("Gargoyle");
    }

    float CalculateDistance()
    {
        /*
         * Calculates the distance of the gargoyle closest to the player.
         */
        GameObject player = GetPlayer();
        float bestDistance = Mathf.Infinity;
        foreach (GameObject g in GetGargoyles())
        {
            bestDistance = Mathf.Min(bestDistance, Vector3.Distance(g.transform.position, player.transform.position));
        }
        return bestDistance;
    }
}

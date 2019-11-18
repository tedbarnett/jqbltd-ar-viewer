using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles dynamically scaling of particles attached to objects.
// Goes through all the sub particle systems storing off initial values.
// Function to call when you update the scale.
public class SetScale : MonoBehaviour
{
    private List<float> initialSizes = new List<float>();
    public float scaleValue;

    void Awake()
    {
        // Save off all the initial scale values at start.
        ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            initialSizes.Add(particle.startSize);
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer)
            {
                initialSizes.Add(renderer.lengthScale);
                initialSizes.Add(renderer.velocityScale);
            }
        }
    }

    public void Start()
    {
        // Scale all the particle components based on parent.
        int arrayIndex = 0;
        ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.startSize = initialSizes[arrayIndex++] * scaleValue; //scaleValue
            //particle.startSize = initialSizes[arrayIndex++] * gameObject.transform.localScale.magnitude; //scaleValue
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer)
            {
                renderer.lengthScale = initialSizes[arrayIndex++] *
                    scaleValue;
                renderer.velocityScale = initialSizes[arrayIndex++] *
                    scaleValue;
            }
        }
    }

}


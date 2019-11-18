using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // Handles dynamically scaling of particles attached to objects.
 // Goes through all the sub particle systems storing off initial values.
 // Function to call when you update the scale.
public class ScaleParticles : MonoBehaviour
{
    private List<float> initialSizes = new List<float>();
    private float currentScaleMagnitude;
    public float minDelta;

    void Awake()
    {
        currentScaleMagnitude = gameObject.transform.localScale.magnitude;
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

    public void Update()
    {
        if(System.Math.Abs(currentScaleMagnitude - gameObject.transform.localScale.magnitude) < minDelta)
        {
            currentScaleMagnitude = gameObject.transform.localScale.magnitude;
            return; // if the parent hasn't changed scale, return 
        } else
        {
            // Scale all the particle components based on parent.
            int arrayIndex = 0;
            ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles)
            {
                particle.startSize = initialSizes[arrayIndex++] * gameObject.transform.localScale.magnitude;
                ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
                if (renderer)
                {
                    renderer.lengthScale = initialSizes[arrayIndex++] *
                        gameObject.transform.localScale.magnitude;
                    renderer.velocityScale = initialSizes[arrayIndex++] *
                        gameObject.transform.localScale.magnitude;
                }
            }
            currentScaleMagnitude = gameObject.transform.localScale.magnitude;
        }
    }

}

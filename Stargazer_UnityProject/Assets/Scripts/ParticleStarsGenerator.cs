using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStarsGenerator : MonoBehaviour
{
    public Transform stars;

    public ParticleSystem particleSystem;
    private ParticleSystem.Particle[] m_particles;
    int counter;

    private void Start()
    {
        foreach (Transform trans in stars.GetComponentInChildren<Transform>())
        {
            //if (trans.localScale.x > 31)
            //{ 
                counter++;

                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = trans.position;
                emitParams.startSize = trans.localScale.x / 2;
                particleSystem.Emit(emitParams, 1);

                var particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
                var currentAmount = particleSystem.GetParticles(particles);
                
                //Change only the particles that are alive
                //particles[counter].position = trans.position;

                // Apply the particle changes to the Particle System
                particleSystem.SetParticles(particles, currentAmount);
           // }
        } 
    }

}

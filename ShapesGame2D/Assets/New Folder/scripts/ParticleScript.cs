using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    ParticleSystem.MainModule mainModule;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem particle_System;
    bool Ready = false;
    public Color ParticleColor = Color.black;
    public int EmissionCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         particle_System = GetComponent<ParticleSystem>();
        emissionModule = particle_System.emission;

        mainModule = particle_System.main;
        mainModule.startColor = ParticleColor;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, EmissionCount);
        emissionModule.SetBursts(new ParticleSystem.Burst[] { burst });


        particle_System.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!particle_System.IsAlive() && Ready)
        {
            Destroy(gameObject);
        }
    }

    public void StartParticles(Color ParticleColor)
    {
        mainModule.startColor = ParticleColor;
        particle_System.Play();
       // Ready = true;
    }

}

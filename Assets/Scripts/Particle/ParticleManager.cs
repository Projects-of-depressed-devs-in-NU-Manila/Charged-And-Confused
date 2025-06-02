using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    public List<Particle> particles;

    public float coulombConstant = 8.99f; // not actual scale; just a tuning parameter
    public float maxForce = 1f;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            Particle particle = child.GetComponent<Particle>();
            particles.Add(particle);
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (var a in particles)
        {
            foreach (var b in particles)
            {
                if (a == b) continue;

                Vector3 dir = b.transform.position - a.transform.position;
                float dist = dir.magnitude + 0.1f;
                Vector3 dirNormalized = dir.normalized;

                if (dist <= 1)
                {
                    continue;
                }

                float product = a.charge * b.charge;
                float forceMagnitude = coulombConstant * Mathf.Abs(product) / (dist * dist);

                Vector3 force = dirNormalized * forceMagnitude * Mathf.Sign(product); // repulsion (+) or attraction (-)

                force = Vector3.ClampMagnitude(force, maxForce);
                a.ApplyForce(-force);
                // b.ApplyForce(-force);
            }
        }

        foreach (var p in particles)
        {
            p.CustomUpdate(dt);
        }
    }
    
    public void RemoveParticle(Particle particle)
    {
        if (particles.Contains(particle))
        {
            particles.Remove(particle);
        }
    }
}


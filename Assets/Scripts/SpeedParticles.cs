using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class SpeedParticles : MonoBehaviour
{

    ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    Spaceship spaceship;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        emission = ps.emission;
        spaceship = GetComponentInParent<Spaceship>();
    }

    // Update is called once per frame
    void Update()
    {
        emission.rateOverTime = (spaceship.rb.velocity.sqrMagnitude + spaceship.rb.angularVelocity.sqrMagnitude);
    }
}

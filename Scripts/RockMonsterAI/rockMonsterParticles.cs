using UnityEngine;

public class rockMonsterParticles : MonoBehaviour
{
    public GameObject footstep;                // Footstep Particle
    public GameObject clapDust;                // Clap Particle
    public GameObject[] laser;                  // Laser Particle

    public Transform leftFoot;                  // Left foot position
    public Transform rightFoot;                 // Right foot position
    public Transform clap;                       // Clap particle spawn position

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetLocomotion(float newValue)
    {
        animator.SetFloat("locomotion", newValue);
    }

    public void LeftFoot()
    {
        if (leftFoot != null)
        {
            GameObject newParticle = Instantiate(footstep, leftFoot.position, Quaternion.identity);
            Destroy(newParticle, 5.0f);
        }
    }

    public void RightFoot()
    {
        if (rightFoot != null)
        {
            GameObject newParticle = Instantiate(footstep, rightFoot.position, Quaternion.identity);
            Destroy(newParticle, 5.0f);
        }
    }

    public void Laser()
    {
        for (int i = 0; i < laser.Length; i++)
        {
            if (laser[i] != null)
            {
                ParticleSystem particleSystem = laser[i].GetComponent<ParticleSystem>();
                Light lightComponent = laser[i].GetComponent<Light>();
                CapsuleCollider capsuleCollider = laser[i].GetComponent<CapsuleCollider>();

                if (particleSystem != null)
                {
                    var emission = particleSystem.emission; emission.enabled = true;
                }
                if (lightComponent != null)
                    laser[i].SetActive(true);
                if (capsuleCollider != null)
                {
                    capsuleCollider.enabled = true;
                }
            }
        }
    }

    public void Clap()
    {
        if (clap != null)
        {
            GameObject newParticle = Instantiate(clapDust, clap.position, Quaternion.identity);
            Destroy(newParticle, 5.0f);
        }
    }

    public void LaserStop()
    {
        for (int i = 0; i < laser.Length; i++)
        {
            if (laser[i] != null)
            {
                ParticleSystem particleSystem = laser[i].GetComponent<ParticleSystem>();
                Light lightComponent = laser[i].GetComponent<Light>();
                CapsuleCollider capsuleCollider = laser[i].GetComponent<CapsuleCollider>();

                if (particleSystem != null)
                {
                    var emission = particleSystem.emission; emission.enabled = false;
                }
                if (lightComponent != null)
                    laser[i].SetActive(false);
                if (capsuleCollider != null)
                {
                    capsuleCollider.enabled = false;
                }
            }
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

public class SkillParticleCollider : MonoBehaviour
{
    private ParticleSystem skillParticle;
    private Collider collider;


    private void Awake()
    {
        skillParticle = GetComponent<ParticleSystem>();
        collider = GetComponent<Collider>();

        if (collider != null)
        {
            collider.enabled = false;
        }

    }

    private void OnEnable()
    {
        StartCoroutine(HandleCollider());
    }

    private IEnumerator HandleCollider()
    {
        float particleStartDelay = skillParticle.main.startDelay.constant;
        float particleDuration = skillParticle.main.duration; 

        yield return new WaitForSeconds(particleStartDelay);

        if (collider != null && skillParticle.isPlaying)
        {
            collider.enabled = true;
        }
        
        yield return new WaitForSeconds(particleDuration);

        if (collider != null && collider.enabled)
        {
            collider.enabled = false;
        }
        
        Destroy(gameObject, 0.1f);
    }
    
}

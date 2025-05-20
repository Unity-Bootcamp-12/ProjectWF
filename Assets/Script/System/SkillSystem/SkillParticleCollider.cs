using System;
using System.Collections;
using UnityEngine;

public class SkillParticleCollider : MonoBehaviour
{
    [SerializeField]private ParticleSystem skillParticle;
    [SerializeField]private Collider collider;


    private void Start()
    {
        skillParticle = GetComponent<ParticleSystem>();
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    private void OnEnable()
    {
        StartCoroutine(HandleCollider());
    }

    private IEnumerator HandleCollider()
    {
        float particleStartDelay = skillParticle.main.startDelay.constant;

        yield return new WaitForSeconds(particleStartDelay);

        if (!collider.enabled && skillParticle.isPlaying)
        {
            collider.enabled = true;
        }
        
        yield return new WaitUntil(IsParticleStopped);

        if (collider.enabled == true)
        {
            collider.enabled = false;
        }
        
        Destroy(gameObject, 0.1f);
    }
    
    private bool IsParticleStopped()
    {
        return !skillParticle.IsAlive();
    }
    
}

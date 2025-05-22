using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField]private int skillAttackPower =1;
    [SerializeField]private float skillDamageInterval = 0.3f;
    private Coroutine skillDamageCoroutine;
    private ParticleSystem skillParticle;
    private Collider collider;

    [SerializeField] private bool isContinous = false;
    private void Awake()
    {
        collider = GetComponent<Collider>();
        skillParticle = GetComponent<ParticleSystem>();
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

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MonsterController monster))
        {
            if (isContinous)
            {
                skillDamageCoroutine = StartCoroutine(SkillDamageIntervalCoroutine(monster));
            }
            else
            {
                monster.TakeDamage(true, skillAttackPower);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MonsterController monster))
        {
            if (isContinous)
            {
                StopDamage();
            }
        }
    }

    private IEnumerator SkillDamageIntervalCoroutine(MonsterController monster)
    {
        while (true)
        {
            monster.TakeDamage(true, skillAttackPower);
            yield return new WaitForSeconds(skillDamageInterval);
        }
    }

    private void StopDamage()
    {
        if (skillDamageCoroutine != null)
        {
            StopCoroutine(skillDamageCoroutine);
        }
    }

    private void OnDestroy()
    {
        StopDamage();
    }
}

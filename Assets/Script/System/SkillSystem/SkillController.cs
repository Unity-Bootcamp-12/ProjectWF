using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    private int skillDamagePower;
    
    public void SetSkillDamagePower(int damage)
    {
        skillDamagePower = damage;
    }
    
    private ElementalAttribute attribute;

    public void SetAttribute(ElementalAttribute attribute)
    {
        this.attribute = attribute;
    }
    
    private EnumSkillType skillType;

    public void SetSkillType(EnumSkillType skillType)
    {
        this.skillType = skillType;
    }
    
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

        GameController.Instance.OnWaveEnd += DestroyParticle;
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
        
        if (skillType == (int)EnumSkillType.Buff)
        {
            Logger.Info(attribute.ToString());
            GameController.Instance.SetBaseAttackAttribute(attribute);
            GameController.Instance.IncreasePlayerAttackPower(1);
        }
        
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
                monster.TakeDamage(true, skillDamagePower, attribute);
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
            monster.TakeDamage(true, skillDamagePower, attribute);
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

    private void DestroyParticle()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameController.Instance.OnWaveEnd -= DestroyParticle;
        StopDamage();
        if (skillType == EnumSkillType.Buff)
        {
            GameController.Instance.SetBaseAttackAttribute(ElementalAttribute.None);
            GameController.Instance.IncreasePlayerAttackPower(-1);
        }
    }
}

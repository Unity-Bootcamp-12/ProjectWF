using System;
using DG.Tweening;
using UnityEngine;

public class LostWisdom : MonoBehaviour
{
    public void Explosion(Vector2 to)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(to, 0.5f).SetEase(Ease.InCubic));  
        sequence.AppendCallback(() => { gameObject.SetActive(false);});
    }
}

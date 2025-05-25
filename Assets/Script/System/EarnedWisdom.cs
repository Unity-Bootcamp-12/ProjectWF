using DG.Tweening;
using UnityEngine;

public class EarnedWisdom : MonoBehaviour
{
    public void Explosion(Vector2 from, Vector2 to, float exploRange)
    {
        transform.position = from;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(from + Random.insideUnitCircle * exploRange, 0.25f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMove(to, 0.5f).SetEase(Ease.InCubic));  
        sequence.AppendCallback(() => { Destroy(gameObject);});
        SoundController.Instance.PlaySFX(SFXType.UpgradeSound);
    }
}

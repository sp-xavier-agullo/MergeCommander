using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DeadSkull : MonoBehaviour
{
    [SerializeField] private Image _skullImg;

    public void Init(GameObject target, Action onSkullEnded)
    {
        var offset = new Vector3(Random.Range(0, 20), Random.Range(0, 40), 0);
        transform.position += offset;
        
        float duration = Random.Range(0.3f, 0.6f);
        var endPos = transform.position + new Vector3(0,Random.Range(50,100),0);
        
        transform.DOMove(endPos, duration);
        
        _skullImg
            .DOFade(0.0f, duration + 0.1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if (onSkullEnded != null)
                {
                    onSkullEnded();
                }
                Destroy(gameObject);
            });
    }
}
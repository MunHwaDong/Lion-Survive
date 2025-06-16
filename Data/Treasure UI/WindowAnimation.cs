using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WindowAnimation : MonoBehaviour
{
    public async UniTask AnimatePopEffect(RectTransform targetImage)
    {
        targetImage.localScale = Vector3.zero;

        // Sequence 끝까지 기다리기 위해 UniTaskCompletionSource 사용
        var tcs = new UniTaskCompletionSource();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(targetImage.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        sequence.Append(targetImage.DOAnchorPosY(targetImage.anchoredPosition.y + 10f, 0.1f).SetLoops(4, LoopType.Yoyo));
        sequence.OnComplete(() => tcs.TrySetResult());

        // Time.timeScale = 0일 때에도 동작하도록 설정
        sequence.SetUpdate(true);
        
        await tcs.Task;
    }
}

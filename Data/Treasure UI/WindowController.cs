using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindowController : MonoBehaviour
{
    public const int MAX_ITEMS = 3;
    private int currentCount = 0;
    private int targetCount = 0;

    [SerializeField] private RectTransform targetImage;
    [SerializeField] private WindowAnimation windowAnimation;
    [SerializeField] private GameObject prefab;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Camera uiCam;
    [SerializeField] private TextMeshProUGUI timeText;
    
    private readonly List<(GameObject, ParticleSystem)> prefabs = new();
    
    void OnEnable()
    {
        GameManager.Instance.TaskQueue.RunTask(() => windowAnimation.AnimatePopEffect(targetImage));
        
        GameManager.Instance.ToggleUseDeltaTime();
        
        StartAnimation();
    }

    void OnDisable()
    {
        foreach (var (item1, item2) in prefabs)
        {
            Destroy(item1);
            Destroy(item2.gameObject);
        }
        
        prefabs.Clear();
        currentCount = 0;
        timeText.gameObject.SetActive(false);
        GameManager.Instance.ToggleUseDeltaTime();
    }

    private void StartAnimation()
    {
        targetCount = Random.Range(1, MAX_ITEMS + 1); // targetCount 저장

        for (int i = 0; i < targetCount; i++)
        {
            GameManager.Instance.TaskQueue.RunTask(CreateCard);
        }
        
        GameManager.Instance.TaskQueue.RunTask(async () =>
        {
            timeText.gameObject.SetActive(true);
            
            float t = 1.99f;

            while (t >= 0f)
            {
                t -= Time.unscaledDeltaTime;

                timeText.text = $"{Mathf.Ceil(t)} 초 후 게임으로 돌아갑니다.";
                
                await UniTask.Yield();
            }
            gameObject.SetActive(false);
        });
    }

    public async UniTask CreateCard()
    {
        await UniTask.Yield();

        if (currentCount >= targetCount)
        {
            Debug.Log("목표 개수 도달");
            return;
        }

        float imageWidth = targetImage.rect.width;
        float spacing = imageWidth / (targetCount + 1);
        float xOffset = spacing * (currentCount + 1) - imageWidth / 2f;
        Vector3 localPos = new Vector3(xOffset, 0f, 0f);

        GameObject obj = Instantiate(prefab, targetImage);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = localPos;

        // 카메라 기준으로 화면 위치 계산
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCam, rect.position);

        // z는 UI 카메라에서 어느 정도 떨어진 위치 (Canvas의 Plane Distance 참고)
        float zOffset = uiCam.nearClipPlane + 0.5f;
        Vector3 worldPos = uiCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zOffset));

        // 파티클 생성
        var particleObj = Instantiate(particle, worldPos, Quaternion.identity);
        particleObj.Play();
        SoundManager.Instance.PlaySFX(SoundID.Treasure_Get, true);
    
        prefabs.Add((obj, particleObj));
        currentCount++;

        if (obj.TryGetComponent<WindowAnimation>(out WindowAnimation anim))
        {
            await anim.AnimatePopEffect(rect);
        }
    }
}

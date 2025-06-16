using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : Singleton<LoadingUI>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image fillImage;
    
    private bool _isFadeIn = true;

    void Awake()
    {
        SceneLoader.SceneAnimation = async () => await Fade();
        SceneLoader.LoadingTask = async () => await ProgressLoading();
    }

    public async UniTask<bool> ProgressLoading()
    {
        fillImage.fillAmount = 0f;

        AsyncOperation op = SceneLoader.AsyncOperation;
        op.allowSceneActivation = false;

        float t = 0f;

        //for test method
        ///////////////////////////////////////////////////////////////////////////////
        _ = GameManager.Instance.TaskQueue.RunTask(async () =>
        {
            Debug.Log("Task 3 Start");
            await UniTask.Delay(1000);
            Debug.Log("Task 3 End");
            return -1;
        });
        
        ///////////////////////////////////////////////////////////////////////////////
        
        float processMileStone = 1f / GameManager.Instance.TaskQueue.TaskCount;
        float progress = processMileStone;
        
        GameManager.Instance.TaskQueue.OnDoneTask += () => progress += processMileStone;
        GameManager.Instance.TaskQueue.OnQueueEmpty += async () =>
        {
            //Fake Load
            await UniTask.WaitForSeconds(0.5f);

            op.allowSceneActivation = true;
        };

        while (!op.isDone)
        {
            await UniTask.Yield();

            if (fillImage.fillAmount < progress)
            {
                t += Time.unscaledDeltaTime;
                fillImage.fillAmount = Mathf.Lerp(0f, progress, t);
            }
            else
                t = progress;
        }
        
        return true;
    }
    
    public async UniTask<bool> Fade()
    {
        float t = 0f;

        while (t <= 1f)
        {
            t += Time.unscaledDeltaTime;

            canvasGroup.alpha = _isFadeIn ? Mathf.Lerp(0f, 1f, t) : Mathf.Lerp(1f, 0f, t);
            await UniTask.Yield();
        }
        
        _isFadeIn = !_isFadeIn;
        
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    private float fadeSpeed = 0.05f;
    CancellationTokenSource source = new CancellationTokenSource();
    

    [SerializeField] private CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartIntro().Forget();
    }

    private void OnEnable()
    {
        if (source != null)
        {
            source.Dispose();
        }
        source = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        if(source!=null)
            source.Cancel();
    }

    private void OnDestroy()
    {
        if (source != null)
        {
            source.Cancel();
            source.Dispose();
        }
    }

    private async UniTask StartIntro()
    {
        await UniTask.Delay(1000);
        while (canvasGroup.alpha < 1)
        {
            await UniTask.Delay(50);
            canvasGroup.alpha += Time.timeScale*fadeSpeed;
        }
        await UniTask.Delay(1000);
        while (canvasGroup.alpha > 0)
        {
            await UniTask.Delay(50);
            canvasGroup.alpha -= Time.timeScale * fadeSpeed;
        }
        SceneManager.LoadScene("Game_Start");
    }
}
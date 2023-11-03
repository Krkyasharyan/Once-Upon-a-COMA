using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class GameLoader : MonoBehaviour
{
    private static GameLoader instance;
    public static GameLoader Instance => instance;

    private bool isUISceneLoaded = false;
    private bool isSinglePlayerSceneLoaded = false;
    private bool isMultiplayerSceneLoaded = false;

    public static bool startFlag;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 在 Awake 方法中使用协程异步加载 UI 场景
        StartCoroutine(LoadSceneAsync("UIScene", () =>
        {
            isUISceneLoaded = true;
            Debug.Log("UI 场景加载完成");
        }));
    }

    private void Update()
    {
        if (startFlag == true)
        {
            LoadMultiplayerScene();
            startFlag = false;
        }
    }

    public static void LoadSinglePlayerScene()
    {
        if (Instance.isSinglePlayerSceneLoaded)
            return;

        Instance.StartCoroutine(Instance.LoadSceneAsync("OfflineScene", () =>
        {
            Instance.isSinglePlayerSceneLoaded = true;
            Debug.Log("单人游戏场景加载完成");
            
            // 在这里调用网络组件的接口来控制场景内的脚本
            // NetworkManager.Instance.EnableSinglePlayerMode();
            //WebController.findPlayers();
        }));
    }

    public static void LoadMultiplayerScene()
    {
        
        Debug.Log(Instance.isMultiplayerSceneLoaded);
        if (Instance.isMultiplayerSceneLoaded)
        {
            return;
        }
        Debug.Log("多人游戏加载完成");
        //SceneManager.LoadScene("OnlineScene");
        Instance.StartCoroutine(Instance.LoadSceneAsync("OnlineScene", () =>
        {
            Instance.isMultiplayerSceneLoaded = true;
            Debug.Log("多人游戏场景加载完成");
        
            // 在这里调用网络组件的接口来控制场景内的脚本
            WebController.findPlayers();
            //WebController.timerstart();
        }));
    }

    private IEnumerator LoadSceneAsync(string sceneName, System.Action onSceneLoaded)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        onSceneLoaded?.Invoke();
    }

    public void SwitchToUI()
    {
        SceneManager.LoadScene("UIScene");
        if(Time.timeScale == 0f) Time.timeScale = 1f;
    }

    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool isGamePaused = false;

    public GameObject gameUIText;
    public GameUIText gameuiText;
    public PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // 以按下P键为示例，你可以根据需要更改按键
        {
            if (isGamePaused)
            {
                QuitPauseUI();
                if(WebController.players.Length == 0)
                    ResumeGame(); // 如果游戏已经暂停，则调用恢复游戏函数
            }
            else
            {
                SetPauseUI();
                if(WebController.players.Length == 0)
                    PauseGame(); // 如果游戏未暂停，则调用暂停游戏函数
            }
        }
    }

    void PauseGame()
    {
       
        Time.timeScale = 0f; // 将时间缩放设置为0，使得游戏逻辑停止
        // 在这里添加其他你需要执行的暂停相关的代码，比如菜单显示等
    }

    void SetPauseUI()
    {
        isGamePaused = true;
        gameUIText.SetActive(true);
        gameuiText.setText("Pause");
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复时间缩放为正常值，使得游戏逻辑继续
        // 在这里添加其他你需要执行的恢复相关的代码，比如菜单关闭等
    }

    void QuitPauseUI()
    {
        isGamePaused = false;
        gameUIText.SetActive(false);
    }

}
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event Action OnScanMenu;
    public event Action OnActivatorMenu;

    public static GameManager instance;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ScanMenu();
    }

    public void ScanMenu()
    {
        OnScanMenu?.Invoke();
        Debug.Log("Scan menu activated");
    }

    public void ActivatorMenu()
    {
        OnActivatorMenu?.Invoke();
        Debug.Log("Activator Menu activated");
    }

    public void ScreenShoot()
    {
        ScreenCapture.CaptureScreenshot("screenshoot");
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}

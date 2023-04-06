using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    /* public string mainMenuScene;
     public GameObject pauseMenu;
     public bool isPaused; */

    private PlayerInput playerInput;
    private InputAction menu;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private bool isPaused;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
    }

    // Update is called once per frame
    void Update()
    {
       /* if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                isPaused = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        } */
    }

    private void OnEnable()
    {
        menu = playerInput.Menu.Escape;
        menu.Enable();

        menu.performed += Pause;
    }

    private void OnDisable()
    {
        menu.Disable();
    }

    void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }
    
    void ActivateMenu()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        pauseUI.SetActive(true);
        //isPaused = true;
    
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        pauseUI.SetActive(false);
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*  public void ResumeGame()
      {
          isPaused = false;
          pauseMenu.SetActive(false);
          Time.timeScale = 1f;
      } */
    
      public void Controls()
      {
          Time.timeScale = 1f;
          SceneManager.LoadScene(2);
      }
     
    public void ReturnToMain()
      {
          Time.timeScale = 1f;
          SceneManager.LoadScene(0);
      }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject levelEndPanel;
    [SerializeField] private GameObject levelEndParticle;
    [SerializeField] private GameObject levelBeginningBorder;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject musicOnButton, musicOffButton;
    [SerializeField] private AudioSource levelEndAudio, welldoneAudio, gameCompletedAudio, levelOpeningAudio, 
                                         backgroundMusic, clickAudio, infoAudio;
    [SerializeField] private Animator transitionAnim, mascotAnim;

    private static int activeLevelIndex, currentLevelIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag!="MainMenu")
            transitionAnim.SetTrigger("StartLevel");
        else
        {
            if(PlayerPrefs.GetString("MainMenuTransition","Close") == "Open")
                transitionAnim.SetTrigger("StartLevel");

            backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
            if (PlayerPrefs.GetString("BackgroundMusic", "On") == "On")
            {
                musicOffButton.SetActive(false);
                musicOnButton.SetActive(true);
            }
            else if(PlayerPrefs.GetString("BackgroundMusic", "On") == "Off")
            {
                TurnOffBackgroundMusic();
            }
        }

        if (levelOpeningAudio != null)
            StartCoroutine(PlayLevelOpening());

        activeLevelIndex = PlayerPrefs.GetInt("ActiveLevelIndex", 1);

        //To reset tutorial level static variables in each level to be able to play tutorial properly whenever start it
        DragAndDrop.currentObjectIndex = 0;
        DragAndDrop.isStarted = false;
    }

    public void RestartLevel()
    {
        ClickButton();
        StartCoroutine(RestartLevelCoroutine());
    }
   
    public void LoadNextLevel()
    {
        ClickButton();
        StartCoroutine(NextLevelCoroutine());
    }

    public void LoadSelectedLevel(string levelName)
    {
        ClickButton();
        StartCoroutine(SelectedLevelCoroutine(levelName));
    }

    public void ShowLevelEndPanel()
    {
        StartCoroutine(LevelEndCoroutine());
    }

    public void TurnOnBackgroundMusic()
    {
        ClickButton();
        backgroundMusic.Play();
        musicOffButton.SetActive(false);
        musicOnButton.SetActive(true);
        PlayerPrefs.SetString("BackgroundMusic", "On");
    }
    public void TurnOffBackgroundMusic()
    {
        ClickButton();
        backgroundMusic.Stop();
        musicOnButton.SetActive(false);
        musicOffButton.SetActive(true);
        PlayerPrefs.SetString("BackgroundMusic", "Off");
    }

    private void ClickButton()
    {
        clickAudio.Play();
    }

    public void PlayInfoAudio()
    {
        ClickButton();
        infoAudio.Play();
    }

    public void QuitGame()
    {
        ClickButton();
        Application.Quit();
    }

    public void SaveActiveLevel()
    {
        if (PlayerPrefs.GetString(("Level" + (currentLevelIndex + 1))) != "Unlocked")
        {
            activeLevelIndex++;
            PlayerPrefs.SetInt("ActiveLevelIndex", activeLevelIndex);
            PlayerPrefs.SetString(("Level" + activeLevelIndex), "Unlocked");
        }
    }

    IEnumerator PlayLevelOpening()
    {
        yield return new WaitForSeconds(0.1f);
        levelOpeningAudio.Play();
        float audioLength = levelEndAudio.clip.length;
        yield return new WaitUntil(() => levelOpeningAudio.isPlaying == false);
        levelBeginningBorder.SetActive(false);
    }

    IEnumerator RestartLevelCoroutine()
    {
        transitionAnim.SetTrigger("EndLevel");
        yield return new WaitForSeconds(1f);
        levelEndPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    IEnumerator NextLevelCoroutine()
    {
        transitionAnim.SetTrigger("EndLevel");

        yield return new WaitForSeconds(1f);

        try
        {
            currentLevelIndex++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        catch
        {
            Debug.Log("last levellll");
            SceneManager.LoadScene("LevelsMenu");
        }
    }
    IEnumerator LevelEndCoroutine()
    {
        SaveActiveLevel();
        yield return new WaitForSeconds(0.2f);
        menuButton.SetActive(false);

        if (gameObject.tag != "LastLevel")
            welldoneAudio.Play();
        else {
            if (PlayerPrefs.GetString("LastLevelCompleted", "false") == "false")
            {
                gameCompletedAudio.Play();
                PlayerPrefs.SetString("LastLevelCompleted", "true");
            }
            else
                welldoneAudio.Play();
        }
                
        mascotAnim.SetTrigger("Roll");
        levelEndAudio.Play();
        levelEndParticle.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        levelEndPanel.SetActive(true);
    }

    IEnumerator SelectedLevelCoroutine(string levelName)
    {
        transitionAnim.SetTrigger("EndLevel");
        yield return new WaitForSeconds(1f);
        
        if(levelName == "MainMenu")
            PlayerPrefs.SetString("MainMenuTransition", "Open");

        if (gameObject.tag == "LevelsMenu" &&
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.tag == "LevelButton")
        {
            currentLevelIndex = int.Parse(levelName.Substring(5));
        }

        SceneManager.LoadScene(levelName);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("MainMenuTransition", "Close");
    }
}

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
    [SerializeField] private AudioSource levelEndAudio, welldoneAudio, levelOpeningAudio;
    [SerializeField] private Animator transitionAnim, mascotAnim;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.tag!="MainMenu")
            transitionAnim.SetTrigger("StartLevel");
        else
        {
            if(PlayerPrefs.GetString("MainMenuTransition","Close") == "Open")
                transitionAnim.SetTrigger("StartLevel");
        }

        if (levelOpeningAudio != null)
            StartCoroutine(PlayLevelOpening());
    }

    public void RestartLevel()
    {
        levelEndPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
   
    public void LoadNextLevel()
    {
        StartCoroutine(NextLevelCoroutine());
    }

    public void LoadSelectedLevel(string levelName)
    {
        StartCoroutine(SelectedLevelCoroutine(levelName));
    }

    public void ShowLevelEndPanel()
    {
        StartCoroutine(LevelEndCoroutine());
    }
    public void QuitGame()
    {
        Application.Quit();
    }


    IEnumerator PlayLevelOpening()
    {
        yield return new WaitForSeconds(0.1f);
        levelOpeningAudio.Play();
        float audioLength = levelEndAudio.clip.length;
        yield return new WaitUntil(() => levelOpeningAudio.isPlaying == false);
        levelBeginningBorder.SetActive(false);
    }
    IEnumerator NextLevelCoroutine()
    {
        transitionAnim.SetTrigger("EndLevel");

        yield return new WaitForSeconds(1f);

        try
        {
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
        yield return new WaitForSeconds(0.2f);
        menuButton.SetActive(false);
        welldoneAudio.Play();
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
        
        if(levelName=="MainMenu")
            PlayerPrefs.SetString("MainMenuTransition", "Open");

        SceneManager.LoadScene(levelName);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("MainMenuTransition", "Close");
    }
}

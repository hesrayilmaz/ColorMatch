using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject levelEndPanel;
    [SerializeField] private GameObject levelEndParticle;
    [SerializeField] private AudioSource levelEndAudio;
    [SerializeField] private Animator transitionAnim;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.tag!="MainMenu")
            transitionAnim.SetTrigger("StartLevel");
    }

    public void RestartLevel()
    {
        levelEndPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevelsMenu()
    {
        StartCoroutine(LevelsMenuCoroutine());
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
            Debug.Log("Last Level");
        }
    }
    IEnumerator LevelEndCoroutine()
    {
        levelEndAudio.Play();
        yield return new WaitForSeconds(0.2f);

        levelEndParticle.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        levelEndPanel.SetActive(true);
    }

    IEnumerator LevelsMenuCoroutine()
    {
        transitionAnim.SetTrigger("EndLevel");

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("LevelsMenu");
    }

    IEnumerator SelectedLevelCoroutine(string levelName)
    {
        transitionAnim.SetTrigger("EndLevel");

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelName);
    }
}

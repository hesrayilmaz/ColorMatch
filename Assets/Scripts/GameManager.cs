using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public static GameManager instance;

    [SerializeField] private GameObject levelEndPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void RestartLevel()
    {
        levelEndPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadLevelsMenu()
    {
        SceneManager.LoadScene("LevelsMenu");
    }
   
    public void LoadNextLevel()
    {
        StartCoroutine(NextLevelCoroutine());
    }

    public void LoadSelectedLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowLevelEndPanel()
    {
        StartCoroutine(LevelEndCoroutine());
    }

    IEnumerator NextLevelCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

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
        yield return new WaitForSeconds(0.5f);

        levelEndPanel.SetActive(true);
    }
}

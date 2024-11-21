using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    Animator animator;
    [SerializeField] float waitDelay = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        animator = GetComponentInChildren<Animator>();
    }

    public IEnumerator LoadLevelByIndex(int levelIndex)
    {
        animator.SetTrigger("Transition");
        yield return new WaitForSeconds(waitDelay);
        SceneManager.LoadScene(levelIndex);
    }

    public IEnumerator LoadLevelByString(string levelName)
    {
        animator.SetTrigger("Transition");
        yield return new WaitForSeconds(waitDelay);
        SceneManager.LoadScene(levelName);
    }
}

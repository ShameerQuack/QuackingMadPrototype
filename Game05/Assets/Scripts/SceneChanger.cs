using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private static SceneChanger _instance;
    public static SceneChanger Instance { get { return _instance; } }

    public Animator animator;
    public GameEvent sceneTransitionStartEvent;

    private int sceneToLoad;

    void Awake(){
        _instance = this;
        Debug.Log(Mark.bookmarkSelected);
    }

    public void FadeToNextScene(){
        sceneTransitionStartEvent.Raise();
        FadeToScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FadeToSameScene(){
        FadeToScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void FadeToScene(int sceneIndex){
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

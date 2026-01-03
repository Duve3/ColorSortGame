using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private int m_loadLevel;

    private void Awake()
    {
        transform.SetAsLastSibling();
    }

    public void FadeToScene(int index)
    {
        m_loadLevel = index;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(m_loadLevel);
    }
}

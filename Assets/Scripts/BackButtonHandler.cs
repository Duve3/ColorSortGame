using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField] private GameHandler gameHandler;
    public void ToMainMenu()
    {
        if (gameHandler != null) { gameHandler.GameSaveData(); }
        SceneManager.LoadScene(0);
    }
}

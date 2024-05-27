using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with the name of your game scene
    }
}

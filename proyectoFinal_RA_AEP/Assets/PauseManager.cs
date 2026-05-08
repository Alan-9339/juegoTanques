using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    private bool isPaused = false;

    // 1. Función para pausar el juego
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // Muestra el menú
        Time.timeScale = 0f;            // Pausa
        isPaused = true;
    }

    // 2. Función para continuar
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // Oculta el menú
        Time.timeScale = 1f;             // Devuelve el tiempo a la normalidad
        isPaused = false;
    }

    // 3. Función para ir al menú principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

public class SceneNavigation : MonoBehaviour
{
    // Esta función cargará el menú principal
    public void GoToMainMenu()
    {
        // Asegúrate de que "MainMenu" sea el nombre exacto de tu escena
        SceneManager.LoadScene("MainMenu");
    }

    // Opcional: Reiniciar la partida actual
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
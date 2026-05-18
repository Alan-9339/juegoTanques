using UnityEngine;
using UnityEngine.SceneManagement; // Obligatorio para cambiar escenas

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; // Arrastra aquí tu panel de ajustes
    [SerializeField] private GameObject controlsPanel;

    // Función para el botón Comenzar
    public void PlayGame()
    {
        // "SampleScene" debe ser el nombre exacto de tu escena de juego
        SceneManager.LoadScene("SampleScene"); 
    }

    // Función para el botón de Engrane (Abrir ajustes)
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // Función para el botón "Cerrar" dentro de ajustes
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }

}
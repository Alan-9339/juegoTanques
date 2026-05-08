using UnityEngine;
using UnityEngine.SceneManagement;

public class EditControlsManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    [SerializeField] private GameObject mainMenuPanel;    // Panel principal del menú
    [SerializeField] private GameObject settingsPanel;    // Panel de ajustes generales
    [SerializeField] private GameObject editControlsPanel; // Panel de edición de joysticks

    [Header("Joysticks de Edición")]
    [SerializeField] private RectTransform leftJoystickEdit;
    [SerializeField] private RectTransform rightJoystickEdit;

    void Start()
    {
        // Al iniciar el juego, cargamos la configuración por si el usuario 
        // entra directo a la escena sin pasar por el panel de edición.
        LoadJoystickState(leftJoystickEdit, "LeftJoy");
        LoadJoystickState(rightJoystickEdit, "RightJoy");
    }

    // --- NAVEGACIÓN ---

    // 1. Abre el panel de edición desde Ajustes
    public void OpenEditControls()
    {
        settingsPanel.SetActive(false);
        editControlsPanel.SetActive(true);
        
        // Cargamos el estado actual para que el preview coincida con lo guardado
        LoadJoystickState(leftJoystickEdit, "LeftJoy");
        LoadJoystickState(rightJoystickEdit, "RightJoy");
    }

    // 2. Guarda y regresa al panel de Ajustes
    public void SaveAndBackToSettings()
    {
        SaveJoystickState(leftJoystickEdit, "LeftJoy");
        SaveJoystickState(rightJoystickEdit, "RightJoy");
        
        PlayerPrefs.Save(); 
        
        editControlsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // 3. Regresa al Menú Principal desde Ajustes
    public void CloseSettingsAndGoToMain()
    {
        settingsPanel.SetActive(false);
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // --- LÓGICA DE PERSISTENCIA ---

    private void SaveJoystickState(RectTransform joy, string prefix)
    {
        if (joy == null) return;
        // Guardamos posición X, Y y Escala
        PlayerPrefs.SetFloat(prefix + "PosX", joy.anchoredPosition.x);
        PlayerPrefs.SetFloat(prefix + "PosY", joy.anchoredPosition.y);
        PlayerPrefs.SetFloat(prefix + "Scale", joy.localScale.x);
        
        Debug.Log($"[Guardado] {prefix}: Pos({joy.anchoredPosition.x}, {joy.anchoredPosition.y}) Escala({joy.localScale.x})");
    }

    private void LoadJoystickState(RectTransform joy, string prefix)
    {
        if (joy == null) return;

        // Si no hay datos guardados, usamos la posición actual del objeto en el inspector como default
        float x = PlayerPrefs.GetFloat(prefix + "PosX", joy.anchoredPosition.x);
        float y = PlayerPrefs.GetFloat(prefix + "PosY", joy.anchoredPosition.y);
        float scale = PlayerPrefs.GetFloat(prefix + "Scale", 1.0f);

        joy.anchoredPosition = new Vector2(x, y);
        joy.localScale = Vector3.one * scale;
    }
}
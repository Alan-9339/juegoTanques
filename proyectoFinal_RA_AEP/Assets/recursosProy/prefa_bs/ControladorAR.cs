using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ControladorAR : MonoBehaviour
{
    [Header("Configuración AR")]
    public GameObject visualPrevia;
    public GameObject mapaPrefab;

    [Header("UI")]
    public GameObject panelEscaneo;
    public GameObject panelEscala;
    public GameObject panelJuego;

    [Header("Escalado")]
    public float escalaInicial = 1f;
    public float escalaMinima = 0.5f;
    public float escalaMaxima = 3f;
    public float velocidadEscala = 0.25f;

    [Header("Player")]
    public GameObject tankPlayerPrefab;

    private GameObject mapaInstanciado;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private ARSession arSession;

    private Pose posicionDetectada;

    private bool posicionValida = false;
    private bool enModoEscaneo = false;

    private float escalaActual = 1f;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        arSession = FindObjectOfType<ARSession>();

        ActivarModoEscaneo();
    }

    void Update()
    {
        if (!enModoEscaneo || mapaInstanciado != null)
            return;

        ActualizarPosicionPreview();
    }

    // =========================================================
    // MODO ESCANEO
    // =========================================================

    void ActivarModoEscaneo()
    {
        enModoEscaneo = true;

        panelEscaneo.SetActive(true);
        panelEscala.SetActive(false);
        panelJuego.SetActive(false);

        if (planeManager != null)
            planeManager.enabled = true;
    }

    void ActualizarPosicionPreview()
    {
        Vector2 centroPantalla = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );

        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(
            centroPantalla,
            hits,
            TrackableType.PlaneWithinPolygon))
            
        {
            Debug.Log("SUPERFICIE DETECTADA");
            posicionValida = true;

            posicionDetectada = hits[0].pose;

            visualPrevia.SetActive(true);

            visualPrevia.transform.position =
                posicionDetectada.position;

            // Rotación horizontal mirando hacia el jugador
            Quaternion rotacionHorizontal =
                Quaternion.Euler(
                    0,
                    Camera.main.transform.eulerAngles.y,
                    0
                );

            visualPrevia.transform.rotation =
                rotacionHorizontal;
        }
        else
        {
            posicionValida = false;
            visualPrevia.SetActive(false);
        }
    }

    // =========================================================
    // BOTONES UI
    // =========================================================

    public void BotonReiniciarEscaneo()
    {
        Debug.Log("Reiniciando sesión AR...");

        if (arSession != null)
            arSession.Reset();

        visualPrevia.SetActive(false);

        posicionValida = false;
    }

    // =========================================================
    // CREAR PREVIEW DEL MAPA
    // =========================================================

    public void BotonListo()
{
    Debug.Log("BOTON LISTO PRESIONADO");

    Debug.Log($"posicionValida: {posicionValida}");

    if (!posicionValida)
    {
        Debug.LogWarning("No hay superficie válida");
        return;
    }

    if (mapaInstanciado != null)
    {
        Debug.LogWarning("Mapa ya instanciado");
        return;
    }

    Quaternion rotacionHorizontal =
        Quaternion.Euler(
            0,
            Camera.main.transform.eulerAngles.y,
            0
        );

    Debug.Log("Instanciando mapa...");

    mapaInstanciado = Instantiate(
        mapaPrefab,
        posicionDetectada.position,
        rotacionHorizontal
    );

    Debug.Log("Mapa instanciado correctamente");

    escalaActual = escalaInicial;

    mapaInstanciado.transform.localScale =
        Vector3.one * escalaActual;

    enModoEscaneo = false;

    visualPrevia.SetActive(false);

    panelEscaneo.SetActive(false);
    panelEscala.SetActive(true);

    Debug.Log("Panel de escala activado");
}

    // =========================================================
    // ESCALADO
    // =========================================================

    public void AumentarEscala()
    {
        if (mapaInstanciado == null)
            return;

        escalaActual += velocidadEscala;

        escalaActual = Mathf.Clamp(
            escalaActual,
            escalaMinima,
            escalaMaxima
        );

        AplicarEscala();
    }

    public void ReducirEscala()
    {
        if (mapaInstanciado == null)
            return;

        escalaActual -= velocidadEscala;

        escalaActual = Mathf.Clamp(
            escalaActual,
            escalaMinima,
            escalaMaxima
        );

        AplicarEscala();
    }

    void AplicarEscala()
    {
        mapaInstanciado.transform.localScale =
            Vector3.one * escalaActual;

        Debug.Log(
            $"Escala actual: {escalaActual}"
        );
    }

    // =========================================================
    // CONFIRMAR MAPA Y COMENZAR PARTIDA
    // =========================================================

    public void ConfirmarMapa()
    {
        if (mapaInstanciado == null)
            return;

        panelEscala.SetActive(false);
        panelJuego.SetActive(true);

        // Ocultar planos AR
        if (planeManager != null)
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }

            planeManager.enabled = false;
        }

        // =====================================================
        // SPAWN DEL JUGADOR
        // =====================================================

        Transform spawnJugador =
            mapaInstanciado.transform.Find("SpawnJugador");

        if (spawnJugador != null)
        {
            Instantiate(
                tankPlayerPrefab,
                spawnJugador.position,
                spawnJugador.rotation,
                mapaInstanciado.transform
            );

            Debug.Log("Tanque del jugador instanciado.");
        }
        else
        {
            Debug.LogWarning(
                "No se encontró un objeto llamado 'SpawnJugador' dentro del mapa."
            );
        }

        Debug.Log("Juego iniciado.");
    }
}
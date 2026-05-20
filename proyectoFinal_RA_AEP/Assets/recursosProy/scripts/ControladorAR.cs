using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ControladorAR : MonoBehaviour
{
    [Header("Configuración AR")]
    public GameObject visualPrevia;
    
    // Arreglo para guardar todos tus niveles
    public GameObject[] mapasNiveles; 
    private int nivelActual = 0;      // Controla en qué nivel vas
    private int enemigosRestantes = 0; // Contador de tanques vivos

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

    [Header("Enemigo 1")]
    public GameObject EnemyTankStatic;

    [Header("Enemigo 2")]
    public GameObject EnemyTankHunter;

    [Header("Joysticks")]
    public FixedJoystick joystickMovimiento;
    public FixedJoystick joystickApuntado;

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
            posicionValida = true;

            posicionDetectada = hits[0].pose;

            visualPrevia.SetActive(true);

            visualPrevia.transform.position =
                posicionDetectada.position;

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
        if (!posicionValida)
        {
            Debug.LogWarning("No hay superficie válida.");
            return;
        }

        if (mapaInstanciado != null)
        {
            Debug.LogWarning("El mapa ya fue instanciado.");
            return;
        }

        // Evitar errores si no hay niveles configurados
        if (mapasNiveles.Length == 0 || mapasNiveles[nivelActual] == null)
        {
            Debug.LogError("No has asignado el Prefab del nivel en el arreglo 'Mapas Niveles'.");
            return;
        }

        Quaternion rotacionJugador =
            Quaternion.Euler(
                0,
                Camera.main.transform.eulerAngles.y,
                0
            );
            
        // Usar el mapa del nivel actual en lugar de mapaPrefab
        Quaternion rotacionFinal =
            rotacionJugador * mapasNiveles[nivelActual].transform.rotation;

        mapaInstanciado = Instantiate(
            mapasNiveles[nivelActual],
            posicionDetectada.position,
            rotacionFinal
        );

        escalaActual = escalaInicial;

        mapaInstanciado.transform.localScale =
            Vector3.one * escalaActual;

        enModoEscaneo = false;

        visualPrevia.SetActive(false);

        panelEscaneo.SetActive(false);
        panelEscala.SetActive(true);
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

        if (spawnJugador == null)
        {
            Debug.LogWarning(
                "No existe SpawnJugador dentro del mapa."
            );

            return;
        }

        GameObject nuevoTanque = Instantiate(
            tankPlayerPrefab,
            spawnJugador.position + Vector3.up * 0.01f,
            spawnJugador.rotation,
            mapaInstanciado.transform
        );

        // =====================================================
        // ASIGNAR JOYSTICKS
        // =====================================================

        TankController tankController =
            nuevoTanque.GetComponent<TankController>();

        if (tankController != null)
        {
            tankController.moveJoystick =
                joystickMovimiento;

            tankController.aimJoystick =
                joystickApuntado;
        }

        // =====================================================
        // SPAWN ENEMIGO 1 (ESTÁTICO)
        // =====================================================

        Transform spawnEnemigo1 =
            mapaInstanciado.transform.Find("SpawnEnemigo1");

        if (spawnEnemigo1 != null)
        {
            GameObject enemigo1 = Instantiate(
                EnemyTankStatic,
                spawnEnemigo1.position + Vector3.up * 0.01f,
                spawnEnemigo1.rotation,
                mapaInstanciado.transform
            );

            RegistrarEnemigo();

            EnemyTankStatic staticEnemy =
                enemigo1.GetComponent<EnemyTankStatic>();

            if (staticEnemy != null)
            {
                staticEnemy.player =
                    nuevoTanque.transform;
            }
        }
        else
        {
            Debug.LogWarning(
                "No existe SpawnEnemigo1 dentro del mapa."
            );
        }

        // =====================================================
        // SPAWN ENEMIGO 2 (HUNTER)
        // =====================================================

        Transform spawnEnemigo2 =
            mapaInstanciado.transform.Find("SpawnEnemigo2");

        if (spawnEnemigo2 != null)
        {
            GameObject enemigo2 = Instantiate(
                EnemyTankHunter,
                spawnEnemigo2.position + Vector3.up * 0.01f,
                spawnEnemigo2.rotation,
                mapaInstanciado.transform
            );
            RegistrarEnemigo();

            EnemyTankHunter hunter2 =
                enemigo2.GetComponent<EnemyTankHunter>();

            if (hunter2 != null)
            {
                hunter2.player =
                    nuevoTanque.transform;
            }
        }
        else
        {
            Debug.LogWarning(
                "No existe SpawnEnemigo2 dentro del mapa."
            );
        }
    }

    // =========================================================
    // LÓGICA DE PROGRESIÓN DE NIVELES
    // =========================================================

    public void RegistrarEnemigo()
    {
        enemigosRestantes++;
    }

    public void EnemigoEliminado()
    {
        enemigosRestantes--;

        // Si ya no quedan enemigos, pasamos de nivel
        if (enemigosRestantes <= 0)
        {
            ComenzarSiguienteNivel();
        }
    }

    void ComenzarSiguienteNivel()
    {
        nivelActual++;

        // Verificar victoria
        if (nivelActual >= mapasNiveles.Length)
        {
            Debug.Log("¡Juego Completado!");
            return;
        }

        // Guardar posición y rotación del mapa actual
        Vector3 posicion = mapaInstanciado.transform.position;
        Quaternion rotacion = mapaInstanciado.transform.rotation;

        // Destruir mapa viejo
        Destroy(mapaInstanciado);

        enemigosRestantes = 0;

        // Crear siguiente nivel
        mapaInstanciado = Instantiate(
            mapasNiveles[nivelActual],
            posicion,
            rotacion
        );

        mapaInstanciado.transform.localScale =
            Vector3.one * escalaActual;

        // ==========================================
        // SPAWN PLAYER
        // ==========================================

        Transform spawnJugador =
            mapaInstanciado.transform.Find("SpawnJugador");

        GameObject nuevoTanque = Instantiate(
            tankPlayerPrefab,
            spawnJugador.position + Vector3.up * 0.01f,
            spawnJugador.rotation,
            mapaInstanciado.transform
        );

        TankController tankController =
            nuevoTanque.GetComponent<TankController>();

        if (tankController != null)
        {
            tankController.moveJoystick = joystickMovimiento;
            tankController.aimJoystick = joystickApuntado;
        }

        // ==========================================
        // ENEMIGO 1
        // ==========================================

        Transform spawnEnemigo1 =
            mapaInstanciado.transform.Find("SpawnEnemigo1");

        if (spawnEnemigo1 != null)
        {
            GameObject enemigo1 = Instantiate(
                EnemyTankStatic,
                spawnEnemigo1.position + Vector3.up * 0.01f,
                spawnEnemigo1.rotation,
                mapaInstanciado.transform
            );

            RegistrarEnemigo();

            EnemyTankStatic staticEnemy =
                enemigo1.GetComponent<EnemyTankStatic>();

            if (staticEnemy != null)
            {
                staticEnemy.player = nuevoTanque.transform;
            }
        }

        // ==========================================
        // ENEMIGO 2
        // ==========================================

        Transform spawnEnemigo2 =
            mapaInstanciado.transform.Find("SpawnEnemigo2");

        if (spawnEnemigo2 != null)
        {
            GameObject enemigo2 = Instantiate(
                EnemyTankHunter,
                spawnEnemigo2.position + Vector3.up * 0.01f,
                spawnEnemigo2.rotation,
                mapaInstanciado.transform
            );

            RegistrarEnemigo();

            EnemyTankHunter hunter =
                enemigo2.GetComponent<EnemyTankHunter>();

            if (hunter != null)
            {
                hunter.player = nuevoTanque.transform;
            }
        }

        Debug.Log("Nivel siguiente cargado.");
    }
}
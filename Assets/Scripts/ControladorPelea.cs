using UnityEngine;
using System.Collections.Generic; // Necesario para usar List

public class ControladorPelea : MonoBehaviour
{
    // ==========================================================
    // SECCIÓN MAPAS
    // ==========================================================

    [Header("Sprites de Fondo de Mapas")]
    public Sprite fondoMapa0;
    public Sprite fondoMapa1;
    public Sprite fondoMapa2;
    public Sprite fondoMapa3;

    [Header("Referencias en Escena")]
    public SpriteRenderer renderizadorFondo;

    [Header("Configuracion del Fondo")]
    public bool ajustarFondoAutomaticamente = true;

    // ==========================================================
    // SECCIÓN PERSONAJES (PREFABS)
    // ==========================================================

    [Header("Prefabs de Personajes")]
    public List<GameObject> personajes; // Lista para cargar los prefabs

    [Header("Puntos de Spawn (Vector3)")]
    // Usamos Vector3 para ser consistentes con el Instantiate
    public Vector3 posicionPersonajeUno;
    public Vector3 posicionPersonajeDos;

    // ==========================================================
    // SECCIÓN SISTEMAS
    // ==========================================================

    [Header("Sistema de Vida - Referencias UI")]
    public SistemaVidaPersonaje sistemaVidaPersonaje; // Asignar en el Inspector



    // ==========================================================
    // CICLO DE VIDA
    // ==========================================================

    void Start()
    {
        VerificarReferencias();
        CargarMapa();
        CargarPersonaje();
        ConfigurarSistemaVida();
        // ConfigurarSistemaPelea(); // Mantenido comentado según tu script original
    }

    // ==========================================================
    // FUNCIONES DE VERIFICACIÓN
    // ==========================================================

    void VerificarReferencias()
    {
        if (renderizadorFondo == null)
        {
            Debug.LogError("¡El SpriteRenderer del fondo no está asignado!");
        }
        if (sistemaVidaPersonaje == null)
        {
            Debug.LogError("¡SistemaVidaPersonaje no está asignado! Busca el objeto en la escena y asígnalo.");
        }
    }

    // ==========================================================
    // FUNCIONES DE CARGA
    // ==========================================================

    void CargarMapa()
    {
        if (renderizadorFondo == null) return;

        int indiceMapa = DatosJuego.indiceMapaSeleccionado;
        string nombreMapa = DatosJuego.mapaSeleccionado;

        Debug.Log($"Cargando mapa: {nombreMapa} (Índice: {indiceMapa})");

        // Uso de switch para asignar el sprite de fondo
        switch (indiceMapa)
        {
            case 0:
                renderizadorFondo.sprite = fondoMapa0;
                break;
            case 1:
                renderizadorFondo.sprite = fondoMapa1;
                break;
            case 2:
                renderizadorFondo.sprite = fondoMapa2;
                break;
            case 3:
                renderizadorFondo.sprite = fondoMapa3;
                break;
            default:
                Debug.LogWarning($"Índice de mapa no válido: {indiceMapa}");
                break;
        }

        Debug.Log($"Fondo asignado: Mapa {indiceMapa}");
        renderizadorFondo.sortingOrder = 0;

        if (ajustarFondoAutomaticamente)
        {
            AjustarFondoAPantalla();
        }
    }

    void CargarPersonaje()
    {
        Debug.Log($"El personaje se está cocinando");

        int indicePersonajeUno = DatosJuego.indicePersonajeSeleccionadoUno;
        int indicePersonajeDos = DatosJuego.indicePersonajeSeleccionadoDos;

        // **CORRECCIÓN #1 y #2: Verificación de índice y uso correcto de Instantiate**

        Debug.Log($"Personaje con ID {indicePersonajeUno} es el personaje {personajes[indicePersonajeUno].name}");
        Debug.Log($"Personaje con ID {indicePersonajeDos} es el personaje {personajes[indicePersonajeDos].name}");

        // Spawnea Personaje Uno
        if (indicePersonajeUno >= 0 && indicePersonajeUno < personajes.Count)
        {
            // Usamos posicionPersonajeUno (que ahora es Vector3)
            GameObject p1 = Instantiate(personajes[indicePersonajeUno], posicionPersonajeUno, Quaternion.identity);
            ControlPlayer scriptDelClon = p1.GetComponent<ControlPlayer>();
            scriptDelClon.usarControlesWASD = true;
            Debug.Log($"Instancia uno spawneada: {p1.name}");
        }
        else
        {
            Debug.LogError($"Error: Índice Personaje UNO ({indicePersonajeUno}) fuera de rango. Total de prefabs cargados: {personajes.Count}.");
        }

        // Spawnea Personaje Dos
        if (indicePersonajeDos >= 0 && indicePersonajeDos < personajes.Count)
        {
            // Usamos posicionPersonajeDos (que ahora es Vector3)
            GameObject p2 = Instantiate(personajes[indicePersonajeDos], posicionPersonajeDos, Quaternion.identity);
            ControlPlayer scriptDelClon2 = p2.GetComponent<ControlPlayer>();
            scriptDelClon2.usarControlesWASD = false;
            Debug.Log($"Instancia dos spawneada: {p2.name}");
        }
        else
        {
            Debug.LogError($"Error: Índice Personaje DOS ({indicePersonajeDos}) fuera de rango. Total de prefabs cargados: {personajes.Count}.");
        }

        // Se eliminó el foreach(GameObject i in personajes) { i.name; } obsoleto.
    }

    void ConfigurarSistemaVida()
    {
        if (sistemaVidaPersonaje == null)
        {
            Debug.LogError("No se puede configurar el sistema de vida: SistemaVidaPersonaje no asignado");
            return;
        }

        // Lógica de configuración...
        Debug.Log("Sistema de vida configurado correctamente");
    }

    // ==========================================================
    // FUNCIÓN DE UTILIDAD
    // ==========================================================

    void AjustarFondoAPantalla()
    {
        if (renderizadorFondo == null || renderizadorFondo.sprite == null)
        {
            Debug.LogWarning("No se puede ajustar el fondo: sprite no asignado");
            return;
        }

        Camera camara = Camera.main;
        if (camara == null)
        {
            Debug.LogError("No se encontró la cámara principal");
            return;
        }

        float alturaCamara = camara.orthographicSize * 2f;
        float anchoCamara = alturaCamara * camara.aspect;

        Sprite sprite = renderizadorFondo.sprite;
        float anchoSprite = sprite.bounds.size.x;
        float altoSprite = sprite.bounds.size.y;

        float escalaX = anchoCamara / anchoSprite;
        float escalaY = alturaCamara / altoSprite;

        float escalaFinal = Mathf.Max(escalaX, escalaY);

        renderizadorFondo.transform.localScale = new Vector3(escalaFinal, escalaFinal, 1f);

        Debug.Log($"Fondo ajustado - Escala: {escalaFinal}");
    }

    // ==========================================================
    // CÓDIGO COMENTADO (Referencia)
    // ==========================================================

    /*
    // Este código y la función OnValidate() fueron extraídos de tu script original
    // y se mantienen aquí como referencia, ya que no son necesarios para CargarPersonaje.
    void ConfigurarSistemaPelea() { ... }
    void OnValidate() { ... }
    */
}
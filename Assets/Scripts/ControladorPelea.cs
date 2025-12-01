using UnityEngine;

public class ControladorPelea : MonoBehaviour
{
    [Header("Sprites de Fondo de Mapas")]
    public Sprite fondoMapa0;
    public Sprite fondoMapa1;
    public Sprite fondoMapa2;
    public Sprite fondoMapa3;

    [Header("Sprites de Personajes")]
    public Sprite personaje0;
    public Sprite personaje1;
    public Sprite personaje2;
    public Sprite personaje3;

    [Header("Referencias en Escena")]
    public SpriteRenderer renderizadorFondo;
    public SpriteRenderer renderizadorJugador;
    public Transform posicionInicialJugador;

    [Header("Configuracion del Personaje")]
    public Vector3 escalaPersonaje = new Vector3(1f, 1f, 1f);
    public int ordenEnCapa = 10;

    [Header("Configuracion del Fondo")]
    public bool ajustarFondoAutomaticamente = true;

    [Header("Sistema de Vida - Referencias UI")]
    public SistemaVidaPersonaje sistemaVidaPersonaje; // Asignar en el Inspector

    [Header("Sistema de Pelea")]
    public ControladorPersonajePelea controladorPelea; // Se asignará automáticamente

    void Start()
    {
        VerificarReferencias();
        CargarMapa();
        CargarPersonaje();
        ConfigurarSistemaVida();
        ConfigurarSistemaPelea();
    }

    void VerificarReferencias()
    {
        if (renderizadorFondo == null)
        {
            Debug.LogError("¡El SpriteRenderer del fondo no está asignado!");
        }

        if (renderizadorJugador == null)
        {
            Debug.LogError("¡El SpriteRenderer del personaje no está asignado!");
        }

        if (posicionInicialJugador == null)
        {
            Debug.LogWarning("No hay posición inicial asignada. El personaje aparecerá en (0,0,0)");
        }

        if (sistemaVidaPersonaje == null)
        {
            Debug.LogError("¡SistemaVidaPersonaje no está asignado! Busca el objeto en la escena y asígnalo.");
        }
    }

    void CargarMapa()
    {
        if (renderizadorFondo == null) return;

        int indiceMapa = DatosJuego.indiceMapaSeleccionado;
        string nombreMapa = DatosJuego.mapaSeleccionado;

        Debug.Log($"Cargando mapa: {nombreMapa} (Índice: {indiceMapa})");

        switch (indiceMapa)
        {
            case 0:
                renderizadorFondo.sprite = fondoMapa0;
                Debug.Log("Fondo asignado: Mapa 0");
                break;
            case 1:
                renderizadorFondo.sprite = fondoMapa1;
                Debug.Log("Fondo asignado: Mapa 1");
                break;
            case 2:
                renderizadorFondo.sprite = fondoMapa2;
                Debug.Log("Fondo asignado: Mapa 2");
                break;
            case 3:
                renderizadorFondo.sprite = fondoMapa3;
                Debug.Log("Fondo asignado: Mapa 3");
                break;
            default:
                Debug.LogWarning($"Índice de mapa no válido: {indiceMapa}");
                break;
        }

        renderizadorFondo.sortingOrder = 0;

        if (ajustarFondoAutomaticamente)
        {
            AjustarFondoAPantalla();
        }
    }

    void CargarPersonaje()
    {
        if (renderizadorJugador == null) return;

        int indicePersonaje = DatosJuego.indicePersonajeSeleccionado;
        string nombrePersonaje = DatosJuego.personajeSeleccionado;

        Debug.Log($"Cargando personaje: {nombrePersonaje} (Índice: {indicePersonaje})");

        switch (indicePersonaje)
        {
            case 0:
                renderizadorJugador.sprite = personaje0;
                Debug.Log("Personaje asignado: Personaje 0");
                break;
            case 1:
                renderizadorJugador.sprite = personaje1;
                Debug.Log("Personaje asignado: Personaje 1");
                break;
            case 2:
                renderizadorJugador.sprite = personaje2;
                Debug.Log("Personaje asignado: Personaje 2");
                break;
            case 3:
                renderizadorJugador.sprite = personaje3;
                Debug.Log("Personaje asignado: Personaje 3");
                break;
            default:
                Debug.LogWarning($"Índice de personaje no válido: {indicePersonaje}");
                break;
        }

        renderizadorJugador.sortingOrder = ordenEnCapa;
        renderizadorJugador.transform.localScale = escalaPersonaje;

        if (posicionInicialJugador != null)
        {
            renderizadorJugador.transform.position = posicionInicialJugador.position;
            Debug.Log($"Personaje colocado en: {posicionInicialJugador.position}");
        }
        else
        {
            renderizadorJugador.transform.position = new Vector3(-3f, -2f, 0f);
            Debug.Log("Personaje colocado en posición por defecto (-3, -2, 0)");
        }

        // IMPORTANTE: Asegurarse de que el personaje tenga un Collider2D
        if (renderizadorJugador.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = renderizadorJugador.gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("Se agregó automáticamente un BoxCollider2D al personaje");
        }
    }

    void ConfigurarSistemaVida()
    {
        if (sistemaVidaPersonaje == null)
        {
            Debug.LogError("No se puede configurar el sistema de vida: SistemaVidaPersonaje no asignado");
            return;
        }

        // El sistema de vida se encargará de cargar los datos automáticamente
        // Solo necesitamos asegurarnos de que tenga las referencias correctas
        Debug.Log("Sistema de vida configurado correctamente");
    }

    void ConfigurarSistemaPelea()
    {
        if (renderizadorJugador == null)
        {
            Debug.LogError("No se puede configurar el sistema de pelea: renderizadorJugador no asignado");
            return;
        }

        // Obtener o agregar el componente ControladorPersonajePelea
        controladorPelea = renderizadorJugador.GetComponent<ControladorPersonajePelea>();
        if (controladorPelea == null)
        {
            controladorPelea = renderizadorJugador.gameObject.AddComponent<ControladorPersonajePelea>();
            Debug.Log("ControladorPersonajePelea agregado automáticamente");
        }

        // Agregar Rigidbody2D si no existe
        Rigidbody2D rb = renderizadorJugador.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = renderizadorJugador.gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2.5f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Debug.Log("Rigidbody2D agregado automáticamente");
        }

        Debug.Log("Sistema de pelea configurado correctamente");
    }

    void OnValidate()
    {
        if (renderizadorJugador != null)
        {
            renderizadorJugador.sortingOrder = ordenEnCapa;
            renderizadorJugador.transform.localScale = escalaPersonaje;
        }
    }

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
}
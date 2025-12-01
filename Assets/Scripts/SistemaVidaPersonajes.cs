using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaVidaPersonaje : MonoBehaviour
{
    [Header("Sprites de Personajes - Asignar en Inspector")]
    public Sprite[] fotosPersonajes; // Arrastra aquí los 4 sprites de personajes (índice 0,1,2,3)

    [Header("Nombres de Personajes - Asignar en Inspector")]
    public string[] nombresPersonajes = new string[]
    {
        "Personaje 1",
        "Personaje 2",
        "Personaje 3",
        "Personaje 4"
    };

    [Header("Configuración de Vida")]
    public float vidaMaxima = 100f;

    // Variables privadas que se cargarán automáticamente
    private string nombrePersonaje;
    private Sprite fotoPersonaje;

    [Header("UI - Barra de Vida")]
    public Image barraVidaFill; // La imagen de la barra (debe tener Image Type: Filled)
    public TextMeshProUGUI textoVida; // Texto que muestra "100/100"
    public TextMeshProUGUI textoNombre; // Nombre del personaje
    public Image imagenPersonaje; // Foto del personaje

    [Header("Configuración de Daño")]
    public float dañoPorClick = 10f; // Cuánto daño hace cada click

    [Header("Efectos Visuales")]
    public Color colorVidaAlta = Color.green;
    public Color colorVidaMedia = Color.yellow;
    public Color colorVidaBaja = Color.red;

    [Header("Animación de la Barra")]
    public float velocidadAnimacion = 0.5f; // Qué tan rápido baja la barra (más alto = más rápido)
    public bool usarAnimacionSuave = true; // Activar/desactivar la animación

    private float vidaActual;
    private bool estaMuerto = false;
    private SpriteRenderer spritePersonaje;

    // Variables para la animación suave de la barra
    private float vidaObjetivo; // La vida a la que debe llegar la barra
    private float vidaMostrada; // La vida que se está mostrando actualmente en la barra

    void Start()
    {
        // Inicializar vida
        vidaActual = vidaMaxima;
        vidaObjetivo = vidaMaxima;
        vidaMostrada = vidaMaxima; // Iniciar con la barra llena

        // Obtener el SpriteRenderer del personaje
        spritePersonaje = GetComponent<SpriteRenderer>();

        // Verificar que la barra esté configurada correctamente
        if (barraVidaFill != null)
        {
            if (barraVidaFill.type != Image.Type.Filled)
            {
                Debug.LogError("¡IMPORTANTE! BarraVidaFill debe tener Image Type configurado como 'Filled'");
            }
            else
            {
                Debug.Log("Barra de vida configurada correctamente con Image Type: Filled");
            }
        }
        else
        {
            Debug.LogError("¡BarraVidaFill no está asignada en el Inspector!");
        }

        // Cargar datos del personaje seleccionado
        CargarDatosPersonaje();

        // Actualizar UI inicial
        ActualizarUI();
    }

    void CargarDatosPersonaje()
    {
        // Obtener el índice del personaje seleccionado
        int indicePersonaje = DatosJuego.indicePersonajeSeleccionado;

        Debug.Log($"Cargando datos del personaje con índice: {indicePersonaje}");

        // Cargar el nombre del personaje
        if (nombresPersonajes != null && indicePersonaje >= 0 && indicePersonaje < nombresPersonajes.Length)
        {
            nombrePersonaje = nombresPersonajes[indicePersonaje];
        }
        else
        {
            nombrePersonaje = DatosJuego.personajeSeleccionado ?? "Personaje Desconocido";
            Debug.LogWarning($"No se pudo cargar el nombre del personaje. Usando: {nombrePersonaje}");
        }

        // Cargar la foto del personaje
        if (fotosPersonajes != null && indicePersonaje >= 0 && indicePersonaje < fotosPersonajes.Length)
        {
            fotoPersonaje = fotosPersonajes[indicePersonaje];
        }
        else if (spritePersonaje != null)
        {
            // Si no hay foto en el array, usar el sprite del personaje en la escena
            fotoPersonaje = spritePersonaje.sprite;
            Debug.LogWarning("No se encontró foto en el array. Usando sprite del personaje.");
        }

        // Actualizar el nombre en la UI
        if (textoNombre != null)
        {
            textoNombre.text = nombrePersonaje;
            Debug.Log($"Nombre mostrado en UI: {nombrePersonaje}");
        }
        else
        {
            Debug.LogError("¡TextoNombre no está asignado en el Inspector!");
        }

        // Actualizar la imagen en la UI
        if (imagenPersonaje != null && fotoPersonaje != null)
        {
            imagenPersonaje.sprite = fotoPersonaje;
            Debug.Log("Foto del personaje cargada en la UI");
        }
        else
        {
            if (imagenPersonaje == null)
                Debug.LogError("¡ImagenPersonaje no está asignada en el Inspector!");
            if (fotoPersonaje == null)
                Debug.LogError("¡No se pudo cargar la foto del personaje!");
        }
    }

    void Update()
    {
        // Si el personaje está muerto, no hacer nada
        if (estaMuerto) return;

        // Animar la barra de vida suavemente
        if (usarAnimacionSuave && Mathf.Abs(vidaMostrada - vidaObjetivo) > 0.01f)
        {
            // Interpolar suavemente entre la vida mostrada y la vida objetivo
            vidaMostrada = Mathf.Lerp(vidaMostrada, vidaObjetivo, velocidadAnimacion * Time.deltaTime * 10f);
            ActualizarBarraVisual();
        }

        // Detectar click sobre el personaje
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            DetectarClickEnPersonaje();
        }
    }

    void DetectarClickEnPersonaje()
    {
        // Convertir la posición del mouse a coordenadas del mundo
        Vector2 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Hacer un raycast para ver si clickeamos el personaje
        RaycastHit2D hit = Physics2D.Raycast(posicionMouse, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            RecibirDano(dañoPorClick);
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (estaMuerto) return;

        // Reducir vida
        vidaActual -= cantidad;
        vidaActual = Mathf.Max(vidaActual, 0); // No puede ser menor a 0

        // Establecer el objetivo para la animación
        vidaObjetivo = vidaActual;

        // Si no se usa animación suave, actualizar inmediatamente
        if (!usarAnimacionSuave)
        {
            vidaMostrada = vidaActual;
            ActualizarUI();
        }
        else
        {
            // Solo actualizar el texto, la barra se animará en Update()
            ActualizarTextoVida();
        }

        // Efecto visual de daño
        StartCoroutine(EfectoDaño());

        // Reproducir sonido (opcional)
        // ControladorSonidos.ObtenerInstancia()?.SonidoDaño();

        Debug.Log($"{nombrePersonaje} recibió {cantidad} de daño. Vida restante: {vidaActual}/{vidaMaxima}");

        // Verificar si murió
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    // Alias del método con ñ para compatibilidad
    public void RecibirDaño(float cantidad)
    {
        RecibirDano(cantidad);
    }

    void ActualizarUI()
    {
        ActualizarBarraVisual();
        ActualizarTextoVida();
    }

    void ActualizarBarraVisual()
    {
        if (barraVidaFill != null)
        {
            // Calcular el porcentaje de vida mostrada (0 a 1)
            float porcentajeVida = vidaMostrada / vidaMaxima;

            // Actualizar el fillAmount (este es el método correcto para Image Type: Filled)
            barraVidaFill.fillAmount = porcentajeVida;

            // Cambiar color según la vida restante (usar vidaActual, no vidaMostrada)
            float porcentajeVidaReal = vidaActual / vidaMaxima;
            if (porcentajeVidaReal > 0.6f)
            {
                barraVidaFill.color = colorVidaAlta;
            }
            else if (porcentajeVidaReal > 0.3f)
            {
                barraVidaFill.color = colorVidaMedia;
            }
            else
            {
                barraVidaFill.color = colorVidaBaja;
            }
        }
        else
        {
            Debug.LogError("¡BarraVidaFill no está asignada!");
        }
    }

    void ActualizarTextoVida()
    {
        if (textoVida != null)
        {
            // Mostrar vida actual/máxima (usar vidaActual, no vidaMostrada)
            textoVida.text = $"{Mathf.CeilToInt(vidaActual)}/{Mathf.CeilToInt(vidaMaxima)}";
        }
        else
        {
            Debug.LogError("¡TextoVida no está asignado!");
        }
    }

    System.Collections.IEnumerator EfectoDaño()
    {
        // Efecto de parpadeo rojo cuando recibe daño
        if (spritePersonaje != null)
        {
            Color colorOriginal = spritePersonaje.color;
            spritePersonaje.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spritePersonaje.color = colorOriginal;
        }
    }

    void Morir()
    {
        estaMuerto = true;

        Debug.Log($"{nombrePersonaje} ha sido derrotado!");

        // Notificar al gestor de juego que el personaje murió
        GestorFinJuego gestorFin = FindObjectOfType<GestorFinJuego>();
        if (gestorFin != null)
        {
            gestorFin.PersonajeDerrotado(nombrePersonaje);
        }
        else
        {
            Debug.LogWarning("No se encontró GestorFinJuego en la escena");
        }

        // Efecto visual de muerte
        StartCoroutine(EfectoMuerte());
    }

    System.Collections.IEnumerator EfectoMuerte()
    {
        // Hacer que el personaje parpadee y se desvanezca
        if (spritePersonaje != null)
        {
            for (int i = 0; i < 5; i++)
            {
                spritePersonaje.enabled = false;
                yield return new WaitForSeconds(0.1f);
                spritePersonaje.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }

            // Desvanecer gradualmente
            float alpha = 1f;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * 2f;
                Color color = spritePersonaje.color;
                color.a = alpha;
                spritePersonaje.color = color;
                yield return null;
            }
        }
    }

    // Métodos públicos útiles
    public void Curar(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual += cantidad;
        vidaActual = Mathf.Min(vidaActual, vidaMaxima); // No puede exceder la vida máxima
        vidaObjetivo = vidaActual;

        if (!usarAnimacionSuave)
        {
            vidaMostrada = vidaActual;
            ActualizarUI();
        }
        else
        {
            ActualizarTextoVida();
        }

        Debug.Log($"{nombrePersonaje} curado por {cantidad}. Vida actual: {vidaActual}/{vidaMaxima}");
    }

    public void EstablecerVidaMaxima(float nuevaVidaMaxima)
    {
        vidaMaxima = nuevaVidaMaxima;
        vidaActual = vidaMaxima;
        vidaObjetivo = vidaMaxima;
        vidaMostrada = vidaMaxima;
        ActualizarUI();
    }

    public bool EstaMuerto()
    {
        return estaMuerto;
    }

    public float ObtenerVidaActual()
    {
        return vidaActual;
    }

    public float ObtenerPorcentajeVida()
    {
        return vidaActual / vidaMaxima;
    }
}
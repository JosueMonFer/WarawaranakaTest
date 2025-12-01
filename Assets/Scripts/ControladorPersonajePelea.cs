using UnityEngine;

public class ControladorPersonajePelea : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    public float fuerzaSalto = 12f;
    public float gravedadMultiplicador = 2.5f;

    [Header("Límites de Pantalla")]
    public bool restringirAPantalla = true;
    public float margenLimite = 0.5f; // Margen adicional desde los bordes

    [Header("Detección de Suelo")]
    public Transform puntoDeteccionSuelo;
    public float radioDeteccionSuelo = 0.2f;
    public LayerMask capaSuelo;

    // Clase para organizar los sprites de cada personaje
    [System.Serializable]
    public class SpritesPersonaje
    {
        public Sprite spriteIdle;
        public Sprite[] spritesCorrer;
        public Sprite spriteSalto;
        public Sprite spriteAtaque1;
        public Sprite spriteAtaque2;
        public Sprite spriteAtaque3;
        public Sprite spriteEspecial1;
        public Sprite spriteEspecial2;
    }

    [Header("Sprites por Personaje (Índices 0-3)")]
    public SpritesPersonaje[] spritesPersonajes = new SpritesPersonaje[4];

    [Header("Configuración de Ataques")]
    public float duracionAtaque = 0.3f;
    public float dañoAtaque1 = 10f;
    public float dañoAtaque2 = 15f;
    public float dañoAtaque3 = 20f;
    public float dañoEspecial1 = 30f;
    public float dañoEspecial2 = 35f;

    [Header("Detector de Golpes")]
    public DetectorGolpes detectorGolpes;

    [Header("Configuración de Animación")]
    public float velocidadAnimacionCorrer = 0.1f;

    // Variables privadas de sprites actuales
    private Sprite spriteIdle;
    private Sprite[] spritesCorrer;
    private Sprite spriteSalto;
    private Sprite spriteAtaque1;
    private Sprite spriteAtaque2;
    private Sprite spriteAtaque3;
    private Sprite spriteEspecial1;
    private Sprite spriteEspecial2;

    // Componentes
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisionador;

    // Variables de estado
    private bool enSuelo;
    private bool estaAtacando;
    private float movimientoHorizontal;
    private bool mirandoDerecha = true;

    // Variables de animación
    private int indiceAnimacionCorrer = 0;
    private float tiempoAnimacion = 0f;
    private float tiempoAtaque = 0f;

    // Límites de la pantalla
    private float limiteIzquierdo;
    private float limiteDerecho;
    private float limiteInferior;
    private float limiteSuperior;
    private Camera camaraPrincipal;

    void Start()
    {
        // Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisionador = GetComponent<Collider2D>();
        camaraPrincipal = Camera.main;

        // Configurar Rigidbody2D
        if (rb != null)
        {
            rb.gravityScale = gravedadMultiplicador;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // Verificar componentes
        if (spriteRenderer == null)
        {
            Debug.LogError("¡No se encontró SpriteRenderer en el personaje!");
        }

        if (puntoDeteccionSuelo == null)
        {
            Debug.LogWarning("No hay punto de detección de suelo asignado. Creando uno automáticamente.");
            CrearPuntoDeteccionSuelo();
        }

        // Calcular límites de la pantalla
        CalcularLimitesPantalla();

        // Cargar sprites del personaje
        CargarSpritesPersonaje();

        Debug.Log("ControladorPersonajePelea inicializado");
    }

    void CalcularLimitesPantalla()
    {
        if (camaraPrincipal == null)
        {
            Debug.LogError("No se encontró la cámara principal");
            return;
        }

        // Calcular los límites en coordenadas del mundo
        float alturaCamara = camaraPrincipal.orthographicSize;
        float anchoCamara = alturaCamara * camaraPrincipal.aspect;

        Vector3 posicionCamara = camaraPrincipal.transform.position;

        limiteIzquierdo = posicionCamara.x - anchoCamara + margenLimite;
        limiteDerecho = posicionCamara.x + anchoCamara - margenLimite;
        limiteInferior = posicionCamara.y - alturaCamara + margenLimite;
        limiteSuperior = posicionCamara.y + alturaCamara - margenLimite;

        Debug.Log($"Límites de pantalla calculados - Izq: {limiteIzquierdo}, Der: {limiteDerecho}, Inf: {limiteInferior}, Sup: {limiteSuperior}");
    }

    void CargarSpritesPersonaje()
    {
        int indicePersonaje = DatosJuego.indicePersonajeSeleccionado;

        Debug.Log($"Cargando sprites para personaje índice: {indicePersonaje}");

        if (indicePersonaje < 0 || indicePersonaje >= spritesPersonajes.Length)
        {
            Debug.LogError($"Índice de personaje inválido: {indicePersonaje}");
            return;
        }

        if (spritesPersonajes[indicePersonaje] == null)
        {
            Debug.LogError($"No hay sprites asignados para el personaje {indicePersonaje}");
            return;
        }

        SpritesPersonaje sprites = spritesPersonajes[indicePersonaje];

        spriteIdle = sprites.spriteIdle;
        spritesCorrer = sprites.spritesCorrer;
        spriteSalto = sprites.spriteSalto;
        spriteAtaque1 = sprites.spriteAtaque1;
        spriteAtaque2 = sprites.spriteAtaque2;
        spriteAtaque3 = sprites.spriteAtaque3;
        spriteEspecial1 = sprites.spriteEspecial1;
        spriteEspecial2 = sprites.spriteEspecial2;

        Debug.Log($"Sprites cargados para {DatosJuego.personajeSeleccionado}");

        if (spriteIdle != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteIdle;
        }
    }

    void Update()
    {
        // No permitir acciones si está muerto
        SistemaVidaPersonaje sistemaVida = GetComponent<SistemaVidaPersonaje>();
        if (sistemaVida != null && sistemaVida.EstaMuerto())
        {
            return;
        }

        // Obtener input de movimiento
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        // Detectar si está en el suelo
        DetectarSuelo();

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo && !estaAtacando)
        {
            Saltar();
        }

        // Ataques
        if (!estaAtacando)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Atacar(1);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                Atacar(2);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Atacar(3);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                AtaqueEspecial(1);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                AtaqueEspecial(2);
            }
        }

        // Actualizar animación
        ActualizarAnimacion();

        // Voltear sprite según dirección
        if (movimientoHorizontal > 0 && !mirandoDerecha)
        {
            Voltear();
        }
        else if (movimientoHorizontal < 0 && mirandoDerecha)
        {
            Voltear();
        }
    }

    void FixedUpdate()
    {
        // Mover personaje
        if (!estaAtacando)
        {
            Mover();
        }

        // Restringir a los límites de la pantalla
        if (restringirAPantalla)
        {
            RestringirAPantalla();
        }
    }

    void Mover()
    {
        // Aplicar movimiento horizontal
        Vector2 velocidad = rb.linearVelocity;
        velocidad.x = movimientoHorizontal * velocidadMovimiento;
        rb.linearVelocity = velocidad;
    }

    void RestringirAPantalla()
    {
        // Obtener la posición actual
        Vector3 posicion = transform.position;

        // Obtener el tamaño del sprite para ajustar mejor los límites
        float anchoSprite = 0.5f;
        float altoSprite = 0.5f;

        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            anchoSprite = spriteRenderer.bounds.extents.x;
            altoSprite = spriteRenderer.bounds.extents.y;
        }

        // Restringir en el eje X (horizontal)
        if (posicion.x - anchoSprite < limiteIzquierdo)
        {
            posicion.x = limiteIzquierdo + anchoSprite;
            // Detener el movimiento horizontal
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (posicion.x + anchoSprite > limiteDerecho)
        {
            posicion.x = limiteDerecho - anchoSprite;
            // Detener el movimiento horizontal
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Restringir en el eje Y (vertical) - opcional, depende de tu juego
        if (posicion.y - altoSprite < limiteInferior)
        {
            posicion.y = limiteInferior + altoSprite;
            // Detener el movimiento vertical hacia abajo
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
        }
        else if (posicion.y + altoSprite > limiteSuperior)
        {
            posicion.y = limiteSuperior - altoSprite;
            // Detener el movimiento vertical hacia arriba
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
        }

        // Aplicar la posición corregida
        transform.position = posicion;
    }

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        Debug.Log("¡Salto!");
    }

    void DetectarSuelo()
    {
        if (puntoDeteccionSuelo == null)
        {
            enSuelo = false;
            return;
        }

        enSuelo = Physics2D.OverlapCircle(puntoDeteccionSuelo.position, radioDeteccionSuelo, capaSuelo);
    }

    void Atacar(int numeroAtaque)
    {
        estaAtacando = true;
        tiempoAtaque = duracionAtaque;

        float dañoAtaque = 0f;

        switch (numeroAtaque)
        {
            case 1:
                if (spriteAtaque1 != null)
                    spriteRenderer.sprite = spriteAtaque1;
                dañoAtaque = dañoAtaque1;
                Debug.Log($"¡Ataque 1! Daño: {dañoAtaque1}");
                break;
            case 2:
                if (spriteAtaque2 != null)
                    spriteRenderer.sprite = spriteAtaque2;
                dañoAtaque = dañoAtaque2;
                Debug.Log($"¡Ataque 2! Daño: {dañoAtaque2}");
                break;
            case 3:
                if (spriteAtaque3 != null)
                    spriteRenderer.sprite = spriteAtaque3;
                dañoAtaque = dañoAtaque3;
                Debug.Log($"¡Ataque 3! Daño: {dañoAtaque3}");
                break;
        }

        if (detectorGolpes != null)
        {
            detectorGolpes.EjecutarAtaque(dañoAtaque);
        }
        else
        {
            Debug.LogWarning("No hay DetectorGolpes asignado");
        }
    }

    void AtaqueEspecial(int numeroEspecial)
    {
        estaAtacando = true;
        tiempoAtaque = duracionAtaque * 1.5f;

        float dañoEspecial = 0f;

        switch (numeroEspecial)
        {
            case 1:
                if (spriteEspecial1 != null)
                    spriteRenderer.sprite = spriteEspecial1;
                dañoEspecial = dañoEspecial1;
                Debug.Log($"¡ESPECIAL 1! Daño: {dañoEspecial1}");
                break;
            case 2:
                if (spriteEspecial2 != null)
                    spriteRenderer.sprite = spriteEspecial2;
                dañoEspecial = dañoEspecial2;
                Debug.Log($"¡ESPECIAL 2! Daño: {dañoEspecial2}");
                break;
        }

        if (detectorGolpes != null)
        {
            detectorGolpes.EjecutarAtaque(dañoEspecial);
        }
    }

    void ActualizarAnimacion()
    {
        if (estaAtacando)
        {
            tiempoAtaque -= Time.deltaTime;
            if (tiempoAtaque <= 0)
            {
                estaAtacando = false;
            }
            return;
        }

        if (!enSuelo)
        {
            if (spriteSalto != null)
                spriteRenderer.sprite = spriteSalto;
        }
        else if (Mathf.Abs(movimientoHorizontal) > 0.01f)
        {
            if (spritesCorrer != null && spritesCorrer.Length > 0)
            {
                tiempoAnimacion += Time.deltaTime;
                if (tiempoAnimacion >= velocidadAnimacionCorrer)
                {
                    tiempoAnimacion = 0f;
                    indiceAnimacionCorrer = (indiceAnimacionCorrer + 1) % spritesCorrer.Length;
                    spriteRenderer.sprite = spritesCorrer[indiceAnimacionCorrer];
                }
            }
        }
        else
        {
            if (spriteIdle != null)
            {
                spriteRenderer.sprite = spriteIdle;
                indiceAnimacionCorrer = 0;
                tiempoAnimacion = 0f;
            }
        }
    }

    void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void CrearPuntoDeteccionSuelo()
    {
        GameObject punto = new GameObject("PuntoDeteccionSuelo");
        punto.transform.parent = transform;
        punto.transform.localPosition = new Vector3(0, -1f, 0);
        puntoDeteccionSuelo = punto.transform;
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar punto de detección de suelo
        if (puntoDeteccionSuelo != null)
        {
            Gizmos.color = enSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(puntoDeteccionSuelo.position, radioDeteccionSuelo);
        }

        // Dibujar límites de la pantalla (solo en modo Play)
        if (Application.isPlaying && restringirAPantalla)
        {
            Gizmos.color = Color.yellow;

            // Dibujar rectángulo de límites
            Vector3 esquinaInfIzq = new Vector3(limiteIzquierdo, limiteInferior, 0);
            Vector3 esquinaInfDer = new Vector3(limiteDerecho, limiteInferior, 0);
            Vector3 esquinaSupDer = new Vector3(limiteDerecho, limiteSuperior, 0);
            Vector3 esquinaSupIzq = new Vector3(limiteIzquierdo, limiteSuperior, 0);

            Gizmos.DrawLine(esquinaInfIzq, esquinaInfDer);
            Gizmos.DrawLine(esquinaInfDer, esquinaSupDer);
            Gizmos.DrawLine(esquinaSupDer, esquinaSupIzq);
            Gizmos.DrawLine(esquinaSupIzq, esquinaInfIzq);
        }
    }

    // Métodos públicos
    public bool EstaEnSuelo()
    {
        return enSuelo;
    }

    public bool EstaAtacando()
    {
        return estaAtacando;
    }

    public bool EstaMirandoDerecha()
    {
        return mirandoDerecha;
    }
}
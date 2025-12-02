using UnityEngine;

public class EstrellaCaidaMinijuego1 : MonoBehaviour
{
    [Header("Configuracion")]
    public float velocidadCaida = 3f;
    public float velocidadRotacion = 100f;
    public float limiteBajo = -6f;

    [Header("Efectos (Opcional)")]
    public GameObject efectoParticulas;

    void Update()
    {
        // Caer hacia abajo
        transform.Translate(Vector3.down * velocidadCaida * Time.deltaTime, Space.World);

        // Rotar para efecto visual
        transform.Rotate(Vector3.forward * velocidadRotacion * Time.deltaTime);

        // Destruir si sale de la pantalla
        if (transform.position.y < limiteBajo)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Crear efecto de partículas al ser atrapada
        if (efectoParticulas != null && other.CompareTag("Player"))
        {
            Instantiate(efectoParticulas, transform.position, Quaternion.identity);
        }
    }
}
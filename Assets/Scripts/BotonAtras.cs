using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonAtras : MonoBehaviour
{
    [Header("Configuracion")]
    public string escenaAnterior; // Nombre de la escena a la que regresar

    private Button boton;

    void Start()
    {
        boton = GetComponent<Button>();
        
        if (boton != null)
        {
            boton.onClick.AddListener(RegresarEscenaAnterior);
        }
        else
        {
            Debug.LogError("BotonAtras: No se encontro el componente Button!");
        }
    }

    public void RegresarEscenaAnterior()
    {
        // Reproducir sonido
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();

        // Verificar que hay una escena configurada
        if (!string.IsNullOrEmpty(escenaAnterior))
        {
            Debug.Log("Regresando a: " + escenaAnterior);
            SceneManager.LoadScene(escenaAnterior);
        }
        else
        {
            Debug.LogWarning("BotonAtras: No se ha configurado la escena anterior!");
        }
    }

    // Metodo alternativo: regresar usando el BuildIndex
    public void RegresarEscenaAnteriorPorIndice()
    {
        ControladorSonidos.ObtenerInstancia()?.SonidoBotonAtras();
        
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        if (escenaActual > 0)
        {
            SceneManager.LoadScene(escenaActual - 1);
        }
        else
        {
            Debug.LogWarning("BotonAtras: Ya estas en la primera escena!");
        }
    }
}
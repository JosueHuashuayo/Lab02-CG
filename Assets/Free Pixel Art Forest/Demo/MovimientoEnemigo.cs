using System.Collections;
using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    [SerializeField] private float minVelocidad = 1f; // Velocidad mínima de movimiento del enemigo
    [SerializeField] private float maxVelocidad = 3f; // Velocidad máxima de movimiento del enemigo
    [SerializeField] private float cambioDireccionMin = 2f; // Tiempo mínimo para cambiar la dirección
    [SerializeField] private float cambioDireccionMax = 5f; // Tiempo máximo para cambiar la dirección

    private Animator animator;
    private bool moving = true; // Variable para controlar el movimiento
    private Vector2 initialPosition; // Posición inicial del enemigo
    private float velocidadActual; // Velocidad actual de movimiento del enemigo
    private float tiempoParaCambioDireccion; // Tiempo restante para cambiar la dirección
    private bool movingRight = true; // Variable para controlar la dirección de movimiento
    public bool isAlive = true;
    void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        velocidadActual = Random.Range(minVelocidad, maxVelocidad); // Velocidad inicial aleatoria
        tiempoParaCambioDireccion = Random.Range(cambioDireccionMin, cambioDireccionMax); // Tiempo inicial aleatorio para cambiar dirección
    }

    void Update()
    {
        if (moving)
        {
            // Mueve al enemigo en la dirección actual
            if (movingRight)
                transform.Translate(Vector3.right * velocidadActual * Time.deltaTime);
            else
                transform.Translate(Vector3.left * velocidadActual * Time.deltaTime);

            // Actualiza el tiempo para cambiar de dirección
            tiempoParaCambioDireccion -= Time.deltaTime;
            if (tiempoParaCambioDireccion <= 0)
            {
                CambiarDireccion();
                tiempoParaCambioDireccion = Random.Range(cambioDireccionMin, cambioDireccionMax); // Reinicia el tiempo para cambiar dirección
            }
        }
    }

    // Método para cambiar la dirección de movimiento del enemigo
    private void CambiarDireccion()
    {
        movingRight = !movingRight; // Cambia la dirección
        // Cambia la orientación del sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

   
   
    public void killEnemy()
    {
        isAlive = false;
        animator.SetTrigger("Death");
        moving = false;
        StartCoroutine(FlyAway());
       /* Destroy(gameObject, 0.3f);*/ // Destruye el enemigo después de la animación de muerte
        GameSingleton.Instance.IncreaseScore(1);
    }
    private IEnumerator FlyAway()
    {
        float timer = 0f;
        while (timer < 1f) // Controla el tiempo de vuelo
        {
            transform.Translate(Vector3.up * Time.deltaTime * 3f); // Mueve hacia arriba
            transform.Rotate(Vector3.forward * Time.deltaTime * 360f); // Da vueltas
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject); // Destruye el enemigo al finalizar el vuelo
    }
}
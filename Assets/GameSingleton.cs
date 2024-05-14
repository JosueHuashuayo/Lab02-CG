using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    // Instancia est�tica del singleton
    private static GameSingleton instance;

    // Variables que quieres almacenar globalmente
    public bool gameOver=false;
    public int playerScore = 0;
    public bool isPaused = false;
    // Referencia al jugador HeroKnight
    [SerializeField]
    private HeroKnight playerHero;

    public int PlayerLife
    {
        get { return playerHero != null ? playerHero.m_vidas : 0; }
    }

    public float gameStartTime; // Momento en que se inici� el juego

    // Tiempo transcurrido desde el inicio del juego
    public float GameTime => Time.time - gameStartTime;
    

    // M�todo est�tico para acceder a la instancia del singleton
    public static GameSingleton Instance
    {
        get
        {
            // Si no hay una instancia, se intenta encontrar una en la escena
            if (instance == null)
            {
                instance = FindObjectOfType<GameSingleton>();

                // Si no se encuentra en la escena, se crea una nueva
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameSingleton).Name);
                    instance = singletonObject.AddComponent<GameSingleton>();
                }
            }
            return instance;
        }

    }

    // M�todo opcional para inicializar variables u otras configuraciones
    private void Awake()
    {
        // Si hay m�s de una instancia del singleton en la escena, se destruye la nueva
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // El objeto no se destruye al cargar una nueva escena
        }
    }

    // M�todo para pausar o reanudar el juego
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f; // Pausa el tiempo
            Debug.Log("Juego pausado");
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el tiempo
            Debug.Log("Juego reanudado");
        }
    }

    // M�todo para aumentar el puntaje
    public void IncreaseScore(int amount)
    {
        playerScore += amount;
        Debug.Log("Puntaje aumentado: " + playerScore);
    }

    // M�todo para reducir la vida
    // M�todo para reducir la vida
    public void ReduceLife(int amount)
    {
        if (playerHero != null)
        {
            playerHero.m_vidas -= amount;
            Debug.Log("Vida reducida: " + PlayerLife);

            // Verificar si el jugador se qued� sin vidas
            if (PlayerLife <= 0)
            {
                playerHero.m_vidas = 0;
                GameOver();
            }
        }
        else
        {
            Debug.LogWarning("PlayerHero no encontrado.");
        }
    }

    // M�todo para aumentar la vida
    public void IncreaseLife(int amount)
    {
        if (playerHero != null)
        {
            playerHero.m_vidas += amount;
            Debug.Log("Vida aumentada: " + PlayerLife);
        }
        else
        {
            Debug.LogWarning("PlayerHero no encontrado.");
        }
    }

    // M�todo para manejar el juego cuando el jugador se queda sin vidas
    private void GameOver()
    {
        Debug.Log("Game Over");
        gameOver = true;
        // Aqu� puedes a�adir l�gica adicional para mostrar una pantalla de Game Over o reiniciar el juego
    }
}
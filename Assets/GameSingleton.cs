using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSingleton : MonoBehaviour
{
    // Instancia estática del singleton
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

    public float gameStartTime; // Momento en que se inició el juego

    // Tiempo transcurrido desde el inicio del juego
    public float GameTime => Time.time - gameStartTime;
    

    // Método estático para acceder a la instancia del singleton
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

    // Método opcional para inicializar variables u otras configuraciones
    private void Awake()
    {
        // Si hay más de una instancia del singleton en la escena, se destruye la nueva
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

    // Método para pausar o reanudar el juego
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

    // Método para aumentar el puntaje
    public void IncreaseScore(int amount)
    {
        playerScore += amount;
        Debug.Log("Puntaje aumentado: " + playerScore);
    }

    // Método para reducir la vida
    // Método para reducir la vida
    public void ReduceLife(int amount)
    {
        if (playerHero != null)
        {
            playerHero.m_vidas -= amount;
            Debug.Log("Vida reducida: " + PlayerLife);

            // Verificar si el jugador se quedó sin vidas
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

    // Método para aumentar la vida
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

    // Método para manejar el juego cuando el jugador se queda sin vidas
    private void GameOver()
    {
        Debug.Log("Game Over");
        gameOver = true;
        // Aquí puedes añadir lógica adicional para mostrar una pantalla de Game Over o reiniciar el juego
    }
}
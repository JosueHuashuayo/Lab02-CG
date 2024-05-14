using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI vidaText;
    public TextMeshProUGUI puntajeText;
    public TextMeshProUGUI puntajeFinalText;

    // Referencia al singleton GameSingleton
    private GameSingleton gameSingleton;

    void Start()
    {
        // Obtener la instancia de GameSingleton
        gameSingleton = GameSingleton.Instance;

        // Actualizar el texto de vida y puntaje al iniciar
        ActualizarVida();
        ActualizarPuntaje();
    }

    void Update()
    {
        if (gameSingleton.gameOver) puntajeFinalText.gameObject.SetActive(true);
        // Puedes actualizar el UI en cada frame si lo necesitas
        ActualizarVida();
        ActualizarPuntaje();
    }

    // Método para actualizar el texto de vida
    private void ActualizarVida()
    {
        vidaText.text = "Vida: " + gameSingleton.PlayerLife.ToString();
    }

    // Método para actualizar el texto de puntaje
    private void ActualizarPuntaje()
    {
        puntajeText.text = "Puntaje: " + gameSingleton.playerScore.ToString();
        puntajeFinalText.text = gameSingleton.playerScore.ToString(); 
    }
}
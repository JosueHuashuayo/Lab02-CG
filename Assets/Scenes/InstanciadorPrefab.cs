using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciadorPrefab : MonoBehaviour
{
    public GameObject[] prefabs; // Lista de prefabs
    public float distancia = 10f; // Distancia desde el centro hacia la izquierda o derecha
    public float intervaloMin = 1f; // Intervalo m�nimo entre instanciaciones
    public float intervaloMax = 3f; // Intervalo m�ximo entre instanciaciones
    public float desplazamientoHorizontalMin = -1f; // Desplazamiento m�nimo horizontal respecto a la posici�n X
    public float desplazamientoHorizontalMax = 1f; // Desplazamiento m�ximo horizontal respecto a la posici�n X

    void Start()
    {
        // Inicia la corrutina para instanciar los prefabs aleatoriamente
        StartCoroutine(InstanciarPrefabAleatorio());
    }

    IEnumerator InstanciarPrefabAleatorio()
    {
        while (true)
        {
            // Instancia un prefab aleatorio
            GameObject prefabInstanciado = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);

            // Determina la posici�n Y del prefab bas�ndose en su posici�n en la jerarqu�a
            float alturaPrefab = prefabInstanciado.transform.position.y;

            // Determina la posici�n X aleatoriamente hacia la izquierda o derecha
            
            float invert = Random.Range(-1f, 1f) > 0 ? -1f : 1f;
            float posX = invert * distancia;

            // Determina el desplazamiento horizontal aleatorio
            float desplazamientoHorizontal = Random.Range(desplazamientoHorizontalMin, desplazamientoHorizontalMax);

            // Aplica la posici�n
            prefabInstanciado.transform.position =  new Vector3(posX + transform.position.x + desplazamientoHorizontal, alturaPrefab);


            // Espera un tiempo aleatorio antes de la pr�xima instanciaci�n
            yield return new WaitForSeconds(Random.Range(intervaloMin, intervaloMax));
        }
    }
}
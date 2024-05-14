using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RepeatBackground : MonoBehaviour
{
        private GameObject prefab; // Prefab del objeto a instanciar
        private GameObject instanciaActual; // Referencia al objeto instanciado actualmente

        private Camera camaraPrincipal;
        private float anchoSprite;

        void Start()
        {
            camaraPrincipal = Camera.main;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            anchoSprite = spriteRenderer.bounds.size.x;
            prefab = gameObject; // El propio objeto se convierte en su prefab
            //Debug.Log("Ancho sprite: " + anchoSprite);
            transform.localScale = new Vector3(1.05f, transform.localScale.y, transform.localScale.z);
        }

        void Update()
        {
            // Si la cámara sobrepasa el límite derecho del sprite
            if (camaraPrincipal.transform.position.x > transform.position.x + anchoSprite)
            {
                Debug.Log("Instanciar derecha");
                // Si no hay instancia actual, o si la instancia actual está a más de dos veces el ancho del sprite hacia la derecha, instanciar
                if (instanciaActual == null || instanciaActual.transform.position.x < transform.position.x)
                {
                    InstanciarDerecha();
                    Destroy(gameObject);
                }
            }
            // Si la cámara sobrepasa el límite izquierdo del sprite
            else if (camaraPrincipal.transform.position.x < transform.position.x - anchoSprite)
            {
                Debug.Log("Instanciar izquierda");
                // Si no hay instancia actual, o si la instancia actual está a más de dos veces el ancho del sprite hacia la izquierda, instanciar
                if (instanciaActual == null || instanciaActual.transform.position.x > transform.position.x)
                {
                    InstanciarIzquierda();
                    Destroy(gameObject);
                }
            }

            // Si la instancia actual sobrepasa la distancia de su ancho más allá de alguno de sus límites, destruirla
            if (instanciaActual != null && (instanciaActual.transform.position.x > camaraPrincipal.transform.position.x + 2 * anchoSprite ||
                                             instanciaActual.transform.position.x < camaraPrincipal.transform.position.x - 2 * anchoSprite))
            {
                Destroy(instanciaActual);
            }
        }

        void InstanciarDerecha()
        {
            if (instanciaActual != null)
            {
                Destroy(instanciaActual); // Destruir instancia actual si existe
            }
            transform.localScale = new Vector3(1f, 1f, 1f);
            Vector3 posicionInstancia = transform.position + new Vector3((anchoSprite * 2f) , 0f, 0f);
            instanciaActual = Instantiate(prefab, posicionInstancia, Quaternion.identity);
        }

        void InstanciarIzquierda()
        {
            if (instanciaActual != null)
            {
                Destroy(instanciaActual); // Destruir instancia actual si existe
            }
            transform.localScale = new Vector3(1f, 1f, 1f);

            Vector3 posicionInstancia = transform.position + new Vector3((-anchoSprite * 2f) , 0f, 0f);
            instanciaActual = Instantiate(prefab, posicionInstancia, Quaternion.identity);
        }
    }

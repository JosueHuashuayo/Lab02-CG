using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesaparecerLetrero : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DesaparecerTrasDosSegundos());
    }

    private IEnumerator DesaparecerTrasDosSegundos()
    {
        yield return new WaitForSeconds(2f); // Espera dos segundos
        Destroy(gameObject); // Destruye el objeto
    }
}

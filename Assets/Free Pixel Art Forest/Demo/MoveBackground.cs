using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour {



	public float speed;

    private float puntoInicial;
    private float posicionInicialCamara;

    private Camera camaraPrincipal;

    // Use this for initialization
    void Start()
    {
        camaraPrincipal = Camera.main;
        puntoInicial = transform.position.x;
        posicionInicialCamara = camaraPrincipal.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float desplazamientoCamara = camaraPrincipal.transform.position.x - posicionInicialCamara;
        float nuevaPosicionX = puntoInicial + (desplazamientoCamara * speed);
        transform.position = new Vector3(nuevaPosicionX, transform.position.y, transform.position.z);
    }
}

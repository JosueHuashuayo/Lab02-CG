using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Cinemachine;
using UnityEditor.Timeline.Actions;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] public int m_vidas = 3; // Atributo de vidas


    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;


    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);
            AttackAction();

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
            

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    private bool puedeRecibirDaño = true;
    public CinemachineVirtualCamera m_virtualCamera;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (puedeRecibirDaño)
        {
            if (collision.gameObject.CompareTag("Bat") || collision.gameObject.CompareTag("Slime") || collision.gameObject.CompareTag("Golem"))
            {
                MovimientoEnemigo enemigo = collision.gameObject.GetComponent<MovimientoEnemigo>();
                if (enemigo.isAlive)
                {
                    Time.timeScale = 0.2f;
                    Invoke("ResetTimeScale", 0.15f);
                    // Reduce una vida
                    GameSingleton.Instance.ReduceLife(1);
                    Debug.Log("Perdiendo vidas");

                    // Desactiva la capacidad de recibir daño temporalmente
                    puedeRecibirDaño = false;
                    m_animator.SetTrigger("Hurt");

                    // Espera un tiempo antes de volver a permitir el daño
                    StartCoroutine(EsperarParaRecibirDaño(1.0f)); // Espera 2 segundos

                    // Agita la cámara
                    ShakeCamera();

                    // Verifica si se han agotado las vidas
                    if (m_vidas <= 0)
                    {
                        m_animator.SetTrigger("Death");
                        // Realiza acciones de Game Over, por ejemplo, reiniciar el nivel o mostrar un mensaje
                        Debug.Log("Game Over");
                        DestroyHero();
                    }
                }
            }
        }
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f; // Restaura el tiempo normal
    }

    private IEnumerator EsperarParaRecibirDaño(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        puedeRecibirDaño = true;
    }
    private void ShakeCamera()
    {
        if (m_virtualCamera != null)
        {
            CinemachineBasicMultiChannelPerlin perlin = m_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (perlin != null)
            {
                perlin.m_AmplitudeGain = 5f; // Ajusta la intensidad del temblor
                perlin.m_FrequencyGain = 10f; // Ajusta la rapidez del temblor
            }
            else
            {
                Debug.LogError("No se encontró ningún componente CinemachineBasicMultiChannelPerlin adjunto a la cámara virtual.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ninguna cámara virtual en la escena.");
        }

        // Reinicia los valores de la cámara después de un tiempo
        StartCoroutine(ResetearTemblorDeCamara(0.1f)); // Después de 0.5 segundos, reinicia la cámara
    }
    private IEnumerator ResetearTemblorDeCamara(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        CinemachineBasicMultiChannelPerlin perlin = m_virtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = 0f; // Vuelve a cero la intensidad del temblor
            perlin.m_FrequencyGain = 0f; // Vuelve a cero la rapidez del temblor
        }
    }

    private void DestroyHero()
    {
        Destroy(gameObject, 1.5f);
    }


    public LayerMask targetLayers;
    public float attackRange = 1f; // Rango de ataque
    public int damage = 10; // Cantidad de daño

    public Transform attackPoint; // Punto de origen del ataque
    public float attackRate = 2f; // Velocidad de ataque (ataques por segundo)

    private float nextAttackTime = 0f;

    void AttackAction()
    {
        // Detectar los objetos en el área de ataque
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayers);

        // Aplicar daño a los objetos en el área de ataque
        foreach (Collider2D hitObject in hitObjects)
        {
            // Aquí puedes manejar la lógica del daño según el tipo de objeto
            if (hitObject.TryGetComponent(out MovimientoEnemigo damageable))
            {
                damageable.killEnemy();
            }
        }
    }

    // Dibuja un gizmo en el editor para visualizar el área de ataque

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}

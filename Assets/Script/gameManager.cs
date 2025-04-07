using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera miniGameCamera;
    public bool miniJuegoActivo = false;
    public int dinero;
    public GameObject npcActual;

    [SerializeField] private TextMeshProUGUI dineroText;
    [SerializeField] private RawImage puntoCursor;
    [SerializeField] private MinijuegoController minijuegoController;

    [Header("Sistema de Día")]
    public float duracionDia = 300f; // 5 minutos reales = 1 día del juego
    private float tiempoTranscurrido = 0f;
    private int diaActual = 1;
    private int dineroInicioDia;
    private bool diaActivo = false;

    [Header("Resumen del Día")]
    [SerializeField] private GameObject resumenCanvas;
    [SerializeField] private TextMeshProUGUI resumenTexto;
    [SerializeField] private CanvasGroup resumenCanvasGroup;

    [Header("Reloj")]
    [SerializeField] public TextMeshProUGUI horaTexto;
    private float minutosDia; // minutos desde 8:00 AM
    private int intervaloMinutos = 10;
    private float tiempoPorIntervalo;
    private float tiempoAcumulado = 0f;
    
    [Header("Comoutadora")]
    public GameObject computadorCanvas; 
    public PlayerController playerController;
    public int deudaJugador = 100;
    public string tiempoRestanteDeuda = "2 días";


    void Start()
    {
        resumenTexto.text = ""; 
        mainCamera.gameObject.SetActive(true);
        miniGameCamera.gameObject.SetActive(false);
        puntoCursor.gameObject.SetActive(true);
        miniJuegoActivo = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        dinero = 0;
        dineroText.text = dinero.ToString();

        resumenCanvas.SetActive(false);
        dineroInicioDia = dinero;

        minutosDia = 480f; // 8:00 AM
        tiempoPorIntervalo = duracionDia / ((1200 - 480) / intervaloMinutos); // ej. 300s / 72 bloques de 10min
        diaActivo = false;

        StartCoroutine(InicioDelDia());
    }

    void Update()
    {
        if (computadorCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarComputador();
        }
        
        if (!diaActivo) return;

        tiempoTranscurrido += Time.deltaTime;
        tiempoAcumulado += Time.deltaTime;

        if (tiempoAcumulado >= tiempoPorIntervalo)
        {
            minutosDia += intervaloMinutos;
            tiempoAcumulado = 0f;
            ActualizarReloj();
        }

        if (tiempoTranscurrido >= duracionDia)
        {
            tiempoTranscurrido = 0f;
            StartCoroutine(FinDelDia());
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Para testeo
        {
            miniJuegoActivo = !miniJuegoActivo;
            ActualizarCamara();
        }
    }
    public void AbrirComputador()
    {
        computadorCanvas.SetActive(true); // Activar la UI del computador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(false); // Desactiva movimiento
        }
    }

    public void CerrarComputador()
    {
        computadorCanvas.SetActive(false); // Ocultar la UI
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(true); // Reactivar movimiento
        }
    }
    

    void ActualizarReloj()
    {
        int horas = Mathf.FloorToInt(minutosDia / 60f);
        int minutos = Mathf.FloorToInt(minutosDia % 60f);

        string ampm = (horas >= 12) ? "PM" : "AM";
        int horas12 = horas % 12;
        if (horas12 == 0) horas12 = 12;

        horaTexto.text = $"{horas12:D2}:{minutos:D2} {ampm}";
    }

    public void ActualizarCamara()
    {
        if (miniJuegoActivo)
        {
            mainCamera.gameObject.SetActive(false);
            miniGameCamera.gameObject.SetActive(true);
            puntoCursor.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            dineroText.text = dinero.ToString();
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            miniGameCamera.gameObject.SetActive(false);
            dinero += minijuegoController.calcularValorFinalAparato();
            minijuegoController.ReiniciarMinijuego();
            Destroy(npcActual);
            puntoCursor.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            dineroText.text = dinero.ToString();
        }
    }

    IEnumerator InicioDelDia()
    {
        diaActivo = false;
        resumenCanvas.SetActive(true);
        resumenCanvasGroup.alpha = 1;

        // Fade out rápido (oscurecer)
        float t = 1;
        while (t > 0f)
        {
            t -= Time.deltaTime / 2f; // más rápido
            resumenCanvasGroup.alpha = t;
            yield return null;
        }

        resumenCanvas.SetActive(false);
        tiempoTranscurrido = 0f;
        tiempoAcumulado = 0f;
        minutosDia = 480f; // 8:00 AM
        ActualizarReloj();
        diaActivo = true;
    }

    IEnumerator FinDelDia()
    {
        diaActivo = false;
        miniJuegoActivo = false;
        ActualizarCamara();

        int gananciaDelDia = dinero - dineroInicioDia;
        diaActual++;
        resumenTexto.text = $"Día {diaActual - 1} terminado\nGanancias: ${gananciaDelDia}";
        resumenCanvas.SetActive(true);
        resumenCanvasGroup.alpha = 0;

        // Fade in rápido (aparece oscuro)
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f; // más rápido
            resumenCanvasGroup.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(2f); // Mostrar resumen

        // Fade out rápido
        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / 2f;
            resumenCanvasGroup.alpha = t;
            yield return null;
        }

        resumenCanvas.SetActive(false);
        dineroInicioDia = dinero;

        StartCoroutine(InicioDelDia()); // Iniciar nuevo día
    }
}

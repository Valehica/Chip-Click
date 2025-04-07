using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Cautin : Interactuable
{
    [Header("Cautín")]
    public bool cautinEncendido = false;
    public float temperatura = 0f;
    public float velocidadDeCalentamiento = 20f;
    public bool tocaGrasa = false;

    [Header("Referencias")]
    public GameObject botonOn;
    public GameObject botonOff;
    [SerializeField] private GameObject botonModelo0n;
    [SerializeField] private GameObject botonModelo0ff;
    [SerializeField] private TextMeshProUGUI textoTemperatura;
    [SerializeField] private GameObject indicadorEnceniddo;

    [Header("Estaño")]
    [SerializeField] private GameObject prefabPelotitaEstanio;
    [SerializeField] private Transform spawnEstanio;
    private GameObject instanciaPelotita;

    private GameObject ultimaConexionTocada;

    // NUEVO: lista de pelotitas colocadas en conexiones
    private List<GameObject> pelotitasColocadas = new List<GameObject>();

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        botonModelo0n.SetActive(false);
        botonModelo0ff.SetActive(true);
        textoTemperatura.text = temperatura.ToString("F0") + "°";
        indicadorEnceniddo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
    }

    void Update()
    {
        if (cautinEncendido)
        {
            if (tocaGrasa)
            {
                temperatura -= (velocidadDeCalentamiento / 2) * Time.deltaTime;
                temperatura = Mathf.Max(0, temperatura);
            }
            else
            {
                if (temperatura <= 150)
                {
                    temperatura += velocidadDeCalentamiento * Time.deltaTime;
                }
            }

            textoTemperatura.text = temperatura.ToString("F0") + "°";
        }
        else
        {
            temperatura = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == botonOn)
                {
                    EncenderCautin();
                }
                else if (hit.collider.gameObject == botonOff)
                {
                    ApagarCautin();
                }
            }
        }
    }

    void EncenderCautin()
    {
        cautinEncendido = true;
        botonModelo0n.SetActive(true);
        botonModelo0ff.SetActive(false);
        indicadorEnceniddo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
    }

    public void ApagarCautin()
    {
        cautinEncendido = false;
        botonModelo0n.SetActive(false);
        botonModelo0ff.SetActive(true);
        textoTemperatura.text = "0°";
        indicadorEnceniddo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grasa"))
        {
            if (instanciaPelotita != null)
            {
                Destroy(instanciaPelotita);
            }

            Debug.Log("Tocó Grasa");
            tocaGrasa = true;
        }

        if (other.CompareTag("Estanio"))
        {
            if (!cautinEncendido)
            {
                Debug.Log("El cautín está apagado. No se puede tomar estaño.");
                return;
            }

            if (instanciaPelotita == null)
            {
                Debug.Log("Tocó Estanio - creando pelotita");
                instanciaPelotita = Instantiate(prefabPelotitaEstanio, spawnEstanio.position, Quaternion.identity, transform);
            }
            else
            {
                Debug.Log("Ya hay una pelotita de estaño");
            }
        }

        if (other.CompareTag("Conexion"))
        {
            ultimaConexionTocada = other.gameObject;
            Debug.Log("Chocó con conexión");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grasa"))
        {
            tocaGrasa = false;
        }

        if (other.CompareTag("Conexion") && other.gameObject == ultimaConexionTocada)
        {
            ultimaConexionTocada = null;
        }
    }

    void OnMouseUp()
    {
        restartPosition();

        if (ultimaConexionTocada != null && instanciaPelotita != null)
        {
            instanciaPelotita.transform.SetParent(null);
            instanciaPelotita.transform.position = ultimaConexionTocada.transform.position + new Vector3(0, 0.05f, 0);

            // NUEVO: agregar a la lista de pelotitas colocadas
            pelotitasColocadas.Add(instanciaPelotita);

            instanciaPelotita = null;

            GetComponentInParent<MinijuegoController>().estanioConexion(
                ultimaConexionTocada.GetComponent<CableConnector3D>().color,
                true,
                temperatura
            );

            Debug.Log("Ubicando pelotita en la conexión");
        }
    }

    // NUEVO: función para resetear todas las pelotitas puestas
    public void ResetEstanios()
    {
        if (instanciaPelotita != null)
        {
            Destroy(instanciaPelotita);
            instanciaPelotita = null;
        }

        foreach (GameObject pelotita in pelotitasColocadas)
        {
            if (pelotita != null)
                Destroy(pelotita);
        }

        pelotitasColocadas.Clear();
        Debug.Log("Pelotitas de estaño reseteadas");
    }
}

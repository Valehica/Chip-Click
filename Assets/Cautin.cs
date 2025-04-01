using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] private GameObject pelotaEstanio;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        
        pelotaEstanio.SetActive(false);
        
        botonModelo0n.SetActive(false);
        botonModelo0ff.SetActive(true);
        textoTemperatura.text = temperatura.ToString("F0") + "°";
        indicadorEnceniddo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);

        if (pelotaEstanio != null)
        {
            pelotaEstanio.SetActive(false);
        }
    }

    void Update()
    {
        if (cautinEncendido)
        {
            if (tocaGrasa)
            {
                temperatura -= (velocidadDeCalentamiento/2) * Time.deltaTime;
                temperatura = Mathf.Max(0, temperatura); // No bajar de 0
                textoTemperatura.text = temperatura.ToString("F0") + "°";
            }
            else
            {
                temperatura += velocidadDeCalentamiento * Time.deltaTime;
                textoTemperatura.text = temperatura.ToString("F0") + "°"; 
            }
            
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
                    cautinEncendido = true;
                    botonModelo0n.SetActive(true);
                    botonModelo0ff.SetActive(false);
                    indicadorEnceniddo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
                }
                else if (hit.collider.gameObject == botonOff)
                {
                    cautinEncendido = false;
                    botonModelo0n.SetActive(false);
                    botonModelo0ff.SetActive(true);
                    textoTemperatura.text = "0°";
                    indicadorEnceniddo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grasa"))
        {
            Debug.Log("Toco Grasa");
            // Disminuye temperatura al mismo ritmo que aumentaría
            pelotaEstanio.SetActive(false);
            tocaGrasa = true;
        }

        if (other.CompareTag("Estanio") && pelotaEstanio != null)
        {
            Debug.Log("Toco Estanio");
            pelotaEstanio.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grasa"))
        {
            tocaGrasa = false;
        }
    }
}

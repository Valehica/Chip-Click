using System;
using System.Collections.Generic;
using UnityEngine;

public struct EstanioConectionData
{
    public Color color;
    public bool conectado;
    public float temperatura;

    public EstanioConectionData(Color color, bool conectado, float temperatura)
    {
        this.color = color;
        this.conectado = conectado;
        this.temperatura = temperatura;
    }
}
public struct ColorData
{
    public Color color;
    public bool estaBienConectado;

    public ColorData(Color color, bool estaBienConectado)
    {
        this.color = color;
        this.estaBienConectado = estaBienConectado;
    }
}

public class MinijuegoController : MonoBehaviour
{
    [SerializeField] private Transform[] positionTornillos;
    [SerializeField] private Transform[] tornillos;
    [SerializeField] private Transform tapa;
    [SerializeField] private Transform posicionFinalTapa;

    [SerializeField] private int tornillosSacado;

    [SerializeField] public Dictionary<Color, bool> coloresCables;
    [SerializeField] public List<EstanioConectionData> cablesConEstanio;

    private float factorDePerdida;
    public float dineroAparato;

    [SerializeField] private GameObject botonFinalizar;
    [SerializeField] private gameManager gameManager;
    
    //Guardado de posiciones iniciales
    private Vector3[] posicionesInicialesTornillos;
    private Quaternion[] rotacionesInicialesTornillos;

    private Vector3 posicionInicialTapa;
    private Quaternion rotacionInicialTapa;
    
    
    //referencia Mecanicas 
    [SerializeField] Cautin cautin;

    void Start()
    {
        // Guardar posiciones y rotaciones iniciales de tornillos
        posicionesInicialesTornillos = new Vector3[positionTornillos.Length];
        rotacionesInicialesTornillos = new Quaternion[positionTornillos.Length];

        for (int i = 0; i < positionTornillos.Length; i++)
        {
            posicionesInicialesTornillos[i] = tornillos[i].position;
            rotacionesInicialesTornillos[i] = tornillos[i].rotation;
        }

        // Guardar posición y rotación inicial de la tapa
        posicionInicialTapa = tapa.position;
        rotacionInicialTapa = tapa.rotation;

        // Inicializar todos los colores como desconectados
        coloresCables = new Dictionary<Color, bool>()
        {
            { Color.red, false },
            { Color.green, false },
            { Color.blue, false },
            { Color.yellow, false }
        };
        cablesConEstanio = new List<EstanioConectionData>()
        {
            new EstanioConectionData(Color.red, false, 0f),
            new EstanioConectionData(Color.blue, false, 0f),
            new EstanioConectionData(Color.green, false, 0f),
            new EstanioConectionData(Color.yellow, false, 0f)
        };
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == botonFinalizar)
                {
                    gameManager.miniJuegoActivo = false;
                    gameManager.ActualizarCamara();
                }
            }
        }
    }

    void calcularFactorDePerdida()
    {
        //Inicializar el valor de descuento por cada cosa mal hecha 
        factorDePerdida = dineroAparato / 12;
    }

    public void sacoTornillos(Transform tornillo)
    {
        tornillo.position = positionTornillos[tornillosSacado].position;
        tornillo.rotation = Quaternion.Euler(0, 20, 90);
        tornillosSacado++;

        if (tornillosSacado >= 4)
        {
            tapa.position = posicionFinalTapa.position;
            tapa.rotation = posicionFinalTapa.rotation;
        }
    }

    public void coneccion(Color colorInicio, bool seConectoBien)
    {
        if (coloresCables.ContainsKey(colorInicio))
        {
            coloresCables[colorInicio] = seConectoBien;
        }
        else
        {
            Debug.LogWarning($"Intento de conectar un color no definido: {colorInicio}");
        }
    }

    public void estanioConexion(Color colorInicio, bool seConectoBien, float temperatura)
    {
        bool colorEncontrado = false;

        for (int i = 0; i < cablesConEstanio.Count; i++)
        {
            if (cablesConEstanio[i].color == colorInicio)
            {
                cablesConEstanio[i] = new EstanioConectionData(colorInicio, seConectoBien, temperatura);
                colorEncontrado = true;
                break;
            }
        }

        if (!colorEncontrado)
        {
            Debug.LogWarning($"Intento de poner estanio en un color no definido: {colorInicio}");
        }

        for (int i = 0; i < cablesConEstanio.Count; i++)
        {
            Debug.Log(
                $"Color: {cablesConEstanio[i].color}, Conectado: {cablesConEstanio[i].conectado}, Temperatura: {cablesConEstanio[i].temperatura}");
        }
    }

    // Método para verificar rápidamente si todos están bien conectados
    public bool TodosBienConectados()
    {
        foreach (var conexion in coloresCables.Values)
        {
            if (!conexion) return false;
        }

        return true;
    }

    public int calcularValorFinalAparato()
    {
        float factorDePerdida = dineroAparato / 12f;
        int errores = 0;

        // 1. Tornillos no sacados (máximo 4)
        errores += Mathf.Max(0, 4 - tornillosSacado);

        // 2. Cables mal conectados
        foreach (var cable in coloresCables)
        {
            if (!cable.Value)
            {
                errores++;
            }
        }

        // 3. Estaños mal conectados
        foreach (var estanio in cablesConEstanio)
        {
            if (!estanio.conectado)
            {
                errores++;
            }
        }

        // Calcular y devolver el valor final
        float resultado = dineroAparato - (errores * factorDePerdida);
        Debug.Log("Errores: " + errores + "factorPerdida: " + factorDePerdida + "resultado: "+ resultado + "Dinero: " + dineroAparato);
        return Mathf.Max(0, Mathf.RoundToInt(resultado));
    }
    
    public void ReiniciarMinijuego()
    {
        tornillosSacado = 0;

        // Reiniciar estado de los cables
        coloresCables = new Dictionary<Color, bool>()
        {
            { Color.red, false },
            { Color.green, false },
            { Color.blue, false },
            { Color.yellow, false }
        };

        // Reiniciar datos de estaño
        for (int i = 0; i < cablesConEstanio.Count; i++)
        {
            cablesConEstanio[i] = new EstanioConectionData(cablesConEstanio[i].color, false, 0f);
        }

        // Devolver tornillos a su posición y rotación original
        for (int i = 0; i < positionTornillos.Length; i++)
        {
            tornillos[i].position = posicionesInicialesTornillos[i];
            tornillos[i].rotation = rotacionesInicialesTornillos[i];
        }

        // Devolver tapa a su posición y rotación original
        tapa.position = posicionInicialTapa;
        tapa.rotation = rotacionInicialTapa;
        
        //apagar Cautin
        GetComponentInChildren<Cautin>().ApagarCautin();
        
        //quietar el estaño 
        GetComponentInChildren<Cautin>().ResetEstanios();
        
        //desconectar los cables 
        CableConnector3D.ResetAllConnections();
    }
}
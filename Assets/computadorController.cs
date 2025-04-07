using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ComputadorController : MonoBehaviour
{
    [Header("Referencias Generales")]
    public gameManager gameManager;
    public TextMeshProUGUI relojTexto;

    [Header("Menu Pagar Cuentas")]
    public GameObject menuPagarCuentas;
    public TextMeshProUGUI textoDeuda;
    public TextMeshProUGUI textoTiempoRestante;
    public TextMeshProUGUI textoDineroInsuficienteCuentas;
    public Button botonPagarCuenta;

    [Header("Menu Tienda")]
    public GameObject menuTienda;
    public TextMeshProUGUI resumenCarro;
    public TextMeshProUGUI totalTexto;
    public TextMeshProUGUI textoDineroInsuficienteTienda;
    public Button botonPagarTienda;
    public Button botonEstaño;
    public Button botonGrasa;
    public Button botonDestornillador;

    [Header("Botones de Interfaz")]
    public Button botonAbrirMenuPagar;
    public Button botonAbrirTienda;
    public Button botonCerrarPagar;
    public Button botonCerrarTienda;

    // Carrito
    private int totalCompra = 0;
    private Dictionary<string, int> itemsSeleccionados = new Dictionary<string, int>();
    private Dictionary<Button, string> botonNombreProducto = new Dictionary<Button, string>();
    private HashSet<Button> botonesSeleccionados = new HashSet<Button>();

    void Start()
    {
        botonAbrirMenuPagar.onClick.AddListener(AbrirMenuPagarCuentas);
        botonAbrirTienda.onClick.AddListener(AbrirMenuTienda);
        botonCerrarPagar.onClick.AddListener(() => menuPagarCuentas.SetActive(false));
        botonCerrarTienda.onClick.AddListener(() => menuTienda.SetActive(false));
        botonPagarCuenta.onClick.AddListener(PagarCuenta);
        botonPagarTienda.onClick.AddListener(PagarCompra);

       // Configurar productos
        botonNombreProducto[botonEstaño] = "Estaño calidad media";
        botonNombreProducto[botonGrasa] = "Grasa de mejor calidad";
        botonNombreProducto[botonDestornillador] = "Destornillador eléctrico";
        
        botonEstaño.onClick.AddListener(() => ToggleItem(botonEstaño, 500));
        botonGrasa.onClick.AddListener(() => ToggleItem(botonGrasa, 800));
        botonDestornillador.onClick.AddListener(() => ToggleItem(botonDestornillador, 1500));

        textoDineroInsuficienteCuentas.gameObject.SetActive(false);
        textoDineroInsuficienteTienda.gameObject.SetActive(false);
    }

    void Update()
    {
        if (relojTexto != null)
            relojTexto.text = gameManager.horaTexto.text;
    }

    void AbrirMenuPagarCuentas()
    {
        textoDeuda.text = $"${gameManager.deudaJugador}";
        textoTiempoRestante.text = $"{gameManager.tiempoRestanteDeuda}";
        textoDineroInsuficienteCuentas.gameObject.SetActive(false);
        menuPagarCuentas.SetActive(true);
    }

    void PagarCuenta()
    {
        int deuda = gameManager.deudaJugador;
        if (gameManager.dinero >= deuda)
        {
            gameManager.dinero -= deuda;
            gameManager.deudaJugador = 0;
            gameManager.tiempoRestanteDeuda = "SIN DEUDA";
            textoTiempoRestante.text = "SIN DEUDA";
            textoDeuda.text = "$0";
            textoDineroInsuficienteCuentas.gameObject.SetActive(false);
        }
        else
        {
            textoDineroInsuficienteCuentas.gameObject.SetActive(true);
        }
    }

    void AbrirMenuTienda()
    {
        // Resetear
        totalCompra = 0;
        itemsSeleccionados.Clear();
        botonesSeleccionados.Clear();
        resumenCarro.text = "Carro vacío";
        totalTexto.text = "Total: $0";
        textoDineroInsuficienteTienda.gameObject.SetActive(false);

        menuTienda.SetActive(true);
    }

    void ToggleItem(Button boton, int precio)
    {
        string nombre = botonNombreProducto[boton];

        if (botonesSeleccionados.Contains(boton))
        {
            // Deseleccionar
            botonesSeleccionados.Remove(boton);
            itemsSeleccionados.Remove(nombre + "  $" + precio);
            totalCompra -= precio;
        }
        else
        {
            // Seleccionar
            botonesSeleccionados.Add(boton);
            itemsSeleccionados[nombre + "  $" + precio] = precio;
            totalCompra += precio;
        }

        ActualizarCarrito();
    }

    void ActualizarCarrito()
    {
        if (itemsSeleccionados.Count == 0)
        {
            resumenCarro.text = "Carro vacío";
            totalTexto.text = "Total: $0";
        }
        else
        {
            resumenCarro.text = string.Join("\n", itemsSeleccionados.Keys);
            totalTexto.text = $"Total: ${totalCompra}";
        }

        textoDineroInsuficienteTienda.gameObject.SetActive(false);
    }


    void PagarCompra()
    {
        if (gameManager.dinero >= totalCompra)
        {
            gameManager.dinero -= totalCompra;
            textoDineroInsuficienteTienda.gameObject.SetActive(false);
            resumenCarro.text = "¡Compra realizada!";
            totalTexto.text = "Total: $0";
        }
        else
        {
            textoDineroInsuficienteTienda.gameObject.SetActive(true);
        }
    }
}

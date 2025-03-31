using UnityEngine;

public class MinijuegoController : MonoBehaviour
{
    [SerializeField] private Transform[] positionTornillos;
    [SerializeField] private Transform tapa;
    [SerializeField] private Vector3 tapaPositionFinal;
    [SerializeField] private Quaternion tapaRotationFinal;

    [SerializeField] private int tornillosSacado; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void sacoTornillos(Transform tornillo)
    {
        tornillo.position = positionTornillos[tornillosSacado].position;
        tornillo.rotation = Quaternion.Euler(0, 0, 90);
        tornillosSacado++;

        if (tornillosSacado >= 4)
        {
            tapa.position += tapaPositionFinal;
            tapa.localRotation = tapaRotationFinal;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}

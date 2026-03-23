using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cell;
    public int rows = 5;
    public int columns = 5;

    void Start()
    {
        
        for (int i = 0; i < rows * columns; i++)
        {
            Instantiate(cell, transform);
        }
        
        
    }
}

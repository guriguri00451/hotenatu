using System;
using UnityEngine;

public class AreaDetector : MonoBehaviour
{
    public Action onPlayerEnter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onPlayerEnter?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{

    [SerializeField] private Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] private float period = 2f;
    [Range(0, 1)] [SerializeField] private float movementFactor; // 0 not moved, 1 fully moved
    private Vector3 startingPosition;
    
    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        //set movement factor
        if (period <= Mathf.Epsilon) return;
        
        float cycles = Time.time / period; //grows continually from 0

        const float tau = Mathf.PI * 2;

        float rawSineWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSineWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}

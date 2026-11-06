using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BlinkColorOnHit))]
public class EnemyShield : MonoBehaviour
{
    [Header("Inscribed")]
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;

    void Start()
    {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ingoreOnCollisionEnter = true;


    }

    void Update()
    {
        
    }
}

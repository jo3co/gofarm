using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayFX : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator StartCooldownTimer() {
        yield return new WaitForSeconds(7);

    }
    void Start()
    {
        StartCoroutine(StartCooldownTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

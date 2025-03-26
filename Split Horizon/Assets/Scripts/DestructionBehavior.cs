using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionBehavior : MonoBehaviour
{
    public GameObject fractured;
    public float breakForce;

    public void FractureObject()
    {
        // Instantiate the fractured prefab with its preset rotation.
        GameObject frac = Instantiate(fractured, transform.position, fractured.transform.rotation);

        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * breakForce;
            // Use an impulse force so the explosion remains strong in slow-mo.
            rb.AddForce(force, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }
}

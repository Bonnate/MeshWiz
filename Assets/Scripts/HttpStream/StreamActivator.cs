using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamActivator : MonoBehaviour
{
    void OnEnable()
    {
        switch (name)
        {
            case "baked_mesh":
                {
                    transform.localPosition = Vector3.zero;
                    transform.localScale = Vector3.one * 2.0f;

                    break;
                }

            case "testa_mesh":
                {
                    transform.position = Vector3.right * 3.0f;
                    transform.localScale = Vector3.one * 2.0f;
                    break;
                }

            case "red_heart":
                {
                    transform.localScale = Vector3.one * 0.05f;

                    transform.position = new Vector3(-3.83204174f, 1.0650152f, 0.24150078f);

                    gameObject.AddComponent<Rigidbody>();
                    gameObject.AddComponent<BoxCollider>();

                    transform.Rotate(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

                    GetComponent<Rigidbody>().AddForce(transform.forward * Random.Range(10, 20), ForceMode.Impulse);
                    break;
                }
        }

        Destroy(this);
    }
}

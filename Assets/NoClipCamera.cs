using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoClipCamera : MonoBehaviour
{
    private Vector2 mKeyboardInput, mMouseInput;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            mKeyboardInput.x = Input.GetAxis("Horizontal");
            mKeyboardInput.y = Input.GetAxis("Vertical");

            mMouseInput.x = Input.GetAxis("Mouse X");
            mMouseInput.y = Input.GetAxis("Mouse Y");

            transform.Translate(new Vector3(mKeyboardInput.x, 0, mKeyboardInput.y) * Time.deltaTime * 15.0f);
            transform.Rotate(new Vector3(0, mMouseInput.x, 0) * Time.deltaTime * 360.0f, Space.World);
            transform.Rotate(new Vector3(-mMouseInput.y, 0, 0) * Time.deltaTime * 360.0f, Space.Self);
        }
    }
}

using UnityEngine;

/**
 * Dieses Skript stellt die Steuerung für die Zuschauerkamera zur Verfügung.
 */
public class SpectatorCameraScript : MonoBehaviour {

    /**
     * Geschwindigkeit der Kamera.
     */
    public float mSpeed = 12f;

    /**
     * C.W.: Used to provide double speed while pressing the shift key
     */
    private float mSpeedDouble;

    /**
     * C.W.: Used to provide normal speed without pressing shift key
     */
    private float mSpeedNormal;

    /**
     * Kameraobjekt.
     */
    private Camera mSpectatorCamera;

    /**
     * Bewegungsvektor.
     */
    private Vector3 mMovement;

    /**
     * Unity Methode, die beim Aufruf des Skripts *einmalig* ausgeführt wird.
     */
    private void Awake()
    {
        mSpectatorCamera = GetComponent<Camera>();

        mSpeedDouble = mSpeed* 2;
        mSpeedNormal = mSpeed;
    }

    /**
     * Unity Methode
     */
    private void FixedUpdate()
    {
        // C.W.: provides double time speed if shift is pressed
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            mSpeed = mSpeedDouble;

        } else
        {
            // C.W.: provides normal speed without pressing the shift key
            mSpeed = mSpeedNormal;
        }

        // C.W.: No else-if to enable multiple key inputs
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + mSpectatorCamera.transform.right * -1 * mSpeed * Time.deltaTime;
        }

        // C.W.: moves camera backwards
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + mSpectatorCamera.transform.forward * -1 * mSpeed * Time.deltaTime;

        }

        // C.W.: moves camera to the right
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + mSpectatorCamera.transform.right * mSpeed * Time.deltaTime;

        }

        // C.W.: moves camera forward
        if (Input.GetKey(KeyCode.W))
        {

            transform.position = transform.position + mSpectatorCamera.transform.forward * mSpeed * Time.deltaTime;
        }

        // C.W.: moves camera upwards
        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + mSpectatorCamera.transform.up * mSpeed * Time.deltaTime;
        }

        // C.W.: moves camera downwards
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + mSpectatorCamera.transform.up * -1 * mSpeed * Time.deltaTime;
        }
    }
}
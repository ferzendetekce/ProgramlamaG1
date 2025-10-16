using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Takip edilecek hedef (karakter)
    public Vector3 offset;          // Kameranýn hedefe göre konumu
    public float smoothSpeed = 0.125f; // Takip yumuþaklýðý

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target); // Kameranýn hedefe bakmasýný saðlar
    }
}
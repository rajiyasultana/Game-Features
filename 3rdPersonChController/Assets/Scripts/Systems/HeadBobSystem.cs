using Unity.Cinemachine;
using UnityEngine;

public class HeadBobSystem : MonoBehaviour
{
    [SerializeField] float bobFrequency = 2f; //bob speed
    [SerializeField] float bobAmplitude = 0.05f; // bob height
    [SerializeField] float smooth = 8f;

    private CinemachineCamera vCam;
    private Vector3 startPos;
    private float timer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

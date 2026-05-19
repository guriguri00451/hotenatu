using Unity.VisualScripting;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    [SerializeField] private Camera[] roomCameras;
    [SerializeField] private AreaDetector[] areaDetectors;
    [SerializeField] private int currentCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        SwitchCamera(0);
        for (int i = 0; i < areaDetectors.Length; i++)
        {
            int index = i;
            areaDetectors[i].onPlayerEnter += () => SwitchCamera(index);
        }
    }

    void SwitchCamera(int index)
    {
        roomCameras[currentCamera].gameObject.SetActive(false);
        roomCameras[index].gameObject.SetActive(true);
        currentCamera = index;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

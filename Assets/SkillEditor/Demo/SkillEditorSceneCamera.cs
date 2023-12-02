using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillEditorSceneCamera : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    [Button]
    public void GetPlayer()
    {
        this.GetComponent<CinemachineVirtualCameraBase>().Follow = GameObject.Find("PreviewCharacterRoot").transform.GetChild(0);
        this.GetComponent<CinemachineVirtualCameraBase>().LookAt = GameObject.Find("PreviewCharacterRoot").transform.GetChild(0);
    }

    [Button]
    public void GetPlayerY()
    {
        this.GetComponent<CinemachineVirtualCameraBase>().Follow = GameObject.Find("CameraPos").transform;
        this.GetComponent<CinemachineVirtualCameraBase>().LookAt = GameObject.Find("CameraPos").transform;
    }
}
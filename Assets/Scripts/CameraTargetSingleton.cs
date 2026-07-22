using UnityEngine;

public class CameraTargetSingleton : MonoBehaviour
{
    public static CameraTargetSingleton Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}

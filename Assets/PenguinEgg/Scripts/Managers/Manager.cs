using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    protected bool _isInitialized = false;

    public abstract void Initialize();
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Checks for button input on an input action and spawns a prefab at the raycast hit point.
/// </summary>
public class OnButtonPress : MonoBehaviour
{
    [Tooltip("Prefab to spawn on button press")]
    public GameObject prefabToSpawn;



    [Tooltip("Transform for raycasting")]
    public Transform raycastOrigin;

    [Tooltip("Maximum distance for the raycast")]
    public float maxRayDistance = 100f;

    [Tooltip("LayerMask to focus raycast")]
    public LayerMask raycastLayerMask;

    [Tooltip("Actions to check")]
    public InputAction action = null;  // If null, it must be assigned in runtime or from another component.

    // When the button is pressed
    public UnityEvent OnPress = new UnityEvent();

    // When the button is released
    public UnityEvent OnRelease = new UnityEvent();

    private void Awake()
    {
        if (action == null)
        {
            Debug.LogError("InputAction is not set. Please assign it.");
            enabled = false;  // Disable script if there is no action set
            return;
        }

        action.started += Pressed;
        action.canceled += Released;
    }

    private void OnDestroy()
    {
        if (action != null)
        {
            action.started -= Pressed;
            action.canceled -= Released;
        }
    }

    private void OnEnable()
    {
        if (action != null)
        {
            action.Enable();
        }
    }

    private void OnDisable()
    {
        if (action != null)
        {
            action.Disable();
        }
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        OnPress.Invoke();
        SpawnPrefab();
        Debug.Log("I am pressed");
    }

    private void Released(InputAction.CallbackContext context)
    {
        OnRelease.Invoke();
    }


    private void SpawnPrefab()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, maxRayDistance, raycastLayerMask))
        {
            GameObject spawnedObject = Instantiate(prefabToSpawn, hit.point, Quaternion.identity);
        }
    }

    public void LogBoy()
    {
        Debug.Log("LogBoy");
    }

    public void ChangePrefab(GameObject newPrefab)
    {
        prefabToSpawn = newPrefab;
    }

}

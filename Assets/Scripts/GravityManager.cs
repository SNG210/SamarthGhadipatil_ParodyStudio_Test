using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    public GameObject hologram; 
    public MonoBehaviour playerMovementScript; 
    public float rotationDuration = 0.5f; 

    public bool canRotate = false;  

    [SerializeField] private RotationStates currentRotationState = RotationStates.Back;

    void Start()
    {
        if (hologram != null)
        {
            hologram.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShowHologram(RotationStates.Front);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowHologram(RotationStates.Back);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ShowHologram(RotationStates.Right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ShowHologram(RotationStates.Left);
        }
        else if (Input.GetKeyDown(KeyCode.Return) && canRotate) 
        {
            StartRotationSequence();
        }

        ValidateRotate();
    }

    void ShowHologram(RotationStates newState)
    {
        if (hologram == null) return;

        currentRotationState = newState;
        Vector3 newLocalRotation = GetRotationFromState(newState);

        hologram.transform.localRotation = Quaternion.Euler(newLocalRotation);
        hologram.SetActive(true);
    }

    void StartRotationSequence()
    {
        if (playerMovementScript != null)
        {
            DisableMovement();
        }

        Invoke(nameof(StartSmoothRotation), 0.1f); 
        Invoke(nameof(EnableMovement), 0.2f); 
    }

    void DisableMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    void StartSmoothRotation()
    {
        StartCoroutine(SmoothRotate(hologram.transform.rotation));
    }

    IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; 
        hologram.SetActive(false); 
    }

    void EnableMovement()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    Vector3 GetRotationFromState(RotationStates state)
    {
        return state switch
        {
            RotationStates.Back => new Vector3(90, 0, 0),
            RotationStates.Front => new Vector3(90, 0, 180),
            RotationStates.Right => new Vector3(90, 0, 90),
            RotationStates.Left => new Vector3(90, 0, 270),
            _ => Vector3.zero
        };
    }

    void ValidateRotate()
    {
        canRotate = hologram.gameObject.activeSelf;
    }
}

public enum RotationStates
{
    Back,
    Front,
    Right,
    Left,
    Default
}



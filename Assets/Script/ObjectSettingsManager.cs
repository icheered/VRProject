using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSettingsManager : MonoBehaviour
{
    public GameObject UICanvas;
    public ObjectData objectData;
    public List<List<float>> covariance_matrix;
    public UIManager uiManager;
    public GameObject duplicatePrefab;

    private string[] blendShapeNames = new string[] { "Height", "Width", "Thickness", "Smoothness", "Warped", "Bulge" };

    public bool isHolding = false;
    private float distance = 2f;
    
    private Transform mainCameraTransform;
    private GameObject duplicateObject;

    // Todo: Pick up item using activate and then allow sliders to be interacted with the trigger
    public void VRActivate() {
        Debug.Log("Activate");
    }

    public void VRActivateExit() {
        Debug.Log("Activate Exit");
    }
    
    public void VRSelect() {
        Debug.Log("Select");
    }

    public void VRSelectExit() {
        Debug.Log("Select exit");
        UICanvas.SetActive(false);
    }

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (isHolding)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, mainCameraTransform.position + mainCameraTransform.forward * distance, Time.deltaTime * 5);

            if (duplicateObject != null)
            {
                duplicateObject.transform.position = this.transform.position + new Vector3(0.75f, 0, 0);  // The duplicate object will be slightly to the right of the original
                duplicateObject.transform.rotation = this.transform.rotation;  // Set the duplicate object's rotation to that of the original

            }

            if (Input.GetMouseButtonUp(0))
            {
                Drop();
            }
        }
    }

    private void OnMouseDown()
    {
        UICanvas.SetActive(true);
        Debug.Log("Clicked on " + objectData.title);
        StartCoroutine(UpdateSlidersNextFrame(objectData));

        Pickup();
    }

    private void OnMouseUp()
    {
       UICanvas.SetActive(false);
    }

    private IEnumerator UpdateSlidersNextFrame(ObjectData objectData)
    {
        // Wait for end of frame before executing the next line of code
        yield return new WaitForEndOfFrame();
        uiManager.UpdateSliders(objectData);
    }

    private void Pickup()
    {
        if (!isHolding)
        {
            isHolding = true;
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<Rigidbody>().detectCollisions = true;

            duplicateObject = Instantiate(duplicatePrefab, this.transform.position + new Vector3(1f, 0, 0), this.transform.rotation);  // Instantiate the duplicate object slightly to the right of the original
            ObjectSettingsManager settingsManager = duplicateObject.GetComponent<ObjectSettingsManager>();
            if (settingsManager != null)
            {
                settingsManager.objectData = objectData;
                settingsManager.covariance_matrix = covariance_matrix;
                settingsManager.uiManager = uiManager;
                settingsManager.UICanvas = UICanvas;
                settingsManager.SetupDuplicateObject();
            }
            else
            {
                Debug.LogError("ObjectSettingsManager not found on " + duplicateObject.name);
            }
            Collider[] colliders = duplicateObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;  // Disable all colliders so the duplicate object does not have any collision
            }
        } 
    }

    private void Drop()
    {
        isHolding = false;
        this.GetComponent<Rigidbody>().useGravity = true;

        if (duplicateObject != null)
        {
            Destroy(duplicateObject);  // Destroy the duplicate object when the original is dropped
        }
    }

    

    public void SetupObject()
    {
        float[] weights = CalculateWeights();
        SetObjectBlendshapeProperties(weights);
        
    }

    public void SetObjectBlendshapeProperties(float[] weights) {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        for (int i = 0; i < blendShapeNames.Length; i++)
        {
            float blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeNames[i]);
            skinnedMeshRenderer.SetBlendShapeWeight((int)blendShapeIndex, weights[i]);
        }
    }


    public void SetupDuplicateObject()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        float[] weights = new float[blendShapeNames.Length];
        for (int i = 0; i < blendShapeNames.Length; i++)
        {
            float blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeNames[i]);
            skinnedMeshRenderer.SetBlendShapeWeight((int)blendShapeIndex, weights[i]);
        }
    }

    private float[] CalculateWeights()
    {
        int settingsCount = objectData.settings.Count;
        float[] settingsArray = new float[settingsCount];
        for (int i = 0; i < settingsCount; i++)
        {
            settingsArray[i] = objectData.settings[i].value;
        }

        int blendShapesCount = blendShapeNames.Length;
        float[] weights = new float[blendShapesCount];
        for (int i = 0; i < blendShapesCount; i++)
        {
            for (int j = 0; j < settingsCount; j++)
            {
                if (i < covariance_matrix.Count && j < covariance_matrix[i].Count) // Check if the element exists in the covariance matrix
                {
                    weights[i] += settingsArray[j] * covariance_matrix[i][j];
                }
            }
        }
        return weights;
    }


    // This function is called by the UI manager when sliders are updated
    public void UpdateObjectData(string key, float value)
    {
        var setting = objectData.settings.FirstOrDefault(s => s.key == key);

        if (setting != null)
        {
            setting.value = value;
            Debug.Log("Updated object data: " + key + " to " + value);
            float[] weights = CalculateWeights();
            SetObjectBlendshapeProperties(weights);
        }
    }

}

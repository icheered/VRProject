using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[System.Serializable]
public class Setting
{
    public string key;
    public float value;
}

[System.Serializable]
public class ObjectData
{
    public string object_id;
    public string title;
    public List<Setting> settings;
}

[System.Serializable]
public class DataContainer
{
    public List<List<float>> covariance_matrix;
    public List<ObjectData> objects;
}

public class ObjectCreator : MonoBehaviour
{
    public GameObject ObjectPrefab;
    public Transform Objects;
    public GameObject UICanvas;
    public UIManager uiManager;

    public float objectSpacing = 0.5f;

    private Vector3 nextPosition;
    public string[] blendShapeNames = new string[] { "Height", "Width", "Thickness", "Smoothness", "Warped", "Bulge" };

    void Start()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("data");

        // Use JsonConvert.DeserializeObject from Json.NET instead of JsonUtility.FromJson
        DataContainer dataContainer = Newtonsoft.Json.JsonConvert.DeserializeObject<DataContainer>(jsonData.text);

        List<ObjectData> objectList = dataContainer.objects;
        List<List<float>> covariance_matrix = dataContainer.covariance_matrix;

        nextPosition = Objects.position;
        UICanvas.SetActive(true);

        foreach (ObjectData objectData in objectList)
        {
            Quaternion rotation = Quaternion.Euler(-90, Random.Range(0, 360), 0);
            GameObject newObject = Instantiate(ObjectPrefab, nextPosition, rotation, Objects);
            nextPosition.x += objectSpacing;

            // Assign objectData to the ObjectSettingsManager
            ObjectSettingsManager settingsManager = newObject.GetComponent<ObjectSettingsManager>();
            if (settingsManager != null)
            {
                settingsManager.objectData = objectData;
                settingsManager.covariance_matrix = covariance_matrix;
                settingsManager.uiManager = uiManager;
                settingsManager.UICanvas = UICanvas;
                settingsManager.SetupObject();
            }
            else
            {
                Debug.LogError("ObjectSettingsManager not found on " + newObject.name);
            }
        }
        UICanvas.SetActive(false);
    }

    public void UpdateSetting(ObjectData objectData, string key, float value)
    {
        // Update the setting
        Setting setting = objectData.settings.Find(s => s.key == key);
        if (setting != null)
        {
            Debug.Log("Setting " + key + " updated to " + value);
            //setting.value = value;
            //calculateWeights(objectData, covariance_matrix); // Assuming covariance_matrix is a class variable
        }
    }
}


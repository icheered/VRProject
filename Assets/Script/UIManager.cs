using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

[Serializable]
public class MachineInfo
{
    public string machine_id;
    public string title;
}

[Serializable]
public class SettingInfo
{
    public string machine_id;
    public string setting_id;
    public string title;
    public float min;
    public float max;
}

[Serializable]
public class ObjectInfo
{
    public string object_id;
    public string title;
    public Dictionary<string, float> settings;
}

[Serializable]
public class DataInfo
{
    public List<MachineInfo> machines;
    public List<SettingInfo> settings;
    public List<ObjectInfo> objects;
}

public class UIManager : MonoBehaviour
{
    public GameObject UIBlockPrefab;
    public GameObject UISliderPrefab;
    public Transform UIGroup;

    private DataInfo data;
    private ObjectData selectedObject;  // The currently selected object


    private void Start()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("data");
        Debug.Log("Loaded data: " + jsonData.text);

        // Parse the JSON data in the correct format
        data = JsonUtility.FromJson<DataInfo>(jsonData.text);
        BuildUI();
    }

    public void BuildUI()
    {
        foreach (var machine in data.machines)
        {
            var newBlock = Instantiate(UIBlockPrefab, UIGroup);
            newBlock.transform.Find("MachineTitle").GetComponent<TextMeshProUGUI>().text = machine.title;

            var machineSettings = data.settings.FindAll(setting => setting.machine_id == machine.machine_id);
            foreach (var setting in machineSettings)
            {
                var newSlider = Instantiate(UISliderPrefab, newBlock.transform);
                newSlider.name = setting.setting_id;
                newSlider.transform.Find("SliderTitle").GetComponent<TextMeshProUGUI>().text = setting.title;

                var sliderComponent = newSlider.transform.Find("Slider").GetComponent<Slider>();
                sliderComponent.minValue = setting.min;
                sliderComponent.maxValue = setting.max;
                sliderComponent.onValueChanged.AddListener((value) =>
                {
                    // Update the slider value text
                    newSlider.transform.Find("Slidervalue").GetComponent<TextMeshProUGUI>().text = value.ToString();

                    // Update the setting for the selected object
                    if (selectedObject != null)
                    {
                        GameObject.FindObjectOfType<ObjectCreator>().UpdateSetting(selectedObject, setting.setting_id, value);
                    }
                });
            }
        }
    }

    public void UpdateSliders(ObjectData objectData)
    {
        // Update the RunTitle with the title of the selected object
        Transform runTitleTransform = GameObject.Find("Canvas/Panel/Header/RunTitle").transform;
        if (runTitleTransform != null)
        {
            TextMeshProUGUI runTitle = runTitleTransform.GetComponent<TextMeshProUGUI>();
            if (runTitle != null)
            {
                runTitle.text = objectData.title;
            }
            else
            {
                Debug.LogError("No TextMeshProUGUI component found on the GameObject: " + runTitleTransform.name);
            }
        }
        else
        {
            Debug.LogError("Could not find a GameObject with the name: Canvas/Panel/Header/RunTitle");
        }

        Debug.Log("Updating sliders for " + objectData.title);
        // Update each slider value based on the object settings
        foreach (var setting in objectData.settings)
        {
            //Debug.Log("Updating slider " + setting.key + " to " + setting.value);
            Transform sliderTransform = RecursiveFind(UIGroup, setting.key);
            if (sliderTransform != null)
            {
                GameObject sliderParent = sliderTransform.gameObject;
                Transform sliderChildTransform = sliderParent.transform.Find("Slider");
                if (sliderChildTransform != null)
                {
                    Slider slider = sliderChildTransform.GetComponent<Slider>();
                    if (slider != null)
                    {
                        slider.value = setting.value;
                        slider.onValueChanged.AddListener(delegate { UpdateObjectData(setting.key, slider.value); });
                    }
                    else
                    {
                        Debug.LogError("No Slider component found on the GameObject: " + sliderChildTransform.name);
                    }
                }
                else
                {
                    Debug.LogError("No child named 'Slider' found on the GameObject: " + sliderParent.name);
                }
                
            }
            else
            {
                Debug.LogError("Could not find a slider parent with the name: " + setting.key);
            }
        }


        // Save the selected object
        selectedObject = objectData;
    }

    private Transform RecursiveFind(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var result = RecursiveFind(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    // When updating the sliders, this function is called to update the object
    public void UpdateObjectData(string key, float value)
    {
        if (selectedObject != null)
        {
            var setting = selectedObject.settings.FirstOrDefault(s => s.key == key);
            if (setting != null)
            {
                setting.value = value;
                Debug.Log("Updated object data: " + key + " to " + value);
            }
        }
    }

}


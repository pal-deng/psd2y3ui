using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;
using UGF.EditorTools.Psd2UGUI;
using static System.Net.Mime.MediaTypeNames;

public class Y3UIExportHelper : MonoBehaviour
{
    private static string _exportPath = "";
    public static void ExportSelectedUIToJson(string iconFolder, GameObject selectedObject, string exportPath)
    {
        if (selectedObject == null)
        {
            Debug.LogError("No object selected.");
            return;
        }

        _exportPath = exportPath;

        y3IconPath = iconFolder;
        var y3UIData = new Dictionary<string, object>();

        y3UIData.Add("adapt_mode", 2);
        y3UIData.Add("anim_data", new Dictionary<string, object>());

        var childrenUIData = new List<object>();
        AddChildInfo(selectedObject, childrenUIData);
        y3UIData.Add("children", childrenUIData);

        y3UIData.Add("name", selectedObject.name);
        y3UIData.Add("opacity", 1.0);
        y3UIData.Add("type", 2);
        y3UIData.Add("visible", true);

        y3UIData.Add("uid", GenerateUid());
        y3UIData.Add("zorder", 400);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented, // 设置为自动换行
        };
        string json = JsonConvert.SerializeObject(y3UIData, settings);
        SaveJsonToFile(selectedObject.name, json);
    }

    private const float ScreenX = 1920;
    private const float ScreenY = 1082;
    private const float LabelSizeAdjustW = 30;
    private const float LabelSizeAdjustH = 10;
    static void AddChildInfo(GameObject obj, List<object> list)
    {
        if (obj == null)
            return;

        var childrenCount = obj.transform.childCount;

        if (childrenCount == 0)
            return;

        for(int i=0; i<childrenCount; i++)
        {
            var child = obj.transform.GetChild(i);
            if (child.gameObject.GetInstanceID() == obj.GetInstanceID())
                continue;

            var childData = new Dictionary<string, object>();

            var childrenInfo = new List<object>();
            AddChildInfo(child.gameObject, childrenInfo);
            childData.Add("children", childrenInfo);
            childData.Add("event_list", new List<object>());
            childData.Add("name", child.name);
            childData.Add("uid", GenerateUid());
            childData.Add("opacity", 1.0);

            var sizeW = 100f;
            var sizeH = 50f;

            var type = GetComponentType(child);
            childData.Add("type", type);
            if (type == ComponentType.Text)
            {
                var textData = new Dictionary<string, object>();
                textData.Add("__tuple__", true);
                var items = new List<object>();
                var textUI = child.GetComponent<UnityEngine.UI.Text>();
                if (null == textUI)
                    continue;

                sizeW = textUI.rectTransform.sizeDelta.x;
                sizeH = textUI.rectTransform.sizeDelta.y;
                items.Add(textUI.text);
                items.Add(false);
                textData.Add("items", items);
                childData.Add("text", textData);
            }
            else if (type == ComponentType.Image)
            {
                var imageUI = child.GetComponent<UnityEngine.UI.Image>();
                if (null == imageUI)
                    continue;
                sizeW = imageUI.rectTransform.sizeDelta.x;
                sizeH = imageUI.rectTransform.sizeDelta.y;
                childData.Add("image", FindImageKey(child.name));
            }

            childData.Add("visible", true);
            childData.Add("zorder", 400);
            childData.Add("adapt_mode", 2);
            childData.Add("prefab_sub_key", null);
            childData.Add("scene_ui_name", null);

            var posData = new Dictionary<string, object>();
            posData.Add("__tuple__", true);
            var posDetail = new float[6];
            posDetail[0] = child.localPosition.x;
            posDetail[1] = child.localPosition.y;

            var screenRatioX = (child.localPosition.x + ScreenX / 2f) / ScreenX * 100;
            var screenRatioY = (child.localPosition.y + ScreenY / 2f) / ScreenY * 100;
            posDetail[2] = screenRatioX;
            posDetail[3] = screenRatioY;
            posDetail[4] = 1;
            posDetail[5] = 1;
            posData.Add("items", posDetail);
            childData.Add("pos_data", posData);

            var sizeData = new Dictionary<string, object>();
            sizeData.Add("__tuple__", true);
            var sizeDetail = new float[6];
            var childRectTransform = child.GetComponent<RectTransform>();
            if (null != childRectTransform)
            {
                sizeDetail[0] = childRectTransform.rect.width + LabelSizeAdjustW;
                sizeDetail[1] = childRectTransform.rect.height + LabelSizeAdjustH;
            }
            else 
            {
                sizeDetail[0] = ScreenX;
                sizeDetail[1] = ScreenY;
            }
            sizeData.Add("items", sizeDetail);
            childData.Add("size", sizeData);

            list.Add(childData);
        }
    }

    static string GenerateUid()
    {
        Guid guid = Guid.NewGuid();
        string uid = guid.ToString("N");
        return uid.Substring(0, 8) + "-" +
               uid.Substring(8, 4) + "-" +
               uid.Substring(12, 4) + "-" +
               uid.Substring(16, 4) + "-" +
               uid.Substring(20, 12);
    }


    public class ComponentType
    {
        public const int Button = 1;
        public const int Layer = 2;
        public const int Text = 3;
        public const int Image = 4;
        public const int LayoutElement = 7;
        public const int Scrollbar = 10;
    }

    static int GetComponentType(Transform uiObj)
    {
        var buttonComponent = uiObj.GetComponent<ButtonHelper>();
        if (buttonComponent != null) 
            return ComponentType.Button;

        var textComponent = uiObj.GetComponent<TextHelper>();
        if (textComponent != null)
            return ComponentType.Text;
        var uiTextComponent = uiObj.GetComponent<UnityEngine.UI.Text>();
        if (uiTextComponent != null)
            return ComponentType.Text;

        var imageComponent = uiObj.GetComponent<ImageHelper>();
        if (imageComponent != null)
            return ComponentType.Image;
        var uiImageComponent = uiObj.GetComponent<UnityEngine.UI.Image>();
        if (uiImageComponent != null)
            return ComponentType.Image;

        /*
        var layoutElementComponent = uiObj.GetComponent<UnityEngine.UI.LayoutElement>();
        if (layoutElementComponent != null)
            return ComponentType.LayoutElement;
        */

        var scrollbarComponent = uiObj.GetComponent<ScrollViewHelper>();
        if (scrollbarComponent != null)
            return ComponentType.Scrollbar;

        return ComponentType.LayoutElement;
    }

    private static string y3IconPath = "";
    static int FindImageKey(string imageFileName)
    {
        // var y3IconPath = "D:\\y3\\games\\2.0\\game\\LocalData\\codeUI\\editor_table\\editoricon";
        if (!Directory.Exists(y3IconPath))
        {
            Debug.LogError($"The directory '{y3IconPath}' does not exist.");
            return -1;
        }

        var resultInt = 0;
        foreach (string file in Directory.EnumerateFiles(y3IconPath, "*.json"))
        {
            try
            {
                string fileContent = File.ReadAllText(file);
                var temp = JsonConvert.DeserializeObject<Hashtable>(fileContent);

                if (temp != null && temp["name"] != null && temp["name"].ToString() == imageFileName)
                {
                    var resultStr = temp["key"].ToString();
                    int.TryParse(resultStr, out resultInt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing file {file}: {ex.Message}");
            }
        }
        return resultInt;
    }


    static void SaveJsonToFile(string fileName, string json)
    {
        string filePath = string.IsNullOrEmpty(_exportPath) ? Path.Combine(UnityEngine.Application.dataPath, $"{fileName}.json") :
            Path.Combine(_exportPath, $"{fileName}.json");
        File.WriteAllText(filePath, json);
        Debug.Log($"JSON file has been saved to: {filePath}");
    }
}

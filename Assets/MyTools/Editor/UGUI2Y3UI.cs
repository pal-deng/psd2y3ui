using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UGUI2Y3UI : Editor
{
    [MenuItem("GameObject/Y3UI/GenerateJson")]
    public static void Converter()
    {
        var iconPath = "D:\\psd2ugui\\editoricon";
        var exportPath = Application.dataPath + "/../demo";
        Y3UIExportHelper.ExportSelectedUIToJson(iconPath, Selection.activeGameObject, exportPath);
    }
}

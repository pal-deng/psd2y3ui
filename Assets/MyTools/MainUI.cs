using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public Text UserName;
    public Button LoginBtn;
    public Button GenerateBtn;
    public TMP_InputField IconFolderInput;
    public TMP_InputField UrlInput;
    public TMP_InputField ExportInput;

    public Text RunResult;

    void Awake()
    {
        Screen.SetResolution(600, 800, false);

        LoadDefaultValue();
        /*

        LoginBtn.onClick.AddListener(OnClickLoginBtn);
        GenerateBtn.onClick.AddListener(OnClickGenerateBtn);

        FCU.Events.OnProjectDownloaded.AddListener(OnDownloadComplete);
        FCU.Events.OnImportComplete.AddListener(OnImportComplete);

        UrlInput.onValueChanged.AddListener(OnUrlInputChanged);
        IconFolderInput.onValueChanged.AddListener(OnIconFolderInputChanged);
        ExportInput.onValueChanged.AddListener(OnExportFolderInputChanged);
        */
    }

    private void LoadDefaultValue()
    {
        var defaultIconFolder = PlayerPrefs.GetString("iconFolder", "");
        IconFolderInput.text = defaultIconFolder;

        var defaultUrl = PlayerPrefs.GetString("url", "");
        UrlInput.text = defaultUrl;

        var defaultExportFolder = PlayerPrefs.GetString("exportFolder", "");
        ExportInput.text = defaultExportFolder;
    }

    private void OnUrlInputChanged(string value)
    {
        PlayerPrefs.SetString("url", UrlInput.text);
    }
    private void OnIconFolderInputChanged(string value)
    {
        PlayerPrefs.SetString("iconFolder", IconFolderInput.text);
    }
    private void OnExportFolderInputChanged(string value)
    {
        PlayerPrefs.SetString("exportFolder", ExportInput.text);
    }

    private void Update()
    {
        if (UserName.text.Equals(""))
        {
            /*
            if (FCU.FigmaSession.IsAuthed())
            {
                UserName.text = FCU.FigmaSession.CurrentFigmaUser.Name;
            }
            */
        }
    }

    /*
    private void OnDownloadComplete(FigmaConverterUnity fcu)
    {
        Debug.Log("OnDownloadComplete");
        RunResult.text = "OnDownloadComplete";
        foreach (var child in FCU.InspectorDrawer.SelectableDocument.Childs)
        {
            child.Selected = true;
        }

        RunResult.text = "准备转化";
        FCU.EventHandlers.ImportSelectedFrames_OnClick((result) => {
            RunResult.text = result;
        });
    }

    private void OnImportComplete(FigmaConverterUnity fcu)
    {
        Debug.Log("OnImportComplete");
        RunResult.text = "OnImportComplete 1";

        var uiRoot = fcu.transform.GetChild(0);
        var iconFolder = IconFolderInput.text;
        var exportPath = ExportInput.text;
        Y3UIExportHelper.ExportSelectedUIToJson(iconFolder, uiRoot.gameObject, exportPath);

        RunResult.text = "转化完成！";
    }
    */
}

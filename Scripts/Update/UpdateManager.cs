﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SLua;

public class UpdateManager : Singleton<UpdateManager>
{
    public enum UpdateFileStateType
    {
        BeginDownLoad,
        FinishDownload,
        MoveFile,
        Finished
    }

    public enum DownloadFileType
    {
        TypeText,
        TypeAssetBundle,
        None
    }

    public class DownloadFileRequest : BaseObject
    {
        public int Id;
        public string fileUrl;
        public string filePath;
        public DownloadFileType fileType;
        public OnScriptDownloadFinishedEvent onScriptDownloaded;
		public bool isSaved;

        public override void Reset()
		{
			Id = 0;
			fileUrl = "";
			filePath = "";
			fileType = DownloadFileType.None;
			onScriptDownloaded = null;
			isSaved = false;
		}
    }

    public delegate void OnScriptDownloadFinishedEvent(string script);
    public delegate void OnUpdateStateChangedEvent(string filePath, UpdateFileStateType updateState);
#if UNITY_EDITOR
	public static string UpdateTest = Application.streamingAssetsPath;
#else
    #if UNITY_ANDROID
	public static string UpdateTest = "jar:file://" + Application.dataPath + "/!/assets";
    #endif
#endif

    public OnUpdateStateChangedEvent OnUpdateStateChanged = null;
    List<DownloadFileRequest> _downloadFileList = new List<DownloadFileRequest>();
    UpdateFileStateType _state = UpdateFileStateType.Finished;
    string _currentUpdateFilePath = "";

    public UpdateFileStateType State
    {
        get 
        {
            return _state;
        }
        set
        {
            _state = value;
            
            if (OnUpdateStateChanged != null)
                OnUpdateStateChanged(System.IO.Path.GetFileName(_currentUpdateFilePath), _state);

            if (value == UpdateFileStateType.Finished)
            {
                StartDownloadFile();
            }
        }
    }

    public static UpdateManager GetInstance()
    {
        return Instance;
    }

    public void Download(string url, string targetPath, DownloadFileType fileType, 
	                     OnScriptDownloadFinishedEvent downloadedEvent = null, bool isSaved = false)
    {
		var file = PoolManager.GetInstance().Get<DownloadFileRequest>("DownloadFileRequest");
        file.Id = System.DateTime.Now.Millisecond;
        file.fileUrl = url;
        file.filePath = targetPath;
        file.fileType = fileType;
        file.onScriptDownloaded = downloadedEvent;
		file.isSaved = isSaved;
        _downloadFileList.Add(file);

		StartDownloadFile();
    }

    void StartDownloadFile()
    {
        if (State != UpdateFileStateType.Finished)
            return;

        if (_downloadFileList.Count <= 0)
            return;

        var req = _downloadFileList[0];
        _currentUpdateFilePath = req.filePath;
        State = UpdateFileStateType.BeginDownLoad;
        StartCoroutine(DownloadFile(req));
    }

    IEnumerator DownloadFile(DownloadFileRequest req)
    {
        WWW www = new WWW(req.fileUrl);
        yield return www;
        State = UpdateFileStateType.FinishDownload;

		if (req.isSaved) 
		{
			State = UpdateFileStateType.MoveFile;
			MoveFile(req, www.text, www.bytes, www.bytes.Length);
		}

		_downloadFileList.Remove(req);
        State = UpdateFileStateType.Finished;

		if (req.onScriptDownloaded != null)
		{
			req.onScriptDownloaded(www.text);
		}
    }

    void MoveFile(DownloadFileRequest req, string info, byte[] bytes, int length)
    {
        if (req.fileType == DownloadFileType.TypeText)
        {
            FileManager.CreateFileWithString(req.filePath, info);
        }
        else if (req.fileType == DownloadFileType.TypeAssetBundle)
        {
            FileManager.CreateFileWithBytes(req.filePath, bytes, length);
        }
    }
}

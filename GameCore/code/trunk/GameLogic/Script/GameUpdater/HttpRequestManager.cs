using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Unity.SharpZipLib.Checksum;
using UnityEngine;

public delegate void ProgressHandler(long loadedsize);
public delegate void ErrorHandler(string message);
public delegate void CompleteHandler();

public class HttpRequestManager : ManagerTemplate<HttpRequestManager>
{
	public static string errorMsg = "";

	public static bool isError { get; private set; }

	public static bool isDone { get => m_currHttpRequest != null ? m_currHttpRequest.IsDone : true; }

	public static long loadedSize { get => m_currHttpRequest != null ? m_currHttpRequest.LoadedFileSize : (m_requestQueue.Count >= 0 ? m_requestQueue.Peek().LoadedFileSize : 0); }

	public static long totalSize { get => m_currHttpRequest != null ? m_currHttpRequest.RequestFileSize : (m_requestQueue.Count >= 0 ? m_requestQueue.Peek().RequestFileSize : 0); }

	public static string errorMessage { get { return errorMsg; } }

	private static Queue<HttpRequest> m_requestQueue;          //下载队列
	private static HttpRequest m_currHttpRequest = null;
	protected override void InitManager()
	{
		m_requestQueue = new Queue<HttpRequest>();

	}

	public static void EnqueueRequest(RequestFileInfo info, string requesturl, string savepath)
	{
		HttpRequest httpRequest = new HttpRequest(info, requesturl, savepath);
		m_requestQueue.Enqueue(httpRequest);
	}

	private static HttpRequest DequeueRequest()
	{
		if (m_requestQueue.Count > 0)
			return m_requestQueue.Dequeue();
		return null;
	}

	public static void StartRequest()
	{
		m_currHttpRequest = DequeueRequest();
		if (m_currHttpRequest != null)
			m_currHttpRequest.Start(Complete, Error);
	}

	private static void Error(string message)
	{
		isError = true;
		errorMsg = message;
	}

	private static void Complete()
	{
		StartRequest();
	}

	public static void Clear()
	{
		if (m_currHttpRequest != null)
		{
			m_currHttpRequest.DisposeExtend();
		}
		if(m_requestQueue != null)
			m_requestQueue.Clear();
		isError = false;
		errorMsg = "";
	}

	public static string GetMessage(Dictionary<string, string> logConfig, out float process)
	{
		if (m_currHttpRequest != null)
		{
			return m_currHttpRequest.GetMessage(logConfig, out process);
		}

		process = 0.0f;
		return "";
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Clear();
	}
}

/// <summary>
/// http请求
/// </summary>
public class HttpRequest : IDisposeExtend
{
	enum RequestStatus
	{
		None,
		Request,
		DownLoading,
		Checking,
		Retry,
		Success,
		Failed,
	}

	private RequestState requestState;
	private RequestFileInfo requestFileInfo;
	public Crc32 CrcCoding = new Crc32();

	private CompleteHandler requestComplete;
	private ErrorHandler requestError;

	private RequestStatus Status = RequestStatus.None;
	private string FileName = "";

	public long RequestFileSize = 0;

	public long LoadedFileSize = 0;

	public bool IsDone => Status == RequestStatus.Success;

	private const int ERROR_COUNT = 60;
	private const int ERROR_DELAY = 3 * 1000;


	public HttpRequest(RequestFileInfo fileinfo, string requestUrl, string savePath)
	{
		requestFileInfo = fileinfo;
		RequestFileSize = fileinfo.size;
		Status = RequestStatus.None;
		FileName = Path.GetFileNameWithoutExtension(fileinfo.name);

		string httpUrl = string.Format("{0}/{1}?{2}", requestUrl, fileinfo.name, System.DateTime.Now.Ticks);
		string saveFile = Path.Combine(savePath, fileinfo.name);

		ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

		if (!Directory.Exists(savePath))
			Directory.CreateDirectory(savePath);

		try
		{
			requestState = RequestState.Create(httpUrl, saveFile);

			LoadedFileSize = requestState.FileStream.Length;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			Error("httpManager checkFull Error: " + e.Message + e.StackTrace);
		}
	}

	private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
	{
		return true; //总是接受  
	}

	public long GetLocalFileSize()
	{
		if (requestState == null || requestState.FileStream == null)
			return 0;

		requestState.FileStream.Flush(true);
		return requestState.FileStream.Length;
	}

	public void Start(CompleteHandler complete, ErrorHandler error)
	{
		try
		{
			requestComplete = complete;
			requestError = error;

			requestState.ErrorTest = 0;
			requestState.ErrorCrcTest = 0;
			Status = RequestStatus.Request;

			requestState.Request.BeginGetResponse(new AsyncCallback(ResponseCallback), requestState);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			Error("httpManager start Error: " + e.Message + e.StackTrace);
		}
	}

	/// <summary>  
	/// 请求资源方法的回调函数  
	/// </summary>  
	/// <param name="asyncResult">用于在回调函数当中传递操作状态</param>  
	private void ResponseCallback(IAsyncResult asyncResult)
	{
		RequestState requestState = (RequestState)asyncResult.AsyncState;

		if (requestState.IsClose)
			return;

		try
		{
			HttpWebResponse webResponse = (HttpWebResponse)requestState.Request.EndGetResponse(asyncResult);

			Stream response = webResponse.GetResponseStream();
			requestState.ResponseStream = response;

			LoadedFileSize = requestState.FileStream.Length;
			RequestFileSize = LoadedFileSize + response.Length;

			Status = RequestStatus.DownLoading;

			//开始异步读取流  
			response.BeginRead(requestState.BufferRead, 0, requestState.BufferRead.Length, ReadCallback, requestState);
			return;
		}
		catch (WebException ex)
		{
			if (ex.Message.IndexOf("(416)") >= 0)
			{
				DownloadSuccess();
				return;
			}

			Debug.LogException(ex);

			if (requestState.ErrorTest >= ERROR_COUNT)
			{
				Error("httpManager ResponseCallback0 Error: " + ex.Message + ex.StackTrace);
				return;
			}
		}
		catch (Exception e)
		{
			Debug.LogException(e);

			if (requestState.ErrorTest >= ERROR_COUNT)
			{
				Error("httpManager ResponseCallback1 Error: " + e.Message + e.StackTrace);
				return;
			}
		}

		RetryError(requestState);
	}

	private void RetryError(RequestState requestState)
	{
		try
		{
			requestState.Close();
			Thread.Sleep(ERROR_DELAY);

			Status = RequestStatus.Retry;
			requestState.ErrorTest++;
			Debug.LogFormat("RetryError :{0}", requestState.ErrorTest);

			//创建一个初始化请求对象  
			requestState.ResetRequest();

			requestState.Request.BeginGetResponse(new AsyncCallback(ResponseCallback), requestState);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			Error("httpManager RetryError: " + e.Message + e.StackTrace);
		}
	}

	/// <summary>  
	/// 异步读取流的回调函数  
	/// </summary>  
	/// <param name="asyncResult">用于在回调函数当中传递操作状态</param>  
	private void ReadCallback(IAsyncResult asyncResult)
	{
		RequestState requestState = (RequestState)asyncResult.AsyncState;

		if (requestState.IsClose)
			return;

		try
		{
			FileStream file = requestState.FileStream;
			Stream response = requestState.ResponseStream;

			int read = response.EndRead(asyncResult);

			if (asyncResult.CompletedSynchronously)
				Debug.LogFormat("read0:{0} IsCompleted:{1} CompletedSynchronously:{2}", read, asyncResult.IsCompleted, asyncResult.CompletedSynchronously);

			if (read <= 0)
			{
				Debug.LogFormat("read1:{0} IsCompleted:{1} CompletedSynchronously:{2}", read, asyncResult.IsCompleted, asyncResult.CompletedSynchronously);
				if (asyncResult.CompletedSynchronously)
				{
					DownloadSuccess();
				}
				else
				{
					RetryError(requestState);
				}
				return;
			}

			//将缓冲区的数据写入该文件流  
			file.Write(requestState.BufferRead, 0, read);

			LoadedFileSize += read;

			//开始异步读取流  
			IAsyncResult result = response.BeginRead(requestState.BufferRead, 0, requestState.BufferRead.Length, ReadCallback, requestState);

			requestState.ErrorTest = 0;

			// 小包下载的时候减少网络带宽
			if (ConfigManager.isDone)
			{
				Thread.Sleep(20);
			}
			else if (!result.AsyncWaitHandle.WaitOne(1 * 1000, false))
			{
				Debug.LogError("result.AsyncWaitHandle.WaitOne time out");
				RetryError(requestState);
			}

			return;
		}
		catch (Exception e)
		{
			Debug.LogException(e);

			if (requestState.ErrorTest >= ERROR_COUNT)
			{
				Error("httpManager ReadCallback1 Error: " + e.Message + e.StackTrace);
				return;
			}
		}

		RetryError(requestState);
	}

	private void Error(string message)
	{
		if (requestState != null)
			requestState.Close();
		Status = RequestStatus.Failed;

		if (requestError != null)
			requestError(message);
	}

	private void DownloadSuccess()
	{
		try
		{
			requestState.Close();
			CrcCoding.Reset();

			Status = RequestStatus.Checking;

			FileStream file = File.OpenRead(requestState.SavePath);
			requestState.FileStream = file;
			RequestFileSize = file.Length;
			LoadedFileSize = 0;

			file.BeginRead(requestState.BufferRead, 0, requestState.BufferRead.Length, CrcCallback, requestState);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);

			File.Delete(requestState.SavePath);
			requestState.ErrorTest = 0;
			RetryError(requestState);
		}
	}

	public void CrcCallback(IAsyncResult asyncResult)
	{
		RequestState requestState = (RequestState)asyncResult.AsyncState;

		try
		{
			FileStream file = requestState.FileStream;
			int read = file.EndRead(asyncResult);
			LoadedFileSize += read;

			CrcCoding.Update(requestState.BufferRead);

			if (asyncResult.CompletedSynchronously)
				Debug.LogFormat("read0:{0} IsCompleted:{1} CompletedSynchronously:{2}", read, asyncResult.IsCompleted, asyncResult.CompletedSynchronously);

			if (read > 0)
			{
				file.BeginRead(requestState.BufferRead, 0, requestState.BufferRead.Length, CrcCallback, requestState);
				return;
			}

			long crc = CrcCoding.Value;
			CrcCoding.Reset();

			file.Close();
			if (requestFileInfo.crc == crc)
			{
				Complete();
				return;
			}

			if (requestState.ErrorCrcTest >= 1)
			{
				Complete();
				return;
			}

			requestState.ErrorCrcTest++;
			File.Delete(requestState.SavePath);
			requestState.ErrorTest = 0;
			RetryError(requestState);
		}
		catch (Exception e)
		{
			Debug.LogException(e);

			File.Delete(requestState.SavePath);
			requestState.ErrorTest = 0;
			RetryError(requestState);
		}
	}

	private void Complete()
	{
		Status = RequestStatus.Success;

		if (requestComplete != null)
			requestComplete();
	}


	public string GetMessage(Dictionary<string, string> logConfig, out float process)
	{
		switch (Status)
		{
		case RequestStatus.Request:
			process = ((float)LoadedFileSize) / ((float)RequestFileSize);
			return string.Format(logConfig["10010"], FileName, LoadedFileSize / 1024, RequestFileSize / 1024);
		case RequestStatus.DownLoading:
			process = ((float)LoadedFileSize) / ((float)RequestFileSize);
			return string.Format(logConfig["10005"], LoadedFileSize / 1024, RequestFileSize / 1024, FileName);
		case RequestStatus.Checking:
			process = ((float)LoadedFileSize) / ((float)RequestFileSize);
			return string.Format(logConfig["10009"], FileName, LoadedFileSize / 1024, RequestFileSize / 1024);
		case RequestStatus.Retry:
			process = ((float)requestState.ErrorTest / (float)ERROR_COUNT);
			return string.Format(logConfig["10011"], FileName, requestState.ErrorTest, ERROR_COUNT);
		case RequestStatus.Success:
			process = 1.0f;
			break;
		case RequestStatus.Failed:
			process = 1.0f;
			break;
		default:
			process = 0.0f;
			break;
		}

		return "";
	}



	public void DisposeExtend()
	{
		requestState.Close();
	}
}

public class RequestState
{
	/// <summary>  
	/// 缓冲区大小  
	/// </summary>  
	public const int BUFFER_SIZE = 1024 * 512;

	/// <summary>  
	/// 缓冲区  
	/// </summary>  
	public byte[] BufferRead = new byte[BUFFER_SIZE];

	public int ErrorTest = 0;
	public int ErrorCrcTest = 0;

	public string httpUrl;

	/// <summary>  
	/// 保存路径  
	/// </summary>  
	public string SavePath;

	/// <summary>  
	/// 请求流  
	/// </summary>  
	public HttpWebRequest Request;

	/// <summary>  
	/// 流对象  
	/// </summary>  
	public Stream ResponseStream;

	/// <summary>  
	/// 文件流  
	/// </summary>  
	public FileStream FileStream;

	// 当关闭 WebResponse 的时候会回调 ReadCallBack
	public bool IsClose = false;

	public static RequestState Create(string httpUrl, string savePath)
	{
		RequestState state = new RequestState();

		//设置下载相关参数  
		state.httpUrl = httpUrl;
		state.SavePath = savePath;

		state.ResetRequest();
		return state;
	}

	public void ResetRequest()
	{
		Debug.Assert(Request == null);
		Debug.Assert(FileStream == null);
		Debug.Assert(ResponseStream == null);

		Uri uri = new Uri(httpUrl);
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

		//创建一个初始化请求对象  
		if (httpUrl.StartsWithNonAlloc("https"))
			request.ProtocolVersion = HttpVersion.Version10;

		request.KeepAlive = false;
		request.ReadWriteTimeout = 1000 * 20;

		FileStream file = File.OpenWrite(SavePath);
		long pos = file.Seek(0, SeekOrigin.End);
		request.AddRange(pos);

		this.FileStream = file;
		this.Request = request;

		IsClose = false;
	}

	public void Close()
	{
		IsClose = true;

		if (FileStream != null)
		{
			try
			{
				FileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			FileStream = null;
		}

		if (ResponseStream != null)
		{
			try
			{
				ResponseStream.Close();

				var response = Request.GetResponse();

				if (response != null)
					response.Close();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			ResponseStream = null;
		}

		if (Request != null)
		{
			try
			{
				Request.Abort();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}

			Request = null;
		}
	}
}
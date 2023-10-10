using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class HttpController : MonoBehaviour
{
    // Spring ���� URL
    private string _url = "https://j9b101.p.ssafy.io/api";

    public string URL
    {
        get { return _url; }
    }

    // HTTP GET ��û �ڷ�ƾ �޼ҵ�
    public IEnumerator DoGet<T>(string url, Action<CommonResponse<T>> success, Action<ErrorResponse> fail)
    {
        // request ����
        using UnityWebRequest request = new UnityWebRequest(url, "GET");

        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();

        // ���� ���
        yield return request.SendWebRequest();

        // ��û ���� ���п� ���� �ݹ� �Լ��� ����� �־� ����
        if (request.result == UnityWebRequest.Result.Success)
        {
            success(JsonConvert.DeserializeObject<CommonResponse<T>>(request.downloadHandler.text));
        }
        else
        {
            fail(JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text));
        }

    }

    // HTTP POST ��û �ڷ�ƾ �޼ҵ�
    public IEnumerator DoPost<T, S>(string url, T data, Action<CommonResponse<S>> success, Action<ErrorResponse> fail)
    {


        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        // ��ü �����͸� ��ü -> JSON -> Bytes �� ��ȯ
        string json = JsonConvert.SerializeObject(data);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            success(JsonConvert.DeserializeObject<CommonResponse<S>>(request.downloadHandler.text));
        }
        else
        {
            fail(JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text));
        }
    }
}

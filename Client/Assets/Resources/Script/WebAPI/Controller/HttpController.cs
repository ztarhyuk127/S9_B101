using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class HttpController : MonoBehaviour
{
    // Spring 서버 URL
    private string _url = "https://j9b101.p.ssafy.io/api";

    public string URL
    {
        get { return _url; }
    }

    // HTTP GET 요청 코루틴 메소드
    public IEnumerator DoGet<T>(string url, Action<CommonResponse<T>> success, Action<ErrorResponse> fail)
    {
        // request 생성
        using UnityWebRequest request = new UnityWebRequest(url, "GET");

        request.SetRequestHeader("Content-Type", "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();

        // 응답 대기
        yield return request.SendWebRequest();

        // 요청 성공 실패에 따라 콜백 함수에 결과를 넣어 실행
        if (request.result == UnityWebRequest.Result.Success)
        {
            success(JsonConvert.DeserializeObject<CommonResponse<T>>(request.downloadHandler.text));
        }
        else
        {
            fail(JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text));
        }

    }

    // HTTP POST 요청 코루틴 메소드
    public IEnumerator DoPost<T, S>(string url, T data, Action<CommonResponse<S>> success, Action<ErrorResponse> fail)
    {


        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        // 객체 데이터를 객체 -> JSON -> Bytes 로 변환
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

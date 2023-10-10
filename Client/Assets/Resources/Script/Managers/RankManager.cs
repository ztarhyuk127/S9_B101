using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class RankManager : HttpController
{
    [SerializeField] Transform _container;
    [SerializeField] GameObject _rankItemPrefab;
    [SerializeField] Color firstColor;
    [SerializeField] Color secondColor;
    [SerializeField] Color thirdColor;

    [SerializeField] GameObject _previous;
    [SerializeField] GameObject _next;

    private int _page = 0;
    private int _size = 5;
    private bool _isTopRank = true;

    public void Save(int recordTime, List<int> userIds, Action<int> callback)
    {
        SaveRecordRequest request = new SaveRecordRequest();
        request.recordTime = recordTime;
        request.userIds = userIds;

        StartCoroutine(DoPost<SaveRecordRequest, int>(URL + "/record/save", request,
            resp =>
            {
                Debug.Log(resp.body);
                callback(resp.body);
            },
            error =>
            {
            }));

    }

    // 전체 랭킹
    public void TopRank()
    {
        StartCoroutine(DoGet<List<RecordDto>>(URL + $"/record/rank?page={_page}&size={_size}",
            resp =>
            {
                ConstructResultScreen(resp.body);
            },
            err =>
            {
                Debug.Log("탑 랭크 가져오기 실패");

            }));
    }

    // 내 최고 랭킹
    public void MyRank()
    {
        string nickname = PlayerPrefs.GetString("Nickname");
        int id = int.Parse(nickname[(nickname.LastIndexOf('#') + 1)..]);
        StartCoroutine(DoGet<RankedRecordResponse>(URL + $"/record/myrank/{id}", 
            resp =>
            {
                ConstructResultScreenRank(resp.body);
            },
            error =>
            {
                Debug.Log("내 랭크 가져오기 실패");
            }));
    }

    public void History()
    {
        string nickname = PlayerPrefs.GetString("Nickname");
        int id = int.Parse(nickname[(nickname.LastIndexOf('#') + 1)..]);
        StartCoroutine(DoGet<List<RecordDto>>(URL + $"/record/findbyuser?userId={id}&page={_page}&size={_size}",
            resp =>
            {
                ConstructResultScreenHistory(resp.body);
            },
            err =>
            {
                Debug.Log("내 히스토리 가져오기 실패");
            }));
    }

    public void GetRecordWithRank(int recordId)
    {
        StartCoroutine(DoGet<RankedRecordResponse>(URL + "/record/rank/" + recordId,
            resp =>
            {
                RankedRecordResponse body = resp.body;
                ConstructResultScreenRank(body);
            },
            error =>
            {
                Debug.Log(error);
            }
            ));
    }

    public void ConstructResultScreen(List<RecordDto> records)
    {
        int rank = _page * 5 + 1;

        foreach (RecordDto record in records)
        {
            int time = record.recordTime;
            List<RecordUserDto> userList = record.users;

            string users = "";

            for (int i = 0; i < userList.Count; i++)
            {
                users += userList[i].nickname;
                if (i != userList.Count - 1)
                {
                    users += "/";
                }
            }

            RankItemUI item = MakeRankItem(rank, time, users);
            rank++;
        }
    }

    public void ConstructResultScreenHistory(List<RecordDto> records)
    {
        foreach (RecordDto record in records)
        {
            int time = record.recordTime;
            List<RecordUserDto> userList = record.users;

            string users = "";

            for (int i = 0; i < userList.Count; i++)
            {
                users += userList[i].nickname;
                if (i != userList.Count - 1)
                {
                    users += "/";
                }
            }

            GameObject rankItem = Instantiate(_rankItemPrefab);
            rankItem.transform.SetParent(_container, false);

            RankItemUI rankItemUI = rankItem.GetComponent<RankItemUI>();

            rankItemUI.rank.text = record.recordHistory[..10];
            rankItemUI.rank.fontSize = 24;

            rankItemUI.time.text = IntToTime(time);

            rankItemUI.players.text = users;

        }
    }

    public void ConstructResultScreenRank(RankedRecordResponse body)
    {
        int myRecord = body.myRecordId;
        List<RankedRecordDto> records = body.records;

        foreach (RankedRecordDto record in records)
        {
            int rank = record.recordRank;
            int time = record.recordTime;
            List<RecordUserDto> userList = record.users;

            string users = "";

            for(int i=0; i<userList.Count; i++)
            {
                users += userList[i].nickname;
                if (i != userList.Count-1)
                {
                    users += "/";
                }
            }

            RankItemUI item = MakeRankItem(rank, time, users);
            
            if (myRecord == record.recordId)
            {
                item.rank.text = "MY";
                item.item.color = Color.yellow;
            }
        }
    }

    public RankItemUI MakeRankItem(int rank, int time, string users)
    {
        GameObject rankItem = Instantiate(_rankItemPrefab);
        rankItem.transform.SetParent(_container, false);

        RankItemUI rankItemUI = rankItem.GetComponent<RankItemUI>();

        string rankText = rank.ToString();
        switch (rank)
        {
            case 1:
                rankText += "st";
                rankItemUI.rank.color = firstColor;
                break;
            case 2:
                rankText += "nd";
                rankItemUI.rank.color = secondColor;
                break;
            case 3:
                rankText += "rd";
                rankItemUI.rank.color = thirdColor;
                break;
            default:
                rankText += "th";
                break;
        }
        rankItemUI.rank.text = rankText;

        rankItemUI.time.text = IntToTime(time);

        rankItemUI.players.text = users;

        return rankItemUI;
    }

    public string IntToTime(int time)
    {
        int sec = time;
        int milisec = sec % 100;
        sec /= 100;
        int min = sec / 60;
        sec %= 60;
        return $"{min}:{sec}:{milisec}";
    }

    public void OnClickTopRank()
    {
        _isTopRank = true;
        _previous.SetActive(true);
        _next.SetActive(true);
        _page = 0;
        ClearContainer();
        TopRank();
    }

    public void OnClickTopRankRight()
    {
        int count = _container.childCount;

        if (count < _size) return;

        _page++;
        ClearContainer();
        if (_isTopRank)
        {
            TopRank();
        }
        else
        {
            History();
        }
    }

    public void OnClickTopRankLeft()
    {
        _page--;
        if (_page < 0)
        {
            _page = 0;
            return;
        }

        ClearContainer();
        if (_isTopRank)
        {
            TopRank();
        }
        else
        {
            History();
        }
    }

    public void OnClickMyRank()
    {
        _previous.SetActive(false);
        _next.SetActive(false);
        ClearContainer();
        MyRank();
    }

    public void OnClickHistory()
    {
        _isTopRank = false;
        _previous.SetActive(true);
        _next.SetActive(true);
        ClearContainer();
        History();
    }

    public void ClearContainer()
    {
        // Transform은 이런식으로 자식에게 접근이 가능!
        foreach(Transform child in _container)
        {
            Destroy(child.gameObject);
        }
    }
}

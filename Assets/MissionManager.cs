using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    private static MissionManager instance; // 싱글톤이 할당될 static 변수

    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static MissionManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MissionManager>();

            return instance;
        }
    }
    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 MissionManager 오브젝트가 있다면 자신을 파괴
        if (Instance != this) Destroy(gameObject);
    }
    // 미션 컨트롤
    // 스타디움까지 좀비를 처치하면서 간 다음
    // 스타디움에서 2분동안 공성전
    // 성공하면 헬기가 구조

    // 좀비 스폰위치 변경으로 좀비를 스타디움에서만 나오게 함
    // 30초 전 헬기가 스타디움 쪽으로 오게 함
    // 시간을 측정하여 2분이 되면 헬기쪽에 밧줄을 내리고 근처로 가면
    // 플레이어를 헬기 속으로 넣고 카메라는 전경을 비추며
    // 헬기는 이동하며 도시를 탈출하는 씬 호출
    // 이후 게임오버

    public bool is_Stadium; // 스타디움까지 도착했는가
    public bool is_StadiumDef; // 스타디움에서 버티는데 성공하였는가
    public bool is_Clear; // 헬기 탑승 했는가

    float stadiumDefTime = 180;
    float checkTime;

    void Start()
    {
        checkTime = stadiumDefTime;
        is_Stadium = false;
        is_StadiumDef = false;
    }

    
    void Update()
    {
        if (is_Stadium)
        {
            Stadium_Defense();
        }
    }

    void Stadium_Defense() // 스타디움 디펜스
    {
        checkTime -= Time.deltaTime;
        UIManager.Instance.UpdateTimeText(Convert.ToInt32(checkTime / 60), Convert.ToInt32(checkTime % 60));
        if (checkTime <= 0)
        {
            is_StadiumDef = true;
        }
    }
}

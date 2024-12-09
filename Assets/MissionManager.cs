using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
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

    bool is_Stadium; // 스타디움까지 도착했는가
    bool is_StadiumDef; // 스타디움에서 버티는데 성공하였는가

    float stadiumDefTime = 180;
    float checkTime;

    void Start()
    {
        checkTime = 0;
    }

    
    void Update()
    {
        
    }

    void Stadium_Defense()
    {
        checkTime += Time.deltaTime;
        if (checkTime >= stadiumDefTime)
        {
            is_StadiumDef = true;
        }
    }
}

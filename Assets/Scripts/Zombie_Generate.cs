using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Generate : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] Zombie_Group; //좀비 무리 담아둘 곳
    public float Distance_Generate = 20.0f;//거리 변경시 변경할 것 
    public Transform[] Pos; // 좀비 젠 위치
    // Start is called before the first frame update
    void Start()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player"); //플레이어 빠지는거 방지
        }
    }

    // Update is called once per frame
    void Update()
    {
        //조건에 플레이어 정지상태를 넣고 싶음
        foreach (Transform p in Pos)
        {
            if(p.position != Vector3.zero && Vector3.Distance(Player.transform.position,p.position) < Distance_Generate) //만약 시간마다 다시 젠 시킬거면 첫번째 조건 주석
            {                                                                                                 
                StartCoroutine(Coroutine_Generate(p.position));
                p.position = Vector3.zero; //만약 시간마다 다시 젠 시킬거면 주석처리, 한번만 실행하도록 만든거
            }
        }
    }
    IEnumerator Coroutine_Generate(Vector3 pos, float time = 0.1f) //시간 지연시키고 싶으면 타임 변경
    {
        yield return new WaitForSeconds(time);
        Instantiate(Zombie_Group[Random.Range(0, Zombie_Group.Length-1)], pos, Quaternion.identity); //랜덤 스폰
        
    }
}

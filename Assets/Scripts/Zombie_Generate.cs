//zombie Generator
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie_Generate : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject[] Zombie; //좀비 담아둘 곳
    [SerializeField] private float Distance_Generate = 20.0f;//거리 변경시 변경할 것 
    public HashSet<Vector3> Area = new HashSet<Vector3> {}; // 젠 위치 근처 좀비 랜덤 스폰할 영역
    [SerializeField] private int Generate_Count = 0;
    [SerializeField] private int Max_Generate = 15; //최대 젠 영역 갯수, 만약 조정할거면 Set_Pos에 있는 Random.Range값이 경우의 수가 충분한지 확인 필요
    Zombie_Generate zombie_Generate;
    public bool is_generated = false;
    float time = 0.1f; //초기 시간값 건들면 안됨
    // Start is called before the first frame update
    void Awake()
    {
        zombie_Generate = GetComponent<Zombie_Generate>();
        if(Player == null)
        {
            Player = GameObject.Find("Player"); //플레이어 빠지는거 방지
        }
        Set_Pos(); //시작할 때 미리 랜덤젠 위치 설정*
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(Player.transform.position,transform.position)<Distance_Generate && !is_generated) //1. 플레이어와의 거리, 2. 소환횟수가 젠 수를 못벗어나도록 
        {
            StartCoroutine(Coroutine_Generate(time)); //생성코루틴 시작
            time = 20.0f; // 리젠 시간
            is_generated = true;
            zombie_Generate.enabled = false;
        }
    }
    void Set_Pos() //랜덤 젠 위치 설정
    {

        while (Area.Count != Max_Generate)
        {
            Vector3 RandPos = new Vector3(transform.position.x + Random.Range(-1, 4), 0, transform.position.z + Random.Range(-1, 4));
            if (!Area.Contains(RandPos)) //Area에 RandPos가 존재하지 않으면
            {
                Area.Add(RandPos);
            }
        }

    }


    IEnumerator Coroutine_Generate(float time = 0.1f) //좀비 생성
    {
        yield return new WaitForSeconds(time);
        
        //Generate_Count = 0; //리젠을 위해서 초기화
        
        foreach(var p in Area) //랜덤젠 스폰 위치
        {
            Instantiate(Zombie[Random.Range(0, Zombie.Length - 1)], p, Quaternion.identity); //랜덤으로 좀비 생성
            Generate_Count++; //코루틴 반복횟수 조절용 변수
        }
    }
}

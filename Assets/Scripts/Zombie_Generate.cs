using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie_Generate : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject[] Zombie; //���� ��Ƶ� ��
    [SerializeField] private float Distance_Generate = 20.0f;//�Ÿ� ����� ������ �� 
    public HashSet<Vector3> Area = new HashSet<Vector3> {}; // �� ��ġ ��ó ���� ���� ������ ����
    [SerializeField] private int Generate_Count = 0;
    [SerializeField] private int Max_Generate = 15; //�ִ� �� ���� ����, ���� �����ҰŸ� Set_Pos�� �ִ� Random.Range���� ����� ���� ������� Ȯ�� �ʿ�
    float time = 0.1f; //�ʱ� �ð��� �ǵ�� �ȵ�
    // Start is called before the first frame update
    void Start()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player"); //�÷��̾� �����°� ����
        }
        Set_Pos(); //������ �� �̸� ������ ��ġ ����*
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Area.Count);
        if(Vector3.Distance(Player.transform.position,transform.position)<Distance_Generate && Generate_Count < Area.Count) //1. �÷��̾���� �Ÿ�, 2. ��ȯȽ���� �� ���� ��������� 
        {
            StartCoroutine(Coroutine_Generate(time)); //�����ڷ�ƾ ����
            time = 20.0f; // ���� �ð�
        }
    }
    void Set_Pos() //���� �� ��ġ ����
    {

        while (Area.Count != Max_Generate)
        {
            Vector3 RandPos = new Vector3(transform.position.x + Random.Range(-1, 4), 0, transform.position.z + Random.Range(-1, 4));
            if (!Area.Contains(RandPos)) //Area�� RandPos�� �������� ������
            {
                Area.Add(RandPos);
            }
        }

    }


    IEnumerator Coroutine_Generate(float time = 0.1f) //���� ����
    {
        yield return new WaitForSeconds(time);
        Generate_Count = 0; //������ ���ؼ� �ʱ�ȭ
        foreach(var p in Area) //������ ���� ��ġ
        {
            Instantiate(Zombie[Random.Range(0, Zombie.Length - 1)], p, Quaternion.identity); //�������� ���� ����
            Generate_Count++; //�ڷ�ƾ �ݺ�Ƚ�� ������ ����
        }
        
        
    }
}

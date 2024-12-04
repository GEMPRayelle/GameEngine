using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Generate : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] Zombie_Group; //���� ���� ��Ƶ� ��
    public float Distance_Generate = 20.0f;//�Ÿ� ����� ������ �� 
    public Transform[] Pos; // ���� �� ��ġ
    // Start is called before the first frame update
    void Start()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player"); //�÷��̾� �����°� ����
        }
    }

    // Update is called once per frame
    void Update()
    {
        //���ǿ� �÷��̾� �������¸� �ְ� ����
        foreach (Transform p in Pos)
        {
            if(p.position != Vector3.zero && Vector3.Distance(Player.transform.position,p.position) < Distance_Generate) //���� �ð����� �ٽ� �� ��ų�Ÿ� ù��° ���� �ּ�
            {                                                                                                 
                StartCoroutine(Coroutine_Generate(p.position));
                p.position = Vector3.zero; //���� �ð����� �ٽ� �� ��ų�Ÿ� �ּ�ó��, �ѹ��� �����ϵ��� �����
            }
        }
    }
    IEnumerator Coroutine_Generate(Vector3 pos, float time = 0.1f) //�ð� ������Ű�� ������ Ÿ�� ����
    {
        yield return new WaitForSeconds(time);
        Instantiate(Zombie_Group[Random.Range(0, Zombie_Group.Length-1)], pos, Quaternion.identity); //���� ����
        
    }
}

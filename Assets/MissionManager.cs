using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // �̼� ��Ʈ��
    // ��Ÿ������ ���� óġ�ϸ鼭 �� ����
    // ��Ÿ��򿡼� 2�е��� ������
    // �����ϸ� ��Ⱑ ����

    // ���� ������ġ �������� ���� ��Ÿ��򿡼��� ������ ��
    // 30�� �� ��Ⱑ ��Ÿ��� ������ ���� ��
    // �ð��� �����Ͽ� 2���� �Ǹ� ����ʿ� ������ ������ ��ó�� ����
    // �÷��̾ ��� ������ �ְ� ī�޶�� ������ ���߸�
    // ���� �̵��ϸ� ���ø� Ż���ϴ� �� ȣ��
    // ���� ���ӿ���

    bool is_Stadium; // ��Ÿ������ �����ߴ°�
    bool is_StadiumDef; // ��Ÿ��򿡼� ��Ƽ�µ� �����Ͽ��°�

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

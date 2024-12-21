using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    private static MissionManager instance; // �̱����� �Ҵ�� static ����

    // �ܺο��� �̱��� ������Ʈ�� �����ö� ����� ������Ƽ
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
        // ���� �̱��� ������Ʈ�� �� �ٸ� MissionManager ������Ʈ�� �ִٸ� �ڽ��� �ı�
        if (Instance != this) Destroy(gameObject);
    }
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

    public bool is_Stadium; // ��Ÿ������ �����ߴ°�
    public bool is_StadiumDef; // ��Ÿ��򿡼� ��Ƽ�µ� �����Ͽ��°�
    public bool is_Clear; // ��� ž�� �ߴ°�

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

    void Stadium_Defense() // ��Ÿ��� ���潺
    {
        checkTime -= Time.deltaTime;
        UIManager.Instance.UpdateTimeText(Convert.ToInt32(checkTime / 60), Convert.ToInt32(checkTime % 60));
        if (checkTime <= 0)
        {
            is_StadiumDef = true;
        }
    }
}

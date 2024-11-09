using UnityEngine;

//데미지를 준 오브젝트에게 전달할 정보를 담은 구조체
public struct DamageMessage
{
    public GameObject damager;//공격을 가한 게임오브젝트
    public float amount;//공격 데미지

    public Vector3 hitPoint;//공격한 위치
    public Vector3 hitNormal;//공격을 받은 대상이 바라보는 방향
}
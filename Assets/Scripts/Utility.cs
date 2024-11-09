using UnityEngine;
using UnityEngine.AI;

public static class Utility
{
    /// <summary>
    /// 센터를 기준으로 distance내에 랜덤한 위치값을 반환시켜주는 함수
    /// </summary>
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)
    {
        var randomPos = Random.insideUnitSphere * distance + center;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);

        return hit.position;
    }

    /// <summary>
    /// 정규분포로부터 랜덤한 값을 가져오는 함수
    /// </summary>
    /// <param name="mean">평균값</param>
    /// <param name="standard">표준편차</param>
    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }
}
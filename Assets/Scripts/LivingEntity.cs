﻿using System;
using UnityEngine;

// 생명체로서 동작할 게임 오브젝트들을 위한 뼈대를 제공
// 체력, 데미지 받아들이기, 사망 기능, 사망 이벤트를 제공
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f; // 시작 체력
    public float health { get; protected set; } // 현재 체력
    public bool dead { get; protected set; } // 사망 상태

    public event Action OnDeath; // 사망시 발동할 이벤트

    //공격과 공격 사이의 최소 대기 시간
    private const float minTimeBetDamaged = 0.1f;
    private float lastDamagedTime;

    //엔티티가 무적 상태인지 반환하는 프로퍼티
    protected bool IsInvulnerable
    {
        get
        {
            //현재시점이 마지막으로 공격을 당한 시점에서 최소 대기시간이상 지났는지 체크
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            //마지막으로 공격을 당한지 0.1초도 지나지않았다면 무적모드로 만들어줌
            return true;
        }
    }

    // 생명체가 활성화될때 상태를 리셋
    protected virtual void OnEnable()
    {
        // 사망하지 않은 상태로 시작
        dead = false;
        // 체력을 시작 체력으로 초기화
        health = startingHealth;
    }

    // 데미지를 입는 기능
    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        //만약 무적상태이거나, 데미지를 주는 주체가 자기 자신이거나, 사망 상태라면 -> 데미지 적용 불가
        if (IsInvulnerable || damageMessage.damager == gameObject || dead) return false;

        //마지막으로 공격한 시간 갱신
        lastDamagedTime = Time.time;

        // 데미지만큼 체력 감소
        health -= damageMessage.amount;

        // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
        if (health <= 0) Die();

        return true;
    }

    // 체력을 회복하는 기능
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;

        // 체력 추가
        health += newHealth;
    }

    // 사망 처리
    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메서드가 있다면 실행
        if (OnDeath != null) OnDeath();

        // 사망 상태를 참으로 변경
        dead = true;
    }
}
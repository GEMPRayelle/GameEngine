using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : LivingEntity
{
    #region Enemy의 상태를 나타내기 위한 프로퍼티와 열거형
    private enum State
    {
        Patrol,//정찰
        Tracking,//추적
        AttackBegin,//공격시작
        Attacking//공격중
    }
    private State state;
    #endregion

    #region 각 컴포넌트 변수들
    private NavMeshAgent agent; // 경로계산 AI 에이전트
    private Animator animator; // 애니메이터 컴포넌트

    public Transform attackRoot;
    public Transform eyeTransform;

    private AudioSource audioPlayer; // 오디오 소스 컴포넌트
    public AudioClip hitClip; // 피격시 재생할 소리
    public AudioClip deathClip; // 사망시 재생할 소리

    private Renderer skinRenderer; // 렌더러 컴포넌트
    #endregion

    #region 좀비의 기본 Status
    public float runSpeed = 10f;
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    public float damage = 30f;
    public float attackRadius = 2f;
    private float attackDistance;

    public float fieldOfView = 50f;
    public float viewDistance = 10f;
    public float patrolSpeed = 3f;
    #endregion

    //추적할 대상을 코드로 통제할 생각이라 public으로 하지만 인스펙터창에서는 가림
    [HideInInspector] public LivingEntity targetEntity;//추적할 대상
    public LayerMask whatIsTarget;//적을 감지할 레이어필터

    private RaycastHit[] hits = new RaycastHit[10];//범위 기반 공격을 할거라 배열을 사용
    //공격을 새로 시작할때마다 초기화시킬 리스트, 공격도중 직전 프레임까지 공격이 적용된 대상을 담아둠
    private List<LivingEntity> lastAttackedTargets = new List<LivingEntity>();

    //추적할 대상이 존재하는지 반환시키는 프로퍼티
    private bool hasTarget => targetEntity != null && !targetEntity.dead;


    //디버그용
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (attackRoot != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRadius);
        }

        var leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);
        var leftRayDirection = leftRayRotation * transform.forward;
        //Handles.color = new Color(1f, 1f, 1f, 0.2f);
        Handles.color = Color.red;
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);
    }
#endif

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        skinRenderer = GetComponentInChildren<Renderer>();

        attackDistance = Vector3.Distance(transform.position,
                             new Vector3(attackRoot.position.x, transform.position.y, attackRoot.position.z)) +
                         attackRadius;

        attackDistance += agent.radius;

        agent.stoppingDistance = attackDistance;
        agent.speed = patrolSpeed;
    }

    //좀비 AI의 초기 스펙을 결정
    public void Setup(float health, float damage, float runSpeed, float patrolSpeed, Color skinColor)
    {
        this.startingHealth = health;
        this.health = health;

        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed;

        this.damage = damage;

        skinRenderer.material.color = skinColor;
    }

    private void Start()
    {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (dead) return;

        //타겟과의 거리가 공격 사거리안에 들어오면
        if (state == State.Tracking &&
            Vector3.Distance(targetEntity.transform.position, transform.position) <= attackDistance)
        {
            //공격시작
            BeginAttack();
        }

        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        animator.SetFloat("Speed", agent.desiredVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (dead) return;

        if (state == State.AttackBegin || state == State.Attacking)
        {
            var lookRotation =
                Quaternion.LookRotation(targetEntity.transform.position - transform.position, Vector3.up);
            var targetAngleY = lookRotation.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
        }

        //공격이 들어가고 있는 상태
        if (state == State.Attacking)
        {
            //좀비가 이동 하고 있는 방향
            var direction = transform.forward;
            //agent가 시간만큼 이동하는 거리를 계산 = 공격의 궤적
            var deltaDistance = agent.velocity.magnitude * Time.deltaTime;

            //SphereCast는 시작지점에서 어떤 거리만큼 이동할때 연속선상에서 겹치는 collider를 가져온다, Raycast의 hit배열을 반환
            //nonAlloc은 return값으로 감지된 collider의 개수만 반환한다, 대신 입력으로 직접 Raycast Hit 배열을 할당한다
            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRadius, direction, hits, deltaDistance, whatIsTarget);

            for (var i = 0; i < size; i++)
            {
                var attackTargetEntity = hits[i].collider.GetComponent<LivingEntity>();

                if (attackTargetEntity != null && !lastAttackedTargets.Contains(attackTargetEntity))
                {
                    //메세지 전달
                    var message = new DamageMessage();
                    message.amount = damage;
                    message.damager = gameObject;
                    message.hitPoint = attackRoot.TransformPoint(hits[i].point);
                    message.hitNormal = attackRoot.TransformDirection(hits[i].normal);

                    attackTargetEntity.ApplyDamage(message);

                    lastAttackedTargets.Add(attackTargetEntity);
                    break;
                }
            }
        }
    }

    /// 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신하는 코루틴 함수
    private IEnumerator UpdatePath()
    {
        while (!dead)
        {
            //추적할 타겟이 존재하고
            if (hasTarget)
            {
                // + 정찰 상태일시
                if (state == State.Patrol)
                {
                    //추적 상태로 바꿈과 동시에 달리는 속도로 agent 속도 갱신
                    state = State.Tracking;
                    agent.speed = runSpeed;
                }

                // 추적 대상 위치를 갱신하여 추적하도록함
                agent.SetDestination(targetEntity.transform.position);
            }
            else//추적 대상이 없으면
            {
                //대상을 null로 바꿔줘서 정찰하면서 플레이어를 찾게한다
                if (targetEntity != null) targetEntity = null;

                //정찰 상태가 아니라면
                if (state != State.Patrol)
                {
                    //정찰 상태로 바꿔주고 정찰하는 속도로 속도 갱신
                    state = State.Patrol;
                    agent.speed = patrolSpeed;
                }

                //정찰중인 상태에서 agent가 목표지점까지 가야할거리가 1m안이라면
                if (agent.remainingDistance <= 1f)
                {
                    //정찰위치를 NavMesh area에서 랜덤한 포지션을 가져온다
                    var patrolPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);//이동할 위치 다시 갱신
                }

                // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
                // 단, whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
                var colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, whatIsTarget);

                // 모든 콜라이더들을 순회하면서, 살아있는 LivingEntity 찾기
                foreach (var collider in colliders)
                {
                    //상대가 시야 내에 없다면
                    if (!IsTargetOnSight(collider.transform)) break;
                    //시야 내에 들어왔으면 LivingEntity로서 가져올 수 있는지 검사
                    var livingEntity = collider.GetComponent<LivingEntity>();

                    // 만약 LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아있다면
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        // 추적 대상을 해당 LivingEntity로 설정
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;

        if (targetEntity == null)//추적할 대상을 못 찾았는데 공격을 당할시
        {
            //타겟대상을 공격한 대상으로 즉시 바꾼다
            targetEntity = damageMessage.damager.GetComponent<LivingEntity>();
        }

        EffectManager.Instance.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal, transform, EffectManager.EffectType.Flesh);
        audioPlayer.PlayOneShot(hitClip);

        return true;
    }

    #region 공격 관련 함수(애니메이션)
    public void BeginAttack()
    {
        state = State.AttackBegin;

        agent.isStopped = true;
        animator.SetTrigger("Attack");
    }

    public void EnableAttack()
    {
        state = State.Attacking;

        lastAttackedTargets.Clear();
    }

    public void DisableAttack()
    {
        state = State.Tracking;

        agent.isStopped = false;
    }
    #endregion

    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;

        //타겟의 위치로 향하는 방향벡터
        var direction = target.position - eyeTransform.position;

        //높이차는 고려하지 않음
        direction.y = eyeTransform.forward.y;

        //두 방향벡터 사이의 각도
        if (Vector3.Angle(direction, eyeTransform.forward) > fieldOfView * 0.5f)
        {
            //눈에서 목표까지 방향과, 눈 앞쪽 방향 사이의 각도가 FOV보다 크다면 
            return false;//arc에서 벗어나게된다
        }

        //eye과 target사이에 가리는 장애물이 있는지 검사
        //direction을 원래 값으로 되돌려야한다
        //direction = target.position - eyeTransform.position;

        //시야각내에 존재하지만 다른 물체에 중간에 가려져서 보이지않는다면
        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance, whatIsTarget))
        {
            //광선에 닿은 물체가 처음 검사했던 상대방이맞다면
            if (hit.transform == target) return true;
            //상대방과 눈 사이에 장애물이 없어서 상대방이 보이게된다
        }
        return false;
    }

    public override void Die()
    {
        //기본 사망 처리 실행
        base.Die();

        // 다른 AI들을 방해하지 않도록 자신의 모든 콜라이더들을 비활성화
        GetComponent<Collider>().enabled = false;

        //AI 추적을 중지
        agent.enabled = false;

        animator.applyRootMotion = true;
        animator.SetTrigger("Die");

        if (deathClip != null) audioPlayer.PlayOneShot(deathClip);
    }
}
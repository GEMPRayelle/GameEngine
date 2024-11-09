using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour
{
    public enum AimState
    {
        Idle,
        HipFire
    }

    public AimState aimState { get; private set; }

    public Gun gun;
    public LayerMask excludeTarget;//조준에서 제외할 레이어

    private PlayerInput playerInput;
    private Animator playerAnimator;
    private Camera playerCamera;

    private Vector3 aimPoint;

    //캐릭터가 바라보는 방향과 카메라가 바라보는 방향 사이에
    //각도가 너무 벌어졌는지 벌어지지않았는지 반환하는 Property
    private bool linedUp => !(Mathf.Abs(playerCamera.transform.eulerAngles.y - transform.eulerAngles.y) > 1f);

    //플레이어가 정면에 총을 발사할 수 있을 정도로 넉넉한 공간을 확보했는지 반환하는 Property
    private bool hasEnoughDistance => !Physics.Linecast(transform.position + Vector3.up * gun.fireTransform.position.y, gun.fireTransform.position, ~excludeTarget);

    void Awake()
    {
        //제외할 레이어에 플레이어 오브젝트의 레이어가 포함되어 있지 않다면 
        if (excludeTarget != (excludeTarget | (1 << gameObject.layer)))
        {
            //플레이어 게임 오브젝트의 레이어를 excludeTarget에 추가한다
            //(예외 처리)플레어가 실수로 자신을 쏘는 현상을 방지
            excludeTarget |= 1 << gameObject.layer;
        }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
        gun.Setup(this);
    }

    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (playerInput.fire)
        {
            Shoot();
        }
        else if (playerInput.reload)
        {
            Reload();
        }
    }

    private void Update()
    {
        UpdateAimTarget();

        var angle = playerCamera.transform.eulerAngles.x;
        if (angle > 270f) angle -= 360f;

        angle = angle / 180f * -1f + 0.5f;

        playerAnimator.SetFloat("Angle", angle);

        UpdateUI();
    }

    public void Shoot()
    {
        if (aimState == AimState.Idle)
        {
            if (linedUp) aimState = AimState.HipFire;
        }
        else if (aimState == AimState.HipFire)
        {
            if (hasEnoughDistance)
            {
                if (gun.Fire(aimPoint)) playerAnimator.SetTrigger("Shoot");
            }
            else
            {
                aimState = AimState.Idle;
            }
        }
    }

    public void Reload()
    {
        // 재장전 입력 감지시 재장전
        if (gun.Reload()) playerAnimator.SetTrigger("Reload");
    }

    private void UpdateAimTarget()
    {
        RaycastHit hit;

        var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));

        if (Physics.Raycast(ray, out hit, gun.fireDistance, ~excludeTarget))
        {
            aimPoint = hit.point;

            if (Physics.Linecast(gun.fireTransform.position, hit.point, out hit, ~excludeTarget))
            {
                aimPoint = hit.point;
            }
        }
        else
        {
            aimPoint = playerCamera.transform.position + playerCamera.transform.forward * gun.fireDistance;
        }
    }

    // 탄약 UI 갱신
    private void UpdateUI()
    {
        if (gun == null || UIManager.Instance == null) return;

        UIManager.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);

        UIManager.Instance.SetActiveCrosshair(hasEnoughDistance);
        UIManager.Instance.UpdateCrossHairPosition(aimPoint);
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        if (gun == null || gun.state == Gun.State.Reloading) return;

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, gun.leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, gun.leftHandMount.rotation);
    }
}
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 플레이어 관련 클래스들을 총괄하는 스크립트
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public AudioClip itemPickupClip;
    public int lifeRemains = 3;
    private AudioSource playerAudioPlayer;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerShooter playerShooter;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAudioPlayer = GetComponent<AudioSource>();
        playerHealth.OnDeath += HandleDeath;

        //UIManager.Instance.UpdateLifeText(lifeRemains);
        Cursor.visible = false;
    }

    //사망했을때 다른 컴포넌트들을 제어하기위한 핸들러
    private void HandleDeath()
    {
        playerMovement.enabled = false;
        playerShooter.enabled = false;

        // if (lifeRemains > 0)
        // {
        //     lifeRemains--;
        //     UIManager.Instance.UpdateLifeText(lifeRemains);
        //     Invoke("Respawn", 3f);
        // }
        // else
        // {
        //     GameManager.Instance.EndGame();
        // }

        //리스폰없이 바로 게임오버 처리
        GameManager.Instance.EndGame();

        Cursor.visible = true;
    }

    public void Respawn()
    {
        //OnEnable, OnDisable을 사용하기에 비활성화 시켰다가 다시 활성화시킴
        gameObject.SetActive(false);
        transform.position = Utility.GetRandomPointOnNavMesh(transform.position, 30f, NavMesh.AllAreas);

        gameObject.SetActive(true);
        playerMovement.enabled = true;
        playerShooter.enabled = true;

        playerShooter.gun.ammoRemain = 120;

        Cursor.visible = false;
    }


    // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
    private void OnTriggerEnter(Collider other)
    {
        // 사망하지 않은 경우
        if (!playerHealth.dead)
        {
            //충돌한 오브젝트로부터 IItem 인터페이스를 가져옴
            var item = other.GetComponent<IItem>();

            //충돌한 상대방으로부터 가져오는데 성공했다면
            if (item != null)
            {
                item.Use(gameObject);
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }
        }
    }
}
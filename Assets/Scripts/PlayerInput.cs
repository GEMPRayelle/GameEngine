﻿using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour
{
    public string fireButton = "Fire1"; // 발사를 위한 입력 버튼 이름
    public string jumpButton = "Jump";
    public string moveHorizontalAxis = "Horizontal"; // 좌우 회전을 위한 입력축 이름
    public string moveVerticalAxis = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    public string reloadButton = "Reload"; // 재장전을 위한 입력 버튼 이름

    // 값 할당은 내부에서만 가능

    public Vector2 moveInput { get; private set; }
    public bool fire { get; private set; } // 감지된 발사 입력값
    public bool reload { get; private set; } // 감지된 재장전 입력값
    public bool jump { get; private set; }


    // 매프레임 사용자 입력을 감지
    private void Update()
    {
        // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
        if (GameManager.Instance != null
            && GameManager.Instance.isGameover)
        {
            moveInput = Vector2.zero;
            fire = false;
            reload = false;
            jump = false;
            return;
        }

        moveInput = new Vector2(Input.GetAxis(moveHorizontalAxis), Input.GetAxis(moveVerticalAxis));
        if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;

        jump = Input.GetButtonDown(jumpButton);
        fire = Input.GetButton(fireButton);
        reload = Input.GetButtonDown(reloadButton);
    }
}
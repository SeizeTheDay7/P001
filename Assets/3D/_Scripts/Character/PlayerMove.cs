using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] CinemachineCamera player_vcam;
    [SerializeField] float walk_gap = 0.5f;
    [SerializeField] float amplitudeGain_walk = 2f;
    [SerializeField] float frequencyGain_walk = 1.5f;
    [SerializeField] float run_gap = 0.25f;
    [SerializeField] float amplitudeGain_run = 3f;
    [SerializeField] float frequencyGain_run = 2.2f;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] private AudioSource footstep;
    public float moveSpeed = 5f;
    [SerializeField] float runMult = 1.5f;
    private CharacterController characterController;
    [SerializeField] private CinemachineImpulseSource impulse_walk;
    [SerializeField] private CinemachineImpulseSource impulse_run;
    private CinemachineBasicMultiChannelPerlin noise;
    private bool wait_nextFootstep = false;
    private float targetAmplitudeGain;
    private float targetFrequencyGain;
    private float gravity = -9.81f;
    [SerializeField] private float gravityMult = 3f;
    [SerializeField] private float jumpMult = 1f;
    private float verticalVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        noise = player_vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        footstep = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked; // 마우스를 화면 중앙에 고정
        Cursor.visible = false; // 마우스 커서 숨김
    }

    void OnEnable()
    {
        wait_nextFootstep = false;
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += player_vcam.transform.forward;
        if (Input.GetKey(KeyCode.S)) direction += -player_vcam.transform.forward;
        if (Input.GetKey(KeyCode.A)) direction += -player_vcam.transform.right;
        if (Input.GetKey(KeyCode.D)) direction += player_vcam.transform.right;

        direction.y = 0f; // y축 이동 방지
        direction.Normalize();
        if (Input.GetKey(KeyCode.LeftShift)) direction *= runMult;

        AddCameraMove(direction);

        AddGravity();
        direction.y = verticalVelocity;

        CollisionFlags flags = characterController.Move(direction * moveSpeed * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0)
            verticalVelocity = 0f; // 천장에 닿았으면 속도 제거
    }

    private void AddGravity()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space)) verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpMult); // 점프
            else verticalVelocity = -2f;
        }
        else verticalVelocity += gravity * Time.deltaTime; // 땅에 붙어있는게 아니면 떨어져
    }

    private void AddCameraMove(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // 뛰는거
            if (Input.GetKey(KeyCode.LeftShift))
            {
                targetAmplitudeGain = amplitudeGain_run;
                targetFrequencyGain = frequencyGain_run;
                if (!wait_nextFootstep && characterController.isGrounded)
                {
                    wait_nextFootstep = true;
                    StartCoroutine(GenerateRunImpulseWithDelay());
                }
            }
            // 걷는거
            else
            {
                targetAmplitudeGain = amplitudeGain_walk;
                targetFrequencyGain = frequencyGain_walk;
                if (!wait_nextFootstep && characterController.isGrounded)
                {
                    wait_nextFootstep = true;
                    StartCoroutine(GenerateWalkImpulseWithDelay());
                }
            }
        }
        // 가만히 있는거
        else
        {
            targetAmplitudeGain = 0.7f;
            targetFrequencyGain = 0.7f;
        }

        noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, targetAmplitudeGain, Time.deltaTime * 5f);
        noise.FrequencyGain = Mathf.Lerp(noise.FrequencyGain, targetFrequencyGain, Time.deltaTime * 5f);
    }

    private IEnumerator GenerateWalkImpulseWithDelay()
    {
        impulse_walk.GenerateImpulse();
        PlayFootstepSound();
        yield return new WaitForSeconds(walk_gap);
        wait_nextFootstep = false;
    }

    private IEnumerator GenerateRunImpulseWithDelay()
    {
        impulse_run.GenerateImpulse();
        PlayFootstepSound();
        yield return new WaitForSeconds(run_gap);
        wait_nextFootstep = false;
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            footstep.clip = footstepSounds[randomIndex];
            footstep.Play();
        }
    }

}
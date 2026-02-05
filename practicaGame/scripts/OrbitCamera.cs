using UnityEngine;
using Cinemachine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 2f;
    public float distance = 5f;
    public float height = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("References")]
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;

    private float currentXAngle = 0f;
    private float currentYAngle = 0f;
    private Vector3 cameraOffset;

    void Start()
    {
        // Находим объекты если не назначены
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        // Инициализация
        if (player != null)
        {
            currentYAngle = player.eulerAngles.y;
            currentXAngle = 20f; // Начальный угол сверху
        }

        // Настраиваем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (player == null || virtualCamera == null) return;

        // Ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Обновляем углы
        currentYAngle += mouseX;
        currentXAngle -= mouseY;
        currentXAngle = Mathf.Clamp(currentXAngle, minVerticalAngle, maxVerticalAngle);

        // Вычисляем позицию камеры
        Quaternion rotation = Quaternion.Euler(currentXAngle, currentYAngle, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        // Устанавливаем позицию Virtual Camera
        virtualCamera.transform.position = player.position + offset;

        // Заставляем камеру смотреть на игрока
        virtualCamera.transform.LookAt(player.position + Vector3.up * 1.5f);

        // Сброс камеры по средней кнопке мыши
        if (Input.GetMouseButtonDown(2))
        {
            ResetCamera();
        }

        // Управление курсором
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0) && Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ResetCamera()
    {
        if (player != null)
        {
            currentYAngle = player.eulerAngles.y;
            currentXAngle = 20f;
        }
    }
}
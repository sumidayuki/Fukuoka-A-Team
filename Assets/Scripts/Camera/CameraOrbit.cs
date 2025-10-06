using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit_MouseDrag : MonoBehaviour
{
    [Header("References")]
    public Transform pivot;   // 注視点（プレイヤーなど）
    public Transform cam;     // メインカメラ

    [Header("Settings")]
    public float distance = 6f;      // 距離
    public float height = 3f;        // 高さ
    public float rotateSpeed = 3f;   // 回転スピード
    public float smooth = 10f;       // スムーズ係数

    private float currentYaw = 0f;   // 現在の角度
    private float targetYaw = 0f;    // 目標の角度
    private Vector3 targetPos;

    void Update()
    {
        // --- マウスドラッグで回転 ---
        if (Input.GetMouseButton(1)) // 右クリックを押している間
        {
            float mouseX = Input.GetAxis("Mouse X");
            targetYaw += mouseX * rotateSpeed;
        }
    }

    void LateUpdate()
    {
        if (!pivot || !cam) return;

        // 位置を追従
        targetPos = Vector3.Lerp(transform.position, pivot.position, Time.deltaTime * smooth);
        transform.position = targetPos;

        // 回転をスムーズに補間
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * smooth);

        // カメラの位置を計算
        Vector3 offset = Quaternion.Euler(0, currentYaw, 0) * new Vector3(0, height, -distance);
        cam.position = transform.position + offset;

        // pivotの少し上を見る
        cam.LookAt(transform.position + Vector3.up * (height * 0.6f));
    }
}


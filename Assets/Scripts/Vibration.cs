using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vibration : Singleton<Vibration>
{
    private Gamepad gamepad;

    void Start()
    {
        // 获取当前连接的手柄
        gamepad = Gamepad.current;

        if (gamepad == null)
        {
            Debug.LogWarning("No gamepad connected.");
        }
    }

    public void TriggerVibration(float lowFrequency, float highFrequency, float duration)
    {
        if (gamepad != null)
        {
            // 设置震动强度
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            Debug.Log($"Vibration started: Low {lowFrequency}, High {highFrequency}");

            // 停止震动计时
            Invoke(nameof(StopVibration), duration);
        }
    }

    private void StopVibration()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f); // 停止震动
            Debug.Log("Vibration stopped.");
        }
    }

    void Update()
    {
        // 按下 A 键触发震动（测试用）
        if (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
        {
            TriggerVibration(0.5f, 1.0f, 1.0f); // 参数: 低频强度，高频强度，持续时间
        }
    }
}

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

    void Update()
    {
        if (gamepad == null)
        {
            Debug.Log("gamepad null");
            gamepad = Gamepad.current;
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
        else
        {
            Debug.Log("gamepad null");
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
}

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads gameplay input from the Input System package (Player Settings uses Input System only).
/// </summary>
public static class NetworkPlayerInput
{
    public static Vector2 ReadMove()
    {
        if (Keyboard.current == null)
        {
            return Vector2.zero;
        }

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            x += 1f;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            y -= 1f;
        }

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            y += 1f;
        }

        Vector2 move = new Vector2(x, y);
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        return move;
    }

    public static bool WasJumpPressed()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    public static bool WasAttackPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        return Keyboard.current != null && Keyboard.current.leftCtrlKey.wasPressedThisFrame;
    }
}

using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    public float InputX { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }

    private void Update()
    {
        InputX = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            JumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            DashPressed = true;
        }
    }

    public void ConsumeJump()
    {
        JumpPressed = false;
    }

    public void ConsumeDash()
    {
        DashPressed = false;
    }
}
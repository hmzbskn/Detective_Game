using UnityEngine;
using UnityEngine.InputSystem;

public class DuraklatmaMenusuController : UIControllerBase
{
    protected override bool TetiklemeTusunaBasildiMi()
    {
        bool basildi = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        if (basildi)
        {
            Debug.Log("ESC algılandı");
        }

        return basildi;
    }
}
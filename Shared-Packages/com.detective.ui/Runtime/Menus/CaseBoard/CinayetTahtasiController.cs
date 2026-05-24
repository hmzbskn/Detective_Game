    using UnityEngine.InputSystem;

public class CinayetTahtasiController : UIControllerBase
{
    protected override bool TetiklemeTusunaBasildiMi()
    {
        return Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame;
    }
}
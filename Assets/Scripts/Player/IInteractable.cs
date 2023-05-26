namespace Player{
    public interface IInteractable{
        string GetInstruction(PowerVolume volume);
        void Interact(PowerVolume volume);
        bool IsActive(PowerVolume volume);
        void OnFocused(PowerVolume volume);
        void OnLoseFocus(PowerVolume volume);
    }
}
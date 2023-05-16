namespace Player{
    public interface IInteractable{
        string GetInstruction();
        void Interact(PowerVolume volume);
    }
}
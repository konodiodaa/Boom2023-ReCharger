namespace Player{
    public interface IInteractable{
        string GetInstruction(PowerVolume volume);
        void Interact(PowerVolume volume);
    }
}
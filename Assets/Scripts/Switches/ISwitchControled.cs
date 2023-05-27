namespace Switches{
    public interface ISwitchControled{
        void OnTurnedOn();
        void OnTurnedOff();
        void Highlight();
        void RemoveHighlight();
    }
}
using Player;
using UnityEngine;

namespace Wires{
    public class PlugInteraction: MonoBehaviour, IInteractable{
        public Plug2 plug;

        public string GetInstruction(PowerVolume volume){
            return plug.state switch{
                Plug2.State.Free => volume.GetCarried() == null ? "Press E to Pick Up" : null,
                Plug2.State.Carried => null,
                Plug2.State.Plugged => null
            };
        }

        public void Interact(PowerVolume volume){
            switch (plug.state){
                case Plug2.State.Free:
                    if (volume.GetCarried() != null) return;
                    Debug.Log("Player pick up Plug!");
                    volume.PickUp(plug);
                    break;
                case Plug2.State.Carried:
                    break;
                case Plug2.State.Plugged:
                    break;
            }
        }

        private void OnDisable(){
            GetComponent<Collider2D>().enabled = false;
        }

        private void OnEnable(){
            GetComponent<Collider2D>().enabled = true;
        }
    }
}
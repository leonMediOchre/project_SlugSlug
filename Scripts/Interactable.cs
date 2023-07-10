using Godot;

public partial class Interactable : Node {

    private Callable _interactCallable;

    public override void _Ready() {
        _interactCallable = new Callable(GetParent(), "OnInteract");
    }

    public void OnBodyEnterArea(Node3D body) {
        if (body is Player p) {
            p.Connect("Interact", _interactCallable);
        }
    }

    public void OnBodyExitArea(Node3D body) {
        if (body is Player p) {
            p.Disconnect("Interact", _interactCallable);
        }
    }

}
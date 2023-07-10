using Godot;

public partial class GravitySwitch : Node3D {

    public void OnInteract() {
        GetTree().CallGroup("Player", "SwitchGravity"); 
    }

}
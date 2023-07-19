using Godot;
using System;

public partial class Character : Node3D
{
    [Export]
    public string CharacterName {get; set;}

    public void SetPlayerFocusOnCharacter(string name) {
        if (CharacterName == name)
            GetTree().CallGroup("Player", "SetCameraFocus", this.Position);
    }

}

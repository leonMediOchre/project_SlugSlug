using Godot;
using System;

public partial class Character : Node3D
{
    [Export]
    public string CharacterName {get; set;}
    public float EngageDistance { get; private set; }
    [Export]
    public bool WantsToTalk = false;
    [Export]
    public string dialogueId;

    private Vector3 _playerPosition;

    public override void _PhysicsProcess(double delta) {
        if (GlobalPosition.DistanceTo(_playerPosition) < EngageDistance)
            DialogueBox.InitiateDialogue(dialogueId);
    }

    public void DrawPlayerFocus(string name) {
        if (CharacterName == name)
            GetTree().CallGroup("Player", "SetCameraFocus", this.Position);
    }

}

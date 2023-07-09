using Godot;

public partial class Player : CharacterBody3D {
    [Export]
	public float CameraSensitivity {get; set;} = 1;
    [Export]
	public float Speed = 5.0f;
	[Export]
    public const float JumpVelocity = 4.5f;
    [Export]
    public PackedScene FireProjectile;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector2 _lookDirection;
	private Vector2 _moveDirection;
    private Camera3D _camera;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _camera = GetNode("Camera3D") as Camera3D;
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseMotion e) {
            _lookDirection = e.Relative * 0.001f;
			RotateCamera();
            return;
        }
        else if (@event.IsActionPressed("fire")) Fire();
    }

    public override void _PhysicsProcess(double delta) {
        Vector3 velocity = Velocity;

        GetTree().CallGroup("Enemies", "UpdatePlayerPosition", GlobalPosition);

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= Gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = JumpVelocity;

        Vector2 inputDir = Input.GetVector(
            "move_left", "move_right",
            "move_forward", "move_backward"
            );
        Vector3 direction = (_camera.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero) {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        } else {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void RotateCamera() {
        float xRotation = Mathf.Clamp(
            _camera.Rotation.X - _lookDirection.Y * CameraSensitivity,
            -1.5f, 1.5f
            );
        float yRotation = _camera.Rotation.Y -_lookDirection.X * CameraSensitivity;
        _camera.Rotation = new Vector3(xRotation, yRotation, 0);
    }
    
    private void Fire() {
        GD.Print(GlobalPosition);
        Projectile proj = FireProjectile.Instantiate() as Projectile;
        proj.GlobalTransform = _camera.GlobalTransform;
        GetParent().AddChild(proj);
    }
}

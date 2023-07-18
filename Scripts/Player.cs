using Godot;

public partial class Player : CharacterBody3D {
	[Export]
	public float CameraSensitivity = 1;
	[Export]
	public float Speed = 5.0f;
	[Export]
	public const float JumpVelocity = 4.5f;
	[Export]
	public PackedScene FireProjectile;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private float _targetXRotation;
	private Vector2 _lookDirection;
	private Vector2 _moveDirection;

	private Camera3D _camera;
	private RayCast3D _interactRaycast;

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera = GetNode("Camera") as Camera3D;
		_interactRaycast = _camera.GetNode("Raycast") as RayCast3D;
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseMotion e) {
			_lookDirection = e.Relative * 0.001f;
			RotateCamera();
			return;
		}
		else if (@event.IsActionPressed("fire")) Fire();
		else if (@event.IsActionPressed("interact")) Interact(); 
	}

	public override void _PhysicsProcess(double delta) {
		Vector3 velocity = Velocity;
		
		GetTree().CallGroup("Enemies", "UpdatePlayerPosition", GlobalPosition);

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= Gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		Vector2 inputDir = Input.GetVector(
			"move_left", "move_right",
			"move_forward", "move_backward"
			);
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero) {
			float velDelta = IsOnFloor() ? .25f * Speed : .025f * Speed;
			velocity.X = Mathf.MoveToward(velocity.X, direction.X * Speed, velDelta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * Speed, velDelta);
		} else {
			float velDelta = IsOnFloor() ? Speed : .01f * Speed;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, velDelta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, velDelta);
		}

		if (Rotation.X != _targetXRotation) {
			var targetRotation = new Vector3(_targetXRotation, Rotation.Y, Rotation.Z);
			Rotation.MoveToward(targetRotation, .25f);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void SwitchGravity() {
		_targetXRotation = Rotation.X == 0 ? Mathf.Pi : 0;
	}

	private void RotateCamera() {
		float xRotation = Mathf.Clamp(
			_camera.Rotation.X - _lookDirection.Y * CameraSensitivity,
			-1.5f, 1.5f
			);
		float yRotation = _camera.Rotation.Y -_lookDirection.X * CameraSensitivity;
		_camera.Rotation = new Vector3(xRotation, 0, 0);
		RotateY(yRotation);
	}
	
	private void Fire() {
		GD.Print(GlobalPosition);
		Projectile proj = FireProjectile.Instantiate() as Projectile;
		proj.GlobalTransform = _camera.GlobalTransform;
		GetParent().AddChild(proj);
	}

	private void Interact() {
		if (_interactRaycast.IsColliding()) {
			var col = _interactRaycast.GetCollider();
			if (col is IInteractable i) i.Interact();
		}
	}

}

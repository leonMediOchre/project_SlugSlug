using Godot;

public partial class Enemy : CharacterBody3D {
    const float DEG90 = Mathf.Pi / 2;

    [Export]
    public float Speed = 5;
    [Export]
    public float FollowDistance { get; private set; } = 2;
    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector3 _gravityVector = Vector3.Up;

    private Vector3 _playerPosition;
    private NavigationAgent3D _navigationAgent;
    private AnimationPlayer _animationPlayer;
    private Vector2 _facingVector = Vector2.Up;
    private Vector3 _targetVelocity;


    public override void _Ready() {
        _navigationAgent = GetNode("NavigationAgent3D") as NavigationAgent3D;
        _animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
    }

    public override void _PhysicsProcess(double delta) {
        if (!IsOnFloor()) Velocity -= Gravity * _gravityVector * (float)delta;

        FollowPlayer();
        SelectFacingSprite();

        _targetVelocity.Y = Velocity.Y;
        Velocity = Velocity.MoveToward(_targetVelocity, .25f);
        MoveAndSlide();
    }

    public void UpdatePlayerPosition(Vector3 position) {
        _playerPosition = position;
    }

    private void FollowPlayer() {
        if (Position.DistanceTo(_playerPosition) < FollowDistance) {
            _targetVelocity = Vector3.Zero;
            return;
        }

        _navigationAgent.TargetPosition = _playerPosition;
        Vector3 nextLocation = _navigationAgent.GetNextPathPosition();
        _targetVelocity = (nextLocation - GlobalPosition).Normalized() * Speed;
    }

    private void SelectFacingSprite() {
        if (Velocity.X != 0 || Velocity.Z != 0)
            _facingVector = new(Velocity.X, Velocity.Z);

        LookAt(_playerPosition);

        float angleToPlayer = Rotation.Y - Vector2.Up.AngleTo(_facingVector);
        if (angleToPlayer > 0)
            if (angleToPlayer < DEG90) _animationPlayer.Play("front_left");
            else _animationPlayer.Play("back_right");
        else if (angleToPlayer < 0)
            if (angleToPlayer > -DEG90) _animationPlayer.Play("front_right");
            else _animationPlayer.Play("back_left");
    }

}

using Godot;
using System;

public partial class Projectile : Area3D {
    [Export]
    public float Speed = 10;
	[Export]
	public float MaxDistance = 20;
	[Export]
    public float PushBackForce = 5; 

    private Vector3 _velocity;
	private Vector3 _initialPosition;
    private MeshInstance3D _mesh;
    private GpuParticles3D _particles;
	private bool _collided;


    public override void _Ready() {
		_mesh = GetNode("Mesh") as MeshInstance3D;
		_particles = GetNode("Particles") as GpuParticles3D;

		_initialPosition = GlobalPosition;
        _velocity = -Transform.Basis.Z * Speed;
		GD.Print("Bullet pos = " + GlobalPosition); 
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta) {
		if (_collided)
			if(_particles.Emitting == false) QueueFree();
			else return;
		if (GlobalPosition.DistanceTo(_initialPosition) > MaxDistance) QueueFree();

		GlobalPosition += _velocity * (float)delta;
	}

    public void On_Body_Entered(Node3D body) {
		if (_collided) return;

		if (body is CharacterBody3D cb)
			cb.Velocity -= Transform.Basis.Z * PushBackForce;

		_mesh.QueueFree();
		_particles.Emitting = true;
		_collided = true;
    }


}

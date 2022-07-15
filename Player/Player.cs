using Additions;
using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export] public float movementSpeed;


    #region AnimationTree Reference

    private AnimationTree storerForAnimationTree;
    public AnimationTree AnimationTree => this.LazyGetNode(ref storerForAnimationTree, "AnimationTree");

    #endregion


    public override void _PhysicsProcess(float delta)
    {
        Vector2 inputVector = InputManager.GetMovementInput();

        Move(inputVector, delta);
        Animate(inputVector);
    }

    private void Move(Vector2 inputVector, float delta)
    {
        MoveAndSlide(inputVector * movementSpeed);
    }

    private void Animate(Vector2 inputVector)
    {
        float lenght = inputVector.Length();

        if (lenght > 0.2f)
        {
            AnimationTree.Set("parameters/MovementState/current", 1);
            AnimationTree.Set("parameters/RunSpeed/scale", lenght.Clamp01());
        }
        else
        {
            AnimationTree.Set("parameters/MovementState/current", 0);
        }
    }
}

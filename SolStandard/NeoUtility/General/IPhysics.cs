using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SolStandard.NeoUtility.General
{
    public interface IPhysics
    {
        float Mass { get; }
        Vector2 Velocity { get; }
        void ApplyForce(Vector2 appliedForce, bool overridePhysics = false);

        bool CollidesWith(IShapeF thingToCollideWith);
        bool ContainsPoint(Vector2 point);
        Vector2 CenterPoint { get; set; }
        IShapeF Collider { get; }
        
        bool IsGapCollider { get; }

        float TopCoordinate => Collider.AsRectangleF().Y;
        float BottomCoordinate => Collider.AsRectangleF().Y + Collider.AsRectangleF().Height;
        float LeftCoordinate => Collider.AsRectangleF().X;
        float RightCoordinate => Collider.AsRectangleF().X + Collider.AsRectangleF().Width;
        
        Vector2 BottomCenter => new Vector2(CenterPoint.X, BottomCoordinate);

        /**
         * <returns>The new position for the collider that was passed in order to not collide</returns>
         */
        Vector2 ResolveCollision(IPhysics otherPhysics);
    }
}
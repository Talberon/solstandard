using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SolStandard.Utility.Exceptions;

namespace Steelbreakers.Utility.General
{
    public static class ShapeExtensions
    {
        public static CircleF AsCircleF(this IShapeF shapeF)
        {
            return shapeF switch
            {
                CircleF circleF => circleF,
                RectangleF rectangleF => new CircleF(
                    rectangleF.Center,
                    (rectangleF.Width > rectangleF.Height) ? rectangleF.Width : rectangleF.Height
                ),
                _ => throw new InvalidShapeException(shapeF)
            };
        }

        public static RectangleF AsRectangleF(this IShapeF shapeF)
        {
            return shapeF switch
            {
                CircleF circleF => circleF.ToRectangle(),
                RectangleF rectangleF => rectangleF,
                _ => throw new InvalidShapeException(shapeF)
            };
        }

        public static Vector2 Size(this IShapeF shapeF)
        {
            return new Vector2(shapeF.AsRectangleF().Size.Width, shapeF.AsRectangleF().Size.Height);
        }

        public static IShapeF Clone(this IShapeF me)
        {
            return me switch
            {
                CircleF circleF => new CircleF(circleF.Center, circleF.Radius),
                RectangleF rectangleF => new RectangleF(rectangleF.Position, rectangleF.Size),
                _ => throw new InvalidShapeException(me)
            };
        }

        public static IShapeF OffsetPosition(this IShapeF me, Vector2 offset)
        {
            IShapeF newShape = me.Clone();
            newShape.Position += offset;
            return newShape;
        }

        public static IShapeF Padded(this IShapeF me, float padding)
        {
            return me switch
            {
                CircleF circleF => new CircleF(circleF.Center, circleF.Radius + padding),
                RectangleF rectangleF => new RectangleF(
                    rectangleF.Position - new Vector2(padding),
                    rectangleF.Size + new Vector2(padding * 2).ToSize()
                ),
                _ => throw new InvalidShapeException(me)
            };
        }

        public static IShapeF TranslateBy(this IShapeF me, Vector2 offset)
        {
            IShapeF newShape = me.Clone();
            newShape.Position += offset;
            return newShape;
        }

        public static RectangleF TopLeftQuarter(this RectangleF me)
        {
            return new RectangleF(me.Position, me.Size / 2);
        }

        public static RectangleF TopRightQuarter(this RectangleF me)
        {
            return new RectangleF(me.Position + new Vector2(me.Size.Width / 2, 0), me.Size / 2);
        }

        public static RectangleF BottomLeftQuarter(this RectangleF me)
        {
            return new RectangleF(me.Position + new Vector2(0, me.Size.Height / 2), me.Size / 2);
        }

        public static RectangleF BottomRightQuarter(this RectangleF me)
        {
            return new RectangleF(me.Position + me.Size / 2, me.Size / 2);
        }
    }
}
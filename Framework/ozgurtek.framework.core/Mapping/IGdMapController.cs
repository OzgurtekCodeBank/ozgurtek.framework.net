using NetTopologySuite.Geometries;

namespace ozgurtek.framework.core.Mapping
{
    public interface IGdMapController
    {
        /// <summary>Controller start interaction.</summary>
        void StartInteraction(IGdMap map);

        /// <summary>Controller ends interaction.</summary>
        void EndInteraction();

        /// <summary>Paints the necessary items if this controller needs to.</summary>
        void Render(IGdRenderContext context);

        /// <summary>A finger or pen was touched on the screen, or a mouse button was pressed.</summary>
        void OnPressed(Coordinate coordinate, long id, bool isConcat);

        /// <summary>A finger or pen was touched on the screen, or a mouse button was pressed.</summary>
        void OnReleased(Coordinate coordinate, long id, bool isConcat);

        /// <summary>The touch/mouse entered the view.</summary>
        void OnEntered(Coordinate coordinate, long id, bool isConcat);

        /// <summary>The touch (while down) or mouse (pressed or released) moved in the view.</summary>
        void OnMoved(Coordinate coordinate, long id, bool isConcat);

        /// <summary>The mouse wheel was scrolled.</summary>
        void OnWheelChanged(Coordinate coordinate, int delta);

        /// <summary>The touch/mouse exited the view.</summary>
        void OnExited(Coordinate coordinate, long id, bool isConcat);

        /// <summary>The touch/mouse exited the view.</summary>
        void OnCanceled(Coordinate coordinate, long id, bool isConcat);
    }
}

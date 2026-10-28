using System.Threading.Tasks;
using Godot;
using static Godot.GD;

namespace MiniGolf.Misc;

public static class SceneManager
{
    internal static class Fade
    {
        private static CanvasLayer _canvas;
        private static ColorRect _rect;
        private const float DefaultDuration = 0.48f;

        private static void EnsureCreated(SceneTree tree)
        {
            if (_rect != null && GodotObject.IsInstanceValid(_rect))
                return;

            _canvas = new CanvasLayer { Layer = 100 }; // draw above normal UI
            _rect = new ColorRect
            {
                Color = new Color(0, 0, 0, 0),
                MouseFilter = Control.MouseFilterEnum.Stop, // block clicks during fade
            };
            // Fill the screen
            _rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);

            _canvas.AddChild(_rect);
            tree.Root.AddChild(_canvas);
        }

        public static async Task Out(SceneTree tree, float duration = DefaultDuration)
        {
            EnsureCreated(tree);
            await TweenAlpha(tree, 0f, 1f, duration);
        }

        public static async Task In(SceneTree tree, float duration = DefaultDuration)
        {
            EnsureCreated(tree);
            await TweenAlpha(tree, 1f, 0f, duration);
            // After fading in, allow input again
            _rect.MouseFilter = Control.MouseFilterEnum.Ignore;
        }

        private static async Task TweenAlpha(SceneTree tree, float from, float to, float duration)
        {
            _rect.MouseFilter = Control.MouseFilterEnum.Stop;
            var tween = tree.CreateTween();
            tween
                .TweenProperty(_rect, "color:a", to, duration)
                .From(from)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

            await tree.ToSignal(tween, Tween.SignalName.Finished);
        }
    }

    public static async Task ExitScene()
    {
        try
        {
            var tree = (SceneTree)Engine.GetMainLoop();

            await Fade.Out(tree, 0.6f);
            tree.Quit();
        }
        catch (System.Exception e)
        {
            PrintErr($"ERROR!!!:{e.Message}");
            System.Environment.Exit(1);
        }
    }
}

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
        private const float DefaultDuration = 0.500f;

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

    public static async Task ChangeScene(string scene)
    {
        var tree = (SceneTree)Engine.GetMainLoop();

        await Fade.Out(tree); // Autoload fade out

        var err = tree.ChangeSceneToFile(scene);
        if (err != Error.Ok)
        {
            await Fade.In(tree);
            return;
        }

        await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);

        await Fade.In(tree);
    }

    public static async Task LoadingScene()
    {
        string defaultScene = "res://Scenes/Menu/Loading.tscn";

        var tree = (SceneTree)Engine.GetMainLoop();

        // 1. Fade to black
        await Fade.Out(tree);

        // 2. Switch scene
        var err = tree.ChangeSceneToFile(defaultScene);
        if (err != Error.Ok)
        {
            PrintErr($"SceneManager: failed to change scene to {defaultScene}: {err}");
            // Fade back in so the player isn't stuck on a black screen
            await Fade.In(tree);
            return;
        }

        // 3. Wait one frame so the new scene is actually in the tree and _Ready has run
        await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);

        // 4. Fade back in
        await Fade.In(tree);
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

    public static string NextScene { get; set; }
}

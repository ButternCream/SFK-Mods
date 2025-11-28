![SFK Logo](https://cdn1.epicgames.com/spt-assets/49d070acfaae4ac192ece9ca9bf30755/super-fantasy-kingdom-logo-1xqx2.png?resize=1&w=480&h=270&quality=medium)

# SFK UILib

A library to allow easy creation of various UI components in game.

Examples

- Overlays
- Text
- Buttons
- Layouts

**This is just a dependency library for developers to use in their mods. It is not a standalone mod.**

Code is available on my github. If you have questions or suggestions feel free to reach out of me in the Super Fantasy Kingdom Discord `@ButterandCream` or raise an issue on github.

All classes use a static factory pattern and have a associated `Create` method. I tried to chain methods as best as possible with a inheritance hierarchy so everything can be used together.

I also tried to mirror the existing in game components as best as possible, so if you are having trouble figuring out a layout, look in unity explorer

Here is an example snippet

```csharp
void CreateModsOverlay()
{
    var overlay = UIOverlay.Create(
        "ModsOverlay",
        new Vector2(-300, 0),
        bgMode: OverlayBackgroundMode.PanelFixed,
        bgColor: new Color(0f, 0f, 0f, 0.7f),
        panelSize: new Vector2(300, 400)
        )
        .AddHeader("SFK UILib");

    foreach (var mod in loadedPlugins)
    {
        var t = UIText.Create(
            mod.Value.Metadata.Name,
            overlay.Menu.container.Rect,
            Vector2.zero,
            size: 16,
            alignment: TextAlignmentOptions.TopLeft
        );
        overlay.Menu.Add(t);
    }
}
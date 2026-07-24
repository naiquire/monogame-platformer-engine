using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace lib.Input;

public enum PlayerAction
{
    Up, Down, Left, Right, Jump, Dash
}
public struct ActionBindings(List<Keys> keys, List<MouseButton> mouseButtons, List<Buttons> buttons)
{
    public IReadOnlyList<Keys> Keys { get; private set; } = keys;
    public IReadOnlyList<MouseButton> MouseButtons { get; private set; } = mouseButtons;
    public IReadOnlyList<Buttons> GamePadButtons { get; private set; } = buttons;

    public void SetBindings(List<Keys> keys, List<MouseButton> mouseButtons, List<Buttons> buttons)
    {
        Keys = keys;
        MouseButtons = mouseButtons;
        GamePadButtons = buttons;
    }
}

public class InputBindings
{

    public InputManager Devices { get; }
    
    private readonly Dictionary<PlayerAction, ActionBindings> KeyBindings;

    public InputBindings()
    {
        Devices = new();

        KeyBindings = [];
        SetBinding(PlayerAction.Up, new ActionBindings([Keys.Up], [], []));
        SetBinding(PlayerAction.Down, new ActionBindings([Keys.Down], [], []));
        SetBinding(PlayerAction.Left, new ActionBindings([Keys.Left], [], []));
        SetBinding(PlayerAction.Right, new ActionBindings([Keys.Right], [], []));

        SetBinding(PlayerAction.Dash, new ActionBindings([Keys.C], [], []));
        SetBinding(PlayerAction.Jump, new ActionBindings([Keys.Z], [], []));
    }

    public void Update(GameTime gameTime)
    {
        Devices.Update(gameTime);
    }

    public void SetBinding(PlayerAction action, ActionBindings binds)
    {
        KeyBindings[action] = binds;
    }

    public void FromFile(string fileName)
    {
        
    }

    private bool CheckBinding(PlayerAction action, Func<Keys, bool> keys, Func<MouseButton, bool> mouseButtons, Func<Buttons, bool> buttons)
    {
        ActionBindings binds = KeyBindings[action];

        foreach (Keys key in binds.Keys)
        {
            if (keys(key)) return true;
        }

        foreach (MouseButton button in binds.MouseButtons)
        {
            if (mouseButtons(button)) return true;
        }

        foreach (Buttons gamePadButton in binds.GamePadButtons)
        {
            if (buttons(gamePadButton)) return true;
        }

        return false;
    }

    public bool IsActionPressed(PlayerAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Devices.Keyboard.IsKeyDown, Devices.Mouse.IsButtonDown, Devices.GamePads[playerIndex].IsButtonDown);
    }

    public bool IsActionReleased(PlayerAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Devices.Keyboard.IsKeyUp, Devices.Mouse.IsButtonUp, Devices.GamePads[playerIndex].IsButtonUp);
    }

    public bool WasActionJustPressed(PlayerAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Devices.Keyboard.WasKeyJustPressed, Devices.Mouse.WasButtonJustPressed, Devices.GamePads[playerIndex].WasButtonJustPressed);
    }

    public bool WasActionJustReleased(PlayerAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Devices.Keyboard.WasKeyJustReleased, Devices.Mouse.WasButtonJustReleased, Devices.GamePads[playerIndex].WasButtonJustReleased);
    }

}
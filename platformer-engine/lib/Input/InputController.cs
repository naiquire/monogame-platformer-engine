using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace lib.Input;

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

public class InputController<TAction>() where TAction : Enum
{
    private readonly Dictionary<TAction, ActionBindings> _keyBindings = [];

    public void Update(GameTime gameTime)
    {
        Core.InputManager.Update(gameTime);
    }

    public void SetBinding(TAction action, ActionBindings binds)
    {
        _keyBindings[action] = binds;
    }

    public void FromFile(string fileName)
    {
        
    }

    private bool CheckBinding(TAction action, Func<Keys, bool> keys, Func<MouseButton, bool> mouseButtons, Func<Buttons, bool> buttons)
    {
        ActionBindings binds = _keyBindings[action];

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

    public bool IsActionPressed(TAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Core.InputManager.Keyboard.IsKeyDown, Core.InputManager.Mouse.IsButtonDown, Core.InputManager.GamePads[playerIndex].IsButtonDown);
    }

    public bool IsActionReleased(TAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Core.InputManager.Keyboard.IsKeyUp, Core.InputManager.Mouse.IsButtonUp, Core.InputManager.GamePads[playerIndex].IsButtonUp);
    }

    public bool WasActionJustPressed(TAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Core.InputManager.Keyboard.WasKeyJustPressed, Core.InputManager.Mouse.WasButtonJustPressed, Core.InputManager.GamePads[playerIndex].WasButtonJustPressed);
    }

    public bool WasActionJustReleased(TAction action, int playerIndex = 0)
    {
        return CheckBinding(action, Core.InputManager.Keyboard.WasKeyJustReleased, Core.InputManager.Mouse.WasButtonJustReleased, Core.InputManager.GamePads[playerIndex].WasButtonJustReleased);
    }

}
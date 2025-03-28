﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Localization.IME;
using System.Text.RegularExpressions;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using Terraria;
using ReLogic.OS;

namespace NewBeginnings.Common.UI;

public enum InputType
{
    Text,
    Integer,
    Number
}

/// <summary>
/// Ported from DragonLens: https://github.com/ScalarVector1/DragonLens/blob/master/Content/GUI/FieldEditors/TextField.cs <br/>
/// Cleaned up and modified to not use DragonLens UI.
/// </summary>
internal partial class UIEditableText : UIElement
{
    public readonly InputType InputType;

    private readonly string _backingString = "Enter text";

    public string currentValue = "";

    private bool _typing;
    private bool _updated;
    private bool _reset;

    // Composition string is handled at the very beginning of the update
    // In order to check if there is a composition string before backspace is typed, we need to check the previous state
    private bool _oldHasCompositionString;

    public UIEditableText(InputType inputType = InputType.Text, string backingText = "")
    {
        _backingString = backingText;
        InputType = inputType;
    }

    public void SetTyping()
    {
        _typing = true;
        Main.blockInput = true;
    }

    public void SetNotTyping()
    {
        _typing = false;
        Main.blockInput = false;
    }

    public override void LeftClick(UIMouseEvent evt) => SetTyping();

    public override void RightClick(UIMouseEvent evt)
    {
        SetTyping();
        currentValue = "";
        _updated = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (_reset)
        {
            _updated = false;
            _reset = false;
        }

        if (_updated)
            _reset = true;

        if (Main.mouseLeft && !IsMouseHovering)
            SetNotTyping();
    }

    public void HandleText()
    {
        if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            SetNotTyping();

        PlayerInput.WritingText = true;
        Main.instance.HandleIME();

        string newText = Main.GetInputText(currentValue);

        // GetInputText() handles typing operation, but there is a issue that it doesn't handle backspace correctly when the composition string is not empty.
        // It will delete a character both in the text and the composition string instead of only the one in composition string.
        // We'll fix the issue here to provide a better user experience
        if (_oldHasCompositionString && Main.inputText.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back))
            newText = currentValue; // force text not to be changed

        if (InputType == InputType.Integer && NumberRegex().IsMatch(newText))
        {
            if (newText != currentValue)
            {
                currentValue = newText;
                _updated = true;
            }
        }
        else if (InputType == InputType.Number && UnreadableRegex().IsMatch(newText)) //I found this regex on SO so no idea if it works right lol
        {
            if (newText != currentValue)
            {
                currentValue = newText;
                _updated = true;
            }
        }
        else
        {
            if (newText != currentValue)
            {
                currentValue = newText;
                _updated = true;
            }
        }

        _oldHasCompositionString = Platform.Get<IImeService>().CompositionString is { Length: > 0 };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_typing)
        {
            HandleText();

            // Draw ime panel, note that if there's no composition string then it won't draw anything
            Main.instance.DrawWindowsIMEPanel(GetDimensions().Position());
        }

        Rectangle rect = GetDimensions().ToRectangle();
        Vector2 pos = GetDimensions().Position() + Vector2.One * 4;
        Color color = Color.White;

        if (rect.Contains(Main.MouseScreen.ToPoint()))
            color = new Color(180, 180, 180);

        const float Scale = 1f;
        string displayed = currentValue ?? "";

        if (displayed != string.Empty)
            Utils.DrawBorderString(spriteBatch, displayed, pos, color, Scale);
        else
            Utils.DrawBorderString(spriteBatch, _backingString, pos, Color.Gray.MultiplyRGB(color), Scale);
            
        // Composition string + cursor drawing below
        if (!_typing)
            return;

        pos.X += FontAssets.MouseText.Value.MeasureString(displayed).X * Scale;
        string compositionString = Platform.Get<IImeService>().CompositionString;

        if (compositionString is { Length: > 0 })
        {
            Utils.DrawBorderString(spriteBatch, compositionString, pos, new Color(255, 240, 20), Scale);
            pos.X += FontAssets.MouseText.Value.MeasureString(compositionString).X * Scale;
        }

        if (Main.GameUpdateCount % 20 < 10)
            Utils.DrawBorderString(spriteBatch, "|", pos, Color.White, Scale);
    }

    [GeneratedRegex("[0-9]*$")]
    private static partial Regex NumberRegex();

    [GeneratedRegex("(?<=^| )[0-9]+(.[0-9]+)?(?=$| )|(?<=^| ).[0-9]+(?=$| )")]
    private static partial Regex UnreadableRegex();
}
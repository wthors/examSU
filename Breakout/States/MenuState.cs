namespace Breakout.States;

using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.GUI;
using DIKUArcade.Input;

/// <summary>
/// Base class for menu-style states with selectable text options.
/// </summary>

public abstract class MenuState : IGameState {
    protected Text[] texts = null!;
    protected int active = 0;
    protected readonly StateMachine stateMachine;
    protected Image? backgroundImage;
    protected StationaryShape? backgroundShape;

    protected MenuState(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
    }

    public virtual void Render(WindowContext context) {
        backgroundImage?.Render(context, backgroundShape!);
        for (int i = 0; i < texts.Length; i++) {
            if (i == active) {
                texts[i].SetColor(255, 255, 0, 255);
            } else {
                texts[i].SetColor(255, 255, 255, 255);
            }
            texts[i].Render(context);
        }
    }

    public virtual void Update() {
        // menus are static by default
    }

    public virtual void HandleKeyEvent(KeyboardAction action, KeyboardKey key) {
        if (action != KeyboardAction.KeyPress)
            return;
        if (key == KeyboardKey.Up) {
            active = (active > 0) ? active - 1 : texts.Length - 1;
        } else if (key == KeyboardKey.Down) {
            active = (active < texts.Length - 1) ? active + 1 : 0;
        } else if (key == KeyboardKey.Enter) {
            PerformAction();
        }
    }

    protected abstract void PerformAction();
}
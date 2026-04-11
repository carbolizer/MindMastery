/* =========================================================================== //
    Harrison Day 04/04/26

    These classes are for a generic state machine, in which you can define
    states by what they should do on entering, on updating, and on exiting.
    transitions can be defined to switch to a certain state under a certain
    condition. Call StateMachine.Update(deltaTime) every frame.
// =========================================================================== */



using System;
using System.Collections.Generic;



/*
    Transition
    Usage: new Transition("Run", () => speed > 0.5f)
    Transition to "Run" state if speed is > 0.5f.
*/
public class Transition {
    public string       targetState;
    public Func<bool>   condition;

    public Transition(string targetState, Func<bool> condition) {
        this.targetState = targetState;
        this.condition = condition;
    }
}



/*
    State
    Usage:  new State(
                "Idle",
                onEnter:    () => { },
                onUpdate:   (dt) => { },
                onExit:     () => { }
            );
*/
public class State {
    public State(
        string              name,
        EnterAction onEnter,
        UpdateAction onUpdate,
        ExitAction onExit
    ) {
        m_name = name;
        m_onEnter = onEnter;
        m_onUpdate = onUpdate;
        m_onExit = onExit;
    }
// public:
    public delegate void EnterAction();
    public delegate void UpdateAction(float dt);
    public delegate void ExitAction();

    public void OnEnter()                               { m_onEnter.Invoke(); }
    public void OnUpdate(float dt)                      { m_onUpdate.Invoke(dt); }
    public void OnExit()                                { m_onExit.Invoke(); }

    public string GetName()                             { return m_name; }
    public void SetName(string name)                    { m_name = name; }

    public void SetOnEnter(EnterAction onEnter)         { m_onEnter = onEnter; }
    public void SetOnUpdate(UpdateAction onUpdate)      { m_onUpdate = onUpdate; }
    public void SetOnExit(ExitAction onExit)            { m_onExit = onExit; }

    public void AddTransition(Transition transition)    { m_transitions.Add(transition); }

    public string CheckTransitions() {
        foreach (Transition transition in m_transitions)
            if (transition.condition()) return transition.targetState;
        return null;
    }

// private:
    private string              m_name;
    private EnterAction         m_onEnter;
    private UpdateAction        m_onUpdate;
    private ExitAction          m_onExit;

    private List<Transition>    m_transitions           = new();
}



/*
    StateMachine
    Usage: StateMachine sm = new StateMachine();
           sm.AddState(idleState);
           sm.SetState("Idle");
           sm.Update(deltaTime);    // call every frame
*/
public class StateMachine {
// public:
    public void AddState(State state) => m_states[state.GetName()] = state;

    public void SetState(string name) { 
        m_currentState?.OnExit(); 
        m_currentState = m_states[name]; 
        m_currentState.OnEnter(); 
    }
    
    public void Update(float dt) {
        string next = m_currentState.CheckTransitions();
        if (next != null) { SetState(next); return; }
        m_currentState.OnUpdate(dt);
    }

    public string GetCurrentState() { return m_currentState?.GetName(); }

// private:
    private Dictionary<string, State> m_states = new();
    private State m_currentState = null;
}
using FluentAssertions;

namespace Statewright.Tests;

public class StateMachineTests
{
    enum TestEnum
    {
        Phase1,
        Phase2
    }

    enum TestTrigger
    {
        GotoPhase2,
        GotoPhase1
    }
    
    [Fact]
    public void Transition_Is_Correct()
    {
        StateMachine<TestEnum, TestTrigger> stateMachine = new(TestEnum.Phase1);
        stateMachine.Configure(TestEnum.Phase1, TestTrigger.GotoPhase2, TestEnum.Phase2);
        stateMachine.Configure(TestEnum.Phase2, TestTrigger.GotoPhase1, TestEnum.Phase1);
        Assert.Equal(TestEnum.Phase1, stateMachine.CurrentState);
        
        bool result = stateMachine.TryTransition(TestTrigger.GotoPhase2);
        
        result.Should().BeTrue();
        stateMachine.CurrentState.Should().Be(TestEnum.Phase2);
        
        result = stateMachine.TryTransition(TestTrigger.GotoPhase1);
        
        result.Should().BeTrue();
        stateMachine.CurrentState.Should().Be(TestEnum.Phase1);
    }    
    
    [Fact]
    public void InitialState_Is_Correct()
    {
        StateMachine<TestEnum, TestTrigger> stateMachine = new(TestEnum.Phase1);
        stateMachine.CurrentState.Should().Be(TestEnum.Phase1);
    }
}

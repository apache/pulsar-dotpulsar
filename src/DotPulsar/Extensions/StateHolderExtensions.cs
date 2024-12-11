/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Extensions;

using DotPulsar.Abstractions;

/// <summary>
/// Extensions for IStateHolder.
/// </summary>
public static class StateHolderExtensions
{
    /// <summary>
    /// Wait for the state to change to a specific state with a delay.
    /// </summary>
    /// <returns>
    /// The current state.
    /// </returns>
    /// <remarks>
    /// If the state change to a final state, then all awaiting tasks will complete.
    /// </remarks>
    public static async ValueTask<TState> OnStateChangeTo<TState>(
        this IStateHolder<TState> stateHolder,
        TState state,
        TimeSpan delay,
        CancellationToken cancellationToken = default) where TState : notnull
        => await stateHolder.State.OnStateChangeTo(state, delay, cancellationToken);

    /// <summary>
    /// Wait for the state to change from a specific state with a delay.
    /// </summary>
    /// <returns>
    /// The current state.
    /// </returns>
    /// <remarks>
    /// If the state change to a final state, then all awaiting tasks will complete.
    /// </remarks>
    public static async ValueTask<TState> OnStateChangeFrom<TState>(
        this IStateHolder<TState> stateHolder,
        TState state,
        TimeSpan delay,
        CancellationToken cancellationToken = default) where TState : notnull
    => await stateHolder.State.OnStateChangeFrom(state, delay, cancellationToken);

    /// <summary>
    /// Will invoke the onStateLeft callback when the state if left (with delay) and onStateReached when it's reached again.
    /// </summary>
    /// <returns>
    /// ValueTask that will run as long as a final state is not entered.
    /// </returns>
    public static async ValueTask DelayedStateMonitor<TEntity, TState, TFaultContext>(
        this TEntity stateHolder,
        TState state,
        TimeSpan delay,
        Func<TEntity, TState, CancellationToken, ValueTask<TFaultContext>> onStateLeft,
        Func<TEntity, TState, TFaultContext, CancellationToken, ValueTask> onStateReached,
        CancellationToken cancellationToken = default) where TEntity : IStateHolder<TState> where TState : notnull where TFaultContext : class
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentState = await stateHolder.OnStateChangeFrom(state, delay, cancellationToken).ConfigureAwait(false);

            TFaultContext? faultContext = null;

            try
            {
                faultContext = await onStateLeft(stateHolder, currentState, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }

            if (stateHolder.State.IsFinalState(currentState))
                return;

            currentState = await stateHolder.State.OnStateChangeTo(state, cancellationToken).ConfigureAwait(false);

            if (stateHolder.State.IsFinalState(currentState))
                return;

            try
            {
                if (faultContext is not null)
                    await onStateReached(stateHolder, currentState, faultContext, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <summary>
    /// Will invoke the onStateLeft callback when the state if left (with delay) and onStateReached when it's reached again.
    /// </summary>
    /// <returns>
    /// ValueTask that will run as long as a final state is not entered.
    /// </returns>
    public static async ValueTask DelayedStateMonitor<TEntity, TState>(
        this TEntity stateHolder,
        TState state,
        TimeSpan delay,
        Func<TEntity, TState, CancellationToken, ValueTask> onStateLeft,
        Func<TEntity, TState, CancellationToken, ValueTask> onStateReached,
        CancellationToken cancellationToken = default) where TEntity : IStateHolder<TState> where TState : notnull
    {
        async ValueTask<string> onStateLeftFunction(TEntity entity, TState state, CancellationToken cancellationToken)
        {
            await onStateLeft(entity, state, cancellationToken).ConfigureAwait(false);
            return string.Empty;
        }

        async ValueTask onStateReachedFunction(TEntity entity, TState state, string faultContext, CancellationToken cancellationToken)
        {
            await onStateReached(entity, state, cancellationToken).ConfigureAwait(false);
        }

        await stateHolder.DelayedStateMonitor(state, delay, onStateLeftFunction, onStateReachedFunction, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Will invoke the onStateLeft callback when the state if left (with delay) and onStateReached when it's reached again.
    /// </summary>
    /// <returns>
    /// ValueTask that will run as long as a final state is not entered.
    /// </returns>
    public static async ValueTask DelayedStateMonitor<TEntity, TState>(
        this TEntity stateHolder,
        TState state,
        TimeSpan delay,
        Action<TEntity, TState> onStateLeft,
        Action<TEntity, TState> onStateReached,
        CancellationToken cancellationToken = default) where TEntity : IStateHolder<TState> where TState : notnull
    {
        ValueTask<string> onStateLeftFunction(TEntity entity, TState state, CancellationToken cancellationToken)
        {
            onStateLeft(entity, state);
            return new ValueTask<string>(string.Empty);
        }

        ValueTask onStateReachedFunction(TEntity entity, TState state, string faultContext, CancellationToken cancellationToken)
        {
            onStateReached(entity, state);
            return new ValueTask();
        }

        await stateHolder.DelayedStateMonitor(state, delay, onStateLeftFunction, onStateReachedFunction, cancellationToken).ConfigureAwait(false);
    }
}

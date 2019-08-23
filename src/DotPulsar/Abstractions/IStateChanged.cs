using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A state change monitoring abstraction
    /// </summary>
    public interface IStateChanged<TState>
    {
        /// <summary>
        /// Wait for the state to change to a specific state
        /// </summary>
        /// <returns>
        /// The current state
        /// </returns>
        /// <remarks>
        /// If the state change to a final state, then all awaiting tasks will complete
        /// </remarks>
        Task<TState> StateChangedTo(TState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Wait for the state to change from a specific state
        /// </summary>
        /// <returns>
        /// The current state
        /// </returns>
        /// <remarks>
        /// If the state change to a final state, then all awaiting tasks will complete
        /// </remarks>
        Task<TState> StateChangedFrom(TState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ask whether the current state is final, meaning that it will never change
        /// </summary>
        /// <returns>
        /// True if it's final and False if it's not
        /// </returns>
        bool IsFinalState();

        /// <summary>
        /// Ask whether the provided state is final, meaning that it will never change
        /// </summary>
        /// <returns>
        /// True if it's final and False if it's not
        /// </returns>
        bool IsFinalState(TState state);
    }
}

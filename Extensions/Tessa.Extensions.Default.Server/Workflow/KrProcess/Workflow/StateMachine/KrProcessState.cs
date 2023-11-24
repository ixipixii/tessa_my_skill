using System;
using System.Collections.Generic;
using System.Linq;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow.StateMachine
{
    public sealed class KrProcessState : IEquatable<KrProcessState>
    {
        public static readonly KrProcessState Default = new KrProcessState(nameof(Default));

        public KrProcessState(
            string name,
            Dictionary<string, object> parameters = null,
            KrProcessState nextState = null)
        {
            this.Name = name;
            this.Parameters = parameters ?? new Dictionary<string, object>();
            this.NextState = nextState;
        }

        /// <summary>
        /// Название состояния, которое уникально идентифицирует его.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Дополнительные параметры состояния.
        /// </summary>
        public Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// Рекомендация, какое состояние следует выставить после текущего.
        /// </summary>
        public KrProcessState NextState { get; }

        /// <inheritdoc />
        public bool Equals(
            KrProcessState other)
        {
            if (other is null)
            {
                return false;
            }
            return ReferenceEquals(this, other) || string.Equals(this.Name, other.Name);
        }

        /// <inheritdoc />
        public override bool Equals(
            object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is KrProcessState state && this.Equals(state);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Name?.GetHashCode() ?? 0;
        }

        public static bool operator ==(
            KrProcessState left,
            KrProcessState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(
            KrProcessState left,
            KrProcessState right)
        {
            return !Equals(left, right);
        }


        public override string ToString() => $"{this.Name}, " +
            $"({string.Join(", ", this.Parameters.Select(p => $"{p.Key} = {p.Value}"))}), " +
            $"Next = {this.NextState?.Name ?? "null"}";
    }
}
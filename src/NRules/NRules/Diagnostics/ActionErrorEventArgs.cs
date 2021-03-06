using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during action execution.
    /// </summary>
    public class ActionErrorEventArgs : RecoverableErrorEventArgs
    {
        private readonly Tuple _tuple;

        internal ActionErrorEventArgs(Exception exception, Expression expression, Tuple tuple) : base(exception)
        {
            _tuple = tuple;
            Action = expression;
        }

        /// <summary>
        /// Action that caused exception.
        /// </summary>
        public Expression Action { get; private set; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<FactInfo> Facts { get { return _tuple.Facts.Reverse().Select(x => new FactInfo(x)).ToArray(); } }
    }
}
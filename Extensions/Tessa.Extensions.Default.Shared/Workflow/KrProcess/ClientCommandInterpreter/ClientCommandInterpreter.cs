using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Tessa.Platform;
using Unity;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess.ClientCommandInterpreter
{
    public sealed class ClientCommandInterpreter : IClientCommandInterpreter
    {
        #region fields

        private readonly ConcurrentDictionary<string, Type> handlers = 
            new ConcurrentDictionary<string, Type>();

        private Lazy<ReadOnlyDictionary<string, Type>> handlersRoLazy;

        private readonly IUnityContainer unityContainer;

        #endregion

        #region constructor

        public ClientCommandInterpreter(
            IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.InitLazy();
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IClientCommandInterpreter RegisterHandler(
            string commandType,
            Type handlerType)
        {
            if (!handlerType.Implements<IClientCommandHandler>())
            {
                throw new ArgumentException($"handlerType doesn't implement {nameof(IClientCommandHandler)}.");
            }
            this.handlers[commandType] = handlerType;
            this.InitLazy();
            return this;
        }

        /// <inheritdoc />
        public IClientCommandInterpreter RegisterHandler<T>(
            string commandType) where T: IClientCommandHandler
        {
            this.RegisterHandler(commandType, typeof(T));
            this.InitLazy();
            return this;
        }

        /// <inheritdoc />
        public void Interpret(
            IEnumerable<KrProcessClientCommand> commands,
            object context)
        {
            var ctx = new ClientCommandHandlerContext
            {
                OuterContext = context,
                Info = new Dictionary<string, object>()
            };
            
            var handlerRo = this.handlersRoLazy.Value;
            foreach (var command in commands)
            {
                if (!handlerRo.TryGetValue(command.CommandType, out var handlerType))
                {
                    continue;
                }

                ctx.Command = command;
                var handler = this.unityContainer.Resolve(handlerType);
                ((IClientCommandHandler)handler).Handle(ctx);
            }
        }

        #endregion

        #region private

        private void InitLazy()
        {
            this.handlersRoLazy = new Lazy<ReadOnlyDictionary<string, Type>>(
                () => new ReadOnlyDictionary<string, Type>(this.handlers.ToDictionary(k => k.Key, v => v.Value)),
                LazyThreadSafetyMode.PublicationOnly);
        }

        #endregion
    }
}
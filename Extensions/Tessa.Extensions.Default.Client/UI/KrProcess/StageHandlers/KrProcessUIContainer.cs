using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Unity;

namespace Tessa.Extensions.Default.Client.UI.KrProcess.StageHandlers
{
    public sealed class KrProcessUIContainer : IKrProcessUIContainer
    {
        #region fields

        private readonly StageTypeDescriptor allStagesFakeDescriptor = StageTypeDescriptor.Create(
            b => b.ID = new Guid(0x92D8F95C, 0x96FA, 0x484A, 0xA6, 0x50, 0xC8, 0x80, 0xC4, 0x4A, 0xC6, 0x6A));
        
        private readonly IUnityContainer unityContainer;
        private readonly ConcurrentDictionary<Guid, List<Type>> items =
            new ConcurrentDictionary<Guid, List<Type>>();

        #endregion

        #region constructor

        public KrProcessUIContainer(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        #endregion

        #region implementation

        /// <inheritdoc />
        public IKrProcessUIContainer RegisterUIHandler<T>()
            where T : IStageTypeUIHandler
        {
            return this.RegisterUIHandler<T>(this.allStagesFakeDescriptor);
        }

        /// <inheritdoc />
        public IKrProcessUIContainer RegisterUIHandler<T>(StageTypeDescriptor descriptor)
            where T : IStageTypeUIHandler
        {
            this.CheckRegistered(typeof(T));
            this.RegisterUIHandlerInternal(descriptor, typeof(T));
            return this;
        }

        /// <inheritdoc />
        public IKrProcessUIContainer RegisterUIHandler(
            Type handlerType)
        {
            return this.RegisterUIHandler(this.allStagesFakeDescriptor, handlerType);
        }

        /// <inheritdoc />
        public IKrProcessUIContainer RegisterUIHandler(
            StageTypeDescriptor descriptor,
            Type formatterType)
        {
            this.CheckRegistered(formatterType);
            this.RegisterUIHandlerInternal(descriptor, formatterType);
            return this;
        }

        /// <inheritdoc />
        public List<IStageTypeUIHandler> ResolveUIHandlers(Guid descriptorID)
        {
            var handlers = new List<IStageTypeUIHandler>();
            if (this.items.TryGetValue(this.allStagesFakeDescriptor.ID, out var allStagesHandlers))
            {
                foreach (var item in allStagesHandlers)
                {
                    handlers.Add((IStageTypeUIHandler)this.unityContainer.Resolve(item));
                }
            }
            
            if (this.items.TryGetValue(descriptorID, out var handlerRegistrationForDescriptor))
            {
                foreach (var item in handlerRegistrationForDescriptor)
                {
                    handlers.Add((IStageTypeUIHandler)this.unityContainer.Resolve(item));
                }
            }
            return handlers;
        }

        #endregion

        #region private

        private void RegisterUIHandlerInternal(StageTypeDescriptor descriptor, Type t)
        {
            var registrationHandlers = this.items.GetOrAdd(descriptor.ID, new List<Type>());
            registrationHandlers.Add(t);
        }

        private void CheckRegistered(Type formatterType)
        {
            if (!this.unityContainer.IsRegistered(formatterType))
            {
                throw new ArgumentException(
                    $"Type {formatterType.FullName} is not registered in UnityContainer.{Environment.NewLine}" +
                    $"Add container.RegisterType<{typeof(IStageTypeUIHandler).Name}, {formatterType.Name}>() in your Registrator class.");
            }
        }
        
        #endregion

    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceScanProvider.cs" company="Syntellect">
//   Tessa Project
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tessa.Extensions.Default.Client.Scanning
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;

    using Tessa.Extensions.Platform.Client.Scanning;
    using Tessa.Host;
    using Tessa.Platform.Validation;
    using Tessa.Properties.Resharper;
    using Tessa.UI;
    using Tessa.UI.Notifications;

    /// <summary>
    ///     Провайдер взаимодействующий с сервисом сканирования
    /// </summary>
    public sealed class ServiceScanProvider : ViewModel<EmptyModel>, IScanProvider
    {
        /// <summary>
        ///     The notification ui manager.
        /// </summary>
        [NotNull]
        private readonly INotificationUIManager notificationUIManager;

        /// <summary>
        ///     The action.
        /// </summary>
        [CanBeNull]
        private Action<MemoryStream> processScannedStreamAction;

        /// <summary>
        ///     The proxy.
        /// </summary>
        [CanBeNull]
        private Lazy<IScanServiceProxy> proxy;

        private readonly ScanServiceProxyFactory proxyFactory;

        /// <inheritdoc />
        public ServiceScanProvider(
            [NotNull] ScanServiceProxyFactory proxyFactory,
            [NotNull] INotificationUIManager notificationUIManager)
        {
            if (proxyFactory == null)
            {
                throw new ArgumentNullException("proxyFactory");
            }

            if (notificationUIManager == null)
            {
                throw new ArgumentNullException("notificationUIManager");
            }

            this.notificationUIManager = notificationUIManager;
            this.proxyFactory = proxyFactory;
            this.proxy = new Lazy<IScanServiceProxy>(this.CreateProxy);
        }

        private IScanServiceProxy CreateProxy()
        {
            var proxy = proxyFactory();
            PropertyChangedEventManager.AddHandler(proxy, this.StateChanged, "State");
            return proxy;
        }

        private void StateChanged(object sender, PropertyChangedEventArgs e)
        {
            DispatcherHelper.InvokeInUI(() => this.OnPropertyChanged("State"));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.proxy.IsValueCreated)
            {
                this.proxy.Value.Dispose();
                this.proxy = null;
            }
        }

        /// <inheritdoc />
        public List<IScanSource> GetSources()
        {
            return this.proxy
                       .Value
                       .GetSources().Select(s => (IScanSource)new ServiceScanSourceViewModel(s))
                       .ToList();
        }

        /// <inheritdoc />
        public bool Scan(IScanSource source)
        {
            try
            {
                var scanSource = new ScanSource(
                    ((ServiceScanSourceViewModel)source).ID,
                    source.Name,
                    source.ProtocolVersion,
                    source.DriverVersion);
                var request = new ScanRequest { ScanSource = scanSource };
                this.proxy.Value.StartScan(
                    request,
                    s => DispatcherHelper.InvokeInUI(() => this.processScannedStreamAction.Invoke(s)),
                    vr =>
                    {
                        if (!vr.IsSuccessful)
                        {
                            this.Cancel();
                        }

                        if (!this.notificationUIManager.IsMuted())
                        {
                            foreach (ValidationResultItem item in vr.Items)
                            {
                                _ = this.notificationUIManager.ShowTextAsync(
                                    item.Message,
                                    clickCommand: new DelegateCommand(p => TessaDialog.ShowNotEmpty(vr)),
                                    textAlignment: TextAlignment.Left);
                            }
                        }
                        else
                        {
                            TessaDialog.ShowNotEmpty(vr);
                        }
                    });
            }
            catch (ScannerBusyException)
            {
                _ = this.notificationUIManager.ShowTextOrMessageBoxAsync("$UI_Misc_ScannerIsBusy");
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void SetProcessAction(Action<MemoryStream> action)
        {
            this.processScannedStreamAction = action;
        }

        /// <inheritdoc />
        public void Cancel()
        {
            if (this.proxy.IsValueCreated)
            {
                this.proxy.Value.Cancel();
            }
        }

        /// <inheritdoc />
        public ScanState State
        {
            get
            {
                return this.proxy.IsValueCreated ? this.proxy.Value.State : ScanState.Completed;
            }
        }
    }
}
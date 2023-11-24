using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Tessa.Extensions.Shared.Models;

namespace Tessa.Extensions.Shared
{
    public static class PnrFdCardStates
    {
        /// <summary>
        /// Договор. На подписании.
        /// </summary>
        public static readonly PnrFdCardState PnrContractOnSigning = new PnrFdCardState(new Guid("77246376-ffae-466e-bbc9-d70e0ffe8964"), "На подписании");

        /// <summary>
        /// Договор. Действует.
        /// </summary>
        public static readonly PnrFdCardState PnrContractActing = new PnrFdCardState(new Guid("a787669e-32f7-470a-839c-d916222fb9da"), "Действует");

        /// <summary>
        /// Регламентирующий документ (ВНД). Проект.
        /// </summary>
        public static readonly PnrFdCardState PnrRegulationProject = new PnrFdCardState(new Guid("a7f3846a-3b55-4c79-9ac0-9070ca2d39f9"), "Проект");

        /// <summary>
        /// Регламентирующий документ (ВНД). Действует.
        /// </summary>
        public static readonly PnrFdCardState PnrRegulationWorks = new PnrFdCardState(new Guid("e7e341cf-70d1-4262-a0ec-7dabbf6a221b"), "Действует");

        /// <summary>
        /// Регламентирующий документ (ВНД). Отменен.
        /// </summary>
        public static readonly PnrFdCardState PnrRegulationCanceled = new PnrFdCardState(new Guid("f3e415f6-2c4e-42b7-a206-44040f60473b"), "Отменен");

    }
}

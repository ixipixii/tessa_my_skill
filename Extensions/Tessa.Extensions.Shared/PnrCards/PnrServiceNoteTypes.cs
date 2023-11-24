using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Shared.PnrCards
{
    public static class PnrServiceNoteTypes
    {
        // По вопросам заключения договоров и выбора подрядчика
        public static readonly Guid ConclusionContracts = new Guid("78C084DD-D65B-4C70-B8DE-D7B22221CA5F");

        // По вопросам заключения договоров и выбора подрядчика ДУП
        public static readonly Guid ConclusionContractsDUP = new Guid("4239B0E7-A25D-40CE-8D8B-900CDA95FB8F");
                                                                       
        // По вопросам финансовой деятельности и обучения
        public static readonly Guid FinancialActivities = new Guid("48A6607B-FB95-452C-A076-746A0A9F0B18");

        // По личному составу
        public static readonly Guid Personnel = new Guid("A53C837D-9D20-4ADA-9D6F-D629F5EAEB66");
                                                          
        // По рабочим вопросам
        public static readonly Guid WorkQuestions = new Guid("D6088DD6-625F-4A7D-85D4-A42270CB4341");
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class KrConstants
    {
        public const string KrProcessName = "KrProcess";

        public const string KrSecondaryProcessName = "KrSecondaryProcess";

        public const string KrNestedProcessName = "KrNestedProcess";

        public const string KrPerformSignal = "KrPerformSignal";

        public const string KrSatelliteInfoKey = CardSatelliteHelper.SatelliteKey + "_krProcess";

        public const string KrSecondarySatelliteListInfoKey = CardSatelliteHelper.SatelliteKey + "_krSecondaryProcessList";
        
        public const string KrDialogSatelliteListInfoKey = CardSatelliteHelper.SatelliteKey + "_krdialogsSatellites";

        public const string TaskSatelliteFileInfoListKey = "KrSecondarySatelliteFileInfoList";

        public const string TaskSatelliteMovedFileInfoListKey = "KrSecondarySatelliteMovedFileInfoList";
        
        public const string DialogSatelliteFileInfoListKey = "KrSecondarySatelliteFileInfoList";
        
        public const string DialogSatelliteMovedFileInfoListKey = "KrSecondarySatelliteMovedFileInfoList";

        public const string KrStartProcessSignal = nameof(KrStartProcessSignal);

        public const string KrStartProcessUnlessStartedGlobalSignal = nameof(KrStartProcessUnlessStartedGlobalSignal);

        public const string KrSkipProcessGlobalSignal = nameof(KrSkipProcessGlobalSignal);

        public const string KrCancelProcessGlobalSignal = nameof(KrCancelProcessGlobalSignal);

        public const string KrTransitionGlobalSignal = nameof(KrTransitionGlobalSignal);

        public const string DialogSaveActionSignal = nameof(DialogSaveActionSignal);
        
        public const string KrTransitionKeepStates = nameof(KrTransitionKeepStates);

        public const string KrTransitionCurrentGroup = nameof(KrTransitionCurrentGroup);

        public const string KrTransitionNextGroup = nameof(KrTransitionNextGroup);

        public const string KrTransitionPrevGroup = nameof(KrTransitionPrevGroup);

        public static readonly Guid LaunchProcessRequestType =
            new Guid(0x05744928, 0xE676, 0x4B69, 0x8A, 0x80, 0x20, 0xBC, 0xE6, 0x1D, 0x8F, 0x31);

        public const string DefaultProcessState = nameof(DefaultProcessState);

        public const string InterruptionProcessState = nameof(InterruptionProcessState);

        public const string CancelellationProcessState = nameof(CancelellationProcessState);

        public const string SkipProcessState = nameof(SkipProcessState);

        public const string TransitionProcessState = nameof(TransitionProcessState);

        public const string AsyncForkedProcessCompletedSingal = nameof(AsyncForkedProcessCompletedSingal);

        public const string ForkAddBranchSignal = nameof(ForkAddBranchSignal);

        public const string ForkRemoveBranchSignal = nameof(ForkRemoveBranchSignal);

        public static readonly Guid NewIterationStageTemplate =
            new Guid(0x8EABC594, 0x4A37, 0x4383, 0xA9, 0x1D, 0xF5, 0x46, 0xBB, 0xB1, 0xDD, 0x17);

        public static readonly Guid EditStageTemplate =
            new Guid(0xECCBE66B, 0x496C, 0x45B0, 0xAC, 0xEA, 0x82, 0x50, 0x82, 0xFA, 0xE5, 0xBD);
        
        public static readonly Guid StartProcessButton =
            new Guid(0x307F34CE, 0x5C09, 0x4F28, 0xB1, 0x06, 0x25, 0xE2, 0xAE, 0xB3, 0x96, 0x0D);

        public static readonly Guid RegisterButton =
            new Guid(0x79B58276, 0xE886, 0x41DD, 0x92, 0x67, 0x64, 0x61, 0x24, 0xA6, 0x21, 0xFC);

        public static readonly Guid DeregisterButton =
            new Guid(0x9D67CE17, 0xFD7A, 0x4994, 0x96, 0x69, 0xDD, 0xF1, 0x67, 0x7F, 0x76, 0x31);

        public static readonly Guid RejectProcessButton =
            new Guid(0x5323C83E, 0xB5D4, 0x4FB4, 0x86, 0x9C, 0x28, 0x5D, 0x51, 0xE7, 0x16, 0xE6);

        public static readonly Guid CancelProcessButton =
            new Guid(0x71E70421, 0x07E3, 0x477E, 0xBC, 0x0C, 0x5F, 0x63, 0x19, 0x84, 0x55, 0xA1);

        public static readonly Guid RebuildProcessButton =
            new Guid(0xF0847865, 0xE89A, 0x4A1E, 0xAE, 0x10, 0x37, 0xA0, 0x7F, 0x3F, 0x71, 0x48);

        public static readonly Guid DefaultApprovalStageGroup =
            new Guid(0x498CB3C3, 0x23B5, 0x469D, 0xA9, 0xA3, 0x05, 0xA6, 0x2A, 0x09, 0x8C, 0x92);

        public static readonly Guid AdvisoryTaskKindID =
            new Guid(0x2E6C5D3E, 0xD408, 0x4F98, 0x8A, 0x55, 0xE9, 0xD1, 0x31, 0x6B, 0xF2, 0xCC);
        
        /// <summary>
        /// ID роли, служащей маркером в списке согласующих.
        /// На место данной роли необходимо подставлять роли, вычисленные через SQL запрос
        /// </summary>
        public static readonly Guid SqlApproverRoleID =
            new Guid(0xcd4d4a0d, 0x414f, 0x478d, 0xa2, 0x26, 0x31, 0x9a, 0xa8, 0x41, 0x7f, 0x88);

        /// <summary>
        /// Количество стандартных этапов в машруте.
        /// На текуший момент 3 - управление историей, доработка и управление процессом.
        /// </summary>
        public const int DefaultStagesCount = 3;

        /// <summary>
        /// Стандартный порядок для группы, генерируемой для вторичного процесса
        /// </summary>
        public const int DefaultSecondaryProcessGroupOrder = 0;
        
        /// <summary>
        /// Стандартный порядок для шаблона, генерируемой для вторичного процесса
        /// </summary>
        public const int DefaultSecondaryProcessTemplateOrder = 0;

        public static class Keys
        {
            public const string KrStageRowsSignatures = "StageSignatures";

            public const string KrStageRowsOrders = "OriginalStageOrders";

            public const string ParentStageRowID = CardHelper.UserKeyPrefix + "ParentStageRowID";
            
            public const string RootStage =  CardHelper.UserKeyPrefix + "RootStage";
            
            public const string NestedStage =  CardHelper.UserKeyPrefix + "NestedStage";

            public const string DocTypeID = "docTypeID";

            public const string DocTypeTitle = "docTypeTitle";

            public const string StateBeforeRegistration = nameof(StateBeforeRegistration);

            public const string Cycle = nameof(Cycle);

            public const string IgnoreChangeState = nameof(IgnoreChangeState);

            public const string Compile = CardHelper.SystemKeyPrefix + "Compile";

            public const string CompileWithValidationResult = CardHelper.SystemKeyPrefix + "CompileWithValidationResult";

            public const string CompileAll = CardHelper.SystemKeyPrefix + "CompileAll";

            public const string CompileAllWithValidationResult = CardHelper.SystemKeyPrefix + "CompileAllWithValidationResult";
            
            public const string KeepStageStatesParam = nameof(KeepStageStatesParam);

            public const string FinalStageRowIDParam = nameof(FinalStageRowIDParam);
            
            public const string PreparingGroupRecalcStrategyParam = nameof(PreparingGroupRecalcStrategyParam);
            
            public const string ForceStartGroupParam = nameof(ForceStartGroupParam);

            public const string DirectionAfterInterruptParam = nameof(DirectionAfterInterruptParam);
            
            public const string ForkNestedProcessInfo = nameof(ForkNestedProcessInfo);

            public const string ProcessHolderID = nameof(ProcessHolderID);
            
            public const string NestedProcessID = nameof(NestedProcessID);

            public const string MainProcessType = nameof(MainProcessType);
            
            public const string ParentProcessType = nameof(ParentProcessType);
            
            public const string ParentProcessID = nameof(ParentProcessID);

            public const string ProcessID = nameof(ProcessID);

            public const string ProcessInfoAtEnd = nameof(ProcessInfoAtEnd);

            public const string ExtraSourcesChanged = nameof(ExtraSourcesChanged);

            public const string NewCard = nameof(NewCard);
            
            public const string NewCardSignature = nameof(NewCardSignature);
            
            public const string NewCardID = "cardID";
            
            public const string TemplateID = nameof(TemplateID);

            public const string TypeID = nameof(TypeID);

            public const string TypeCaption = nameof(TypeCaption);

            public const string Tasks = nameof(Tasks);

            public const string ProcessInstance = nameof(ProcessInstance);
            
            public const string CompletionOptionSettings = nameof(CompletionOptionSettings);

            public const string NotReturnEdit = nameof(NotReturnEdit);

            public const string Disapproved = nameof(Disapproved);
        }

        public static class Ui
        {
            public const string DefaultTileGroupIcon = "Thin109";

            public const string CanMoveCheckboxAlias = "CanMoveCheckboxAlias";

            public const string KrApprovalProcessFormAlias = "ApprovalProcess";
            public const string KrApprovalStagesBlockAlias = "ApprovalStagesBlock";
            public const string KrApprovalStagesControlAlias = "ApprovalStagesTable";
            public const string KrSummaryBlockAlias = "SummaryBlock";
            public const string KrDisclaimerBlockAlias = "DisclaimerBlock";

            public const string KrBlockForDocStatusAlias = "KrBlockForDocStatus";

            public const string KrStageCommonInfoBlock = "StageCommonInfoBlock";
            public const string KrTimeLimitInputAlias = "TimeLimitInput";
            public const string KrPlannedInputAlias = "PlannedInput";
            public const string KrHiddenStageCheckboxAlias = "HiddenStageCheckbox";
            public const string KrCanBeSkippedCheckboxAlias = "CanBeSkippedCheckbox";

            public const string KrPerformersBlockAlias = "PerformersBlock";
            public const string KrSinglePerformerEntryAcAlias = "SinglePerformerEntryAC";
            public const string KrMultiplePerformersTableAcAlias = "MultiplePerformersTableAC";
            public const string AddComputedRoleLink = "AddComputedRoleLink";

            public const string AuthorBlockAlias = "AuthorBlock";
            public const string AuthorEntryAlias = "AuthorEntryAC";

            public const string TaskKindBlockAlias = "TaskKindBlock";
            public const string TaskKindEntryAlias = "TaskKindEntryAC";
            
            public const string KrTaskHistoryBlockAlias = "KrTaskHistoryBlockAlias";
            public const string KrTaskHistoryGroupTypeControlAlias = "KrTaskHistoryGroupTypeControlAlias";
            public const string KrParentTaskHistoryGroupTypeControlAlias = "KrParentTaskHistoryGroupTypeControlAlias";
            public const string KrTaskHistoryGroupNewIterationControlAlias = "KrTaskHistoryGroupNewIterationControlAlias";


            public const string KrSqlPerformersLinkBlock = "KrSqlPerformersLinkBlock";
            public const string KrStageSettingsBlockAlias = "SettingsBlock";

            public const string StageHandlerDescriptorIDSetting = "StageHandlerDescriptorIDSetting";
            public const string TagsListSetting = "TagsListSetting";
            public const string VisibleForTypesSetting = nameof(VisibleForTypesSetting);
            public const string RequiredForTypesSetting = nameof(RequiredForTypesSetting);
            public const string ControlCaptionsSetting = nameof(ControlCaptionsSetting);
            public const string TileInfo = "TileInfo";

            public const string PureProcessParametersBlock = nameof(PureProcessParametersBlock);
            public const string ActionParametersBlock = nameof(ActionParametersBlock);
            public const string TileParametersBlock = nameof(TileParametersBlock);
            public const string ExecutionAccessDeniedBlock = nameof(ExecutionAccessDeniedBlock);
            public const string RestictionsBlock = nameof(RestictionsBlock);
            public const string VisibilityScriptsBlock = nameof(VisibilityScriptsBlock);
            public const string ExecutionScriptsBlock = nameof(ExecutionScriptsBlock);
            public const string CheckRecalcRestrictionsCheckbox = nameof(CheckRecalcRestrictionsCheckbox);

            public const string CSharpSourceTable = nameof(CSharpSourceTable);
            public const string CSharpSourceTableDesign = nameof(CSharpSourceTableDesign);
            public const string CSharpSourceTableRuntime = nameof(CSharpSourceTableRuntime);
            
            public const string ExtendedTaskForm = "Extended";
            public const string MessageLabel = nameof(MessageLabel);
            public const string Comment = nameof(Comment);
            public const string ReturnIfNotApproved = nameof(ReturnIfNotApproved);
            public const string ReturnAfterApproval = nameof(ReturnAfterApproval);
        }

        public static class Views
        {
            public const string KrProcessStageTypes = "KrFilteredStageTypes";

            public const string KrStageGroups = "KrFilteredStageGroups";
            
            public const string TaskKinds = "TaskKinds";
        }

        public const string RowID = nameof(RowID);
        public const string ID = nameof(ID);
        public const string Name = nameof(Name);
        public const string Description = nameof(Description);
        public const string Order = nameof(Order);
        public const string SourceAfter = nameof(SourceAfter);
        public const string SourceBefore = nameof(SourceBefore);
        public const string SourceCondition = nameof(SourceCondition);
        public const string SqlCondition = nameof(SqlCondition);
        public const string RuntimeSourceAfter = nameof(RuntimeSourceAfter);
        public const string RuntimeSourceBefore = nameof(RuntimeSourceBefore);
        public const string RuntimeSourceCondition = nameof(RuntimeSourceCondition);
        public const string RuntimeSqlCondition = nameof(RuntimeSqlCondition);
        public const string Info = nameof(Info);
        public const string StageGroupID = nameof(StageGroupID);
        public const string StageGroupName = nameof(StageGroupName);
        public const string StageGroupOrder = nameof(StageGroupOrder);
        public const string RoleID = nameof(RoleID);
        public const string TypeID = nameof(TypeID);
        public const string StateID = nameof(StateID);
        public const string StateName = nameof(StateName);
        public const string StageRowID = nameof(StageRowID);
        public const string Caption = nameof(Caption);
        public const string Comment = nameof(Comment);
        public const string StageReferenceToOwner = "Stage";
        public const string StageRowIDReferenceToOwner = "StageRowID";

        #region settings sections

        public static class KrSinglePerformerVirtual
        {
            public static readonly string PerformerID = StageTypeSettingsNaming.PlainColumnName(nameof(KrSinglePerformerVirtual), nameof(PerformerID));
            public static readonly string PerformerName = StageTypeSettingsNaming.PlainColumnName(nameof(KrSinglePerformerVirtual), nameof(PerformerName));
        }

        public static class KrPerformersVirtual
        {
            public const string Name = nameof(KrPerformersVirtual);
            public static readonly string Synthetic = StageTypeSettingsNaming.SectionName(nameof(KrPerformersVirtual));

            public const string PerformerID = nameof(PerformerID);
            public const string PerformerName = nameof(PerformerName);
            public const string SQLApprover = nameof(SQLApprover);
        }

        public static class KrAuthorSettingsVirtual
        {
            public static readonly string AuthorID = StageTypeSettingsNaming.PlainColumnName(nameof(KrAuthorSettingsVirtual), nameof(AuthorID));
            public static readonly string AuthorName = StageTypeSettingsNaming.PlainColumnName(nameof(KrAuthorSettingsVirtual), nameof(AuthorName));
        }

        public static class KrAcquaintanceSettingsVirtual
        {
            public static readonly string Comment = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(Comment));
            public static readonly string AliasMetadata = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(AliasMetadata));
            public static readonly string SenderID = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(SenderID));
            public static readonly string SenderName = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(SenderName));
            public static readonly string NotificationID = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(NotificationID));
            public static readonly string NotificationName = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(NotificationName));
            public static readonly string ExcludeDeputies = StageTypeSettingsNaming.PlainColumnName(nameof(KrAcquaintanceSettingsVirtual), nameof(ExcludeDeputies));
        }

        public static class KrAddFromTemplateSettingsVirtual
        {
            public static readonly string Name = StageTypeSettingsNaming.PlainColumnName(nameof(KrAddFromTemplateSettingsVirtual), nameof(Name));
            public static readonly string FileTemplateID = StageTypeSettingsNaming.PlainColumnName(nameof(KrAddFromTemplateSettingsVirtual), nameof(FileTemplateID));
            public static readonly string FileTemplateName = StageTypeSettingsNaming.PlainColumnName(nameof(KrAddFromTemplateSettingsVirtual), nameof(FileTemplateName));
        }
        
        public static class KrAdditionalApprovalUsersCardVirtual
        {
            public static readonly string Synthetic = StageTypeSettingsNaming.SectionName(nameof(KrAdditionalApprovalUsersCardVirtual));

            public const string RoleID = nameof(RoleID);
            public const string RoleName = nameof(RoleName);
            public const string MainApproverRowID = nameof(MainApproverRowID);
            public const string IsResponsible = nameof(IsResponsible);
            public const string BasedOnTemplateAdditionalApprovalRowID = nameof(BasedOnTemplateAdditionalApprovalRowID);
        }

        public static class KrApprovalSettingsVirtual
        {
            public static readonly string IsParallel = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(IsParallel));
            public static readonly string ReturnToAuthor = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(ReturnToAuthor));
            public static readonly string ReturnWhenDisapproved = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(ReturnWhenDisapproved));
            public static readonly string CanEditCard = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(CanEditCard));
            public static readonly string CanEditFiles = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(CanEditFiles));
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly string Comment = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(Comment));
            public static readonly string DisableAutoApproval = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(DisableAutoApproval));
            public static readonly string FirstIsResponsible = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(FirstIsResponsible));
            public static readonly string ChangeStateOnStart = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(ChangeStateOnStart));
            public static readonly string ChangeStateOnEnd = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(ChangeStateOnEnd));
            public static readonly string Advisory = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(Advisory));
            public static readonly string NotReturnEdit = StageTypeSettingsNaming.PlainColumnName(nameof(KrApprovalSettingsVirtual), nameof(NotReturnEdit));
        }

        public static class KrCreateCardStageSettingsVirtual
        {
            public static readonly string TemplateID = StageTypeSettingsNaming.PlainColumnName(nameof(KrCreateCardStageSettingsVirtual), nameof(TemplateID));
            public static readonly string TemplateCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrCreateCardStageSettingsVirtual), nameof(TemplateCaption));
            public static readonly string TypeID = StageTypeSettingsNaming.PlainColumnName(nameof(KrCreateCardStageSettingsVirtual), nameof(TypeID));
            public static readonly string TypeCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrCreateCardStageSettingsVirtual), nameof(TypeCaption));
            public static readonly string ModeID = StageTypeSettingsNaming.PlainColumnName(nameof(KrCreateCardStageSettingsVirtual), nameof(ModeID));
            public static readonly string ModeName = StageTypeSettingsNaming.PlainColumnName(nameof(KrCreateCardStageSettingsVirtual), nameof(ModeName));
        }

        public static class KrChangeStateSettingsVirtual
        {
            public static readonly string StateID = StageTypeSettingsNaming.PlainColumnName(nameof(KrChangeStateSettingsVirtual), nameof(StateID));
        }

        public static class KrDialogButtonSettingsVirtual
        {
            public static readonly string Synthetic = StageTypeSettingsNaming.SectionName(nameof(KrDialogButtonSettingsVirtual));

            public const string Name = nameof(Name);
            public const string TypeID = nameof(TypeID);
            public const string TypeName = nameof(TypeName);
            public const string Caption = nameof(Caption);
            public const string Icon = nameof(Icon);
            public const string Cancel = nameof(Cancel);
        }

        public static class KrDialogStageTypeSettingsVirtual
        {
            public static readonly string DialogTypeID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(DialogTypeID));
            public static readonly string DialogTypeName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(DialogTypeName));
            public static readonly string CardStoreModeID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(CardStoreModeID));
            public static readonly string CardStoreModeName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(CardStoreModeName));
            public static readonly string DialogActionScript =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(DialogActionScript));
            public static readonly string ButtonName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(ButtonName));
            public static readonly string DialogName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(DialogName));
            public static readonly string DialogAlias =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(DialogAlias));
            public static readonly string OpenModeID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(OpenModeID));
            public static readonly string OpenModeName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(OpenModeName));
            public static readonly string TaskDigest =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(TaskDigest));
            public static readonly string DialogCardSavingScript =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrDialogStageTypeSettingsVirtual), nameof(DialogCardSavingScript));
        }
        
        public static class KrEditSettingsVirtual
        {
            public static readonly string Comment = StageTypeSettingsNaming.PlainColumnName(nameof(KrEditSettingsVirtual), nameof(Comment));
            public static readonly string ChangeState = StageTypeSettingsNaming.PlainColumnName(nameof(KrEditSettingsVirtual), nameof(ChangeState));
            public static readonly string IncrementCycle = StageTypeSettingsNaming.PlainColumnName(nameof(KrEditSettingsVirtual), nameof(IncrementCycle));
            public static readonly string DoNotSkipStage = StageTypeSettingsNaming.PlainColumnName(nameof(KrEditSettingsVirtual), nameof(DoNotSkipStage));
            public static readonly string ManageStageVisibility = StageTypeSettingsNaming.PlainColumnName(nameof(KrEditSettingsVirtual), nameof(ManageStageVisibility));
        }

        public static class KrForkSettingsVirtual
        {
            public static readonly string AfterEachNestedProcess = StageTypeSettingsNaming.PlainColumnName(nameof(KrForkSettingsVirtual), nameof(AfterEachNestedProcess));
        }

        public static class KrForkSecondaryProcessesSettingsVirtual
        {
            public static readonly string Synthetic = StageTypeSettingsNaming.SectionName(nameof(KrForkSecondaryProcessesSettingsVirtual));

            public const string SecondaryProcessID = nameof(SecondaryProcessID);
            public const string SecondaryProcessName = nameof(SecondaryProcessName);
        }
        
        public static class KrForkNestedProcessesSettingsVirtual
        {
            public static readonly string Synthetic = StageTypeSettingsNaming.SectionName(nameof(KrForkNestedProcessesSettingsVirtual));

            public const string NestedProcessID = nameof(NestedProcessID);
        }
        
        public static class KrForkManagementSettingsVirtual
        {
            public static readonly string ModeID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrForkManagementSettingsVirtual), nameof(ModeID));
            public static readonly string ModeName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrForkManagementSettingsVirtual), nameof(ModeName));
            public static readonly string ManagePrimaryProcess =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrForkManagementSettingsVirtual), nameof(ManagePrimaryProcess));
            public static readonly string DirectionAfterInterrupt =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrForkManagementSettingsVirtual), nameof(DirectionAfterInterrupt));
        }
        
        public static class KrHistoryManagementStageSettingsVirtual
        {
            public static readonly string TaskHistoryGroupTypeID = StageTypeSettingsNaming.PlainColumnName(nameof(KrHistoryManagementStageSettingsVirtual), nameof(TaskHistoryGroupTypeID));
            public static readonly string TaskHistoryGroupTypeCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrHistoryManagementStageSettingsVirtual), nameof(TaskHistoryGroupTypeCaption));
            public static readonly string ParentTaskHistoryGroupTypeID = StageTypeSettingsNaming.PlainColumnName(nameof(KrHistoryManagementStageSettingsVirtual), nameof(ParentTaskHistoryGroupTypeID));
            public static readonly string ParentTaskHistoryGroupTypeCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrHistoryManagementStageSettingsVirtual), nameof(ParentTaskHistoryGroupTypeCaption));
            public static readonly string NewIteration = StageTypeSettingsNaming.PlainColumnName(nameof(KrHistoryManagementStageSettingsVirtual), nameof(NewIteration));
        }

        public static class KrNotificationSettingVirtual
        {
            public static readonly string NotificationID = StageTypeSettingsNaming.PlainColumnName(nameof(KrNotificationSettingVirtual), nameof(NotificationID));
            public static readonly string NotificationName = StageTypeSettingsNaming.PlainColumnName(nameof(KrNotificationSettingVirtual), nameof(NotificationName));
            public static readonly string ExcludeDeputies = StageTypeSettingsNaming.PlainColumnName(nameof(KrNotificationSettingVirtual), nameof(ExcludeDeputies));
            public static readonly string ExcludeSubscribers = StageTypeSettingsNaming.PlainColumnName(nameof(KrNotificationSettingVirtual), nameof(ExcludeSubscribers));
            public static readonly string EmailModificationScript = StageTypeSettingsNaming.PlainColumnName(nameof(KrNotificationSettingVirtual), nameof(EmailModificationScript));
        }

        public static class KrRegistrationStageSettingsVirtual
        {
            public static readonly string Comment = StageTypeSettingsNaming.PlainColumnName(nameof(KrRegistrationStageSettingsVirtual), nameof(Comment));
            public static readonly string CanEditCard = StageTypeSettingsNaming.PlainColumnName(nameof(KrRegistrationStageSettingsVirtual), nameof(CanEditCard));
            public static readonly string CanEditFiles = StageTypeSettingsNaming.PlainColumnName(nameof(KrRegistrationStageSettingsVirtual), nameof(CanEditFiles));
            public static readonly string WithoutTask = StageTypeSettingsNaming.PlainColumnName(nameof(KrRegistrationStageSettingsVirtual), nameof(WithoutTask));
        }

        public static class KrResolutionSettingsVirtual
        {
            public static readonly string KindID = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(KindID));
            public static readonly string KindCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(KindCaption));
            public static readonly string ControllerID = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(ControllerID));
            public static readonly string ControllerName = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(ControllerName));
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly string Comment = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(Comment));
            public static readonly string Planned = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(Planned));
            public static readonly string DurationInDays = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(DurationInDays));
            public static readonly string WithControl = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(WithControl));
            public static readonly string MassCreation = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(MassCreation));
            public static readonly string MajorPerformer = StageTypeSettingsNaming.PlainColumnName(nameof(KrResolutionSettingsVirtual), nameof(MajorPerformer));
        }

        public static class KrProcessManagementStageSettingsVirtual
        {
            public static readonly string ModeID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "ModeID");
            public static readonly string ModeName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "ModeName");
            public static readonly string StageGroupID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "StageGroupID");
            public static readonly string StageGroupName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "StageGroupName");
            public static readonly string StageRowGroupName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "StageRowGroupName");
            public static readonly string StageRowID =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "StageRowID");
            public static readonly string StageName =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "StageName");
            public static readonly string ManagePrimaryProcess =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "ManagePrimaryProcess");
            public static readonly string Signal =
                StageTypeSettingsNaming.PlainColumnName(nameof(KrProcessManagementStageSettingsVirtual), "Signal");
        }

        public static class KrSigningStageSettingsVirtual
        {
            public static readonly string IsParallel = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(IsParallel));
            public static readonly string ReturnToAuthor = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(ReturnToAuthor));
            public static readonly string ReturnWhenDeclined = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(ReturnWhenDeclined));
            public static readonly string CanEditCard = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(CanEditCard));
            public static readonly string CanEditFiles = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(CanEditFiles));
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static readonly string Comment = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(Comment));
            public static readonly string ChangeStateOnStart = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(ChangeStateOnStart));
            public static readonly string ChangeStateOnEnd = StageTypeSettingsNaming.PlainColumnName(nameof(KrSigningStageSettingsVirtual), nameof(ChangeStateOnEnd));
        }
        
        public static class KrTaskKindSettingsVirtual
        {
            public static readonly string KindID = StageTypeSettingsNaming.PlainColumnName(nameof(KrTaskKindSettingsVirtual), nameof(KindID));
            public static readonly string KindCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrTaskKindSettingsVirtual), nameof(KindCaption));
        }

        public static class KrTypedTaskSettingsVirtual
        {
            public static readonly string TaskTypeID = StageTypeSettingsNaming.PlainColumnName(nameof(KrTypedTaskSettingsVirtual), nameof(TaskTypeID));
            public static readonly string TaskTypeName = StageTypeSettingsNaming.PlainColumnName(nameof(KrTypedTaskSettingsVirtual), nameof(TaskTypeName));
            public static readonly string TaskTypeCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrTypedTaskSettingsVirtual), nameof(TaskTypeCaption));
            public static readonly string AfterTaskCompletion = StageTypeSettingsNaming.PlainColumnName(nameof(KrTypedTaskSettingsVirtual), nameof(AfterTaskCompletion));
            public static readonly string TaskDigest = StageTypeSettingsNaming.PlainColumnName(nameof(KrTypedTaskSettingsVirtual), nameof(TaskDigest));
        }
        
        public static class KrUniversalTaskOptionsSettingsVirtual
        {
            public static readonly string Synthetic = StageTypeSettingsNaming.SectionName(nameof(KrUniversalTaskOptionsSettingsVirtual));

            public const string OptionID = nameof(OptionID);
            public const string Caption = nameof(Caption);
            public const string ShowComment = nameof(ShowComment);
            public const string Additional = nameof(Additional);
            public const string Order = nameof(Order);
            public const string Message = nameof(Message);
        }

        public static class KrUniversalTaskSettingsVirtual
        {
            public static readonly string Digest = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(Digest));
            public static readonly string AuthorID = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(AuthorID));
            public static readonly string AuthorName = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(AuthorName));
            public static readonly string CanEditCard = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(CanEditCard));
            public static readonly string CanEditFiles = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(CanEditFiles));
            public static readonly string KindID = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(KindID));
            public static readonly string KindCaption = StageTypeSettingsNaming.PlainColumnName(nameof(KrUniversalTaskSettingsVirtual), nameof(KindCaption));
        }

        #endregion

        #region document sections

        public static class DocumentCommonInfo
        {
            public static readonly Guid ID = new Guid("a161e289-2f99-4699-9e95-6e3336be8527");
            public static readonly Guid SecondaryNumberID = new Guid("46115821-4126-40ab-a14f-64f33d17d271");
            public static readonly Guid SecondaryFullNumberID = new Guid("e43c836d-63d7-460d-ab81-b5823383e150");
            public static readonly Guid SecondarySequenceID = new Guid("dadb1130-034d-493c-b7a0-a7c74eae078b");
            public const string Name = nameof(DocumentCommonInfo);
            public const string CardTypeID = "CardTypeID";
            public const string DocTypeID = "DocTypeID";
            public const string DocTypeTitle = "DocTypeTitle";
            public const string Number = "Number";
            public const string FullNumber = "FullNumber";
            public const string Sequence = "Sequence";
            public const string SecondaryNumber = "SecondaryNumber";
            public const string SecondaryFullNumber = "SecondaryFullNumber";
            public const string SecondarySequence = "SecondarySequence";
            public const string Subject = "Subject";
            public const string DocDate = "DocDate";
            public const string CreationDate = "CreationDate";
            public const string OutgoingNumber = "OutgoingNumber";
            public const string Amount = "Amount";
            public const string Barcode = "Barcode";
            public const string CurrencyID = "CurrencyID";
            public const string CurrencyName = "CurrencyName";
            public const string AuthorID = "AuthorID";
            public const string AuthorName = "AuthorName";
            public const string RegistratorID = "RegistratorID";
            public const string RegistratorName = "RegistratorName";
            public const string SignedByID = "SignedByID";
            public const string SignedByName = "SignedByName";
            public const string DepartmentID = "DepartmentID";
            public const string DepartmentName = "DepartmentName";
            public const string PartnerID = "PartnerID";
            public const string PartnerName = "PartnerName";
            public const string RefDocID = "RefDocID";
            public const string RefDocDescription = "RefDocDescription";
            public const string ReceiverRowID = "ReceiverRowID";
            public const string ReceiverName = "ReceiverName";
        }

        public abstract class KrProcessCommonInfo
        {
            public const string MainCardID = nameof(MainCardID);
            public const string CurrentApprovalStageRowID = nameof(CurrentApprovalStageRowID);
            public const string NestedWorkflowProcesses = nameof(NestedWorkflowProcesses);
        }

        public abstract class KrApprovalCommonInfo : KrProcessCommonInfo
        {
            public const string Name = nameof(KrApprovalCommonInfo);
            public const string Virtual = Name + "Virtual";

            public const string ApprovedBy = nameof(ApprovedBy);
            public const string DisapprovedBy = nameof(DisapprovedBy);
            public const string AuthorComment = nameof(AuthorComment);
            public const string AuthorID = nameof(AuthorID);
            public const string AuthorName = nameof(AuthorName);
            public const string StateChangedDateTimeUTC = nameof(StateChangedDateTimeUTC);
            public const string CurrentHistoryGroup = nameof(CurrentHistoryGroup);
        }

        public abstract class KrSecondaryProcessCommonInfo : KrProcessCommonInfo
        {
            public const string Name = nameof(KrSecondaryProcessCommonInfo);
            public const string Virtual = nameof(KrSecondaryProcessCommonInfo);

            public const string SecondaryProcessID = nameof(SecondaryProcessID);
        }

        public abstract class KrDialogSatellite
        {
            public static readonly Guid SectionID =
                new Guid(0x32561DD5, 0xA010, 0x4C57, 0x8B, 0x3D, 0x9C, 0x3E, 0x62, 0x10, 0x34, 0xF4);
            
            public static readonly Guid MainCardIDFieldID =
                new Guid(0xFA36CAB2, 0xCA56, 0x45C0, 0x88, 0x5C, 0x7E, 0xDA, 0xBA, 0x28, 0x22, 0x76);
            
            public static readonly Guid TypeIDFieldID =
                new Guid(0x8137DAF7, 0xD529, 0x4047, 0xA8, 0xDA, 0x0F, 0xA4, 0xCC, 0x5C, 0x65, 0x9F);
            
            public const string Name = nameof(KrDialogSatellite);

            public const string MainCardID = nameof(MainCardID);
            
            public const string TypeID = nameof(TypeID);
        }

        public static class KrStages
        {
            public const string Name = nameof(KrStages);
            public const string Virtual = Name + "Virtual";
            public const string StageStateID = "StateID";
            public const string StageStateName = "StateName";
            public const string TimeLimit = nameof(TimeLimit);
            public const string Hidden = nameof(Hidden);
            public const string SqlApproverRole = nameof(SqlApproverRole);
            public const string RowChanged = nameof(RowChanged);
            public const string OrderChanged = nameof(OrderChanged);
            public const string BasedOnStageRowID = nameof(BasedOnStageRowID);
            public const string BasedOnStageTemplateID = nameof(BasedOnStageTemplateID);
            public const string BasedOnStageTemplateName = nameof(BasedOnStageTemplateName);
            public const string BasedOnStageTemplateOrder = nameof(BasedOnStageTemplateOrder);
            public const string BasedOnStageTemplateGroupPositionID = nameof(BasedOnStageTemplateGroupPositionID);
            public const string StageTypeID = nameof(StageTypeID);
            public const string StageTypeCaption = nameof(StageTypeCaption);
            public const string DisplayTimeLimit = nameof(DisplayTimeLimit);
            public const string DisplayParticipants = nameof(DisplayParticipants);
            public const string DisplaySettings = nameof(DisplaySettings);
            public const string Settings = nameof(Settings);
            public const string NestedProcessID = nameof(NestedProcessID);
            public const string ParentStageRowID = nameof(ParentStageRowID);
            public const string NestedOrder = nameof(NestedOrder);
            public const string ExtraSources = nameof(ExtraSources);
            public const string Planned = nameof(Planned);
            public const string Skip = nameof(Skip);
            public const string CanBeSkipped = nameof(CanBeSkipped);
        }

        #endregion

        #region route sections

        public static class KrStageDocStates
        {
            public const string Name = nameof(KrStageDocStates);
        }
        public static class KrStageTypes
        {
            public const string Name = nameof(KrStageTypes);
        }
        public static class KrStageRoles
        {
            public const string Name = nameof(KrStageRoles);
        }

        public static class KrStageBuildOutput
        {
            public const string Name = nameof(KrStageBuildOutput);

            public const string BuildDateTime = nameof(BuildDateTime);
            public const string Output = nameof(Output);
            public const string Assembly = nameof(Assembly);
            public const string CompilationResult = nameof(CompilationResult);
        }

        public static class KrStageBuildOutputVirtual
        {
            public const string Name = nameof(KrStageBuildOutputVirtual);

            public const string LocalBuildOutput = nameof(LocalBuildOutput);
            public const string GlobalBuildOutput = nameof(GlobalBuildOutput);
        }

        public static class KrStageCommonMethods
        {
            public const string Name = nameof(KrStageCommonMethods);
            public const string Source = nameof(Source);
        }

        public static class KrStageTemplates
        {
            public const string Name = nameof(KrStageTemplates);
            public const string CanChangeOrder = nameof(CanChangeOrder);
            public const string IsStagesReadonly = nameof(IsStagesReadonly);
            public const string GroupPositionID = nameof(GroupPositionID);
            public const string GroupPositionName = nameof(GroupPositionName);
        }

        public static class KrStageGroupTemplatesVirtual
        {
            public const string Name = nameof(KrStageGroupTemplatesVirtual);
            public const string TemplateID = nameof(TemplateID);
            public const string TemplateName = nameof(TemplateName);
        }

        public static class KrStageGroups
        {
            public const string Name = nameof(KrStageGroups);
            public const string IsGroupReadonly = nameof(IsGroupReadonly);
            public const string KrSecondaryProcessID = nameof(KrSecondaryProcessID);
            public const string KrSecondaryProcessName = nameof(KrSecondaryProcessName);
            public const string Ignore = nameof(Ignore);
        }

        public static class KrSecondaryProcessGroupsVirtual
        {
            public const string Name = nameof(KrSecondaryProcessGroupsVirtual);
        }

        public static class KrSecondaryProcesses
        {
            public const string Name = nameof(KrSecondaryProcesses);

            public const string TileGroup = nameof(TileGroup);
            public const string IsGlobal = nameof(IsGlobal);
            public const string Async = nameof(Async);
            public const string Message = nameof(Message);
            public const string RefreshAndNotify = nameof(RefreshAndNotify);
            public const string Tooltip = nameof(Tooltip);
            public const string Icon = nameof(Icon);
            public const string TileSizeID = nameof(TileSizeID);
            public const string TileSizeName = nameof(TileSizeName);
            public const string AskConfirmation = nameof(AskConfirmation);
            public const string ConfirmationMessage = nameof(ConfirmationMessage);
            public const string ActionGrouping = nameof(ActionGrouping);
            public const string VisibilitySqlCondition = nameof(VisibilitySqlCondition);
            public const string ExecutionSqlCondition = nameof(ExecutionSqlCondition);
            public const string VisibilitySourceCondition = nameof(VisibilitySourceCondition);
            public const string ExecutionSourceCondition = nameof(ExecutionSourceCondition);
            public const string ExecutionAccessDeniedMessage = nameof(ExecutionAccessDeniedMessage);
            public const string ModeID = nameof(ModeID);
            public const string ModeName = nameof(ModeName);
            public const string ActionEventType = nameof(ActionEventType);
            public const string AllowClientSideLaunch = nameof(AllowClientSideLaunch);
            public const string CheckRecalcRestrictions = nameof(CheckRecalcRestrictions);
            public const string RunOnce = nameof(RunOnce);
            public const string ButtonHotkey = nameof(ButtonHotkey);
        }

        public static class KrSecondaryProcessContextRoles
        {
            public const string Name = nameof(KrSecondaryProcessContextRoles);
        }

        public static class KrSecondaryProcessModes
        {
            public sealed class Entry
            {
                internal Entry(
                    int id,
                    string name)
                {
                    this.ID = id;
                    this.Name = name;
                }

                public int ID { get; }
                public string Name { get; }
            }


            public const string Name = nameof(KrSecondaryProcessModes);

            public static readonly Entry PureProcess = new Entry(0, "$KrSecondaryProcess_Mode_PureProcess");
            public static readonly Entry Button = new Entry(1, "$KrSecondaryProcess_Mode_Button");
            public static readonly Entry Action = new Entry(2, "$KrSecondaryProcess_Mode_Action");

        }

        #endregion

        #region task sections

        public static class KrAdditionalApproval
        {
            public const string Name = nameof(KrAdditionalApproval);

            public const string TimeLimitation = nameof(TimeLimitation);
            public const string FirstIsResponsible = nameof(FirstIsResponsible);
        }

        public static class KrAdditionalApprovalInfo
        {
            public const string Name = nameof(KrAdditionalApprovalInfo);
            public const string Virtual = Name + "Virtual";

            public const string IsResponsible = nameof(IsResponsible);
        }

        public static class KrAdditionalApprovalTaskInfo
        {
            public const string Name = nameof(KrAdditionalApprovalTaskInfo);

            public const string AuthorRoleID = nameof(AuthorRoleID);
            public const string AuthorRoleName = nameof(AuthorRoleName);
            public const string IsResponsible = nameof(IsResponsible);
        }

        public static class KrAdditionalApprovalUsers
        {
            public const string Name = nameof(KrAdditionalApprovalUsers);
            public const string RoleID = nameof(RoleID);
            public const string RoleName = nameof(RoleName);
        }

        public static class KrCommentators
        {
            public const string Name = nameof(KrCommentators);
            public const string CommentatorID = nameof(CommentatorID);
            public const string CommentatorName = nameof(CommentatorName);
        }

        public static class KrCommentsInfo
        {
            public const string Name = nameof(KrCommentsInfo);
            public const string Virtual = Name + "Virtual";

            public const string Question = nameof(Question);
            public const string Answer = nameof(Answer);
            public const string CommentatorID = nameof(CommentatorID);
            public const string CommentatorName = nameof(CommentatorName);
        }


        public static class KrInfoForInitiator
        {
            public const string Name = nameof(KrInfoForInitiator);
            public const string ApproverRole = nameof(ApproverRole);
            public const string ApproverUser = nameof(ApproverUser);
            public const string InProgress = nameof(InProgress);
        }

        public static class KrTask
        {
            public const string Name = nameof(KrTask);

            public const string DelegateID = nameof(DelegateID);
            public const string DelegateName = nameof(DelegateName);
            public const string Comment = nameof(Comment);
        }

        public static class KrTaskCommentVirtual
        {
            public const string Name = nameof(KrTaskCommentVirtual);

            public const string Comment = nameof(Comment);
        }

        public static class TaskCommonInfo
        {
            public const string Name = nameof(TaskCommonInfo);
            public const string Info = nameof(Info);
            public const string KindID = nameof(KindID);
            public const string KindCaption = nameof(KindCaption);
        }

        public static class KrRequestComment
        {
            public const string Name = nameof(KrRequestComment);
            public const string Comment = nameof(Comment);
            public const string AuthorRoleID = nameof(AuthorRoleID);
            public const string AuthorRoleName = nameof(AuthorRoleName);
        }

        public static class KrUniversalTaskOptions
        {
            public const string Name = nameof(KrUniversalTaskOptions);

            public const string OptionID = nameof(OptionID);
            public const string Caption = nameof(Caption);
            public const string ShowComment = nameof(ShowComment);
            public const string Additional = nameof(Additional);
            public const string Order = nameof(Order);
            public const string Message = nameof(Message);
        }

        #endregion

        #region misc sections

        public static class KrActiveTasks
        {
            public const string Name = nameof(KrActiveTasks);
            public const string Virtual = Name + "Virtual";
            public const string TaskID = nameof(TaskID);
        }

        public static class KrApprovalHistory
        {
            public const string Name = nameof(KrApprovalHistory);
            public const string Virtual = Name + "Virtual";
            public const string Cycle = nameof(Cycle);
            public const string Advisory = nameof(Advisory);
            public const string HistoryRecord = nameof(HistoryRecord);
        }

        public static class KrDocType
        {
            public const string Name = nameof(KrDocType);
        }

        public static class KrSettings
        {
            public const string Name = nameof(KrSettings);
            public const string HideCommentForApprove = nameof(HideCommentForApprove);
        }

        public static class KrSettingsCardTypes
        {
            public const string Name = nameof(KrSettingsCardTypes);
            public const string CardTypeID = nameof(CardTypeID);
        }

        public static class KrSettingsRouteExtraTaskTypes
        {
            public const string Name = nameof(KrSettingsRouteExtraTaskTypes);
            public const string TaskTypeID = nameof(TaskTypeID);
            public const string TaskTypeName = nameof(TaskTypeName);
            public const string TaskTypeCaption = nameof(TaskTypeCaption);
        }
        
        public static class KrSettingsRouteDialogCardTypes
        {
            public const string Name = nameof(KrSettingsRouteDialogCardTypes);
            public const string CardTypeID = nameof(CardTypeID);
            public const string CardTypeName = nameof(CardTypeName);
            public const string CardTypeCaption = nameof(CardTypeCaption);
            public const string IsSatellite = nameof(IsSatellite);
        }

        #endregion

        #region task ids

        /// <summary>
        /// Идентификаторы всех типов заданий, которые относятся к типовому процессу согласования,
        /// кроме виртуальных заданий <see cref="DefaultTaskTypes.KrInfoForInitiatorTypeID"/>.
        /// </summary>
        public static readonly Guid[] KrTaskTypeIDList =
        {
            DefaultTaskTypes.KrApproveTypeID,
            DefaultTaskTypes.KrAdditionalApprovalTypeID,
            DefaultTaskTypes.KrEditTypeID,
            DefaultTaskTypes.KrEditInterjectTypeID,
            DefaultTaskTypes.KrRegistrationTypeID,
            DefaultTaskTypes.KrRequestCommentTypeID,
            DefaultTaskTypes.KrSigningTypeID,
            DefaultTaskTypes.KrShowDialogTypeID,
            DefaultTaskTypes.KrUniversalTaskTypeID,
        };

        #endregion

        #region compiled task types

        public static readonly Guid[] CompiledCardTypes =
        {
            DefaultCardTypes.KrStageTemplateTypeID,
            DefaultCardTypes.KrStageCommonMethodTypeID,
            DefaultCardTypes.KrStageGroupTypeID,
            DefaultCardTypes.KrSecondaryProcessTypeID,
        };

        #endregion
    }
}
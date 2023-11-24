using System;
using System.Collections.Generic;
using System.Linq;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared.Workflow.KrCompilers;
using Tessa.Platform.Collections;

namespace Tessa.Extensions.Default.Server.Workflow.KrCompilers.UserAPI
{
    public sealed class StagesContainer
    {
        #region boolean comparer for CanChangeOrder

        private sealed class CanChangeOrderComparer : IComparer<Tuple<int?, bool>>
        {
            /// <summary>
            /// Сравнение для сортировки этапов согласования в подгруппах "в начале", "в конце", "неопределено". 
            /// Если группа "В начале", ID == 0, то порядок false меньше true
            /// Если группа "В конце", ID == 1, то порядок true меньше false
            /// Если группа "неопределено" и ID == null, то true == false
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(Tuple<int?, bool> x, Tuple<int?, bool> y)
            {
                // на этом моменте предполагается x.Item1 == x.Item2,
                // т.к. соритровка по CanChangeOrder ведется второй
                if (x.Item1 == null || x.Item2 == y.Item2)
                {
                    return 0;
                }
                if (x.Item1 == 0 && !x.Item2 && y.Item2
                    || x.Item1 == 1 && x.Item2 && !y.Item2)
                {
                    return -1;
                }
                return 1;
            }
        }

        #endregion

        #region fields
        
        private readonly IObjectModelMapper objectModelMapper;

        private readonly WorkflowProcess process;

        private bool needSorting;

        private readonly Guid stageGroupID;

        #endregion

        #region constructor

        /// <summary>
        /// Конструктор создает контейнер для этапов наследования 
        /// на основе существующей карточки с этапом согласования.
        /// Предполагается, что в карточке есть секции с этапами, согласующими и доп. согласующими
        /// Необходимо учитывать, что этапы в карточке уже могли быть созданы на основе шаблона.
        /// Так как для этапов хранится только ID KrStageTemplates и StageRowID, необходима исходная карточка шаблона
        /// </summary>
        /// <param name="objectModelMapper"></param>
        /// <param name="process"></param>
        /// <param name="stageGroupID"></param>
        public StagesContainer(
            IObjectModelMapper objectModelMapper,
            WorkflowProcess process,
            Guid stageGroupID)
        {
            this.objectModelMapper = objectModelMapper;
            this.process = process;
            this.stageGroupID = stageGroupID;

            this.DetermineUserModifiedArea();

            this.needSorting = false;
        }

        #endregion

        #region properties

        /// <summary>
        /// Маршрут согласования на момент формирования маршрута.
        /// </summary>
        public SealableObjectList<Stage> InitialStages => this.process.InitialWorkflowProcess.Stages;

        /// <summary>
        /// Этапы согласования
        /// </summary>
        public SealableObjectList<Stage> Stages
        {
            get
            {
                if (this.needSorting)
                {
                    this.process.Stages = this.GetSortedStages().ToSealableObjectList();
                    this.needSorting = false;
                }
                return this.process.Stages;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Вставить этап в отсортированный массив
        /// </summary>
        /// <param name="stage"></param>
        public void InsertStage(params Stage[] stage)
        {
            this.MergeStages(stage);
        }

        /// <summary>
        /// Заменить этап по индексу <paramref name="index"/> на новый
        /// </summary>
        /// <param name="index"></param>
        /// <param name="stage"></param>
        public void ReplaceStage(int index, Stage stage)
        {
            this.process.Stages[index] = stage;
            this.needSorting = true;
        }

        /// <summary>
        /// Выполнить слияние текущих этапов с этапами из карточки шаблона этапов
        /// </summary>
        /// <param name="template"></param>
        /// <param name="stages"></param>
        public void MergeWith(IKrStageTemplate template, IReadOnlyList<IKrRuntimeStage> stages)
        {
            var templateStages = this.objectModelMapper.CardRowsToObjectModel(
                stageTemplate: template,
                runtimeStages: stages,
                primaryPci: null,
                initialStage: false,
                saveInitialStages: false);

            this.MergeStages(templateStages.Stages);
        }

        /// <summary>
        /// Выполнить слияние текущих этапов с этапами из нескольких карточкек шаблона этапов
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="stages"></param>
        public void MergeWith(
            IEnumerable<IKrStageTemplate> templates, 
            IReadOnlyDictionary<Guid, IReadOnlyList<IKrRuntimeStage>> stages)
        {
            var templateStages = templates.Select(t =>
                {
                    var st = stages.TryGetValue(t.ID, out var s) ? s : EmptyHolder<IKrRuntimeStage>.Collection;
                    return this.objectModelMapper.CardRowsToObjectModel(
                            stageTemplate: t,
                            runtimeStages: st,
                            primaryPci: null,
                            initialStage: false,
                            saveInitialStages: false)
                        .Stages;
                })
                .SelectMany(x => x)
                .ToList();

            this.MergeStages(templateStages);
        }

        /// <summary>
        /// Удалить этапы, подставленные из шаблонов ранее, которые при текущем пересчете не заменены.
        /// </summary>
        public void DeleteUnconfirmedStages()
        {
            // Оставляем только ручные, обновленные или измененные пользователем
            this.process.Stages = this.process.Stages
                .Where(this.StageHasRightToLive)
                .ToSealableObjectList();

            foreach (var stage in this.process.Stages)
            {
                // Этапы с флагом InitialStage, которые дошли до этого момента, изменены пользователем
                // Значит шаблон не подтвержден или удален.
                // Нельзя позволять таким этапам лезть наверх.
                if (KrCompilersHelper.ReferToGroup(this.stageGroupID, stage)
                    && stage.BasedOnTemplateStage 
                    && stage.InitialStage)
                {
                    stage.SetCanChangeOrderTrue();
                }
            }

            this.needSorting = true;
        }

        /// <summary>
        /// Восстановление всем этапам внутри контейнера флага "Начальный этап"
        /// </summary>
        public void RestoreFlags()
        {
            var currentGroupProcessed = false;
            foreach (var stage in this.process.Stages)
            {
                if (KrCompilersHelper.ReferToGroup(this.stageGroupID, stage))
                {
                    currentGroupProcessed = true;

                    if (stage.InitialStage)
                    {
                        // Если этап дожил до текущего момента как начальный,
                        // то плохие новости - его шаблон потерялся и этап теперь безшаблонная сирота.
                        stage.UnbindTemplate = true;
                    }
                    else
                    {
                        stage.InitialStage = true;
                    }
                }
                else if (currentGroupProcessed)
                {
                    break;
                }
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Этап может остаться после пересчета
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool StageHasRightToLive(
            Stage p) =>
            !KrCompilersHelper.ReferToGroup(this.stageGroupID, p)
            || !p.BasedOnTemplate
            || !p.InitialStage
            || p.RowChanged
            || p.OrderChanged;

        /// <summary>
        /// Сортировка сначала разделяет все этапы по группам.
        /// Сортировка для групп происходит по паре (StageGroupOrder, StageGroupID),
        /// что позволяет получить уникальность каждого элемента и стабильность сортировки
        ///
        /// В каждой группе проводится сортировка по следующим признакам:
        /// AtFirst(ID=0) &amp;&amp; !CanChangeOrder
        /// AtFirst(ID=0) &amp;&amp; CanChangeOrder
        /// Unspecified(ID=0.5)
        /// AtLast(ID=1) &amp;&amp; CanChangeOrder
        /// AtLast(ID=1) &amp;&amp; !CanChangeOrder
        /// 
        /// Внутри каждой такой подгруппы производится дополнительная сортировка по Stage.TemplateOrder.
        /// Это поле хранит TemplateOrder из карточки шаблона этапов KrStageTemplates. Данный TemplateOrder 
        /// не имеет отношения к обычному Order в таблице этапов карточки
        /// 
        /// Последним ключом сортировки является Order этапов из шаблона.
        /// Это необходимо для переноса порядка сортировки из подмаршрута шаблона в целый маршрут документа.
        /// 
        /// Важным свойством является стабильность Linq OrderBy сортировки.
        /// пруф: https://msdn.microsoft.com/ru-ru/library/bb534966(v=vs.110).aspx абзац "комментарии", предпослед. предложение.
        /// "This method performs a stable sort; that is, if the keys of two elements are equal, the order of the elements is preserved."
        /// Это позволяет при сортировке сохранить порядок Unspecified этапов таким, каким его указал пользователь.
        /// </summary>
        /// <returns>
        /// Перечисление, основанное на поле объекта this.stages. 
        /// В случае необходимости нужно вызывать GetSortedStages().ToList()
        /// </returns>
        private IOrderedEnumerable<Stage> GetSortedStages()
        {
            return this.process.Stages
                .OrderBy(p => p.StageGroupOrder)
                .ThenBy(p => p.StageGroupID)
                .ThenBy(x => x.GroupPosition.ID ?? 0.5)
                .ThenBy(x => new Tuple<int?, bool>(x.GroupPosition.ID, x.CanChangeOrder), new CanChangeOrderComparer())
                .ThenBy(x => x.TemplateOrder)
                .ThenBy(x => x.TemplateStageOrder);
        }

        /// <summary>
        /// Поиск области в середине списка, в которые пользователь вносил изменения
        /// Для них будет выставлен GroupPosition.Unspecified, что позволит сохранить положение, указанное пользователем
        /// </summary>
        private void DetermineUserModifiedArea()
        {
            var currentStages = this.process.Stages;
            int firstUserModifiedStageIndex = currentStages.IndexOf(
                p => KrCompilersHelper.ReferToGroup(this.stageGroupID, p) 
                    && (p.GroupPosition == GroupPosition.Unspecified 
                        || p.GroupPosition == GroupPosition.AtLast 
                        || p.OrderChanged));
            if (firstUserModifiedStageIndex == -1)
            {
                // Все этапы строго по шаблону, не тронуты пользователем
                return;
            }

            int lastUserModifiedStageIndex = currentStages.LastIndexOf(
                p => KrCompilersHelper.ReferToGroup(this.stageGroupID, p) 
                    && (p.GroupPosition == GroupPosition.Unspecified 
                        || p.GroupPosition == GroupPosition.AtFirst
                        || p.OrderChanged));
            if (lastUserModifiedStageIndex == -1)
            {
                // Довольно странное поведение. Если нашли выше, 
                // то скорее всего и здесь должны что нибудь найти
                return;
            }

            // Помечаем область, тронутую пользователем, как "неопределенную"
            // Для всех элементов сортировка будет применятся по одному ключу,
            // а, за счет стабильности linq-сортировки, этапы передвинуты не будут.
            for (int currentStageIndex = firstUserModifiedStageIndex;
                currentStageIndex <= lastUserModifiedStageIndex;
                currentStageIndex++)
            {
                // Необходимо исключить возможные вкрапления неперемещаемых этапов из 
                // центральной области "неопределенной" группы, чтобы при одной из сортировок
                // они встали на свои места
                if (currentStages[currentStageIndex].CanChangeOrder)
                {
                    currentStages[currentStageIndex].SetGroupPositionUnspecified();
                }
            }
        }

        #endregion

        #region Merge stages

        /// <summary>
        /// "Влить" в существующие этапы новые
        /// </summary>
        /// <param name="stages"></param>
        private void MergeStages(IEnumerable<Stage> stages)
        {
            var currentStages = this.process.Stages;
            var oldStagesTable = new Dictionary<Guid, int>(currentStages.Count);
            for (int stageIndex = 0; stageIndex < currentStages.Count; stageIndex++)
            {
                oldStagesTable.Add(currentStages[stageIndex].ID, stageIndex);
            }

            foreach (var newStage in stages)
            {
                if (oldStagesTable.TryGetValue(newStage.ID, out var oldStageIndex))
                {
                    // Меняем только если строка не изменена пользователем.
                    if (!currentStages[oldStageIndex].RowChanged)
                    {
                        newStage.Inherit(currentStages[oldStageIndex]);
                        currentStages[oldStageIndex] = newStage;
                    }
                    else
                    {
                        currentStages[oldStageIndex].InitialStage = false;
                        // Если порядок не изменен, нужно добавить для обновления порядка сортировки
                        if (!currentStages[oldStageIndex].OrderChanged)
                        {
                            currentStages[oldStageIndex].InheritPosition(newStage);
                        }
                    }
                }
                else
                {
                    currentStages.Add(newStage);
                }
            }

            this.needSorting = true;
        }

        #endregion

    }
}

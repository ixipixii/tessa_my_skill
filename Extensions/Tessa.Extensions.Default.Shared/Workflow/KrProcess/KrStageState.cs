using System;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    /// <summary>
    /// Статусы этапов согласования
    /// </summary>
    public struct KrStageState :
        IEquatable<KrStageState>,
        IEquatable<int>
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр состояния с указанием его идентификатора.
        /// </summary>
        /// <param name="id">Идентификатор состояния.</param>
        public KrStageState(int id) => this.ID = id;

        #endregion

        #region Properties

        /// <summary>
        /// Идентификатор состояния.
        /// </summary>
        public int ID { get; }

        #endregion

        #region methods

        public bool IsDefault() => 0 <= this.ID && this.ID < DefaultStates.Length;

        public string TryGetDefaultName()
        {
            try
            {
                return DefaultStateNames[this.ID];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="object" and @item="ToString"]'/>
        public override string ToString() => this.ID.ToString();

        /// <doc path='info[@type="object" and @item="Equals"]'/>
        public override bool Equals(object obj) => obj is KrStageState type && this.Equals(type);

        /// <doc path='info[@type="object" and @item="GetHashCode"]'/>
        public override int GetHashCode() => this.ID.GetHashCode();

        #endregion

        #region Operators

        /// <doc path='info[@type="object" and @item="OperatorEquals"]'/>
        public static bool operator ==(KrStageState first, KrStageState second) => first.Equals(second);

        /// <doc path='info[@type="object" and @item="OperatorNotEquals"]'/>
        public static bool operator !=(KrStageState first, KrStageState second) => !first.Equals(second);

        public static bool operator ==(KrStageState first, int second) => first.Equals(second);

        public static bool operator !=(KrStageState first, int second) => !first.Equals(second);

        public static bool operator ==(int first, KrStageState second) => second.Equals(first);

        public static bool operator !=(int first, KrStageState second) => !second.Equals(first);

        public static explicit operator int(KrStageState type) => type.ID;

        public static explicit operator KrStageState(int typeID) => new KrStageState(typeID);

        #endregion

        #region IEquatable<> Members

        public bool Equals(KrStageState other) => this.ID == other.ID;

        public bool Equals(int other) => this.ID == other;

        #endregion

        #region Static Fields

        /// <summary>
        /// Не активен / не запущен
        /// </summary>
        public static readonly KrStageState Inactive = new KrStageState(0);

        /// <summary>
        /// Активен
        /// </summary>
        public static readonly KrStageState Active = new KrStageState(1);

        /// <summary>
        /// Завершен
        /// </summary>
        public static readonly KrStageState Completed = new KrStageState(2);

        /// <summary>
        /// Пропущен
        /// </summary>
        public static readonly KrStageState Skipped = new KrStageState(3);

        /// <summary>
        /// Список всех стандартных состояния
        /// </summary>
        public static readonly KrStageState[] DefaultStates =
        {
            Inactive,
            Active,
            Completed,
            Skipped,
        };

        /// <summary>
        /// Список всех стандартных названий состояний
        /// </summary>
        public static readonly string[] DefaultStateNames =
        {
            "$KrStates_Stage_Inactive",
            "$KrStates_Stage_Active",
            "$KrStates_Stage_Completed",
            "$KrStates_Stage_Skipped",
        };

        #endregion
    }
}

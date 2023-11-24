using System;
using Tessa.Extensions.Default.Shared.Notices;
using Tessa.Properties.Resharper;

namespace Tessa.Extensions.Default.Shared.Workflow.KrCompilers
{
    public sealed class GroupPosition: IEquatable<GroupPosition>
    {
        #region static fields

        public static readonly GroupPosition Unspecified = new GroupPosition(null, null);
        public static readonly GroupPosition AtFirst = new GroupPosition(0, "AtFirst");
        public static readonly GroupPosition AtLast = new GroupPosition(1, "AtLast");

        private static readonly GroupPosition[] positions = { AtFirst, AtLast };

        #endregion

        #region constructor

        [UsedImplicitly]
        private GroupPosition()
        {
        }
        
        private GroupPosition(int? id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        #endregion

        #region properties

        public int? ID { get; [UsedImplicitly]  private set; }
        public string Name { get; [UsedImplicitly] private set; }

        #endregion

        #region static methods

        public static GroupPosition GetByID(object idObj)
        {
            return idObj is int id ?
                GetByID(id) :
                Unspecified;
        }
        
        public static GroupPosition GetByID(int? id)
        {
            return id.HasValue && (0 <= id && id < 2) ?
                positions[id.Value] :
                Unspecified;
        }

        #endregion

        #region operators

        public static bool operator ==(GroupPosition gp1, GroupPosition gp2)
        {
            return !ReferenceEquals(null, gp1) && gp1.Equals(gp2);
        }

        public static bool operator !=(GroupPosition gp1, GroupPosition gp2)
        {
            return !ReferenceEquals(null, gp1) && !gp1.Equals(gp2);
        }

        #endregion

        #region object

        public override bool Equals(object obj)
        {
            return this.Equals(obj as GroupPosition);
        }
        
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override string ToString()
        {
            switch (this.ID)
            {
                case 0:
                    return "AtFirst";
                case 1:
                    return "AtLast";
                default:
                    return "Unspecified";
            }
        }

        #endregion

        #region IEquatable

        public bool Equals(GroupPosition other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.ID == this.ID || (other.ID == null && this.ID == null);
        }

        #endregion
    }
}
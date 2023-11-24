using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform.Data;
using Tessa.Roles;
using System.Linq;
using Tessa.Extensions.Shared.Models;

namespace Tessa.Extensions.Server.Helpers
{
    public static class ServerHelper
    {
        /// <summary>
        /// Получение списка категорий файлов по карточке
        /// </summary>
        public static async Task<List<Guid>> GetFileCategories(IDbScope dbScope, Card card)
        {
            List<Guid> fileCategories = new List<Guid>();
            foreach (var file in card.Files)
            {
                if (file.CategoryID.HasValue)
                    fileCategories.Add(file.CategoryID.Value);
            }
            if (fileCategories.Count < 1)
            {
                await using (dbScope.Create())
                {
                    var db = dbScope.Db;
                    db.SetCommand(@"SELECT CategoryID
                              FROM Files
                              WHERE ID = @CardID
                              and CategoryID is not null",
                          db.Parameter("@CardID", card.ID))
                        .LogCommand();
                    using (IDataReader reader = await db.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            fileCategories.Add(reader.GetGuid(0));
                        }
                    }
                }
            }
            return fileCategories;
        }

        public static async Task<bool> IsUserInStaticRoleAsync(IRoleRepository roleRepository, Guid roleId, Guid userId)
        {
            CancellationToken cancellationToken = default;

            List<RoleUserRecord> roleRepositoryList = await roleRepository.GetUsersAsync(roleId, cancellationToken).ConfigureAwait(false);

            foreach (var user in roleRepositoryList)
            {
                if (user.UserID == userId)
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task<bool> IsUserInContextRole(IRoleRepository roleRepository, Guid cardId, Guid contextRoleId, Guid userId)
        {
            var role = await roleRepository.GetContextRoleAsync(contextRoleId);
            if (role == null)
            {
                return false;
            }

            var test1 = await roleRepository.GetCardContextUsersAsync(role.ID, role.Name, role.SqlTextForCard, cardId);
            return test1.Select(u => u.UserID).Contains(userId);
        }

        /// <summary>
        /// Получение полей карточки сотрудника по ID
        /// </summary>
        public static async Task<PersonalRolesFields> GetPersonalRolesFields(IDbScope dbScope, Guid? userID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db.SetCommand(@"SELECT 
                                [Name]
                                ,[FirstName]
                                ,[LastName]
                                ,[FullName]
                                ,[Email]
                                ,[Login]
                                ,[MobilePhone]
                              FROM [PersonalRoles]
                              WHERE [ID] = @userID",
                       db.Parameter("@userID", userID))
                     .LogCommand()
                     .ExecuteAsync<PersonalRolesFields>();
            }
        }

        /// <summary>
        /// Получение даты установки последнего состояния карточки по ID.
        /// </summary>
        public static async Task<DateTime?> GetDateCardStateByID(IDbScope dbScope, Guid? cardID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db.SetCommand(@"
                        select top 1 ins.Created from FdSatelliteCommonInfo as f
                        inner join Instances as ins on ins.ID = f.ID
                        where f.MainCardId = @cardID
                        ",
                       db.Parameter("@cardID", cardID))
                     .LogCommand()
                     .ExecuteAsync<DateTime?>();
            }
        }

        /// <summary>
        /// Сотрудник
        /// </summary>
        public class PersonalRolesFields
        {
            public string Name { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Login { get; set; } = string.Empty;
            public string MobilePhone { get; set; } = string.Empty;
        }

        /// <summary>
        /// Обновление номера документа в DocumentCommonInfo
        /// </summary>
        public static async Task UpdateDocumentNumber(IDbScope dbScope, Guid cardID, string fullNumber)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;

                await db.SetCommand(
                    "UPDATE [dbo].[DocumentCommonInfo] "
                    + "SET FullNumber = @fullNumber "
                    + "WHERE ID = @cardID; ",
                db.Parameter("cardID", cardID),
                db.Parameter("fullNumber", fullNumber))
                .LogCommand()
                .ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Удаление относящихся к cardID записей из PnrConfidantsDepartmentsHeads
        /// </summary>
        public static async Task DeleteConfidantsDepartmentsHeadsByCardID(IDbScope dbScope, Guid cardID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                await db.SetCommand(
                    "DELETE [dbo].[PnrConfidantsDepartmentsHeads] "
                    + "WHERE ID = @cardID; ",
                db.Parameter("cardID", cardID))
                .LogCommand()
                .ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Получение руководителя дирекции по ID доверенного лица, и его последующая запись
        /// в PnrConfidantsDepartmentsHeads с привязкой к cardID
        /// </summary>
        public class DepartmentHeadUser
        {
            public Guid? CuratorID { get; set; }
            public string CuratorName { get; set; }
        }
        public static async Task SetConfidantsDepartmentsHeadsByCardID(IDbScope dbScope, Guid cardID, List<Dictionary<string, object>> confidants)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;

                for (int i = 0; i < confidants.Count; i++)
                {
                    // подчитываем руководителя дирекции
                    DepartmentHeadUser departmentHead = await db
                    .SetCommand(
                        @"SELECT TOP 1 
                            DR.ID, 
                            DR.CuratorID, 
                            DR.CuratorName  
                            FROM DepartmentRoles as DR WITH(NOLOCK) 
                            LEFT JOIN RoleUsers as RU WITH(NOLOCK) ON DR.ID = RU.ID 
                            WHERE RU.UserID = @authorID AND RU.UserID <> RU.ID;",
                        db.Parameter("authorID", confidants[i]["UserID"]))
                    .ExecuteAsync<DepartmentHeadUser>();

                    if(departmentHead != null && departmentHead.CuratorID != null)
                    {
                        // проверка на возможное дублирование
                        List<Guid> departmentsHeads = await db
                        .SetCommand(
                            @"SELECT 
                                UserID 
                                FROM [dbo].[PnrConfidantsDepartmentsHeads] WITH(NOLOCK) 
                                WHERE ID = @cardID;",
                            db.Parameter("cardID", cardID)) 
                        .ExecuteListAsync<Guid>();

                        if((departmentsHeads !=null && !departmentsHeads.Contains((Guid)departmentHead.CuratorID)) || departmentsHeads == null)
                        {
                            // подразделение и руководитель успешно прочитаны, в PnrConfidantsDepartmentsHeads такой записи или вообще записей по этой карточке еще нет
                            // пишем руководителя его в PnrConfidantsDepartmentsHeads с привязкой к карточке
                            Guid rowID = Guid.NewGuid();
                            await db.SetCommand(
                                @"INSERT INTO [dbo].[PnrConfidantsDepartmentsHeads] (ID, RowID, UserID, UserName)
                                VALUES(@cardID, @rowID, @userID, @userName); ",
                                db.Parameter("cardID", cardID),
                                db.Parameter("rowID", rowID),
                                db.Parameter("userID", departmentHead.CuratorID),
                                db.Parameter("userName", departmentHead.CuratorName))
                            .LogCommand()
                            .ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получение количества файлов, прикрепленных к карточке
        /// </summary>
        public static async Task<int> GetNumberOfFiles(IDbScope dbScope, Guid? cardID)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;

                return
                    await db.SetCommand(@"
                            SELECT COUNT(RowID) 
                            FROM Files 
                            WHERE ID = @cardID",
                        db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .ExecuteAsync<int>();
            }
        }
    }
}

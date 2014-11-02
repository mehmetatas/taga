using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Taga.Core.Dynamix;
using Taga.Core.Repository;
using Taga.Core.Repository.Mapping;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Tests.TableClassMappingTests
{
    [TestClass]
    public class DatabaseMappingTests
    {
        [TestMethod, TestCategory("DatabaseMappingTests")]
        public void Should_Map_Correctly()
        {
            var dbMapping = DatabaseMapping.For(DbSystem.MySql)
                .Map<User>()
                .Map<Category>()
                .Map<UserRole>(ur => ur.UserId, ur => ur.RoleId);

            IMappingProvider mappingProvider = new MappingProvider();

            mappingProvider.SetDatabaseMapping(dbMapping);

            var userMapping = mappingProvider.GetTableMapping<User>();

            Assert.AreEqual(typeof(User), userMapping.Type);
            Assert.AreEqual("users", userMapping.TableName);

            var columnMappings = userMapping.Columns;

            Assert.AreEqual("id", columnMappings[0].ColumnName);
            Assert.AreEqual("username", columnMappings[1].ColumnName);
            Assert.AreEqual("password", columnMappings[2].ColumnName);

            var catMapping = mappingProvider.GetTableMapping<Category>();

            Assert.AreEqual(typeof(Category), catMapping.Type);
            Assert.AreEqual("categories", catMapping.TableName);

            columnMappings = catMapping.Columns;

            Assert.AreEqual("id", columnMappings[0].ColumnName);
            Assert.AreEqual("user_id", columnMappings[1].ColumnName);

            var userRoleMapping = mappingProvider.GetTableMapping<UserRole>();

            Assert.AreEqual(typeof(UserRole), userRoleMapping.Type);
            Assert.AreEqual("user_roles", userRoleMapping.TableName);

            columnMappings = userRoleMapping.Columns;

            Assert.AreEqual("user_id", columnMappings[0].ColumnName);
            Assert.AreEqual("role_id", columnMappings[1].ColumnName);
        }

        [TestMethod, TestCategory("DatabaseMappingTests")]
        public void Should_Create_Dynamic_Poco()
        {
            var type = PocoBuilder.BuildPoco(new Dictionary<string, Type>
            {
                {"UserId", typeof(long)},
                {"RoleId", typeof(long)}
            });

            var poco = Activator.CreateInstance(type);

            poco.GetType().GetProperty("UserId").SetValue(poco, 1);
            poco.GetType().GetProperty("RoleId").SetValue(poco, 2);

            Assert.AreEqual(1L, poco.GetType().GetProperty("UserId").GetValue(poco));
            Assert.AreEqual(2L, poco.GetType().GetProperty("RoleId").GetValue(poco));
        }
    }
}

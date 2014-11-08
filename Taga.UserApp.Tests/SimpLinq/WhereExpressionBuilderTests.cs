using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taga.Core.Repository;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.SimpLinq;
using Taga.SimpLinq.QueryBuilder;
using Taga.SimpLinq.QueryBuilder.Impl;
using Taga.UserApp.Core.Model.Common.Enums;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Tests.DbTests;

namespace Taga.UserApp.Tests.SimpLinq
{
    [TestClass]
    public class WhereExpressionBuilderTests
    {
        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Equals()
        {
            var exp = GetExpression(u => u.Id == 3);

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Equals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_Equals()
        {
            var exp = GetExpression(u => u.Id != 3);

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotEquals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Greater_Than()
        {
            var exp = GetExpression(u => u.Id > 3);

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.GreaterThan, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Greater_Than_Or_Equals()
        {
            var exp = GetExpression(u => u.Id >= 3);

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.GreaterThanOrEquals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Less_Than()
        {
            var exp = GetExpression(u => u.Id < 3);

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LessThan, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Less_Than_Or_Equals()
        {
            var exp = GetExpression(u => u.Id <= 3);

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LessThanOrEquals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Like_Contains()
        {
            var exp = GetExpression(u => u.Username.Contains("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LikeContains, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), where.Operand1);
            Assert.AreEqual("Abc", where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Like_Starts_With()
        {
            var exp = GetExpression(u => u.Username.StartsWith("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LikeStartsWith, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), where.Operand1);
            Assert.AreEqual("Abc", where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Like_Ends_With()
        {
            var exp = GetExpression(u => u.Username.EndsWith("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LikeEndsWith, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), where.Operand1);
            Assert.AreEqual("Abc", where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_In_With_List()
        {
            var list = new List<long> {1, 2, 3};

            var exp = GetExpression(u => list.Contains(u.Id));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.In, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(list, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_In_With_Array()
        {
            var arr = new[] {1L, 2L, 3L};

            var exp = GetExpression(u => arr.Contains(u.Id));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.In, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(arr, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_And()
        {
            var exp = GetExpression(u => u.Id == 3 && u.Username.Contains("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.And, where.Operator);

            var operand1 = (IWhere) where.Operand1;
            Assert.AreEqual(Operator.Equals, operand1.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand1.Operand1);
            Assert.AreEqual(3L, operand1.Operand2);

            var operand2 = (IWhere) where.Operand2;
            Assert.AreEqual(Operator.LikeContains, operand2.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand2.Operand1);
            Assert.AreEqual("Abc", operand2.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Or()
        {
            var exp = GetExpression(u => u.Id == 3 || u.Username.Contains("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Or, where.Operator);

            var operand1 = (IWhere) where.Operand1;
            Assert.AreEqual(Operator.Equals, operand1.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand1.Operand1);
            Assert.AreEqual(3L, operand1.Operand2);

            var operand2 = (IWhere) where.Operand2;
            Assert.AreEqual(Operator.LikeContains, operand2.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand2.Operand1);
            Assert.AreEqual("Abc", operand2.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_And_Or_And()
        {
            var exp = GetExpression(u =>
                (u.Id == 3 && u.Username.Contains("Abc")) ||
                (u.Id == 2 && u.Username.StartsWith("Abc")));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Or, where.Operator);

            var operand1 = (IWhere) where.Operand1;
            var operand11 = (IWhere) operand1.Operand1;
            var operand12 = (IWhere) operand1.Operand2;

            Assert.AreEqual(Operator.And, operand1.Operator);

            Assert.AreEqual(Operator.Equals, operand11.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand11.Operand1);
            Assert.AreEqual(3L, operand11.Operand2);

            Assert.AreEqual(Operator.LikeContains, operand12.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand12.Operand1);
            Assert.AreEqual("Abc", operand12.Operand2);

            var operand2 = (IWhere) where.Operand2;
            var operand21 = (IWhere) operand2.Operand1;
            var operand22 = (IWhere) operand2.Operand2;

            Assert.AreEqual(Operator.And, operand2.Operator);

            Assert.AreEqual(Operator.Equals, operand21.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand21.Operand1);
            Assert.AreEqual(2L, operand21.Operand2);

            Assert.AreEqual(Operator.LikeStartsWith, operand22.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand22.Operand1);
            Assert.AreEqual("Abc", operand22.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Equals_With_Not()
        {
            var exp = GetExpression(u => !(u.Id != 3));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Equals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_Equals_With_Not()
        {
            var exp = GetExpression(u => !(u.Id == 3));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotEquals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Greater_Than_With_Not()
        {
            var exp = GetExpression(u => !(u.Id <= 3));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.GreaterThan, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Greater_Than_Or_Equals_With_Not()
        {
            var exp = GetExpression(u => !(u.Id < 3));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.GreaterThanOrEquals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Less_Than_With_Not()
        {
            var exp = GetExpression(u => !(u.Id >= 3));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LessThan, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Less_Than_Or_Equals_With_Not()
        {
            var exp = GetExpression(u => !(u.Id > 3));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.LessThanOrEquals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_Like_Contains()
        {
            var exp = GetExpression(u => !u.Username.Contains("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotLikeContains, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), where.Operand1);
            Assert.AreEqual("Abc", where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_Like_Starts_With()
        {
            var exp = GetExpression(u => !u.Username.StartsWith("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotLikeStartsWith, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), where.Operand1);
            Assert.AreEqual("Abc", where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_Like_Ends_With()
        {
            var exp = GetExpression(u => !u.Username.EndsWith("Abc"));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotLikeEndsWith, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), where.Operand1);
            Assert.AreEqual("Abc", where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_In_With_List()
        {
            var list = new List<long> {1, 2, 3};

            var exp = GetExpression(u => !list.Contains(u.Id));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotIn, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(list, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Not_In_With_Array()
        {
            var arr = new[] {1L, 2L, 3L};

            var exp = GetExpression(u => !arr.Contains(u.Id));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.NotIn, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(arr, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Equals_With_Not_Not()
        {
            var exp = GetExpression(u => !(!(u.Id == 3)));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Equals, where.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), where.Operand1);
            Assert.AreEqual(3L, where.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_And_With_Not()
        {
            var exp = GetExpression(u => !(u.Id != 3 || !u.Username.Contains("Abc")));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.And, where.Operator);

            var operand1 = (IWhere) where.Operand1;
            Assert.AreEqual(Operator.Equals, operand1.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand1.Operand1);
            Assert.AreEqual(3L, operand1.Operand2);

            var operand2 = (IWhere) where.Operand2;
            Assert.AreEqual(Operator.LikeContains, operand2.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand2.Operand1);
            Assert.AreEqual("Abc", operand2.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_Or_With_Not()
        {
            var exp = GetExpression(u => !(u.Id != 3 && !u.Username.Contains("Abc")));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Or, where.Operator);

            var operand1 = (IWhere) where.Operand1;
            Assert.AreEqual(Operator.Equals, operand1.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand1.Operand1);
            Assert.AreEqual(3L, operand1.Operand2);

            var operand2 = (IWhere) where.Operand2;
            Assert.AreEqual(Operator.LikeContains, operand2.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand2.Operand1);
            Assert.AreEqual("Abc", operand2.Operand2);
        }

        [TestMethod, TestCategory("WhereExpressionBuilder")]
        public void Should_Build_And_Or_And_With_Not()
        {
            var exp = GetExpression(u =>
                !((u.Id != 3 || !u.Username.Contains("Abc")) &&
                  (u.Id != 2 || !u.Username.StartsWith("Abc"))));

            var where = WhereExpressionBuilder.Build(exp);

            Assert.AreEqual(Operator.Or, where.Operator);

            var operand1 = (IWhere) where.Operand1;
            var operand11 = (IWhere) operand1.Operand1;
            var operand12 = (IWhere) operand1.Operand2;

            Assert.AreEqual(Operator.And, operand1.Operator);

            Assert.AreEqual(Operator.Equals, operand11.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand11.Operand1);
            Assert.AreEqual(3L, operand11.Operand2);

            Assert.AreEqual(Operator.LikeContains, operand12.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand12.Operand1);
            Assert.AreEqual("Abc", operand12.Operand2);

            var operand2 = (IWhere) where.Operand2;
            var operand21 = (IWhere) operand2.Operand1;
            var operand22 = (IWhere) operand2.Operand2;

            Assert.AreEqual(Operator.And, operand2.Operator);

            Assert.AreEqual(Operator.Equals, operand21.Operator);
            Assert.AreEqual(GetProperty(u => u.Id), operand21.Operand1);
            Assert.AreEqual(2L, operand21.Operand2);

            Assert.AreEqual(Operator.LikeStartsWith, operand22.Operator);
            Assert.AreEqual(GetProperty(u => u.Username), operand22.Operand1);
            Assert.AreEqual("Abc", operand22.Operand2);
        }

        [TestMethod, TestCategory("MySqlSimpLinqResolver")]
        public void Should_GetValue()
        {
            var list = new List<long> {1, 2, 3};

            DbTestInitializer.Initialize(DbSystem.MySql);

            for (var i = 0; i < 500; i++)
            {
                GetValue(list, i, "Abc", PostStatus.Active, null);
            }
        }

        private static void GetValue(List<long> ids, int x, string s, PostStatus status, string cc)
        {
            var postTypes = new[] {PostType.Image, PostType.Video};

            var selectQuery = Select.From<User>(new TagaPropertyFilter())
                .Include<Category>(c => c.Title)
                .Exclude<Post>(p => p.Content)
                .InnerJoin<User, Category>(u => u.Id, c => c.UserId)
                .LeftJoin<Category, Post>(c => c.Id, p => p.CategoryId)
                .Where<User>(u =>
                    (u.Id == 13 && u.Username.Contains("Al") && u.Username.StartsWith(s) && u.Username.EndsWith("li")) ||
                    !(ids.Contains(u.Id) && u.Id != 2 && u.Id < 3 && u.Id > 0 && u.Id >= x || u.Id <= 3))
                .Where<Post>(p => p.Status == status && !postTypes.Contains(p.PostType))
                .Where<Category>(c => c.Title == null && c.Description != cc && c.Id < 9)
                .OrderBy<Post>(p => p.CreateDate)
                .OrderBy<Category>(c => c.Title, true)
                .Page(3, 10);

            var resolver = new MySqlSimpLinqResolver();
            var sql = resolver.Resolve(selectQuery);

            Assert.AreEqual(18, sql.Parameters.Length);
            Assert.AreEqual(13L, sql.GetParameterValue("p_users_id"));
            Assert.AreEqual("%Al%", sql.GetParameterValue("p_users_username"));
            Assert.AreEqual(s + "%", sql.GetParameterValue("p_users_username_1"));
            Assert.AreEqual("%li", sql.GetParameterValue("p_users_username_2"));
            Assert.AreEqual(ids[0], sql.GetParameterValue("p_users_id_1"));
            Assert.AreEqual(ids[1], sql.GetParameterValue("p_users_id_2"));
            Assert.AreEqual(ids[2], sql.GetParameterValue("p_users_id_3"));
            Assert.AreEqual(2L, sql.GetParameterValue("p_users_id_4"));
            Assert.AreEqual(3L, sql.GetParameterValue("p_users_id_5"));
            Assert.AreEqual(0L, sql.GetParameterValue("p_users_id_6"));
            Assert.AreEqual((long) x, sql.GetParameterValue("p_users_id_7"));
            Assert.AreEqual(3L, sql.GetParameterValue("p_users_id_8"));
            Assert.AreEqual((int) status, sql.GetParameterValue("p_posts_status"));
            Assert.AreEqual(postTypes[0], sql.GetParameterValue("p_posts_post_type"));
            Assert.AreEqual(postTypes[1], sql.GetParameterValue("p_posts_post_type_1"));
            Assert.AreEqual(9L, sql.GetParameterValue("p_categories_id"));
            Assert.AreEqual(10, sql.GetParameterValue("p__limit_"));
            Assert.AreEqual(20, sql.GetParameterValue("p__offset_"));
        }

        private static PropertyInfo GetProperty(Expression<Func<User, object>> propExpression)
        {
            MemberExpression memberExp;
            if (propExpression.Body is UnaryExpression)
            {
                memberExp = (MemberExpression) ((UnaryExpression) propExpression.Body).Operand;
            }
            else
            {
                memberExp = (MemberExpression) propExpression.Body;
            }
            return memberExp.Member as PropertyInfo;
        }

        private static Expression<Func<User, bool>> GetExpression(Expression<Func<User, bool>> expression)
        {
            return expression;
        }
    }
}
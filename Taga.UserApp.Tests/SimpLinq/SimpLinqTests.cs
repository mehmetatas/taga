using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Criterion;
using Taga.SimpLinq.QueryBuilder;
using Taga.SimpLinq.QueryBuilder.Impl;
using Taga.UserApp.Core.Model.Common.Enums;
using Taga.UserApp.Core.Model.Database;
using System.Linq.Expressions;

namespace Taga.UserApp.Tests.SimpLinq
{
    [TestClass]
    public class SimpLinqTests
    {
        [TestMethod]
        public void Should_Build_Sql()
        {
            //var ids = new List<long>
            //{
            //    1, 2, 3
            //};

            var ids = new long[]
            {
                1, 2, 3
            };

            var exp = GetExpression(u => 
                (u.Id == 13 && u.Username.Contains("Al") && u.Username.StartsWith("Ve") && u.Username.EndsWith("li")) ||
                !(ids.Contains(u.Id) && u.Id != 2 && u.Id < 3 && u.Id > 0 && u.Id >= 0 || u.Id <= 3));

            WhereExpressionBuilder.Build(exp);
        }

        [TestMethod]
        public void Should_Build_SelectQuery()
        {
            //var ids = new long[]
            //{
            //    1, 2, 3
            //};

            var ids = new List<long>
            {
                1, 2, 3
            };

            GetValue(ids, 3, "VE", PostStatus.Passive, null);
        }

        private static void GetValue(List<long> ids, int x, string s, PostStatus status, string cc)
        {
            var selectQuery = Select.From<User>()
                .Include<Category>(c => c.Title)
                .Exclude<Post>(p => p.Content)
                .InnerJoin<User, Category>(u => u.Id, c => c.UserId)
                .LeftJoin<Category, Post>(c => c.Id, p => p.CategoryId)
                .Where<User>(u =>
                    (u.Id == 13 && u.Username.Contains("Al") && u.Username.StartsWith(s) && u.Username.EndsWith("li")) ||
                    !(ids.Contains(u.Id) && u.Id != 2 && u.Id < 3 && u.Id > 0 && u.Id >= x || u.Id <= 3))
                .Where<Post>(p => p.Status == status && p.Content != cc)
                .Where<Category>(c => c.Title == null)
                .OrderBy<Post>(p => p.CreateDate)
                .OrderBy<Category>(c => c.Title, true)
                .Page(3, 10)
                .Build();

            var sql = selectQuery.ToSql();
        }

        private Expression<Func<User, bool>> GetExpression(Expression<Func<User, bool>> expression)
        {
            return expression;
        }
    }
}

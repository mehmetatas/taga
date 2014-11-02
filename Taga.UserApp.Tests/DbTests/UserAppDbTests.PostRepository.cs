using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Taga.UserApp.Core.Database;
using Taga.UserApp.Core.Model.Common.Enums;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Tests.DbTests
{
    public abstract partial class UserAppDbTests
    {
        [TestMethod, TestCategory("PostRepository")]
        public void Should_Create_Category()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            var time = DateTime.Now;
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            Category category;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = new Category
                {
                    UserId = user.Id,
                    Title = "Test Category",
                    Description = "Description",
                    CreateDate = time
                };

                repo.Save(category);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = repo.GetCategory(category.Id);
            }

            Assert.AreEqual(user.Id, category.UserId);
            Assert.AreEqual("Test Category", category.Title);
            Assert.AreEqual("Description", category.Description);
            Assert.AreEqual(time, category.CreateDate);
        }

        [TestMethod, TestCategory("PostRepository")]
        public void Should_Update_Category()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            var time = DateTime.Now;
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            Category category;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = new Category
                {
                    UserId = user.Id,
                    Title = "Test Category",
                    Description = "Description",
                    CreateDate = time
                };

                repo.Save(category);
                db.Save();
            }

            var id = category.Id;

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = repo.GetCategory(category.Id);

                category.Title = "Test Category-Updated";
                category.Description = "Description-Updated";
                category.CreateDate = time.AddDays(1);

                repo.Save(category);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = repo.GetCategory(id);
            }

            Assert.AreEqual(user.Id, category.UserId);
            Assert.AreEqual("Test Category-Updated", category.Title);
            Assert.AreEqual("Description-Updated", category.Description);
            Assert.AreEqual(time.AddDays(1), category.CreateDate);
        }

        [TestMethod, TestCategory("PostRepository")]
        public void Should_Delete_Category()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            var time = DateTime.Now;
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            Category category;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = new Category
                {
                    UserId = user.Id,
                    Title = "Test Category",
                    Description = "Description",
                    CreateDate = time
                };

                repo.Save(category);
                db.Save();
            }

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                repo.Delete(new Category
                {
                    Id = category.Id
                });

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = repo.GetCategory(category.Id);
            }

            Assert.IsNull(category);
        }

        [TestMethod, TestCategory("PostRepository")]
        public void Should_Get_Categories_Of_User()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            var time = DateTime.Now;

            for (var i = 0; i < 5; i++)
            {
                using (var db = Db.ReadWrite())
                {
                    var repo = db.GetRepository<IPostRepository>();

                    var category = new Category
                    {
                        UserId = i % 2 == 1 ? user.Id + 1 : user.Id,
                        Title = "Test Category " + i,
                        Description = "Description " + i,
                        CreateDate = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second).AddDays(i)
                    };

                    repo.Save(category);
                    db.Save();
                }
            }

            Category[] categories;

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IPostRepository>();

                categories = repo.GetCategoriesOfUser(user.Id);
            }

            Assert.AreEqual(3, categories.Length);

            var j = 0;
            for (var i = 0; i < 5; i++)
            {
                if (i % 2 == 1)
                {
                    continue;
                }

                var cat = categories[j++];

                Assert.AreEqual(user.Id, cat.UserId);
                Assert.AreEqual("Test Category " + i, cat.Title);
                Assert.AreEqual("Description " + i, cat.Description);
                Assert.AreEqual(new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second).AddDays(i), cat.CreateDate);
            }
        }

        [TestMethod, TestCategory("PostRepository")]
        public void Should_Create_Post()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            var time = DateTime.Now;
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            Category category;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = new Category
                {
                    UserId = user.Id,
                    Title = "Test Category",
                    Description = "Description",
                    CreateDate = time
                };

                repo.Save(category);
                db.Save();
            }

            var tags = new List<Tag>
            {
                new Tag {Name = "Tag 1"},
                new Tag {Name = "Tag 2"},
                new Tag {Name = "Tag 3"}
            };
            var imageBytes = new byte[] { 1, 2, 3 };

            Post post;
            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IPostRepository>();

                post = new Post
                {
                    CategoryId = category.Id,
                    PostType = PostType.Video,
                    Title = "Post Title",
                    Content = "Post Content",
                    CreateDate = time,
                    ImageBytes = imageBytes,
                    QuoteAuthor = "Quote Author",
                    QuoteText = "Quote Text",
                    Status = PostStatus.Active,
                    VideoUrl = "Video Url",
                    Tags = tags
                };

                repo.Save(post);
                db.Save(true);
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IPostRepository>();

                post = repo.GetPost(post.Id);
            }

            Assert.AreEqual(user.Id, post.Category.UserId);
            Assert.AreEqual("Test Category", post.Category.Title);
            Assert.AreEqual("Description", post.Category.Description);
            Assert.AreEqual(time, post.Category.CreateDate);

            Assert.AreEqual(category.Id, post.CategoryId);
            Assert.AreEqual(PostType.Video, post.PostType);
            Assert.AreEqual("Post Title", post.Title);
            Assert.AreEqual("Post Content", post.Content);
            Assert.AreEqual(time, post.CreateDate);
            Assert.IsTrue(imageBytes.SequenceEqual(post.ImageBytes));
            Assert.AreEqual("Quote Author", post.QuoteAuthor);
            Assert.AreEqual("Quote Text", post.QuoteText);
            Assert.AreEqual(PostStatus.Active, post.Status);
            Assert.AreEqual("Video Url", post.VideoUrl);

            Assert.AreEqual(3, post.Tags.Count);
            Assert.AreEqual("Tag 1", post.Tags[0].Name);
            Assert.AreEqual("Tag 2", post.Tags[1].Name);
            Assert.AreEqual("Tag 3", post.Tags[2].Name);
        }

        [TestMethod, TestCategory("PostRepository")]
        public void Should_Update_Post()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            var time = DateTime.Now;
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            Category category;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IPostRepository>();

                category = new Category
                {
                    UserId = user.Id,
                    Title = "Test Category",
                    Description = "Description",
                    CreateDate = time
                };

                repo.Save(category);
                db.Save();
            }

            var tags = new List<Tag>
            {
                new Tag {Name = "Tag 1"},
                new Tag {Name = "Tag 2"},
                new Tag {Name = "Tag 3"}
            };
            var imageBytes = new byte[] { 1, 2, 3 };

            Post post;
            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IPostRepository>();

                post = new Post
                {
                    CategoryId = category.Id,
                    PostType = PostType.Video,
                    Title = "Post Title",
                    Content = "Post Content",
                    CreateDate = time,
                    ImageBytes = imageBytes,
                    QuoteAuthor = "Quote Author",
                    QuoteText = "Quote Text",
                    Status = PostStatus.Active,
                    VideoUrl = "Video Url",
                    Tags = tags
                };

                repo.Save(post);
                db.Save(true);
            }

            tags.RemoveAt(1);
            tags.Add(new Tag{Name = "Tag 4"});
            imageBytes = new byte[] { 4, 5, 6 };

            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IPostRepository>();

                var updated = new Post
                {
                    Id = post.Id,
                    CategoryId = category.Id,
                    PostType = PostType.Image,
                    Title = "Post Title Updated",
                    Content = "Post Content Updated",
                    CreateDate = time.AddDays(1),
                    ImageBytes = imageBytes,
                    QuoteAuthor = "Quote Author Updated",
                    QuoteText = "Quote Text Updated",
                    Status = PostStatus.Passive,
                    VideoUrl = "Video Url Updated",
                    Tags = tags
                };

                repo.Save(updated);
                db.Save(true);
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IPostRepository>();

                post = repo.GetPost(post.Id);
            }

            Assert.AreEqual(user.Id, post.Category.UserId);
            Assert.AreEqual("Test Category", post.Category.Title);
            Assert.AreEqual("Description", post.Category.Description);
            Assert.AreEqual(time, post.Category.CreateDate);

            Assert.AreEqual(category.Id, post.CategoryId);
            Assert.AreEqual(PostType.Image, post.PostType);
            Assert.AreEqual("Post Title Updated", post.Title);
            Assert.AreEqual("Post Content Updated", post.Content);
            Assert.AreEqual(time.AddDays(1), post.CreateDate);
            Assert.IsTrue(imageBytes.SequenceEqual(post.ImageBytes));
            Assert.AreEqual("Quote Author Updated", post.QuoteAuthor);
            Assert.AreEqual("Quote Text Updated", post.QuoteText);
            Assert.AreEqual(PostStatus.Passive, post.Status);
            Assert.AreEqual("Video Url Updated", post.VideoUrl);

            Assert.AreEqual(3, post.Tags.Count);
            Assert.AreEqual("Tag 1", post.Tags[0].Name);
            Assert.AreEqual("Tag 3", post.Tags[1].Name);
            Assert.AreEqual("Tag 4", post.Tags[2].Name);
        }

        //[TestMethod, TestCategory("PostRepository")]
        //public void Should_Delete_Post()
        //{
        //    User user;
        //    using (var db = Db.ReadWrite())
        //    {
        //        var repo = db.GetRepository<IUserRepository>();

        //        user = new User
        //        {
        //            Username = "taga",
        //            Password = "1234"
        //        };

        //        repo.Save(user);
        //        db.Save();
        //    }

        //    var time = DateTime.Now;
        //    time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

        //    Post post;
        //    using (var db = Db.ReadWrite())
        //    {
        //        var repo = db.GetRepository<IPostRepository>();

        //        post = new Post
        //        {
        //            UserId = user.Id,
        //            Title = "Test Post",
        //            Description = "Description",
        //            CreateDate = time
        //        };

        //        repo.Save(post);
        //        db.Save();
        //    }

        //    using (var db = Db.ReadWrite())
        //    {
        //        var repo = db.GetRepository<IPostRepository>();

        //        repo.Delete(new Post
        //        {
        //            Id = post.Id
        //        });

        //        db.Save();
        //    }

        //    using (var db = Db.Readonly())
        //    {
        //        var repo = db.GetRepository<IPostRepository>();

        //        post = repo.GetPost(post.Id);
        //    }

        //    Assert.IsNull(post);
        //}

        //[TestMethod, TestCategory("PostRepository")]
        //public void Should_Get_Posts_Of_User()
        //{
        //    User user;
        //    using (var db = Db.ReadWrite())
        //    {
        //        var repo = db.GetRepository<IUserRepository>();

        //        user = new User
        //        {
        //            Username = "taga",
        //            Password = "1234"
        //        };

        //        repo.Save(user);
        //        db.Save();
        //    }

        //    var time = DateTime.Now;

        //    for (var i = 0; i < 5; i++)
        //    {
        //        using (var db = Db.ReadWrite())
        //        {
        //            var repo = db.GetRepository<IPostRepository>();

        //            var post = new Post
        //            {
        //                UserId = i % 2 == 1 ? user.Id + 1 : user.Id,
        //                Title = "Test Post " + i,
        //                Description = "Description " + i,
        //                CreateDate = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second).AddDays(i)
        //            };

        //            repo.Save(post);
        //            db.Save();
        //        }
        //    }

        //    Post[] posts;

        //    using (var db = Db.Readonly())
        //    {
        //        var repo = db.GetRepository<IPostRepository>();

        //        posts = repo.GetPostsOfUser(user.Id);
        //    }

        //    Assert.AreEqual(3, posts.Length);

        //    var j = 0;
        //    for (var i = 0; i < 5; i++)
        //    {
        //        if (i % 2 == 1)
        //        {
        //            continue;
        //        }

        //        var cat = posts[j++];

        //        Assert.AreEqual(user.Id, cat.UserId);
        //        Assert.AreEqual("Test Post " + i, cat.Title);
        //        Assert.AreEqual("Description " + i, cat.Description);
        //        Assert.AreEqual(new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second).AddDays(i), cat.CreateDate);
        //    }
        //}
    }
}

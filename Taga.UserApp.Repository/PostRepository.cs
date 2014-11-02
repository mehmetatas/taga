﻿using System;
using System.Linq;
using Taga.Core.Repository;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly IRepository _repository;

        public PostRepository(IRepository repository)
        {
            _repository = repository;
        }

        public void Save(Category category)
        {
            _repository.Save(category);
        }

        public Category GetCategory(long categoryId)
        {
            return _repository.Select<Category>()
                .SingleOrDefault(c => c.Id == categoryId);
        }

        public Category[] GetCategoriesOfUser(long userId)
        {
            return _repository.Select<Category>()
                .Where(c => c.UserId == userId)
                .ToArray();
        }

        public void Delete(Category category)
        {
            _repository.Delete(category);
        }

        public void Save(Post post)
        {
            if (post.Id > 0)
            {
                var postTags = _repository.Select<PostTag>()
                    .Where(pt => pt.PostId == post.Id);

                foreach (var postTag in postTags)
                {
                    _repository.Delete(postTag);
                    _repository.Delete(new TagPost
                    {
                        TagId = postTag.TagId,
                        PostId = post.Id
                    });
                }
                
                //_repository.Delete<PostTag>(pt => pt.PostId, post.Id);
                //_repository.Delete<TagPost>(pt => pt.PostId, post.Id);
            }

            _repository.Save(post);

            foreach (var tag in post.Tags.Where(tag => tag.Id < 1))
            {
                _repository.Insert(tag);
            }

            _repository.Flush();

            foreach (var tag in post.Tags)
            {
                _repository.Insert(new PostTag
                {
                    PostId = post.Id,
                    TagId = tag.Id
                });

                _repository.Insert(new TagPost
                {
                    PostId = post.Id,
                    TagId = tag.Id
                });
            }
        }

        public Post GetPost(long postId)
        {
            var post = _repository.Select<Post>()
                .SingleOrDefault(p => p.Id == postId);

            if (post != null)
            {
                SetCategories(post);
                SetTags(post);
            }

            return post;
        }

        public void Delete(Post post)
        {
            _repository.Delete(post);
        }

        public IPage<Post> GetPostsOfUser(long userId, int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public IPage<Post> GetPostsOfCategory(long catId, int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        private void SetCategories(params Post[] posts)
        {
            var categoryIds = posts.Select(p => p.CategoryId);

            var categories = _repository.Select<Category>()
                .Where(cat => categoryIds.Contains(cat.Id))
                .ToList();

            foreach (var category in categories)
            {
                foreach (var post in posts)
                {
                    if (post.CategoryId == category.Id)
                    {
                        post.Category = category;
                    }
                }
            }
        }

        private void SetTags(params Post[] posts)
        {
            var postIds = posts.Select(p => p.Id);

            var postTags = _repository.Select<PostTag>();
            var tags = _repository.Select<Tag>();

            var result =
                (from postTag in postTags
                from tag in tags
                where
                    postTag.TagId == tag.Id &&
                    postIds.Contains(postTag.PostId)
                select new
                {
                    Tag = tag,
                    postTag.PostId
                }).ToList();

            foreach (var post in posts)
            {
                post.Tags = result.Where(r => r.PostId == post.Id)
                    .Select(r => r.Tag)
                    .ToList();
            }
        }
    }
}
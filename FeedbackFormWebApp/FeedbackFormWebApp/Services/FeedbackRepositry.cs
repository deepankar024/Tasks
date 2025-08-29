using FeedbackFormWebApp.Models;
using FeedbackFormWebApp.Caching;
using Microsoft.Testing.Platform.Extensions.Messages;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Caching;

namespace FeedbackFormWebApp.Services
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ICacheService _cacheService;

        public FeedbackRepository()
        {
            _cacheService = new MemoryCacheService();
        }

        public FeedbackRepository(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public void Add(Feedback entry)
        {
            using (var db = new FeedbackDbContext())
            {
                db.Feedbacks.Add(entry);
                db.SaveChanges();
            }

            _cacheService.RemoveByPrefix("Feedback_Page");
        }

        public List<Feedback> GetAllOrdered()
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Feedbacks
                         .OrderByDescending(f => f.SubmittedAt)
                         .ToList();
            }
        }

        // NEW: Get single feedback by ID
        public Feedback GetById(int id)
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Feedbacks.FirstOrDefault(f => f.Id == id);
            }
        }

        // NEW: Update feedback
        public void Update(Feedback feedback)
        {
            using (var db = new FeedbackDbContext())
            {
                var existing = db.Feedbacks.Find(feedback.Id);
                if (existing != null)
                {
                    existing.Name = feedback.Name;
                    existing.Email = feedback.Email;
                    existing.Category = feedback.Category;
                    existing.Message = feedback.Message;
                    // Don't update SubmittedAt - keep original timestamp

                    db.SaveChanges();
                }
            }

            _cacheService.RemoveByPrefix("Feedback_Page");
        }

        // NEW: Delete feedback
        public void Delete(int id)
        {
            using (var db = new FeedbackDbContext())
            {
                var feedback = db.Feedbacks.Find(id);
                if (feedback != null)
                {
                    db.Feedbacks.Remove(feedback);
                    db.SaveChanges();
                }
            }

            _cacheService.RemoveByPrefix("Feedback_Page");
        }

        public PagedResult<Feedback> GetPagedFeedback(int pageNumber, int pageSize, string sortField = "SubmittedAt", string sortDirection = "DESC")
        {
            string cacheKey = $"Feedback_Page{pageNumber}_Size{pageSize}_Sort{sortField}{sortDirection}";

            var cachedResult = _cacheService.Get<PagedResult<Feedback>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            using (var db = new FeedbackDbContext())
            {
                var query = db.Feedbacks.AsQueryable();
                query = ApplySorting(query, sortField, sortDirection);

                var totalRecords = query.Count();

                var items = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResult<Feedback>
                {
                    Items = items,
                    CurrentPage = pageNumber,
                    TotalRecords = totalRecords,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    SortField = sortField,
                    SortDirection = sortDirection
                };

                _cacheService.Set(cacheKey, pagedResult, TimeSpan.FromMinutes(5));

                return pagedResult;
            }
        }

        private IQueryable<Feedback> ApplySorting(IQueryable<Feedback> query, string sortField, string sortDirection)
        {
            if (string.IsNullOrEmpty(sortField))
                sortField = "SubmittedAt";

            var isDescending = sortDirection?.ToUpper() == "DESC";

            switch (sortField.ToLower())
            {
                case "id":
                    return isDescending ? query.OrderByDescending(f => f.Id) : query.OrderBy(f => f.Id);
                case "name":
                    return isDescending ? query.OrderByDescending(f => f.Name) : query.OrderBy(f => f.Name);
                case "email":
                    return isDescending ? query.OrderByDescending(f => f.Email) : query.OrderBy(f => f.Email);
                case "category":
                    return isDescending ? query.OrderByDescending(f => f.Category) : query.OrderBy(f => f.Category);
                case "message":
                    return isDescending ? query.OrderByDescending(f => f.Message) : query.OrderBy(f => f.Message);
                case "submittedat":
                    return isDescending ? query.OrderByDescending(f => f.SubmittedAt) : query.OrderBy(f => f.SubmittedAt);
                default:
                    return query.OrderByDescending(f => f.SubmittedAt);
            }
        }

        public int GetTotalCount()
        {
            using (var db = new FeedbackDbContext())
            {
                return db.Feedbacks.Count();
            }
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
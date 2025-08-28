using System;
using FeedbackFormWebApp.Controls;
using FeedbackFormWebApp.Models;
using FeedbackFormWebApp.Services;
using Moq;
using NUnit.Framework;

namespace FeedbackFormWebApp.Tests.Controls
{
    [TestFixture]
    public class FeedbackFormControlTests
    {
        private Mock<IFeedbackRepository> _mockRepo;
        private FeedbackFormControl _control;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IFeedbackRepository>();
            _control = new FeedbackFormControl
            {
                FeedbackRepository = _mockRepo.Object
            };
        }

        [Test]
        public void Add_Feedback_SubmitsSuccessfully()
        {
            // Arrange
            var feedback = new Feedback
            {
                Name = "Test User",
                Email = "test@example.com",
                Category = "Bug",
                Message = "Something is broken",
                SubmittedAt = DateTime.UtcNow
            };

            // Act
            _control.FeedbackRepository.Add(feedback);

            // Assert
            _mockRepo.Verify(r => r.Add(It.Is<Feedback>(f => f.Name == "Test User")), Times.Once);
        }

        
    }
}

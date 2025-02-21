using AutoMapper;
using Dhbw.ThesisManager.Domain.Data;
using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Domain.Events;
using Dhbw.ThesisManager.Api.Services;
using Dhbw.ThesisManager.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using InCompanySupervisor = Dhbw.ThesisManager.Domain.Data.Entities.InCompanySupervisor;
using Student = Dhbw.ThesisManager.Domain.Data.Entities.Student;

namespace Dhbw.ThesisManager.Api.Tests.Controller;

public class ThesisControllerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly ThesisManagerDbContext _dbContext;
    private readonly ThesisControllerImpl _controller;

    public ThesisControllerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _eventPublisherMock = new Mock<IEventPublisher>();

        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ThesisManagerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;
        _dbContext = new ThesisManagerDbContext(options);

        // Setup event publisher mock
        _eventPublisherMock.Setup(x => x.PublishAsync(It.IsAny<NewThesisAdded>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _controller = new ThesisControllerImpl(_dbContext, _mapperMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task ThesesPOSTAsync_ShouldPublishNewThesisAddedEvent()
    {
        // Arrange
        var thesis = new Thesis { Topic = "Test Thesis" };
        var thesisEntity = new Domain.Data.Entities.Thesis
        {
            Topic = "Test Thesis",
            StudentId = 1,
            PreparationPeriodFrom = DateTime.UtcNow,
            PreparationPeriodTo = DateTime.UtcNow.AddMonths(6),
            Student = new Student
            {
                Title = "Mr.",
                FirstName = "John",
                LastName = "Doe",
                RegistrationNumber = "12345",
                Course = "TINF22"
            },
            InCompanySupervisor = new InCompanySupervisor
            {
                Title = "Dr.",
                AcademicTitle = "Prof.",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@company.com",
                PhoneNumber = "+49123456789",
                AcademicDegree = "Ph.D."
            }
        };

        _mapperMock.Setup(m => m.Map(thesis, It.IsAny<Domain.Data.Entities.Thesis>()))
            .Callback<Thesis, Domain.Data.Entities.Thesis>((src, dest) => {
                dest.Topic = thesisEntity.Topic;
                dest.StudentId = thesisEntity.StudentId;
                dest.PreparationPeriodFrom = thesisEntity.PreparationPeriodFrom;
                dest.PreparationPeriodTo = thesisEntity.PreparationPeriodTo;
                dest.Student = thesisEntity.Student;
                dest.InCompanySupervisor = thesisEntity.InCompanySupervisor;
            })
            .Returns(thesisEntity);
        _mapperMock.Setup(m => m.Map<Thesis>(It.IsAny<Domain.Data.Entities.Thesis>()))
            .Returns(new Thesis { Topic = thesisEntity.Topic });

        // Act
        await _controller.ThesesPOSTAsync(thesis);

        // Assert
        _eventPublisherMock.Verify(x => x.PublishAsync(
            It.Is<NewThesisAdded>(e =>
                e.Title == "Test Thesis" &&
                e.StudentName == "John Doe" &&
                e.SupervisorName == "Jane Smith" &&
                e.SupervisorEmail == "jane.smith@company.com"),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task ThesesPOSTAsync_WithNullRelatedEntities_ShouldPublishEventWithDefaultValues()
    {
        // Arrange
        var thesis = new Thesis { Topic = "Test Thesis" };
        var thesisEntity = new Domain.Data.Entities.Thesis
        {
            Topic = "Test Thesis",
            StudentId = 1,
            PreparationPeriodFrom = DateTime.UtcNow,
            PreparationPeriodTo = DateTime.UtcNow.AddMonths(6),
            Student = null,
            InCompanySupervisor = null
        };

        _mapperMock.Setup(m => m.Map(thesis, It.IsAny<Domain.Data.Entities.Thesis>()))
            .Callback<Thesis, Domain.Data.Entities.Thesis>((src, dest) => {
                dest.Topic = thesisEntity.Topic;
                dest.StudentId = thesisEntity.StudentId;
                dest.PreparationPeriodFrom = thesisEntity.PreparationPeriodFrom;
                dest.PreparationPeriodTo = thesisEntity.PreparationPeriodTo;
                dest.Student = thesisEntity.Student;
                dest.InCompanySupervisor = thesisEntity.InCompanySupervisor;
            })
            .Returns(thesisEntity);
        _mapperMock.Setup(m => m.Map<Thesis>(It.IsAny<Domain.Data.Entities.Thesis>()))
            .Returns(new Thesis { Topic = thesisEntity.Topic });

        // Act
        await _controller.ThesesPOSTAsync(thesis);

        // Assert
        _eventPublisherMock.Verify(x => x.PublishAsync(
            It.Is<NewThesisAdded>(e =>
                e.Title == "Test Thesis" &&
                e.StudentName == "" &&
                e.StudentEmail == "Unknown" &&
                e.SupervisorName == "" &&
                e.SupervisorEmail == "Unknown"),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task ThesesPOSTAsync_ShouldSaveThesisAndPublishEvent()
    {
        // Arrange
        var thesis = new Thesis { Topic = "Test Thesis" };
        var thesisEntity = new Domain.Data.Entities.Thesis
        {
            Topic = "Test Thesis",
            StudentId = 1,
            PreparationPeriodFrom = DateTime.UtcNow,
            PreparationPeriodTo = DateTime.UtcNow.AddMonths(6)
        };

        _mapperMock.Setup(m => m.Map(thesis, It.IsAny<Domain.Data.Entities.Thesis>()))
            .Callback<Thesis, Domain.Data.Entities.Thesis>((src, dest) => {
                dest.Topic = thesisEntity.Topic;
                dest.StudentId = thesisEntity.StudentId;
                dest.PreparationPeriodFrom = thesisEntity.PreparationPeriodFrom;
                dest.PreparationPeriodTo = thesisEntity.PreparationPeriodTo;
            })
            .Returns(thesisEntity);
        _mapperMock.Setup(m => m.Map<Thesis>(It.IsAny<Domain.Data.Entities.Thesis>()))
            .Returns(new Thesis { Topic = thesisEntity.Topic });

        // Act
        var result = await _controller.ThesesPOSTAsync(thesis);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Ok);
        
        var savedThesis = await _dbContext.Theses.FirstOrDefaultAsync();
        Assert.NotNull(savedThesis);
        Assert.Equal("Test Thesis", savedThesis.Topic);

        _eventPublisherMock.Verify(x => x.PublishAsync(
            It.Is<NewThesisAdded>(e => e.Title == "Test Thesis"),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task ThesesAllAsync_ShouldReturnAllTheses()
    {
        // Arrange
        var theses = new List<Domain.Data.Entities.Thesis>
        {
            new() { 
                Id = 1, 
                Topic = "Test Thesis 1",
                Student = new Student 
                { 
                    FirstName = "John", 
                    LastName = "Doe",
                    Title = "Mr.",
                    RegistrationNumber = "12345",
                    Course = "TINF22"
                }
            },
            new() { 
                Id = 2, 
                Topic = "Test Thesis 2",
                Student = new Student 
                { 
                    FirstName = "Jane", 
                    LastName = "Smith",
                    Title = "Ms.",
                    RegistrationNumber = "12346",
                    Course = "TINF22"
                }
            }
        };
        await _dbContext.Theses.AddRangeAsync(theses);
        await _dbContext.SaveChangesAsync();

        var expectedSummaries = theses.Select(t => new ThesisSummary 
        { 
            Id = t.Id, 
            Title = t.Topic,
            StudentFirstName = t.Student.FirstName,
            StudentLastName = t.Student.LastName
        }).ToList();
        _mapperMock.Setup(m => m.Map<List<ThesisSummary>>(It.IsAny<List<Domain.Data.Entities.Thesis>>()))
            .Returns(expectedSummaries);

        // Act
        var result = await _controller.ThesesAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Thesis 1", result.First().Title);
        Assert.Equal("Test Thesis 2", result.Last().Title);
    }

    [Fact]
    public async Task MineGETAsync_ShouldReturnUserThesis()
    {
        // Arrange
        var thesis = new Domain.Data.Entities.Thesis
        {
            Id = 1,
            Topic = "My Thesis",
            Student = new Student 
            { 
                Id = 1, 
                FirstName = "John", 
                LastName = "Doe",
                Title = "Mr.",
                RegistrationNumber = "12345",
                Course = "TINF22"
            }
        };
        await _dbContext.Theses.AddAsync(thesis);
        await _dbContext.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<Thesis>(It.IsAny<Domain.Data.Entities.Thesis>()))
            .Returns(new Thesis { Topic = thesis.Topic });

        // Act
        var result = await _controller.MineGETAsync();

        // Assert
        Assert.NotNull(result.Ok);
        Assert.Equal("My Thesis", result.Ok.Topic);
    }

    [Fact]
    public async Task MinePUTAsync_ShouldUpdateUserThesis()
    {
        // Arrange
        var thesis = new Domain.Data.Entities.Thesis
        {
            Id = 1,
            Topic = "Original Topic",
            Student = new Student 
            { 
                Id = 1, 
                FirstName = "John", 
                LastName = "Doe",
                Title = "Mr.",
                RegistrationNumber = "12345",
                Course = "TINF22"
            }
        };
        await _dbContext.Theses.AddAsync(thesis);
        await _dbContext.SaveChangesAsync();

        var updatedThesis = new Thesis { Topic = "Updated Topic" };

        _mapperMock.Setup(m => m.Map(updatedThesis, thesis));
        _mapperMock.Setup(m => m.Map<Thesis>(It.IsAny<Domain.Data.Entities.Thesis>()))
            .Returns(updatedThesis);

        // Act
        var result = await _controller.MinePUTAsync(updatedThesis);

        // Assert
        Assert.NotNull(result.Ok);
        Assert.Equal("Updated Topic", result.Ok.Topic);
    }

    [Fact]
    public async Task ThesesGETAsync_ShouldReturnThesisById()
    {
        // Arrange
        var thesis = new Domain.Data.Entities.Thesis
        {
            Id = 1,
            Topic = "Test Thesis",
            Student = new Student 
            { 
                Id = 1, 
                FirstName = "John", 
                LastName = "Doe",
                Title = "Mr.",
                RegistrationNumber = "12345",
                Course = "TINF22"
            }
        };
        await _dbContext.Theses.AddAsync(thesis);
        await _dbContext.SaveChangesAsync();

        _mapperMock.Setup(m => m.Map<Thesis>(It.IsAny<Domain.Data.Entities.Thesis>()))
            .Returns(new Thesis { Topic = thesis.Topic });

        // Act
        var result = await _controller.ThesesGETAsync(1);

        // Assert
        Assert.NotNull(result.Ok);
        Assert.Equal("Test Thesis", result.Ok.Topic);
    }

    [Fact]
    public async Task ThesesGETAsync_ShouldThrowException_WhenThesisNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.ThesesGETAsync(999));
    }

    [Fact]
    public async Task CommentsPOSTAsync_ShouldAddComment()
    {
        // Arrange
        var thesis = new Domain.Data.Entities.Thesis
        {
            Id = 1,
            Topic = "Test Thesis"
        };
        await _dbContext.Theses.AddAsync(thesis);
        await _dbContext.SaveChangesAsync();

        var comment = new Comment
        {
            Author = 1,
            Message = "Test Comment"
        };

        _mapperMock.Setup(m => m.Map<Comment>(It.IsAny<Domain.Data.Entities.Comment>()))
            .Returns(comment);

        // Act
        var result = await _controller.CommentsPOSTAsync(1, comment);

        // Assert
        Assert.NotNull(result.Ok);
        Assert.Equal(1, result.Ok.Author);
        Assert.Equal("Test Comment", result.Ok.Message);

        var savedComment = await _dbContext.Comments.FirstOrDefaultAsync();
        Assert.NotNull(savedComment);
        Assert.Equal(1, savedComment.ThesisId);
        Assert.Equal("Test Comment", savedComment.Message);
    }

    [Fact]
    public async Task CommentsGETAsync_ShouldReturnComments()
    {
        // Arrange
        var thesis = new Domain.Data.Entities.Thesis { Id = 1, Topic = "Test Thesis" };
        await _dbContext.Theses.AddAsync(thesis);
        
        var comments = new List<Domain.Data.Entities.Comment>
        {
            new() { ThesisId = 1, Author = 1, Message = "Comment 1", CreatedAt = DateTime.UtcNow },
            new() { ThesisId = 1, Author = 2, Message = "Comment 2", CreatedAt = DateTime.UtcNow }
        };
        await _dbContext.Comments.AddRangeAsync(comments);
        await _dbContext.SaveChangesAsync();

        var expectedComments = comments.Select(c => new Comment { Author = c.Author, Message = c.Message }).ToList();
        _mapperMock.Setup(m => m.Map<List<Comment>>(It.IsAny<List<Domain.Data.Entities.Comment>>()))
            .Returns(expectedComments);

        // Act
        var result = await _controller.CommentsGETAsync(1);

        // Assert
        Assert.NotNull(result.Ok);
        Assert.Equal(2, result.Ok.Count);
        Assert.Equal("Comment 1", result.Ok.First().Message);
        Assert.Equal("Comment 2", result.Ok.Last().Message);
    }
}

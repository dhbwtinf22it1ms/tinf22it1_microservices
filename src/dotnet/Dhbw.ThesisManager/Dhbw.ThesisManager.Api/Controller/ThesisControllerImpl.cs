using AutoMapper;
using Dhbw.ThesisManager.Api.Data;
using Dhbw.ThesisManager.Api.Models;
using Dhbw.ThesisManager.Domain.Events;
using Dhbw.ThesisManager.Api.Services;
using Dhbw.ThesisManager.Api.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dhbw.ThesisManager.Api;

using Entities = Dhbw.ThesisManager.Api.Data.Entities;

public class ThesisControllerImpl : IThesisController
{
    private readonly ThesisManagerDbContext _context;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public ThesisControllerImpl(ThesisManagerDbContext context, IMapper mapper, IEventPublisher eventPublisher)
    {
        _context = context;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<ICollection<ThesisSummary>> ThesesAllAsync(CancellationToken cancellationToken = default)
    {
        var theses = await _context.Theses
            .Include(t => t.Student)
            .ToListAsync(cancellationToken);
            
        return _mapper.Map<List<ThesisSummary>>(theses);
    }

    public async Task<ThesisResponse> ThesesPOSTAsync(Thesis body, CancellationToken cancellationToken = default)
    {
        var entity = new Entities.Thesis();
        _mapper.Map(body, entity);
        _context.Theses.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<Thesis>(entity);

        // Publish NewThesisAdded event
        await _eventPublisher.PublishAsync(new NewThesisAdded
        {
            ThesisId = Guid.NewGuid(), // Since we don't have a GUID in the entity, generate a new one
            Title = entity.Topic,
            StudentName = $"{entity.Student?.FirstName} {entity.Student?.LastName}".Trim(),
            StudentEmail = "Unknown", // Student entity doesn't have email
            SupervisorName = $"{entity.InCompanySupervisor?.FirstName} {entity.InCompanySupervisor?.LastName}".Trim(),
            SupervisorEmail = entity.InCompanySupervisor?.Email ?? "Unknown",
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);
        
        return new ThesisResponse { Ok = result };
    }

    public async Task<ThesisResponse> MineGETAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from auth context
        var userId = 1; // Placeholder
        
        var thesis = await _context.Theses
            .Include(t => t.Student)
            .Include(t => t.PartnerCompany)
            .Include(t => t.OperationalLocation)
            .Include(t => t.InCompanySupervisor)
            .FirstOrDefaultAsync(t => t.Student.Id == userId, cancellationToken);
            
        if (thesis == null)
        {
            throw new Exception("Thesis not found");
        }
        
        return new ThesisResponse { Ok = _mapper.Map<Thesis>(thesis) };
    }

    public async Task<ThesisResponse> MinePUTAsync(Thesis body, CancellationToken cancellationToken = default)
    {
        // TODO: Get current user ID from auth context
        var userId = 1; // Placeholder
        
        var thesis = await _context.Theses
            .Include(t => t.Student)
            .FirstOrDefaultAsync(t => t.Student.Id == userId, cancellationToken);
            
        if (thesis == null)
        {
            throw new Exception("Thesis not found");
        }
        
        _mapper.Map(body, thesis);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new ThesisResponse { Ok = _mapper.Map<Thesis>(thesis) };
    }

    public async Task<ThesisResponse> ThesesGETAsync(long id, CancellationToken cancellationToken = default)
    {
        var thesis = await _context.Theses
            .Include(t => t.Student)
            .Include(t => t.PartnerCompany)
            .Include(t => t.OperationalLocation)
            .Include(t => t.InCompanySupervisor)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            
        if (thesis == null)
        {
            throw new Exception("Thesis not found");
        }
        
        return new ThesisResponse { Ok = _mapper.Map<Thesis>(thesis) };
    }

    public async Task<ThesisResponse> ThesesPUTAsync(long id, Thesis body, CancellationToken cancellationToken = default)
    {
        var thesis = await _context.Theses
            .Include(t => t.Student)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            
        if (thesis == null)
        {
            throw new Exception("Thesis not found");
        }
        
        _mapper.Map(body, thesis);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new ThesisResponse { Ok = _mapper.Map<Thesis>(thesis) };
    }

    public async Task<CommentResponse> CommentsPOSTAsync(long id, Comment body, CancellationToken cancellationToken = default)
    {
        var thesis = await _context.Theses
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
            
        if (thesis == null)
        {
            throw new Exception("Thesis not found");
        }
        
        var comment = new Entities.Comment
        {
            ThesisId = id,
            CreatedAt = DateTime.UtcNow,
            Author = body.Author,
            Message = body.Message
        };
        
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);
        
        var result = _mapper.Map<Comment>(comment);
        return new CommentResponse { Ok = result };
    }

    public async Task<CommentListResponse> CommentsGETAsync(long id, CancellationToken cancellationToken = default)
    {
        var comments = await _context.Comments
            .Where(c => c.ThesisId == id)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
            
        return new CommentListResponse { Ok = _mapper.Map<List<Comment>>(comments) };
    }
}

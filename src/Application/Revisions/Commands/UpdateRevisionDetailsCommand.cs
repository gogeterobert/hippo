using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Interfaces.BindleService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Revisions.Commands;

public class UpdateRevisionDetailsCommand : IRequest
{
    [Required]
    public Guid RevisionId { get; set; }

    [Required]
    public RevisionDetails? RevisionDetails { get; set; }
}

public class UpdateRevisionDetailsCommandHandler : IRequestHandler<UpdateRevisionDetailsCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateRevisionDetailsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateRevisionDetailsCommand request, CancellationToken cancellationToken)
    {
        var revision = await _context.Revisions
            .Where(a => a.Id == request.RevisionId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = revision ?? throw new NotFoundException(nameof(Core.Entities.Revision), request.RevisionId);
        var revisionDetails = request.RevisionDetails;
        if (revisionDetails == null || revisionDetails.SpinToml == null)
        {
            return Unit.Value;
        }

        revision.Description = revisionDetails.Description;
        var newComponents = revisionDetails.SpinToml.Component
            .Select(c => new Core.Entities.RevisionComponent
            {
                Source = c.Source,
                Name = c.Id,
                Route = c.Trigger?.Route ?? "/",
                Revision = revision,
            });
        _context.RevisionComponents.AddRange(newComponents);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
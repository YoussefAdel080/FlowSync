namespace FlowSync.Application.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}

namespace AppointmentManagement.Application.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync();
}
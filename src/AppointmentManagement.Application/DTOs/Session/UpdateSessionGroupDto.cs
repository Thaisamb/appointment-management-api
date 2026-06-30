namespace AppointmentManagement.Application.DTOs.Session;

public class UpdateSessionGroupDto
{
    public Guid GroupId { get; set; }
    public DateTime FromDateTime { get; set; }  // data da sessão selecionada (inclusive)
    public DateTime NewDateTime { get; set; }   // novo horário base
    public int Duration { get; set; }
    public decimal PricePerSession { get; set; }
}
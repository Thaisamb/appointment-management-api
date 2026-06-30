using System.ComponentModel;

public enum SessionStatus
{
    [Description("Agendado")]
    Scheduled = 1,
    [Description("Confirmado")]
    Confirmed = 2,
    [Description("Realizado")]
    Done = 3,
    [Description("Pago")]
    Paid = 4,
    [Description("Cancelado")]
    Cancelled = 5
}
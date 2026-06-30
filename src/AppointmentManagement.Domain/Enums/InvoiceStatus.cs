using System.ComponentModel;

public enum InvoiceStatus
{
    [Description("Pendente")]
    Pending = 1,
    [Description("Processando")]
    Processing = 2,
    [Description("Emitida")]
    Issued = 3,
    [Description("Erro")]
    Error = 4,
    [Description("Cancelada")]
    Cancelled = 5
}
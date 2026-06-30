namespace AppointmentManagement.Application.Constants;

public static class ValidationMessages
{
	public const string RequiredField = "Campo obrigatório";
	public const string InvalidClient = "Cliente inválido.";
	public const string PastMonthTime = "A datas passadas apenas no mês vigente.";
	public const string EndDate = "A data da sessão deve ser futura.";
	public const string InvalidPrice = "Valor da sessão deve ser maior que zero.";
	public const string OverlappingSession = "Já existe uma sessão neste horário.";
	public const string InvalidRepetitionType = "Tipo de repetição inválido.";
	public const string WeekDaysRequired = "Selecione um dia da semana.";
	public const string RepetitionEndDateInvalid = "Data de término deve ser posterior à data da sessão.";

    public const string CpfJaExiste = "O CPF informado já está em uso.";
}
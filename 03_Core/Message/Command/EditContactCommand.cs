namespace Core.Message.Command;
public record EditContactCommand(int Id, string Name, string Phone, string Email, int DddId);


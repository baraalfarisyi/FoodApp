namespace UserService.GraphQL
{
    public record ChangePassUser
    (
        int? Id,
        string UserName,
        string Password
    );
}

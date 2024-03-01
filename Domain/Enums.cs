namespace Domain
{
    public enum ReplyEnum
    {
        Valid,
        EmailExist,
        AccountNotFound,
        InvalidEmail,
        InvalidOldPassword,
        InvalidPassword,
        ExpiredToken,
        InvalidToken,
        AlreadyActivated,
    }
    public enum ValidateTokenEnum
    {
        Valid,
        Invalid,
        InvalidPermission,
        Expired
    }
}

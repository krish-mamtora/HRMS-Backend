namespace HRMS_Backend.Common.Enums
{
    public enum ResponseCode
    {
        Success = 200,

        Created = 201,

        BadRequest = 400,

        InvalidCredentials = 401,

        Unauthorized = 403,

        NotFound = 404,

        ValidationError = 422,

        ServerError = 500,

        UserAlreadyExists = 409
    }
}
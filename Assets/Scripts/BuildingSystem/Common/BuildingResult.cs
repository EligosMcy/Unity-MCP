public struct BuildingResult
{
    public bool Success { get; }
    public BuildingErrorCode ErrorCode { get; }
    public string Message { get; }

    public BuildingResult(bool success, BuildingErrorCode errorCode, string message)
    {
        Success = success;
        ErrorCode = errorCode;
        Message = message;
    }

    public static BuildingResult Ok()
    {
        return new BuildingResult(true, BuildingErrorCode.None, "");
    }

    public static BuildingResult Fail(BuildingErrorCode errorCode, string message)
    {
        return new BuildingResult(false, errorCode, message);
    }
}

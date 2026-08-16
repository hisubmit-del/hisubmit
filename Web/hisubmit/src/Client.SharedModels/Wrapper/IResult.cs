using System.Collections.Generic;

namespace HiSubmit.Client.SharedModels.Wrapper;

public interface IResult
{
    List<string> Messages { get; set; }

    bool Succeeded { get; set; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}

public class BaseDeleteRequest
{
    public int Id { get; set; }
}
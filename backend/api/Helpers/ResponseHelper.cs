using System;
using api.Dtos;

namespace api.Helpers;

public static class ResponseHelper
{
    public static ResponseDto<T> GenerateResponse<T>(T Data,bool Success,string Message)
    {
        return new ResponseDto<T>()
        {
          Data=Data,
          Message=Message,
          Success=Success  
        };
    }
}

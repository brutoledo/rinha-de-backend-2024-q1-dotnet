using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RinhaBackend._2024.Q1.Api.Validations;

public class ValidationFailedResult : ObjectResult  
{  
    public ValidationFailedResult(ModelStateDictionary modelState)  
        : base(new ValidationResultModel(modelState))  
    {
        StatusCode = StatusCodes.Status422UnprocessableEntity; //change the http status code to 422.  
    }  
}  
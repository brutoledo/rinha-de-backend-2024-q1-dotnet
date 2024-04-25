namespace RinhaBackend._2024.Q1.Api.Validations;

public class ValidationError  
{  
    public string Field { get; }  
    public int Code { get; set; }  
    public string Message { get; }  
    public ValidationError(string field,int code, string message)  
    {  
        Field = field != string.Empty ? field : null;  
        Code = code != 0 ? code : 422;   
        Message = message;  
    }  
} 
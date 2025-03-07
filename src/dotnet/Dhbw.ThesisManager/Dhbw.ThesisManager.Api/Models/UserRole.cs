using System.Runtime.Serialization;

namespace Dhbw.ThesisManager.Api.Models
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public enum UserRole
    {
        [EnumMember(Value = @"student")]
        student = 0,

        [EnumMember(Value = @"supervisor")]
        supervisor = 1,

        [EnumMember(Value = @"secretary")]
        secretary = 2,

        [EnumMember(Value = @"administrator")]
        administrator = 3,
    }
}

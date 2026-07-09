using System.Security.Principal;

namespace GreenLine.Security
{
    public interface ICustomIdentity : IIdentity
    {
        bool IsInRole(string role);
        string ToJson();
    }
}
using CK.Core;

namespace CK.DB.User.SimpleInvitation
{
    /// <summary>
    /// Package that supports simple user invitation. 
    /// </summary>
    [SqlPackage( Schema = "CK", ResourcePath = "Res" )]
    [Versions("1.0.0")]
    public abstract class Package : SqlPackage
    {
        void StObjConstruct( Actor.Package actor )
        {
        }
    }
}

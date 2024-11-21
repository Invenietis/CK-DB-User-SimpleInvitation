using CK.Core;
using CK.SqlServer;
using System;
using System.Threading.Tasks;

namespace CK.DB.User.SimpleInvitation;

/// <summary>
/// The tUserSimpleInvitation table contains user invitations.
/// </summary>
[SqlTable( "tUserSimpleInvitation", Package = typeof( Package ) )]
[Versions( "1.0.0" )]
public abstract partial class UserSimpleInvitationTable : SqlTable
{
    IPocoFactory<IUserSimpleInvitationInfo> _infoFactory;

    void StObjConstruct( IPocoFactory<IUserSimpleInvitationInfo> infoFactory )
    {
        _infoFactory = infoFactory;
    }

    /// <summary>
    /// Creates a new <see cref="IUserSimpleInvitationInfo"/> poco.
    /// </summary>
    /// <returns>A new poco instance.</returns>
    public IUserSimpleInvitationInfo CreateInfo() => _infoFactory.Create();

    /// <summary>
    /// Creates and configure a new <see cref="IUserSimpleInvitationInfo"/> poco.
    /// </summary>
    /// <typeparam name="T">The actual poco type to create.</typeparam>
    /// <param name="configurator">Configuration function.</param>
    /// <returns>A new configured poco instance.</returns>
    public T CreateInfo<T>( Action<T> configurator ) where T : IUserSimpleInvitationInfo
    {
        return ((IPocoFactory<T>)_infoFactory).Create( configurator );
    }

    /// <summary>
    /// Captures the result of <see cref="Create"/> or <see cref="CreateAsync"/> calls.
    /// </summary>
    public struct CreateResult
    {
        /// <summary>
        /// The invitation identifier.
        /// </summary>
        public readonly int InvitationId;

        /// <summary>
        /// The invitation token to use.
        /// </summary>
        public readonly string InvitationToken;

        /// <summary>
        /// Gets whether the invitation has been successfully created.
        /// </summary>
        public bool Success => InvitationId > 0;

        /// <summary>
        /// Initializes a new <see cref="CreateResult"/>.
        /// </summary>
        /// <param name="invitationIdResult">The invitation identifier.</param>
        /// <param name="invitationTokenResult">The invitation token.</param>
        public CreateResult( int invitationIdResult, string invitationTokenResult )
        {
            InvitationId = invitationIdResult;
            InvitationToken = invitationTokenResult;
        }
    }

    /// <summary>
    /// Captures the result of <see cref="StartResponse"/> or <see cref="StartResponseAsync"/> calls.
    /// </summary>
    public struct StartResponseResult
    {
        /// <summary>
        /// The invitation identifier.
        /// </summary>
        public readonly int InvitationId;

        /// <summary>
        /// The optional invitation options.
        /// </summary>
        public readonly byte[] Options;

        /// <summary>
        /// Gets whether the call is successful: typically the invitation has not yet expired.
        /// </summary>
        public bool Success => InvitationId > 0;

        /// <summary>
        /// Initializes a new <see cref="CreateResult"/>.
        /// </summary>
        /// <param name="invitationId">The invitation identifier.</param>
        /// <param name="options">The optional invitation options.</param>
        public StartResponseResult( int invitationId, byte[] options )
        {
            InvitationId = invitationId;
            Options = options;
        }
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <param name="ctx">The call context to use.</param>
    /// <param name="actorId">The current actor identifier (becomes the sender of the invitation).</param>
    /// <param name="info">Invitation information.</param>
    /// <param name="firstInvitationSent">True if an initial invitation mail has already been sent, prior to the invitation creation.</param>
    /// <returns>The <see cref="CreateResult"/> with the invitation identifier and token to use.</returns>
    [SqlProcedure( "sUserSimpleInvitationCreate" )]
    public abstract Task<CreateResult> CreateAsync( ISqlCallContext ctx, int actorId, [ParameterSource] IUserSimpleInvitationInfo info, bool firstInvitationSent = false );

    /// <summary>
    /// Starts a response to an invitation.
    /// </summary>
    /// <param name="ctx">The call context to use.</param>
    /// <param name="actorId">The current actor identifier.</param>
    /// <param name="invitationToken">The invitation token.</param>
    /// <returns>The <see cref="IUserSimpleInvitationInfo"/>.</returns>
    [SqlProcedure( "sUserSimpleInvitationStartResponse" )]
    public abstract Task<IUserSimpleInvitationInfo> StartResponseAsync( ISqlCallContext ctx, int actorId, string invitationToken );

    /// <summary>
    /// Destroys an existing invitation either because it succeeded or it must be canceled.
    /// </summary>
    /// <param name="ctx">The call context to use.</param>
    /// <param name="actorId">The current actor identifier.</param>
    /// <param name="invitationId">The invitation identifier to destroy.</param>
    /// <param name="successful">True to indicate a successful registration. False to cancel it.</param>
    /// <returns>The awaitable.</returns>
    [SqlProcedure( "sUserSimpleInvitationDestroy" )]
    public abstract Task DestroyAsync( ISqlCallContext ctx, int actorId, int invitationId, bool successful = false );

}

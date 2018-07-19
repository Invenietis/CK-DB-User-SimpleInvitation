using CK.Core;
using CK.Setup;
using CK.SqlServer;
using CK.SqlServer.Setup;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CK.DB.User.SimpleInvitation
{
    /// <summary>
    /// The tUserSimpleInvitation table contains user invitations.
    /// </summary>
    [SqlTable( "tUserSimpleInvitation", Package = typeof( Package ) )]
    [Versions( "1.0.0" )]
    public abstract partial class UserSimpleInvitationTable : SqlTable
    {
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
        /// <param name="actorId">The current actor identifier.</param>
        /// <param name="senderId">The user that sends the invitation.</param>
        /// <param name="email">The email of the invited user.</param>
        /// <param name="expirationDateUtc">The expiration date. Must be later than now otherwise an exception is thrown.</param>
        /// <param name="firstInvitationSent">True if an initial invitation mail has already been sent, prior to the invitation creation.</param>
        /// <param name="options">Optional payload that may contain invitation specific data.</param>
        /// <returns>The <see cref="CreateResult"/> with the invitation identifier and token to use.</returns>
        [SqlProcedure( "sUserSimpleInvitationCreate" )]
        public abstract Task<CreateResult> CreateAsync( ISqlCallContext ctx, int actorId, int senderId, string email, DateTime expirationDateUtc, bool firstInvitationSent = false, byte[] options = null );

        /// <summary>
        /// Starts a response to an invitation.
        /// </summary>
        /// <param name="ctx">The call context to use.</param>
        /// <param name="actorId">The current actor identifier.</param>
        /// <param name="invitationToken">The invitation token.</param>
        /// <returns>The <see cref="StartResponseResult"/> with the invitation identifier and options if any.</returns>
        [SqlProcedure( "sUserSimpleInvitationStartResponse" )]
        public abstract Task<StartResponseResult> StartResponseAsync( ISqlCallContext ctx, int actorId, string invitationToken );

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
}

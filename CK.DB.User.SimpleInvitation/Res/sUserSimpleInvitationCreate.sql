-- SetupConfig: {}
--
-- Creates a new invitation for a unique EMail: if the @EMail already exists,
-- the zero @InvitationIdResult is returned with an empty @InvitationTokenResult.
--
-- When @FirstInvitationSent is 1, the InvitationSendCount is set to 1 and LastInvitationSendDate
-- is set to now.
--
create procedure CK.sUserSimpleInvitationCreate
(
	@ActorId int,
	@SenderId int,
    @EMail nvarchar(255),
	@ExpirationDateUtc datetime2(2),
    @FirstInvitationSent bit = 0,
    @Options varbinary(max) = null,
	@InvitationIdResult int output,
	@InvitationTokenResult varchar(128) output
)
as
begin
    if @ActorId <= 0 throw 50000, 'Security.AnonymousNotAllowed', 1;
    if @ExpirationDateUtc is null or @ExpirationDateUtc <= sysutcdatetime() throw 50000, 'Argument.InvalidExpirationDateUtc', 1;

    --[beginsp]

    select @InvitationIdResult = InvitationId from CK.tUserSimpleInvitation where EMail = @EMail;
    if @@RowCount = 0
    begin
        declare @InvitationSendCount int = 0;
        declare @LastInvitationSendDate datetime2(2) = '0001-01-01';
        if @FirstInvitationSent = 1
        begin
            set @InvitationSendCount = 1;
            set @LastInvitationSendDate = sysutcdatetime();
        end

	    --<PreCreate revert />

	    insert into CK.tUserSimpleInvitation( SenderId, EMail, ExpirationDate, InvitationSendCount, LastInvitationSendDate, Options )
            values( @SenderId, @EMail, @ExpirationDateUtc, @InvitationSendCount, @LastInvitationSendDate, @Options );
	    set @InvitationIdResult = SCOPE_IDENTITY();
        select @InvitationTokenResult = InvitationToken from CK.tUserSimpleInvitation where InvitationId = @InvitationIdResult; 

        --<PostCreate />

    end
    else
    begin
        --<OnInvitationDuplicate />
	    set @InvitationIdResult = 0;
	    set @InvitationTokenResult = '';
    end
	--[endsp]
end


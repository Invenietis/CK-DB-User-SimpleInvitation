-- SetupConfig: {}
--
create procedure CK.sUserSimpleInvitationStartResponse
(
	@ActorId int,
	@InvitationToken varchar(128),
    @InvitationId int output,
    @EMail nvarchar(255) output,
	@ExpirationDateUtc datetime2(2) output,
    @InvitationSendCount int output,
    @LastInvitationSendDate datetime2(2) output,
    @Options varbinary(max) output
)
as
begin
    if @ActorId <= 0 throw 50000, 'Security.AnonymousNotAllowed', 1;

    set @InvitationId = cast( parsename(@InvitationToken,2) as int);
    declare @TokenGuid uniqueidentifier = cast( parsename(@InvitationToken,1) as uniqueidentifier);
    declare @Now datetime2(2) = sysutcdatetime();

	--[beginsp]

    select  @ExpirationDateUtc = ExpirationDateUtc,
            @InvitationSendCount = InvitationSendCount,
            @LastInvitationSendDate = LastInvitationSendDate,
            @EMail = EMail,
            @Options = Options
        from CK.tUserSimpleInvitation
        where InvitationId = @InvitationId;

    if @@RowCount = 0
    begin
        set @InvitationId = 0;
        set @ExpirationDateUtc = '0001-01-01';
        set @InvitationSendCount = 0;
        set @LastInvitationSendDate = @ExpirationDateUtc;
        --<OnInvitationMissing />
    end
    else if @ExpirationDateUtc < @Now
    begin
        declare @dummyStatement int = 0;
        --<OnInvitationExpired />
    end
    else
    begin
        --<OnInvitationStartResponse />
        declare @SafeExpire datetime2(2) = dateadd(minute, 10, @Now);
        if @ExpirationDateUtc < @SafeExpire
        begin
            set @ExpirationDateUtc = @SafeExpire;
        end
        update CK.tUserSimpleInvitation set
                ExpirationDateUtc = @ExpirationDateUtc,
                ResponseStartedDate = @Now
            where InvitationId = @InvitationId;
    end

	--[endsp]
end


-- SetupConfig: {}
--
create procedure CK.sUserSimpleInvitationStartResponse
(
	@ActorId int,
	@InvitationToken varchar(128),
    @InvitationId int output,
    @Options varbinary(max) output
)
as
begin
    if @ActorId <= 0 throw 50000, 'Security.AnonymousNotAllowed', 1;

    set @InvitationId = cast( parsename(@InvitationToken,2) as int);
    declare @TokenGuid uniqueidentifier = cast( parsename(@InvitationToken,1) as uniqueidentifier);
    declare @ExpirationDate datetime2(2);
    declare @Now datetime2(2) = sysutcdatetime();

	--[beginsp]

    select @ExpirationDate = ExpirationDate, @Options = Options
        from CK.tUserSimpleInvitation
        where InvitationId = @InvitationId and ExpirationDate > @Now;

    if @@RowCount = 0
    begin
        set @InvitationId = 0;
        set @Options = null;
        --<OnInvitationExpired />
    end
    else
    begin
        --<OnInvitationStartResponse />
        declare @SafeExpire datetime2(2) =  dateadd(minute, 10, @Now);
        update CK.tUserSimpleInvitation set
                ExpirationDate = case when @ExpirationDate < @SafeExpire
                                        then @SafeExpire
                                        else @ExpirationDate
                                 end,
                ResponseStartedDate = sysutcdatetime()
            where InvitationId = @InvitationId;
    end

	--[endsp]
end


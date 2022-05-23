CREATE PROCEDURE [sp_UpdateElectricMeter]
    @number int,
    @typeID int,
    @dateAcceptance date,
    @stateVerificationPeriod int
AS
    UPDATE [ElectricMeter]
	SET 
	[TypeID] = @typeID,
	[DateAcceptance] = @dateAcceptance,
	[StateVerificationPeriod] = @stateVerificationPeriod
    WHERE [Number] = @number
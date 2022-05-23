CREATE PROCEDURE [sp_CreateElectricMeter]
    @number int out,
    @typeID int,
    @dateAcceptance date,
    @stateVerificationPeriod int
AS
    INSERT INTO [ElectricMeter]([Number], [TypeID], [DateAcceptance], [StateVerificationPeriod])
    VALUES (@number, @typeID, @dateAcceptance, @stateVerificationPeriod)

	SELECT SCOPE_IDENTITY()
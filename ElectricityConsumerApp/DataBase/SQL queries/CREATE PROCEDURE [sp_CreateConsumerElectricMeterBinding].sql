CREATE PROCEDURE [sp_CreateConsumerElectricMeterBinding]
	@consumerID int,
	@electricMeterNumber int
AS
	INSERT INTO [ConsumerElectricMeterBinding]([ConsumerID], [ElectricMeterNumber])
	VALUES (@consumerID, @electricMeterNumber)
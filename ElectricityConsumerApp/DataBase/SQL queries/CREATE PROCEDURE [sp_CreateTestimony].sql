CREATE PROCEDURE [sp_CreateTestimony]
    @id int out,
    @electricMeterNumber int,
    @value float
AS
    INSERT INTO [TestimonyHistory]([ElectricMeterNumber], [Value])
    VALUES (@electricMeterNumber, @value)

	SELECT SCOPE_IDENTITY()
CREATE TABLE ElectricMeter
(
	[Number] INT PRIMARY KEY,
	[TypeID] INT NULL,
	[DateAcceptance] DATETIME NOT NULL,
	[StateVerificationPeriod] INT NOT NULL,

	CONSTRAINT FK_ElectricMeter_ElectricMeterType FOREIGN KEY ([TypeID]) REFERENCES [ElectricMeterType] ([ID]) 
)
CREATE TABLE [ConsumerElectricMeterBinding]
(
	[ConsumerID] INT,
	[ElectricMeterNumber] INT


	CONSTRAINT FK_ConsumerElectricMeterBinding_Consumer FOREIGN KEY ([ConsumerID]) REFERENCES Consumer ([ID]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT FK_ConsumerElectricMeterBinding_ElectricMeter FOREIGN KEY ([ElectricMeterNumber]) REFERENCES ElectricMeter ([Number]) ON UPDATE CASCADE ON DELETE CASCADE,

	CONSTRAINT PK_ConsumerElectricMeterBinding PRIMARY KEY ([ConsumerID], [ElectricMeterNumber])
)
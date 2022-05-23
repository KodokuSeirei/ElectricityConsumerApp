CREATE TABLE TestimonyHistory
(
	ID INT PRIMARY KEY IDENTITY,
	ElectricMeterNumber INT NULL,
	Value INT NOT NULL,
	Date DATETIME DEFAULT GETDATE(),
    
	CONSTRAINT FK_TestimonyHistory_ElectricMeter FOREIGN KEY (ElectricMeterNumber) REFERENCES [ElectricMeter] (Number) ON UPDATE CASCADE ON DELETE SET NULL
)
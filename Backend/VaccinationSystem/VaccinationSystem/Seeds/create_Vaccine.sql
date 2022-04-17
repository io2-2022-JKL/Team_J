  INSERT INTO [dbo].[Vaccine] ([Id]
      ,[Company]
      ,[Name]
      ,[NumberOfDoses]
      ,[MinDaysBetweenDoses]
      ,[MaxDaysBetweenDoses]
      ,[Virus]
      ,[MinPatientAge]
      ,[MaxPatientAge]
      ,[Active])
	VALUES ('2dba46e3-a040-4dab-9ec4-600e44dbaf8e', 'BioNTech', 'Pfizer', 2, 14, 30, 0, 18, 150, 1),
    ('aa82a7a5-d92c-45d2-b47f-f2a3a5e0b3f7', 'Janssen Pharmaceutical', 'Johnson & Johnson', 1, -1, -1, 0, 10, 100, 1),
    ('31d9b4bf-5c1c-4f2d-b997-f6096758eac9', 'Moderna', 'Moderna', 2, 30, -1, 0, 18, 75, 1),
    ('1fe2941e-f2f3-4edf-9ae3-712460e88ec7', 'Oxford', 'AstraZeneca', 2, -1, 30, 0, -1, -1, 1),
    ('90a43a67-34d3-40b7-bbbb-f5a4c7f4576f', 'SputnikV', 'SputnikV', 3, 15, 45, 0, 15, -1, 0)
Vaccination system project backend

W folderze VaccinationSystem/VaccinationSystem/Seeds znajdują się przykładowe dane które można wczytać na bazę danych w postaci skryptów wypełniających poszczególne tabele.
Jako iż w bazie istnieje wiele powiązań, trzeba uważać na kolejność wstawiania danych i ich usuwania.
Przykładową poprawną kolejnością wywoływania skryptów jest:
create_Admin.sql -> create_Vaccine.sql -> create_VaccinationCenter.sql -> create_Patient.sql -> create_VaccineInVaccinationCenter.sql -> create_Doctor.sql -> create_OpeningHours.sql -> create_TimeSlots.sql -> create_Appointment.sql -> create_Certificate.sql
Przykładowa poprawna kolejność usuwania jest odwrotna do podanej kolejności dodawania (oczywiście nie wywołujemy skryptów a zapytanie SQL: DELETE FROM [nazwa tabeli].

Postać URL w storage: https://vaccinationsystem.blob.core.windows.net/certificates/Imiona%20Oddzielone%20Spacjami_Nazwisko%20Oddzielone%20Spacjami/guid.pdf, np.
https://vaccinationsystem.blob.core.windows.net/certificates/James_Bond/53cf7004-db63-4d53-a6c7-313168a71af5.pdf

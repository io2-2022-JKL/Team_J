# Obrazy Dockera
## W kolejnych folderach znajdują się instrukcje z dołączonymi plikami pozwalającymi skonfigurować lokalne obrazy dockera.
Polecam utworzyć obrazy od zera, a nie pobierać z mojego repozytorium, mogą się dziwnie zachowywać te pobrane.
Póki co mamy 2 obrazy:
- obraz bazy danych;
- obraz backendu (bez bazy danych, sama aplikacja udostępniająca API);  

Aby poprawnie wszystko zadziałało, w momencie włączania kontenera backendu należy mieć uruchomiony (prawidłowo utworzony, włącznie z przeczekaniem aż się utworzy baza danych)
kontener z bazą danych.  
Całe piękno tego rozwiązania polega na tym, że będziemy mieli lokalną bazę danych i backend, zmiany są niezależne od innych kontenerów (oraz oczywiście naszego serwera API/SQL).
Jeżeli będziemy chcieli od nowa zacząć działać na czystej bazie danych, możemy po prostu utworzyć nowy kontener bazy danych z obrazu bazy danych, wyłączyć poprzedni (i usunąć jeśli już nam nie potrzebny)
kontener i możemy zacząć na nowych danych (na podstawie plików w `SEEDS` w projekcie backendu).  
**UWAGA**  
Tworząc obrazy należy tworzyć je z brancha `docker-deploy`.
Dodatkowo, obraz bazy tworzymy z folderu `/Docker/vaccinationsystemdb/`, a obraz backendu z `/Backend/VaccinationSystem/`.  
Nie pogubcie też `.` kropek w odpowiednich komendach wymienionych w `README.md`.

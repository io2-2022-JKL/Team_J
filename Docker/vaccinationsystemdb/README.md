# Obraz przykładowej bazy danych.
## Aby utworzyć obraz, trzeba zainstalować Dockera, a następnie:
- otwieramy konsolę, przechodzimy do tego folderu;
- wpisujemy `docker build -t [nazwa_tworzonego_obrazu] .`;
- uruchamiamy kontener:
	- przez aplikację Dockera: images -> utworzony obraz -> run -> optional settings -> w local host wpisać port (np. 1433);
	- przez konsolę: `docker run -p 1433:1433 -d [nazwa_utworzonego_obrazu]`;
- w Dockerze w containers/apps widzimy nowy kontener (o losowej nazwie), wchodzimy w niego i widzimy konsolę;
- czekamy aż w konsoli utworzy się baza danych vaccinationSystem (ok. 30 sekund);
- łączymy się w MS SQL SMS:
	- Server type: Database Engine
	- Server name: localhost
	- Authentication: SQL Server Authentication
	- Login: sa
	- Password: Pass123word!
- mamy lokalną bazę danych!  

Możemy też użyć obrazu `docker pull jakubgazewski/io2-2022-j:vaccinationsystemdb` zamiast samemu go tworzyć lokalnie (bezpieczniej jednak utworzyć).

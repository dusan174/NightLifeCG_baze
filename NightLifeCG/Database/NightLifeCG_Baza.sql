CREATE DATABASE NightLifeCG;
GO
USE NightLifeCG;
GO
CREATE TABLE Uloge (
    UlogaID  INT PRIMARY KEY IDENTITY(1,1),
    Naziv    NVARCHAR(30) NOT NULL
);

CREATE TABLE Gradovi (
    GradID INT PRIMARY KEY IDENTITY(1,1),
    Naziv  NVARCHAR(50) NOT NULL
);

CREATE TABLE Korisnici (
    KorisnikID   INT PRIMARY KEY IDENTITY(1,1),
    KorisnickoIme NVARCHAR(50)  NOT NULL UNIQUE,
    Email        NVARCHAR(100) NOT NULL UNIQUE,
    LozinkaHash  NVARCHAR(256) NOT NULL,
    DatumRegistracije DATETIME NOT NULL DEFAULT GETDATE(),
    UlogaID      INT NOT NULL FOREIGN KEY REFERENCES Uloge(UlogaID)
);

CREATE TABLE MuzickiZanrovi (
    ZanrID INT PRIMARY KEY IDENTITY(1,1),
    Naziv  NVARCHAR(50) NOT NULL
);

CREATE TABLE Klubovi (
    KlubID        INT PRIMARY KEY IDENTITY(1,1),
    Naziv         NVARCHAR(100) NOT NULL,
    Opis          NVARCHAR(500),
    Adresa        NVARCHAR(200) NOT NULL,
    CijenaUlaza   DECIMAL(10,2),
    ProsjecnaOcjena FLOAT       NOT NULL DEFAULT 0,
    BrojOcjena    INT           NOT NULL DEFAULT 0,
    Aktivan       BIT           NOT NULL DEFAULT 1,
    GradID        INT           NOT NULL FOREIGN KEY REFERENCES Gradovi(GradID),
    ZanrID        INT           FOREIGN KEY REFERENCES MuzickiZanrovi(ZanrID)
);

CREATE TABLE Stolovi (
    StolID      INT PRIMARY KEY IDENTITY(1,1),
    BrojStola   NVARCHAR(20)  NOT NULL,
    BrojMjesta  INT           NOT NULL,
    JeVIP       BIT           NOT NULL DEFAULT 0,
    KlubID      INT           NOT NULL FOREIGN KEY REFERENCES Klubovi(KlubID)
);

CREATE TABLE Rezervacije (
    RezervacijaID   INT PRIMARY KEY IDENTITY(1,1),
    DatumRezervacije DATETIME NOT NULL,
    BrojGostiju     INT      NOT NULL,
    Status          INT      NOT NULL DEFAULT 0, -- 0=Na cekanju, 1=Potvrdjeno, 2=Otkazano
    DatumKreiranja  DATETIME NOT NULL DEFAULT GETDATE(),
    KorisnikID      INT      NOT NULL FOREIGN KEY REFERENCES Korisnici(KorisnikID),
    StolID          INT      NOT NULL FOREIGN KEY REFERENCES Stolovi(StolID)
);

CREATE TABLE Recenzije (
    RecenzijaID INT PRIMARY KEY IDENTITY(1,1),
    Ocjena      INT           NOT NULL CHECK (Ocjena BETWEEN 1 AND 5),
    Komentar    NVARCHAR(500),
    DatumOcjene DATETIME      NOT NULL DEFAULT GETDATE(),
    KorisnikID  INT           NOT NULL FOREIGN KEY REFERENCES Korisnici(KorisnikID),
    KlubID      INT           NOT NULL FOREIGN KEY REFERENCES Klubovi(KlubID)
);

CREATE TABLE Dogadjaji (
    DogadjajID  INT PRIMARY KEY IDENTITY(1,1),
    Naziv       NVARCHAR(100) NOT NULL,
    DatumOdrzavanja DATETIME  NOT NULL,
    CijenaKarte DECIMAL(10,2),
    KlubID      INT           NOT NULL FOREIGN KEY REFERENCES Klubovi(KlubID)
);

CREATE TABLE SlikeKluba (
    SlikaID   INT PRIMARY KEY IDENTITY(1,1),
    Putanja   NVARCHAR(300) NOT NULL,
    Glavna    BIT           NOT NULL DEFAULT 0,
    KlubID    INT           NOT NULL FOREIGN KEY REFERENCES Klubovi(KlubID)
);
-- =============================================
-- FUNKCIJE
-- =============================================

-- Funkcija 1: Vraca prosjecnu ocjenu kluba koristeci kursor
CREATE FUNCTION fn_ProsjecnaOcjena (@KlubID INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @ocjena INT
    DECLARE @ukupno FLOAT
    DECLARE @broj   INT
    DECLARE @prosjek FLOAT

    SET @ukupno = 0
    SET @broj   = 0

    DECLARE kursor_ocjene CURSOR FOR
        SELECT Ocjena FROM Recenzije WHERE KlubID = @KlubID

    OPEN kursor_ocjene
    FETCH NEXT FROM kursor_ocjene INTO @ocjena

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @ukupno = @ukupno + @ocjena
        SET @broj   = @broj + 1
        FETCH NEXT FROM kursor_ocjene INTO @ocjena
    END

    CLOSE kursor_ocjene
    DEALLOCATE kursor_ocjene

    IF @broj = 0
        SET @prosjek = 0
    ELSE
        SET @prosjek = @ukupno / @broj

    RETURN @prosjek
END;
GO
-- Funkcija 2: Vraca broj slobodnih stolova kluba za odredjeni datum koristeci kursor
CREATE FUNCTION fn_SlobodniStolovi (@KlubID INT, @Datum DATE)
RETURNS INT
AS
BEGIN
    DECLARE @stolID    INT
    DECLARE @ukupno    INT
    DECLARE @zauzeto   INT
    DECLARE @rezervisan INT

    SET @ukupno  = 0
    SET @zauzeto = 0

    DECLARE kursor_stolovi CURSOR FOR
        SELECT StolID FROM Stolovi WHERE KlubID = @KlubID

    OPEN kursor_stolovi
    FETCH NEXT FROM kursor_stolovi INTO @stolID

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @ukupno = @ukupno + 1

        SELECT @rezervisan = COUNT(*)
        FROM Rezervacije
        WHERE StolID = @stolID
          AND CAST(DatumRezervacije AS DATE) = @Datum
          AND Status != 2

        IF @rezervisan > 0
            SET @zauzeto = @zauzeto + 1

        FETCH NEXT FROM kursor_stolovi INTO @stolID
    END

    CLOSE kursor_stolovi
    DEALLOCATE kursor_stolovi

    RETURN @ukupno - @zauzeto
END;
GO

-- =============================================
-- STORED PROCEDURE
-- =============================================

-- SP 1: Ispis svih klubova iz odabranog grada koristeci kursor
CREATE PROCEDURE sp_KluboviPoGradu
    @GradID INT
AS
BEGIN
    DECLARE @klubID       INT
    DECLARE @naziv        NVARCHAR(100)
    DECLARE @adresa       NVARCHAR(200)
    DECLARE @ocjena       FLOAT
    DECLARE @cijena       DECIMAL(10,2)

    DECLARE kursor_klubovi CURSOR FOR
        SELECT KlubID, Naziv, Adresa, ProsjecnaOcjena, CijenaUlaza
        FROM Klubovi
        WHERE GradID = @GradID AND Aktivan = 1

    OPEN kursor_klubovi
    FETCH NEXT FROM kursor_klubovi INTO @klubID, @naziv, @adresa, @ocjena, @cijena

    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT 'Klub: ' + @naziv + ' | Adresa: ' + @adresa +
              ' | Ocjena: ' + CAST(@ocjena AS NVARCHAR) +
              ' | Cijena ulaza: ' + ISNULL(CAST(@cijena AS NVARCHAR), 'besplatno')

        FETCH NEXT FROM kursor_klubovi INTO @klubID, @naziv, @adresa, @ocjena, @cijena
    END

    CLOSE kursor_klubovi
    DEALLOCATE kursor_klubovi
END;
GO

-- SP 2: Ispis svih rezervacija korisnika koristeci kursor
CREATE PROCEDURE sp_RezervacijeKorisnika
    @KorisnikID INT
AS
BEGIN
    DECLARE @rezID       INT
    DECLARE @datum       DATETIME
    DECLARE @gosti       INT
    DECLARE @status      INT
    DECLARE @brojStola   NVARCHAR(20)
    DECLARE @nazivKluba  NVARCHAR(100)
    DECLARE @statusTekst NVARCHAR(20)

    DECLARE kursor_rezervacije CURSOR FOR
        SELECT r.RezervacijaID, r.DatumRezervacije, r.BrojGostiju, r.Status,
               s.BrojStola, k.Naziv
        FROM Rezervacije r
        JOIN Stolovi s ON r.StolID = s.StolID
        JOIN Klubovi k ON s.KlubID = k.KlubID
        WHERE r.KorisnikID = @KorisnikID

    OPEN kursor_rezervacije
    FETCH NEXT FROM kursor_rezervacije INTO @rezID, @datum, @gosti, @status, @brojStola, @nazivKluba

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @status = 0 SET @statusTekst = 'Na cekanju'
        ELSE IF @status = 1 SET @statusTekst = 'Potvrdjeno'
        ELSE IF @status = 2 SET @statusTekst = 'Otkazano'
        ELSE SET @statusTekst = 'Nepoznato'

        PRINT 'Rezervacija #' + CAST(@rezID AS NVARCHAR) +
              ' | Klub: ' + @nazivKluba +
              ' | Sto: ' + @brojStola +
              ' | Datum: ' + CAST(@datum AS NVARCHAR) +
              ' | Gosti: ' + CAST(@gosti AS NVARCHAR) +
              ' | Status: ' + @statusTekst

        FETCH NEXT FROM kursor_rezervacije INTO @rezID, @datum, @gosti, @status, @brojStola, @nazivKluba
    END

    CLOSE kursor_rezervacije
    DEALLOCATE kursor_rezervacije
END;
GO

-- =============================================
-- TRIGERI
-- =============================================

-- Triger 1: Nakon dodavanja recenzije - azuriraj prosjecnu ocjenu kluba
CREATE TRIGGER trg_NakonDodavanjaRecenzije
ON Recenzije AFTER INSERT
AS
BEGIN
    DECLARE @klubID INT
    SELECT @klubID = KlubID FROM inserted

    UPDATE Klubovi
    SET ProsjecnaOcjena = dbo.fn_ProsjecnaOcjena(@klubID),
        BrojOcjena      = (SELECT COUNT(*) FROM Recenzije WHERE KlubID = @klubID)
    WHERE KlubID = @klubID
END;
GO

-- Triger 2: Nakon brisanja recenzije - azuriraj prosjecnu ocjenu kluba
CREATE TRIGGER trg_NakonBrisanjaRecenzije
ON Recenzije AFTER DELETE
AS
BEGIN
    DECLARE @klubID INT
    SELECT @klubID = KlubID FROM deleted

    UPDATE Klubovi
    SET ProsjecnaOcjena = dbo.fn_ProsjecnaOcjena(@klubID),
        BrojOcjena      = (SELECT COUNT(*) FROM Recenzije WHERE KlubID = @klubID)
    WHERE KlubID = @klubID
END;
GO
-- =============================================
-- POCETNI PODACI
-- =============================================

INSERT INTO Uloge (Naziv) VALUES ('Admin'), ('Korisnik');

INSERT INTO Gradovi (Naziv) VALUES ('Podgorica'), ('Budva'), ('Bar'), ('Kotor'), ('Tivat');

INSERT INTO MuzickiZanrovi (Naziv) VALUES ('House'), ('Techno'), ('Hip-Hop'), ('Komercijalna'), ('Latino');

-- Admin korisnik (lozinka: Admin123)
INSERT INTO Korisnici (KorisnickoIme, Email, LozinkaHash, UlogaID)
VALUES ('admin', 'admin@nightlifecg.me',
        'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1);

-- Budva: 1,2,3 | Kotor: 4 | Podgorica: 5,6,7 | Bar: 8 | Tivat: 9
INSERT INTO Klubovi (Naziv, Opis, Adresa, CijenaUlaza, GradID, ZanrID) VALUES
('Top Hill',     'Otvoreni klub na vrhu brda iznad Budve sa pogledom na more i internacionalnim DJ-evima.',  'Becici bb, Budva',                      15.00, 2, 1),
('Omnia',        'Jedan od najvecih i najprestiznih klubova na Jadranu, poznat po spektakularnim zabavama.', 'Slovenska obala bb, Budva',              20.00, 2, 1),
('Cascada',      'Popularan beach club sa bazenom i zurkami koje traju do jutra.',                           'Becici, Budva',                          10.00, 2, 4),
('Maximus Kotor','Klub unutar kotorskih zidina sa odlicnom atmosferom i raznovrsnom muzikom.',              'Stari grad bb, Kotor',                    NULL, 4, 4),
('WWW Club',     'Najpopularniji klub u Podgorici, centar nocnog zivota glavnog grada.',                    'Ulica Slobode 56, Podgorica',             5.00, 1, 3),
('Why Bistro',   'Moderan bar i klub u srcu Podgorice, idealan za druzenje i zurke vikendom.',              'Bulevar Svetog Petra Cetinjskog, Podgorica', NULL, 1, 4),
('Welder Pub',   'Popularan pub sa zivom muzikom i odlicnom ponudom piva i koktelа.',                       'Ulica 19. decembra bb, Podgorica',        NULL, 1, 4),
('Aqua Club',    'Beach club na obali u Baru sa zurkami tokom cijelog ljeta.',                              'Obala bb, Bar',                           8.00, 3, 1),
('Lido Tivat',   'Elegantni club uz Portonovi marinu sa pogledom na Bokokotorski zaliv.',                  'Obala bb, Tivat',                        12.00, 5, 1);

INSERT INTO Stolovi (BrojStola, BrojMjesta, JeVIP, KlubID) VALUES
-- Top Hill (1)
('S1', 4, 0, 1), ('S2', 4, 0, 1), ('S3', 6, 0, 1), ('VIP1', 8, 1, 1), ('VIP2', 10, 1, 1),
-- Omnia (2)
('S1', 4, 0, 2), ('S2', 4, 0, 2), ('VIP1', 8, 1, 2), ('VIP2', 12, 1, 2),
-- Cascada (3)
('S1', 4, 0, 3), ('S2', 6, 0, 3), ('VIP1', 8, 1, 3),
-- Maximus Kotor (4)
('S1', 2, 0, 4), ('S2', 4, 0, 4), ('S3', 4, 0, 4),
-- WWW Club (5)
('S1', 4, 0, 5), ('S2', 4, 0, 5), ('VIP1', 6, 1, 5),
-- Why Bistro (6)
('S1', 2, 0, 6), ('S2', 4, 0, 6), ('S3', 4, 0, 6),
-- Welder Pub (7)
('S1', 4, 0, 7), ('S2', 4, 0, 7), ('S3', 6, 0, 7),
-- Aqua Club (8)
('S1', 4, 0, 8), ('S2', 6, 0, 8), ('VIP1', 8, 1, 8),
-- Lido Tivat (9)
('S1', 4, 0, 9), ('S2', 4, 0, 9), ('VIP1', 10, 1, 9);

INSERT INTO Dogadjaji (Naziv, DatumOdrzavanja, CijenaKarte, KlubID) VALUES
('Top Hill Opening Night',   '2026-06-01 22:00', 20.00, 1),
('Omnia Summer Festival',    '2026-06-14 23:00', 30.00, 2),
('Cascada Pool Party',       '2026-07-05 20:00', 15.00, 3),
('Stari Grad Night',         '2026-07-12 21:00',  NULL, 4),
('WWW Friday Night',         '2026-06-20 23:00', 10.00, 5),
('Why Bistro Live Music',    '2026-06-27 21:00',  NULL, 6),
('Welder Live Band',         '2026-07-04 21:00',  NULL, 7),
('Aqua Club Beach Bash',     '2026-07-19 20:00', 10.00, 8),
('Lido Sunset Party',        '2026-07-26 19:00', 12.00, 9);
GO






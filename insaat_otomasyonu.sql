create database InsaatOtomasyonDB

CREATE TABLE Malzemeler (
    MalzemeID INT PRIMARY KEY IDENTITY(1,1),
    MalzemeAdi NVARCHAR(100) NOT NULL,
    Birim NVARCHAR(50) NOT NULL,
    BirimFiyat DECIMAL(18,2) NOT NULL,
    StokMiktari DECIMAL(18,2) NOT NULL,
    Aciklama NVARCHAR(250)
);

ALTER TABLE Malzemeler ADD Kategori NVARCHAR(50);

CREATE TABLE Musteriler (
    MusteriID INT PRIMARY KEY IDENTITY(1,1),
    AdSoyad NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20),
    Eposta NVARCHAR(100),
    Adres NVARCHAR(250),
    VergiNo NVARCHAR(50)
);

ALTER TABLE Musteriler ADD FirmaAdi NVARCHAR(50);


CREATE TABLE Satislar (
    SatisID INT PRIMARY KEY IDENTITY(1,1),
    Tarih DATE NOT NULL,
    MusteriID INT NOT NULL,
    OdemeTuru NVARCHAR(50),
    ToplamTutar DECIMAL(18,2),
    FOREIGN KEY (MusteriID) REFERENCES Musteriler(MusteriID)
);

ALTER TABLE Satislar ADD Personel NVARCHAR(50);
ALTER TABLE Satislar ADD Aciklama NVARCHAR(250);

select * from Satislar

CREATE TABLE SatisDetaylari (
    DetayID INT PRIMARY KEY IDENTITY(1,1),
    SatisID INT NOT NULL,
    MalzemeID INT NOT NULL,
    Miktar DECIMAL(18,2) NOT NULL,
    BirimFiyat DECIMAL(18,2) NOT NULL,
    Tutar AS (Miktar * BirimFiyat) PERSISTED,
    FOREIGN KEY (SatisID) REFERENCES Satislar(SatisID),
    FOREIGN KEY (MalzemeID) REFERENCES Malzemeler(MalzemeID)
);

ALTER TABLE SatisDetaylari ADD TedarikciAdi NVARCHAR(50);
ALTER TABLE SatisDetaylari ADD AlisTarihi DATETIME;

select * from SatisDetaylari



CREATE TABLE StokHareketleri (
    HareketID INT PRIMARY KEY IDENTITY(1,1),
    MalzemeID INT NOT NULL,
    Tarih DATE NOT NULL,
    HareketTuru NVARCHAR(50) NOT NULL, -- Giriþ / Çýkýþ
    Miktar DECIMAL(18,2) NOT NULL,
    Aciklama NVARCHAR(250),
    FOREIGN KEY (MalzemeID) REFERENCES Malzemeler(MalzemeID)
);

ALTER TABLE StokHareketleri ADD Personel NVARCHAR(50);
ALTER TABLE StokHareketleri ADD ReferansNo NVARCHAR(30);

select * from StokHareketleri

CREATE TABLE Tedarikciler (
    TedarikciID INT PRIMARY KEY IDENTITY(1,1),
    FirmaAdi NVARCHAR(100) NOT NULL,
    YetkiliAdi NVARCHAR(100),
    Telefon NVARCHAR(20),
    Eposta NVARCHAR(100),
    Adres NVARCHAR(250)
);

ALTER TABLE Tedarikciler ADD VergiNo NVARCHAR(50);

select * from Tedarikciler

CREATE TABLE Kullanicilar (
	KullaniciAdi NVARCHAR(50) NOT NULL,
	Sifre NVARCHAR(50) NOT NULL
);

INSERT INTO Kullanicilar (KullaniciAdi, Sifre) VALUES ('admin', '1234');



Select * From Musteriler

select * from Tedarikciler

select * from SatisDetaylari
select * from StokHareketleri
select * from Satislar
-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server-Version:               PostgreSQL 17.3 (Debian 17.3-3.pgdg120+1) on x86_64-pc-linux-gnu, compiled by gcc (Debian 12.2.0-14) 12.2.0, 64-bit
-- Server-Betriebssystem:        
-- HeidiSQL Version:             12.10.0.7000
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES  */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle public.company
CREATE TABLE IF NOT EXISTS "company" (
	"companyId" SERIAL NOT NULL,
	"street" VARCHAR NOT NULL,
	"zipcode" INTEGER NOT NULL,
	"city" VARCHAR NOT NULL,
	"name" VARCHAR NOT NULL,
	"country" VARCHAR NOT NULL,
	PRIMARY KEY ("companyId")
);

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle public.thesis
CREATE TABLE IF NOT EXISTS "thesis" (
	"thesisId" SERIAL NOT NULL,
	"topic" VARCHAR NOT NULL,
	"studentId" INTEGER NOT NULL,
	"companyId" INTEGER NOT NULL,
	"operationalLocationDepartment" INTEGER NOT NULL,
	"operationalLocationStreet" VARCHAR NOT NULL,
	"operationalLocationZipcode" INTEGER NOT NULL,
	"operationalLocationCity" VARCHAR NOT NULL,
	"operationalLocationCountry" VARCHAR NOT NULL,
	"inCompanySupervisorTitle" VARCHAR NULL DEFAULT NULL::character varying,
	"inCompanySupervisorAcademicTitle" VARCHAR NULL DEFAULT NULL::character varying,
	"inCompanySupervisorFirstName" VARCHAR NOT NULL,
	"inCompanySupervisorLastName" VARCHAR NOT NULL,
	"inCompanySupervisorEmail" VARCHAR NOT NULL,
	"inCompanySupervisorPhoneNumber" VARCHAR NOT NULL,
	"inCompanySupervisorAcademicDegree" VARCHAR NOT NULL,
	"preparationPeriodBegin" DATE NOT NULL,
	"preparationPeriodEnd" DATE NOT NULL,
	PRIMARY KEY ("thesisId"),
	CONSTRAINT "FK_Thesis_company" FOREIGN KEY ("companyId") REFERENCES "company" ("companyId") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle public.SupervisorsExclusion
CREATE TABLE IF NOT EXISTS "supervisorsExclusion" (
	"thesisId" INTEGER NOT NULL,
	"companyId" INTEGER NOT NULL,
	PRIMARY KEY ("thesisId", "companyId"),
	CONSTRAINT "FK__Thesis" FOREIGN KEY ("thesisId") REFERENCES "thesis" ("thesisId") ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT "FK__partnerCompany" FOREIGN KEY ("companyId") REFERENCES "company" ("companyId") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Exportiere Struktur von Tabelle public.comments
CREATE TABLE IF NOT EXISTS "comments" (
	"commentId" SERIAL NOT NULL,
	"thesisId" INTEGER NOT NULL,
	"author" INTEGER NOT NULL,
	"msg" TEXT NOT NULL,
	"createdAt" TIME NOT NULL,
	PRIMARY KEY ("commentId"),
	CONSTRAINT "FK__Thesis" FOREIGN KEY ("thesisId") REFERENCES "thesis" ("thesisId") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Daten-Export vom Benutzer nicht ausgewählt

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;

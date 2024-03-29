/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */
 
 
CREATE TABLE IF NOT EXISTS Documents(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	TypeName VARCHAR(255),
	Created DATETIME NULL,
	Modified DATETIME NULL,
	Parent INT unsigned zerofill NULL,
	index(Parent),
	ParentPropertyName VARCHAR(255) NULL,
	FOREIGN KEY(Parent) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS Documents2Documents(
	Document1ID INT unsigned zerofill NOT NULL,
	INDEX(Document1ID),
	Document2ID INT unsigned zerofill NOT NULL,
	INDEX(Document2ID),
	PropertyName varchar(255) NULL,
	FOREIGN KEY(Document1ID) REFERENCES Documents(ID),
	FOREIGN KEY(Document2ID) REFERENCES Documents(ID)
);


CREATE TABLE IF NOT EXISTS PropertyDates(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value DATETIME NULL,
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS PropertyDecimals(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value DECIMAL(19, 5) NULL,
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS PropertyBools(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value BIT NULL,
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS PropertyBLOBS(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value BLOB NULL,
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS PropertyStrings(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value VARCHAR(4096),
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS PropertyLongStrings(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value TEXT,
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE IF NOT EXISTS PropertyInts(
	ID INT unsigned zerofill NOT NULL AUTO_INCREMENT PRIMARY KEY,
	INDEX(ID),
	Name VARCHAR(255),
	Value INT,
	FK_Document INT unsigned zerofill NOT NULL,
	FOREIGN KEY(FK_Document) REFERENCES Documents(ID) ON DELETE CASCADE ON UPDATE CASCADE
);



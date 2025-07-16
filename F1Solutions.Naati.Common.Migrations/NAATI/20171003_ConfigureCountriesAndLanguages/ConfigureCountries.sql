DELETE FROM tblCountry WHERE CountryId = 29
DELETE FROM tblCountry WHERE CountryId = 31
UPDATE tblCountry SET Name = 'Brunei' WHERE CountryId = 32 UPDATE tblCountry SET Code = 'BN' WHERE CountryId = 32
UPDATE tblCountry SET Name = 'Cote d''Ivoire' WHERE CountryId = 52 UPDATE tblCountry SET Code = 'CI' WHERE CountryId = 52
UPDATE tblCountry SET Name = 'Croatia' WHERE CountryId = 53 UPDATE tblCountry SET Code = 'HR' WHERE CountryId = 53
UPDATE tblCountry SET Name = 'San Salvador' WHERE CountryId = 64 UPDATE tblCountry SET Code = 'SV' WHERE CountryId = 64
UPDATE tblCountry SET Name = 'Falkland Islands (Islas Malvinas)' WHERE CountryId = 69 UPDATE tblCountry SET Code = 'FK' WHERE CountryId = 69
DELETE FROM tblCountry WHERE CountryId = 74
DELETE FROM tblCountry WHERE CountryId = 77
UPDATE tblCountry SET Name = 'The Gambia' WHERE CountryId = 79 UPDATE tblCountry SET Code = 'GM' WHERE CountryId = 79
DELETE FROM tblCountry WHERE CountryId = 94
UPDATE tblCountry SET Name = 'Federated States of Micronesia' WHERE CountryId = 139 UPDATE tblCountry SET Code = 'FM' WHERE CountryId = 139
UPDATE tblCountry SET Name = 'Pitcairn Islands' WHERE CountryId = 169 UPDATE tblCountry SET Code = 'PN' WHERE CountryId = 169
UPDATE tblCountry SET Name = 'St Kitts and Nevis' WHERE CountryId = 178 UPDATE tblCountry SET Code = 'KN' WHERE CountryId = 178
UPDATE tblCountry SET Name = 'St Lucia' WHERE CountryId = 179 UPDATE tblCountry SET Code = 'LC' WHERE CountryId = 179
UPDATE tblCountry SET Name = 'St Vincent and the Grenadines' WHERE CountryId = 180 UPDATE tblCountry SET Code = 'VC' WHERE CountryId = 180
UPDATE tblCountry SET Name = 'São Tomé and Príncipe' WHERE CountryId = 183 UPDATE tblCountry SET Code = 'ST' WHERE CountryId = 183
UPDATE tblCountry SET Name = 'Slovakia' WHERE CountryId = 189 UPDATE tblCountry SET Code = 'SK' WHERE CountryId = 189
DELETE FROM tblCountry WHERE CountryId = 194
UPDATE tblCountry SET Name = 'St Helena, Ascension and Tristan da Cunha' WHERE CountryId = 197 UPDATE tblCountry SET Code = 'SH' WHERE CountryId = 197
UPDATE tblCountry SET Name = 'Svalbard and Jan Mayen' WHERE CountryId = 201 UPDATE tblCountry SET Code = 'SJ' WHERE CountryId = 201
UPDATE tblCountry SET Name = 'United States of America' WHERE CountryId = 223 UPDATE tblCountry SET Code = 'USA' WHERE CountryId = 223
UPDATE tblCountry SET Name = 'Vatican City' WHERE CountryId = 228 UPDATE tblCountry SET Code = 'VA' WHERE CountryId = 228
UPDATE tblCountry SET Name = 'British Virgin Islands' WHERE CountryId = 231 UPDATE tblCountry SET Code = 'VG' WHERE CountryId = 231
UPDATE tblCountry SET Name = 'US Virgin Islands' WHERE CountryId = 232 UPDATE tblCountry SET Code = 'VI' WHERE CountryId = 232
UPDATE tblCountry SET Name = 'Wallis and Futuna' WHERE CountryId = 233 UPDATE tblCountry SET Code = 'WF' WHERE CountryId = 233
DELETE FROM tblCountry WHERE CountryId = 240
UPDATE tblCountry SET Name = 'Palestine' WHERE CountryId = 241 UPDATE tblCountry SET Code = 'PS' WHERE CountryId = 241
UPDATE tblCountry SET Name = 'Serbia and Montenegro' WHERE CountryId = 243 UPDATE tblCountry SET Code = 'SO' WHERE CountryId = 243
DELETE FROM tblCountry WHERE CountryId = 441
DELETE FROM tblCountry WHERE CountryId = 741
INSERT INTO tblCountry (CountryId, Name, Code) VALUES (1000, 'Montenegro', '')
INSERT INTO tblCountry (CountryId, Name, Code) VALUES (1001, 'South Sudan', '')
INSERT INTO tblCountry (CountryId, Name, Code) VALUES (1002, 'Serbia', '')
UPDATE tblTableData SET NextKey = 1003 WHERE TableName = 'Country'

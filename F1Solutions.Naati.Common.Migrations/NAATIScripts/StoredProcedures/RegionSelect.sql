CREATE PROCEDURE REGIONSELECT (@RowVersion ROWVERSION = NULL) 
AS 
    SELECT RegionId, 
           [Name], 
           StateId, 
           CountryId, 
           RowVersion 
    FROM   tluRegion 
    WHERE  ( tluRegion.RowVersion > @RowVersion 
              OR @RowVersion IS NULL ) 
    ORDER  BY NAME 
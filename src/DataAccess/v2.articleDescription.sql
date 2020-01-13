ALTER TABLE articles
ADD COLUMN Description string DEFAULT '';

ALTER TABLE articles
DROP DEFAULT FOR Description;

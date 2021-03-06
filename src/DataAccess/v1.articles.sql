create table articles
    ( Id INT PRIMARY KEY autoincrement,
      Title string,
      Source string,
      Parsed string,
      Tooltips string,
      CreatedOn datetime
    );

create table tags
    ( Id INT PRIMARY KEY autoincrement,
      Name string unique
    );


create table article_tags
    ( ArticleId INT REFERENCES Articles(Id),
      TagId INT REFERENCES Tags(Id)
    );
